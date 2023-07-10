using System.Collections.Generic;
using Hazel;
using UnityEngine;

namespace TownOfHost
{
    public static class Examiner
    {
        private static readonly int Id = 520400;
        public static List<byte> playerIdList = new();
        public static Dictionary<byte, bool> hasSeered = new();

        public static CustomOption ExaminerKillCooldown;
        //private static CustomOption SpyShowsAsRed;


        public static bool SeeredCSheriff;
        public static Dictionary<byte, float> ShotLimit = new();
        public static Dictionary<byte, float> CurrentKillCooldown = new();
        public static void SetupCustomOption()
        {
            Options.SetupSingleRoleOptions(Id, CustomRoles.Examiner, 1, AmongUsExtensions.OptionType.Crewmate);
            ExaminerKillCooldown = CustomOption.Create(Id + 10, Color.white, "ExaminerCooldown", AmongUsExtensions.OptionType.Crewmate, 45, 1, 990, 1, Options.CustomRoleSpawnChances[CustomRoles.Examiner]);
            //SpyShowsAsRed = CustomOption.Create(Id + 19, Color.white, "SpyShowsAsRed", AmongUsExtensions.OptionType.Crewmate, false, Options.CustomRoleSpawnChances[CustomRoles.Examiner]);

        }
        public static void Init()
        {
            playerIdList = new();
            hasSeered = new();
            ShotLimit = new();
            CurrentKillCooldown = new();
            SeeredCSheriff = false;
        }
        public static void Add(byte playerId)
        {
            playerIdList.Add(playerId);
            CurrentKillCooldown.Add(playerId, ExaminerKillCooldown.GetFloat());

            if (!Main.ResetCamPlayerList.Contains(playerId))
                Main.ResetCamPlayerList.Add(playerId);

            Logger.Info($"{Utils.GetPlayerById(playerId)?.GetNameWithRole()} : Add Examiner Role", "Examiner");
        }
        public static bool IsEnable => playerIdList.Count > 0;
        private static void SendRPC(byte playerId)
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetSheriffShotLimit, SendOption.Reliable, -1);
            writer.Write(playerId);
            writer.Write(ShotLimit[playerId]);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }
        public static void ReceiveRPC(MessageReader reader)
        {
            byte SheriffId = reader.ReadByte();
            float Limit = reader.ReadSingle();
        }
        public static void SetKillCooldown(byte id) => Main.AllPlayerKillCooldown[id] = CurrentKillCooldown[id];
        public static bool CanUseKillButton(PlayerControl player)
        {
            if (player.Data.IsDead)
                return false;

            return true;
        }
        public static bool OnCheckMurder(PlayerControl killer, PlayerControl target, string Process)
        {
            Logger.Info($"{killer.GetNameWithRole()} : Checked Player: {target.GetNameWithRole()}", "Checked");
            SendRPC(killer.PlayerId);
            killer.RpcGuardAndKill(target);
            return true;
        }
        public static bool IsImp(this PlayerControl player)
        {
            var cRole = player.GetCustomRole();
            return cRole switch
            {
                   //CustomRoles.Spy => SpyShowsAsRed.GetBool(),

                _ => cRole.GetRoleType() switch
                {
                    RoleType.Impostor => true,
                    RoleType.Madmate => true,
                    _ => false,
                }
            };
        }
        public static bool IsCrew(this PlayerControl player)
        {
            var cRole = player.GetCustomRole();
            return cRole switch
            {
                //    CustomRoles.Spy => !SpyShowsAsRed.GetBool(),

                _ => cRole.GetRoleType() switch
                {
                    RoleType.Crewmate => true,
                    //     RoleType.Madmate => true,
                    _ => false,
                }
            };
        }
        public static bool IsNeutral(this PlayerControl player)
        {
            var cRole = player.GetCustomRole();
            return cRole switch
            {

                _ => cRole.GetRoleType() switch
                {
                    RoleType.Neutral => true,
                    //   RoleType.Madmate => true,
                    _ => false,
                }
            };
        }
        public static bool IsCoven(this PlayerControl player)
        {
            var cRole = player.GetCustomRole();
            return cRole switch
            {

                _ => cRole.GetRoleType() switch
                {
                    RoleType.Coven => true,
                    //   RoleType.Madmate => true,
                    _ => false,
                }
            };
        }
    }
}