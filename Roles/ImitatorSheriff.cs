using System.Collections.Generic;
using Hazel;
using System;
using UnityEngine;

namespace TownOfHost
{
    public static class ImitatorSheriff
    {
        private static readonly int Id = 20400;
        public static List<byte> playerIdList = new();
        public static PlayerControl seer;
        public static bool imitator = false;

        private static CustomOption KillCooldown;
        private static CustomOption CanKillArsonist;
        private static CustomOption CanKillMadmate;
        private static CustomOption CanKillJester;
        private static CustomOption CanKillTerrorist;
        private static CustomOption CanKillOpportunist;
        private static CustomOption CanKillEgoist;
        private static CustomOption CanKillEgoShrodingerCat;
        private static CustomOption CanKillExecutioner;
        private static CustomOption CanKillJackal;
        private static CustomOption CanKillJShrodingerCat;
        private static CustomOption CanKillPlagueBearer;
        private static CustomOption CanKillCrewmatesAsIt;
        private static CustomOption CanKillJug;
        // public static CustomOption PlayersForTraitor;
        private static CustomOption CanKillVulture;
        private static CustomOption ImitatorCanKillCoven;
        private static CustomOption CanKillGlitch;
        private static CustomOption CanKillWerewolf;
        private static CustomOption CanKillHitman;
        private static CustomOption CanKillAgitater;
        private static CustomOption CanKillClumsy;
        public static CustomOption NoDeathPenalty;
        public static CustomOption CanKillPostman;

        public static Dictionary<byte, float> ShotLimit = new();
        public static Dictionary<byte, float> CurrentKillCooldown = new();
        
        public static void Init()
        {
            playerIdList = new();
            ShotLimit = new();
            CurrentKillCooldown = new();
            seer = null;
            var number = System.Convert.ToUInt32(PercentageChecker.CheckPercentage(CustomRoles.CorruptedSheriff.ToString(), role: CustomRoles.CorruptedSheriff));
            imitator = UnityEngine.Random.Range(1, 100) <= number;
        }
        public static void Add(byte playerId)
        {
            playerIdList.Add(playerId);
            CurrentKillCooldown.Add(playerId, 25);

            if (!Main.ResetCamPlayerList.Contains(playerId))
                Main.ResetCamPlayerList.Add(playerId);

            ShotLimit.TryAdd(playerId, 15);
            Logger.Info($"{Utils.GetPlayerById(playerId)?.GetNameWithRole()} : 残り{ShotLimit[playerId]}発", "Imitator");
        }
        public static bool IsEnable => playerIdList.Count > 0;
        private static void SendRPC(byte playerId)
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetImitatorShotLimit, SendOption.Reliable, -1);
            writer.Write(playerId);
            writer.Write(ShotLimit[playerId]);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }
        public static void ReceiveRPC(MessageReader reader)
        {
            byte ImitatorSId = reader.ReadByte();
            float Limit = reader.ReadSingle();
            if (ShotLimit.ContainsKey(ImitatorSId))
                ShotLimit[ImitatorSId] = Limit;
            else
                ShotLimit.Add(ImitatorSId, 15);
        }
        public static void SetKillCooldown(byte id) => Main.AllPlayerKillCooldown[id] = CurrentKillCooldown[id];
        public static bool CanUseKillButton(PlayerControl player)
        {
            if (player.Data.IsDead)
                return false;


            if (ShotLimit[player.PlayerId] == 15)
            {
                //Logger.info($"{player.GetNameWithRole()} はキル可能回数に達したため、RoleTypeを守護天使に変更しました。", "Imitator");
                //player.RpcSetRoleDesync(RoleTypes.GuardianAngel);
                //Utils.hasTasks(player.Data, false);
                //Utils.NotifyRoles();
                return false;
            }
            return true;
        }
        public static bool OnCheckMurder(PlayerControl killer, PlayerControl target, string Process)
        {
            switch (Process)
            {
                case "RemoveShotLimit":
                    ShotLimit[killer.PlayerId]--;
                    Logger.Info($"{killer.GetNameWithRole()} : 残り{ShotLimit[killer.PlayerId]}発", "Imitator");
                    SendRPC(killer.PlayerId);
                    //SwitchToCorrupt(killer, target);
                    break;
                case "Suicide":
                    if (!target.CanBeKilledByImitator())
                    {
                        PlayerState.SetDeathReason(killer.PlayerId, PlayerState.DeathReason.Misfire);
                        killer.RpcMurderPlayer(killer);
                        if (CanKillCrewmatesAsIt.GetBool())
                            killer.RpcMurderPlayer(target);
                        return false;
                    }
                    break;
            }
            return true;
        }
        public static string GetShotLimit(byte playerId)
        {
            return Helpers.ColorString(Color.yellow, ShotLimit.TryGetValue(playerId, out var shotLimit) ? $"({shotLimit})" : "Invalid");
        }
        public static bool CanBeKilledByImitator(this PlayerControl player)
        {
            var cRole = player.GetCustomRole();
            return cRole switch
            {
                CustomRoles.Jester => true,
                CustomRoles.Terrorist => true,
                CustomRoles.CrewPostor => true,
                CustomRoles.Executioner => true,
                CustomRoles.Swapper => true,
                CustomRoles.Opportunist => true,
                CustomRoles.Survivor => true,
                CustomRoles.Arsonist => true,
                CustomRoles.Egoist => true,
                CustomRoles.EgoSchrodingerCat => true,
                CustomRoles.Jackal => true,
                CustomRoles.Sidekick => true,
                CustomRoles.JSchrodingerCat => true,
                CustomRoles.PlagueBearer => true,
                CustomRoles.Juggernaut => true,
                CustomRoles.Marksman => true,
                CustomRoles.Hitman => true,
                CustomRoles.BloodKnight => true,
                CustomRoles.Vulture => true,
                CustomRoles.TheGlitch => true,
                CustomRoles.Werewolf => true,
                CustomRoles.AgiTater => true,
                CustomRoles.Clumsy => true,
                CustomRoles.Pirate => true,
                CustomRoles.Dracula => true,
                CustomRoles.Magician => true,
                CustomRoles.TemplateRole => true,
                CustomRoles.Wraith => false,
                CustomRoles.Hustler => true,
                CustomRoles.Unseeable => true,
                CustomRoles.ImitatorHitman => true,
                // COVEN //
                CustomRoles.Coven => true,
                CustomRoles.CovenWitch => true,
                CustomRoles.Poisoner => true,
                CustomRoles.HexMaster => true,
                CustomRoles.PotionMaster => true,
                CustomRoles.Medusa => true,
                CustomRoles.Mimic => true,
                CustomRoles.Necromancer => true,
                CustomRoles.Conjuror => true,
                // AFTER COVEN //
                CustomRoles.SchrodingerCat => true,
                CustomRoles.Phantom => true,
                CustomRoles.Hacker => true,
                CustomRoles.NeutWitch => true,
                _ => cRole.GetRoleType() switch
                {
                    RoleType.Impostor => true,
                    RoleType.Madmate => true,
                    _ => false,
                }
            } ;
        }
    }
}