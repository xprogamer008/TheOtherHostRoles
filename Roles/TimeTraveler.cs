using System.Collections.Generic;
using AmongUs.GameOptions;
using Hazel;
using TownOfHost.PrivateExtensions;
using TownOfHost.RoleHelpers;
using UnityEngine;

namespace TownOfHost;

public static class TimeTraveler
{
    public static readonly int ID = 132746701;
    public static List<byte> playerIdlist;

    public static CustomOption MarkCooldown;
    public static CustomOption RecallCooldown;
    public static CustomOption TimeTravelerCanVent;

    public static Dictionary<byte, TimeTravelerState> CurrentTimeTravelerState;
    public static Dictionary<byte, string> TimeTravelerStateString;

    public static Dictionary<byte, Vector2> MarkedArea;
    public static Dictionary<byte, bool> InCooldown;

    public static void SetupCustomOption()
    {
        Options.SetupRoleOptions(ID, CustomRoles.TimeTraveler, AmongUsExtensions.OptionType.Crewmate);
        MarkCooldown = CustomOption.Create(ID + 10, Color.white, "MarkCooldown", AmongUsExtensions.OptionType.Crewmate, 25f, 2.5f, 60f, 2.5f, Options.CustomRoleSpawnChances[CustomRoles.TimeTraveler]);
        RecallCooldown = CustomOption.Create(ID + 11, Color.white, "RecallCooldown", AmongUsExtensions.OptionType.Crewmate, 25f, 2.5f, 60f, 2.5f, Options.CustomRoleSpawnChances[CustomRoles.TimeTraveler]);
        TimeTravelerCanVent = CustomOption.Create(ID + 12, Color.white, "TimeTravelerCanVent", AmongUsExtensions.OptionType.Crewmate, true, Options.CustomRoleSpawnChances[CustomRoles.TimeTraveler]);
    }

    public static void Init()
    {
        CurrentTimeTravelerState = new Dictionary<byte, TimeTravelerState>();
        TimeTravelerStateString = new Dictionary<byte, string>();
        MarkedArea = new Dictionary<byte, Vector2>();
        InCooldown = new Dictionary<byte, bool>();

        playerIdlist = new List<byte>();
    }

    public static void Add(PlayerControl player)
    {
        playerIdlist.Add(player.PlayerId);
        CurrentTimeTravelerState.Add(player.PlayerId, TimeTravelerState.Default);
        TimeTravelerStateString.Add(player.PlayerId, "gameStart");
        SendRpc(player.PlayerId);
    }

    public static void StartCooldown(PlayerControl player)
    {
        if (!CurrentTimeTravelerState.ContainsKey(player.PlayerId)) return;
        if (!TimeTravelerStateString.ContainsKey(player.PlayerId)) return;

        switch (CurrentTimeTravelerState[player.PlayerId])
        {
            case TimeTravelerState.OnMark:
                InCooldown[player.PlayerId] = true;
                TimeTravelerStateString[player.PlayerId] = "recall-cooldown";
                SendRpc(player.PlayerId);
                _ = new LateTask(() =>
                {
                    InCooldown[player.PlayerId] = false;
                    TimeTravelerStateString[player.PlayerId] = "recall-ready";
                    CurrentTimeTravelerState[player.PlayerId] = TimeTravelerState.OnRecall;
                    SendRpc(player.PlayerId);
                }, RecallCooldown.GetFloat(), "Recall Cooldown");
                break;
            case TimeTravelerState.OnRecall:
                InCooldown[player.PlayerId] = true;
                TimeTravelerStateString[player.PlayerId] = "mark-cooldown";
                SendRpc(player.PlayerId);
                _ = new LateTask(() =>
                {
                    InCooldown[player.PlayerId] = false;
                    TimeTravelerStateString[player.PlayerId] = "mark-ready";
                    CurrentTimeTravelerState[player.PlayerId] = TimeTravelerState.OnMark;
                    SendRpc(player.PlayerId);
                }, MarkCooldown.GetFloat(), "Mark Cooldown");
                break;
        }
    }

