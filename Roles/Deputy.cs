using System.Collections.Generic;
using Hazel;
using System;
using UnityEngine;

namespace TownOfHost
{
    public static class Deputy
    {
        private static readonly int Id = 20400;
        public static List<byte> playerIdList = new();
        public static PlayerControl seer;
        public static bool cDeputy = false;

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
        private static CustomOption ShotLimitOpt;
        // public static CustomOption PlayersForTraitor;
        // public static CustomOption DeputyCorrupted;
        private static CustomOption CanKillVulture;
        private static CustomOption DeputyCanKillCoven;
        private static CustomOption CanKillGlitch;
        private static CustomOption CanKillWerewolf;
        private static CustomOption CanKillHitman;
        private static CustomOption CanKillAgitater;
        private static CustomOption CanKillClumsy;
        public static CustomOption NoDeathPenalty;
        public static CustomOption CanKillPostman;

        public static Dictionary<byte, float> ShotLimit = new();
        public static Dictionary<byte, float> CurrentKillCooldown = new();
        public static void SetupCustomOption()
        {
            Options.SetupRoleOptions(Id, CustomRoles.Deputy, AmongUsExtensions.OptionType.Crewmate);
            KillCooldown = CustomOption.Create(Id + 10, Color.white, "DeputyKillCooldown", AmongUsExtensions.OptionType.Crewmate, 30, 0, 120, 1, Options.CustomRoleSpawnChances[CustomRoles.Deputy]);
            CanKillArsonist = CustomOption.Create(Id + 11, Color.white, "DeputyCanKillArsonist", AmongUsExtensions.OptionType.Crewmate, true, Options.CustomRoleSpawnChances[CustomRoles.Deputy]);
            CanKillMadmate = CustomOption.Create(Id + 12, Color.white, "DeputyCanKillMadmate", AmongUsExtensions.OptionType.Crewmate, true, Options.CustomRoleSpawnChances[CustomRoles.Deputy]);
            CanKillJester = CustomOption.Create(Id + 13, Color.white, "DeputyCanKillJester", AmongUsExtensions.OptionType.Crewmate, true, Options.CustomRoleSpawnChances[CustomRoles.Deputy]);
            CanKillTerrorist = CustomOption.Create(Id + 14, Color.white, "DeputyCanKillTerrorist", AmongUsExtensions.OptionType.Crewmate, true, Options.CustomRoleSpawnChances[CustomRoles.Deputy]);
            CanKillOpportunist = CustomOption.Create(Id + 15, Color.white, "DeputyCanKillOpportunist", AmongUsExtensions.OptionType.Crewmate, true, Options.CustomRoleSpawnChances[CustomRoles.Deputy]);
            CanKillEgoist = CustomOption.Create(Id + 16, Color.white, "DeputyCanKillEgoist", AmongUsExtensions.OptionType.Crewmate, true, Options.CustomRoleSpawnChances[CustomRoles.Deputy]);
            CanKillEgoShrodingerCat = CustomOption.Create(Id + 17, Color.white, "DeputyCanKillEgoShrodingerCat", AmongUsExtensions.OptionType.Crewmate, true, Options.CustomRoleSpawnChances[CustomRoles.Deputy]);
            CanKillExecutioner = CustomOption.Create(Id + 18, Color.white, "DeputyCanKillExecutioner", AmongUsExtensions.OptionType.Crewmate, true, Options.CustomRoleSpawnChances[CustomRoles.Deputy]);
            CanKillJackal = CustomOption.Create(Id + 19, Color.white, "DeputyCanKillJackal", AmongUsExtensions.OptionType.Crewmate, true, Options.CustomRoleSpawnChances[CustomRoles.Deputy]);
            CanKillJShrodingerCat = CustomOption.Create(Id + 20, Color.white, "DeputyCanKillJShrodingerCat", AmongUsExtensions.OptionType.Crewmate, true, Options.CustomRoleSpawnChances[CustomRoles.Deputy]);
            CanKillPlagueBearer = CustomOption.Create(Id + 21, Color.white, "DeputyCanKillPB", AmongUsExtensions.OptionType.Crewmate, true, Options.CustomRoleSpawnChances[CustomRoles.Deputy]);
            CanKillJug = CustomOption.Create(Id + 22, Color.white, "DeputyCanKillJug", AmongUsExtensions.OptionType.Crewmate, true, Options.CustomRoleSpawnChances[CustomRoles.Deputy]);
            DeputyCanKillCoven = CustomOption.Create(Id + 23, Color.white, "DeputyCanKillCoven", AmongUsExtensions.OptionType.Crewmate, true, Options.CustomRoleSpawnChances[CustomRoles.Deputy]);
            CanKillVulture = CustomOption.Create(Id + 24, Color.white, "DeputyCanKillVulture", AmongUsExtensions.OptionType.Crewmate, true, Options.CustomRoleSpawnChances[CustomRoles.Deputy]);
            CanKillGlitch = CustomOption.Create(Id + 25, Color.white, "DCKTG", AmongUsExtensions.OptionType.Crewmate, true, Options.CustomRoleSpawnChances[CustomRoles.Deputy]);
            CanKillWerewolf = CustomOption.Create(Id + 26, Color.white, "DCKWW", AmongUsExtensions.OptionType.Crewmate, true, Options.CustomRoleSpawnChances[CustomRoles.Deputy]);
            CanKillHitman = CustomOption.Create(Id + 29, Color.white, "DeputyCanKillHitman", AmongUsExtensions.OptionType.Crewmate, true, Options.CustomRoleSpawnChances[CustomRoles.Deputy]);
            CanKillAgitater = CustomOption.Create(Id + 30, Color.white, "DeputyCanKillAgitater", AmongUsExtensions.OptionType.Crewmate, true, Options.CustomRoleSpawnChances[CustomRoles.Deputy]);
            CanKillPostman = CustomOption.Create(Id + 32, Color.white, "DeputyCanKillPostman", AmongUsExtensions.OptionType.Crewmate, true, Options.CustomRoleSpawnChances[CustomRoles.Deputy]);
            CanKillClumsy = CustomOption.Create(Id + 33, Color.white, "DeputyCanKillClumsy", AmongUsExtensions.OptionType.Crewmate, true, Options.CustomRoleSpawnChances[CustomRoles.Deputy]);
            CanKillCrewmatesAsIt = CustomOption.Create(Id + 27, Color.white, "DeputyCanKillCrewmatesAsIt", AmongUsExtensions.OptionType.Crewmate, false, Options.CustomRoleSpawnChances[CustomRoles.Deputy]);
            NoDeathPenalty = CustomOption.Create(Id + 31, Color.white, "NoDeathPenalty", AmongUsExtensions.OptionType.Crewmate, false, Options.CustomRoleSpawnChances[CustomRoles.Deputy]);
            ShotLimitOpt = CustomOption.Create(Id + 28, Color.white, "DeputyShotLimit", AmongUsExtensions.OptionType.Crewmate, 15, 0, 15, 1, Options.CustomRoleSpawnChances[CustomRoles.Deputy]);
        }
        public static void Init()
        {
            playerIdList = new();
            ShotLimit = new();
            CurrentKillCooldown = new();
            seer = null;
            var number = System.Convert.ToUInt32(PercentageChecker.CheckPercentage(CustomRoles.CorruptedSheriff.ToString(), role: CustomRoles.CorruptedSheriff));
            cDeputy = UnityEngine.Random.Range(1, 100) <= number;
        }
        public static void Add(byte playerId)
        {
            playerIdList.Add(playerId);
            CurrentKillCooldown.Add(playerId, KillCooldown.GetFloat());

            if (!Main.ResetCamPlayerList.Contains(playerId))
                Main.ResetCamPlayerList.Add(playerId);

            ShotLimit.TryAdd(playerId, ShotLimitOpt.GetFloat());
            Logger.Info($"{Utils.GetPlayerById(playerId)?.GetNameWithRole()} : 残り{ShotLimit[playerId]}発", "Deputy");
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
            byte DeputyId = reader.ReadByte();
            float Limit = reader.ReadSingle();
            if (ShotLimit.ContainsKey(DeputyId))
                ShotLimit[DeputyId] = Limit;
            else
                ShotLimit.Add(DeputyId, ShotLimitOpt.GetFloat());
        }
        public static void SetKillCooldown(byte id) => Main.AllPlayerKillCooldown[id] = CurrentKillCooldown[id];
        public static bool CanUseKillButton(PlayerControl player)
        {
            if (player.Data.IsDead)
                return false;

            if (ShotLimitOpt.GetFloat() == 0)
                return true;

            if (ShotLimit[player.PlayerId] == 0)
            {
                //Logger.info($"{player.GetNameWithRole()} はキル可能回数に達したため、RoleTypeを守護天使に変更しました。", "Deputy");
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
                    Logger.Info($"{killer.GetNameWithRole()} : 残り{ShotLimit[killer.PlayerId]}発", "Deputy");
                    SendRPC(killer.PlayerId);
                    //SwitchToCorrupt(killer, target);
                    break;
                case "Suicide":
                    if (!target.CanBeKilledByDeputy())
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
        public static void SwitchToCorrupt(PlayerControl killer, PlayerControl target)
        {
            try
            {
                if (!cDeputy)
                    if ((target.GetCustomRole().IsImpostor() | target.GetCustomRole().IsNeutralKilling() | target.GetCustomRole().IsCoven() | Main.AliveImpostorCount <= 0) && CustomRoles.CorruptedSheriff.IsEnable())
                    {
                        if (Options.SheriffCorrupted.GetBool())
                        {
                            if (!cDeputy)
                            {
                                int IsAlive = 0;
                                int numCovenAlive = 0;
                                int numImpsAlive = 0;
                                int numNKalive = 0;
                                List<PlayerControl> couldBeTraitors = new();
                                List<byte> couldBeTraitorsid = new();
                                var rando = new System.Random();
                                foreach (var pc in PlayerControl.AllPlayerControls)
                                {
                                    if (pc == null) continue;
                                    if (!pc.Data.Disconnected)
                                        if (!pc.Data.IsDead)
                                        {
                                            IsAlive++;
                                            if (pc.GetCustomRole().IsNeutralKilling() && !Options.TraitorCanSpawnIfNK.GetBool())
                                                numNKalive++;
                                            if (pc.GetCustomRole().IsCoven() && !Options.TraitorCanSpawnIfCoven.GetBool())
                                                numCovenAlive++;
                                            if (pc.Is(CustomRoles.Deputy) || pc.Is(CustomRoles.Investigator) || pc.Is(CustomRoles.Hitman))
                                                couldBeTraitors.Add(pc);
                                            if (pc.Is(CustomRoles.Deputy) || pc.Is(CustomRoles.Investigator) || pc.Is(CustomRoles.Hitman))
                                                couldBeTraitorsid.Add(pc.PlayerId);
                                            if (pc.GetCustomRole().IsImpostor())
                                                numImpsAlive++;
                                        }
                                }

                                foreach (var pc in PlayerControl.AllPlayerControls)
                                {
                                    if (pc == null) continue;
                                    if (!pc.Data.Disconnected)
                                        if (!pc.Data.IsDead)
                                        {
                                            if (!pc.IsModClient()) continue;
                                            if (!pc.GetCustomRole().IsCrewmate()) continue;
                                            if (!couldBeTraitorsid.Contains(pc.PlayerId))
                                            {
                                                couldBeTraitors.Add(pc);
                                                couldBeTraitorsid.Add(pc.PlayerId);
                                            }
                                        }
                                }

                                seer = couldBeTraitors[rando.Next(0, couldBeTraitors.Count)];

                                //foreach (var pva in __instance.playerStates)
                                if (IsAlive >= Options.PlayersForTraitor.GetFloat() && seer != null)
                                {
                                    if (seer.GetCustomRole() == CustomRoles.Deputy && numCovenAlive == 0 && numNKalive == 0 && numImpsAlive == 0)
                                    {
                                        seer.RpcSetCustomRole(CustomRoles.CorruptedSheriff);
                                        seer.CustomSyncSettings();
                                        cDeputy = true;
                                        RPC.SetTraitor(seer.PlayerId);
                                    }
                                }
                            }
                        }
                    }
            }
            catch (Exception e)
            {
                Logger.Error($"Error encountered while checking if Deputy could turn into corrupt.\n{e}", "Deputy.cs");
            }
        }

        public static string GetShotLimit(byte playerId)
        {
            if (ShotLimitOpt.GetInt() == 0) return "";
            return Helpers.ColorString(Color.yellow, ShotLimit.TryGetValue(playerId, out var shotLimit) ? $"({shotLimit})" : "Invalid");
        }
        public static bool CanBeKilledByDeputy(this PlayerControl player)
        {
            var cRole = player.GetCustomRole();
            return cRole switch
            {
                CustomRoles.Jester => CanKillJester.GetBool(),
                CustomRoles.Terrorist => CanKillTerrorist.GetBool(),
                CustomRoles.CrewPostor => CanKillTerrorist.GetBool(),
                CustomRoles.Executioner => CanKillExecutioner.GetBool(),
                CustomRoles.Swapper => CanKillExecutioner.GetBool(),
                CustomRoles.Opportunist => CanKillOpportunist.GetBool(),
                CustomRoles.Survivor => CanKillOpportunist.GetBool(),
                CustomRoles.Arsonist => CanKillArsonist.GetBool(),
                CustomRoles.Egoist => CanKillEgoist.GetBool(),
                CustomRoles.EgoSchrodingerCat => CanKillEgoShrodingerCat.GetBool(),
                CustomRoles.Jackal => CanKillJackal.GetBool(),
                CustomRoles.Sidekick => CanKillJackal.GetBool(),
                CustomRoles.JSchrodingerCat => CanKillJShrodingerCat.GetBool(),
                CustomRoles.PlagueBearer => CanKillPlagueBearer.GetBool(),
                CustomRoles.Juggernaut => CanKillJug.GetBool(),
                CustomRoles.Marksman => CanKillJug.GetBool(),
                CustomRoles.Hitman => CanKillHitman.GetBool(),
                CustomRoles.BloodKnight => CanKillJug.GetBool(),
                CustomRoles.Vulture => CanKillVulture.GetBool(),
                CustomRoles.TheGlitch => CanKillGlitch.GetBool(),
                CustomRoles.Werewolf => CanKillWerewolf.GetBool(),
                CustomRoles.AgiTater => CanKillAgitater.GetBool(),
                CustomRoles.Clumsy => CanKillClumsy.GetBool(),
                CustomRoles.Pirate => true,
                CustomRoles.Dracula => true,
                CustomRoles.Wraith => false,
                CustomRoles.Magician => true,
                CustomRoles.TemplateRole => true,
                CustomRoles.Occultist => true,
                CustomRoles.Unseeable => true,
                // COVEN //
                CustomRoles.Coven => DeputyCanKillCoven.GetBool(),
                CustomRoles.CovenWitch => DeputyCanKillCoven.GetBool(),
                CustomRoles.Poisoner => DeputyCanKillCoven.GetBool(),
                CustomRoles.HexMaster => DeputyCanKillCoven.GetBool(),
                CustomRoles.PotionMaster => DeputyCanKillCoven.GetBool(),
                CustomRoles.Medusa => DeputyCanKillCoven.GetBool(),
                CustomRoles.Mimic => DeputyCanKillCoven.GetBool(),
                CustomRoles.Necromancer => DeputyCanKillCoven.GetBool(),
                CustomRoles.Conjuror => DeputyCanKillCoven.GetBool(),
                // AFTER COVEN //
                CustomRoles.SchrodingerCat => true,
                CustomRoles.Phantom => true,
                CustomRoles.Hacker => true,
                CustomRoles.NeutWitch => true,
                _ => cRole.GetRoleType() switch
                {
                    RoleType.Impostor => true,
                    RoleType.Madmate => CanKillMadmate.GetBool(),
                    _ => false,
                }
            };
        }
    }
}