    public static float GetCooldown(PlayerControl player)
    {
        if (!TimeTravelerStateString.ContainsKey(player.PlayerId)) return 99999f;

        switch (TimeTravelerStateString[player.PlayerId])
        {
            case "recall-cooldown":
                return MarkCooldown.GetFloat();
            case "mark-cooldown":
                return RecallCooldown.GetFloat();
            case "recall-ready":
            case "mark-ready":

                return 0f;
            default:
                return 10f;
        }
    }

    public static void OnPet(PlayerControl player)
    {
        if (!CurrentTimeTravelerState.ContainsKey(player.PlayerId)) return;
        if (!TimeTravelerStateString.ContainsKey(player.PlayerId)) return;

        string currentState = TimeTravelerStateString[player.PlayerId];

        switch (CurrentTimeTravelerState[player.PlayerId])
        {
            case TimeTravelerState.Default:
                if (currentState == "gameStart")
                {
                    CurrentTimeTravelerState[player.PlayerId] = TimeTravelerState.OnMark;
                    MarkedArea.Add(player.PlayerId, player.GetTruePosition());
                    StartCooldown(player);
                }
                break;
            case TimeTravelerState.OnMark:
                if (currentState == "mark-ready")
                {
                    //CurrentTimeTravelerState[player.PlayerId] = TimeTravelerState.OnRecall;
                    MarkedArea.Add(player.PlayerId, player.GetTruePosition());
                    StartCooldown(player);
                }
                break;
            case TimeTravelerState.OnRecall:
                if (currentState == "recall-ready")
                {
                    //CurrentTimeTravelerState[player.PlayerId] = TimeTravelerState.OnMark;
                    var position = MarkedArea[player.PlayerId];
                    MarkedArea.Remove(player.PlayerId);
                    Utils.TP(player.NetTransform, new Vector2(position.x, position.y));
                    StartCooldown(player);
                }
                break;
        }
    }

    public static string GetTimeTravelerState(PlayerControl player)
    {
        if (!TimeTravelerStateString.ContainsKey(player.PlayerId)) return "No State Found.";
        return TimeTravelerStateString[player.PlayerId];
    }

    public static string GetAbilityButtonText(PlayerControl player)
    {
        string currentState = TimeTravelerStateString[player.PlayerId];
        switch (currentState)
        {
            case "recall-ready":
            case "recall-cooldown":
                return "Recall";
            case "mark-ready":
            case "mark-cooldown":
                return "Mark";
            default:
                return Translator.GetString("DefaultShapeshiftText");
        }
    }

    public static bool CanVent() => TimeTravelerCanVent.GetBool();

    public static void ApplyGameOptions(PlayerControl player, NormalGameOptionsV07 options)
    {
        options.GetShapeshifterOptions().ShapeshifterCooldown = GetCooldown(player);
        options.GetShapeshifterOptions().ShapeshifterDuration = 1f;
        options.GetShapeshifterOptions().ShapeshifterLeaveSkin = false;
    }
    public static void SendRpc(byte playerId)
    {
        if (!TimeTravelerStateString.ContainsKey(playerId)) return;
        if (!InCooldown.ContainsKey(playerId)) return;
        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetTimeTravelerState, SendOption.Reliable, -1);
        writer.Write(playerId);
        writer.Write(TimeTravelerStateString[playerId]);
        writer.Write(InCooldown[playerId]);
        AmongUsClient.Instance.FinishRpcImmediately(writer);
    }

    public static void HandleRpc(MessageReader reader)
    {
        byte playerId = reader.ReadByte();
        string TimeTravelerState = reader.ReadString();
        bool inCooldown = reader.ReadBoolean();
        TimeTravelerStateString[playerId] = TimeTravelerState;
        InCooldown[playerId] = inCooldown;
    }
}