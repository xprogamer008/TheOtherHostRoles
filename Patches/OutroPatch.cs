using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using static TownOfHost.Translator;
using TownOfHost.PrivateExtensions;

namespace TownOfHost
{
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
    class EndGamePatch
    {
        public static void Postfix(AmongUsClient __instance, [HarmonyArgument(0)] ref EndGameResult endGameResult)
        {
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            GameStates.InGame = false;

            Logger.Info("-----------ゲーム終了-----------", "Phase");
            GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown = Options.DefaultKillCooldown;
            //winnerListリセット
            TempData.winners = new Il2CppSystem.Collections.Generic.List<WinningPlayerData>();
            Main.additionalwinners = new HashSet<AdditionalWinners>();
            var winner = new List<PlayerControl>();
            //勝者リスト作成
            if (GameManager.Instance.DidHumansWin(endGameResult.GameOverReason) || endGameResult.GameOverReason.Equals(GameOverReason.HumansByTask) || endGameResult.GameOverReason.Equals(GameOverReason.HumansByVote))
            {
                if (Main.currentWinner == CustomWinner.Default)
                {
                    Main.currentWinner = CustomWinner.Crewmate;
                }
                foreach (var p in PlayerControl.AllPlayerControls)
                {
                    //if (p.GetCustomSubRole() == CustomRoles.LoversRecode) continue;
                    bool canWin = p.Is(RoleType.Crewmate) || p.Is(CustomRoles.Communist);
                    if (canWin) winner.Add(p);
                    if (canWin) p.RpcSetRole(AmongUs.GameOptions.RoleTypes.CrewmateGhost);
                    if (!canWin) p.RpcSetRole(AmongUs.GameOptions.RoleTypes.ImpostorGhost);
                }
            }
            if (GameManager.Instance.DidImpostorsWin(endGameResult.GameOverReason))
            {
                int deathAmount = 0;
                if (Main.currentWinner == CustomWinner.Default)
                    Main.currentWinner = CustomWinner.Impostor;
                foreach (var p in PlayerControl.AllPlayerControls)
                {
                    //if (p.GetCustomSubRole() == CustomRoles.LoversRecode) continue;
                    bool canWin = p.Is(RoleType.Impostor) || p.Is(RoleType.Madmate) || p.Is(CustomRoles.CrewPostor) || p.Is(CustomRoles.CPSchrodingerCat) || p.Is(CustomRoles.CorruptedSheriff) || p.Is(CustomRoles.Communist) || p.Is(CustomRoles.IMPCat);
                    if (canWin) winner.Add(p);
                    if (canWin) p.RpcSetRole(AmongUs.GameOptions.RoleTypes.ImpostorGhost);
                    if (!canWin) p.RpcSetRole(AmongUs.GameOptions.RoleTypes.CrewmateGhost);
                    if (p.Data.IsDead | PlayerState.isDead[p.PlayerId]) deathAmount++;
                }
                Egoist.OverrideCustomWinner(deathAmount);
            }
            if (Options.CurrentGameMode() != CustomGameMode.HideAndSeek)
            {
                if (Main.currentWinner == CustomWinner.Jackal)
                {
                    winner.Clear();
                    foreach (var p in PlayerControl.AllPlayerControls)
                    {
                        if (p.GetCustomRole().IsJackalTeam()) winner.Add(p);
                        if (p.GetCustomRole().IsJackalTeam()) p.RpcSetRole(AmongUs.GameOptions.RoleTypes.ImpostorGhost);
                        if (!p.GetCustomRole().IsJackalTeam()) p.RpcSetRole(AmongUs.GameOptions.RoleTypes.CrewmateGhost);
                    }
                }
            }
            else
            {
                if (Options.FreeForAllOn.GetBool())
                {
                    if (Main.currentWinner == CustomWinner.Jackal)
                    {
                        winner.Clear();
                        foreach (var p in PlayerControl.AllPlayerControls)
                        {
                            if (p.PlayerId == Main.WonFFAid || p.CurrentOutfit.ColorId == Main.WonFFATeam) winner.Add(p);
                        }
                    }
                }
            }
            if (Main.currentWinner == CustomWinner.Werewolf)
            {
                winner.Clear();
                foreach (var p in PlayerControl.AllPlayerControls)
                {
                    if (p.Is(CustomRoles.Werewolf) | p.Is(CustomRoles.WWSchrodingerCat) | p.Is(CustomRoles.WWCat)) winner.Add(p);
                    if (p.Is(CustomRoles.Werewolf) | p.Is(CustomRoles.WWSchrodingerCat) | p.Is(CustomRoles.WWCat)) p.RpcSetRole(AmongUs.GameOptions.RoleTypes.ImpostorGhost);
                    if (!p.Is(CustomRoles.Werewolf) | !p.Is(CustomRoles.WWSchrodingerCat) | !p.Is(CustomRoles.WWCat)) p.RpcSetRole(AmongUs.GameOptions.RoleTypes.CrewmateGhost);
                }
            }
            if (Main.currentWinner == CustomWinner.AgiTater)
            {
                winner.Clear();
                foreach (var p in PlayerControl.AllPlayerControls)
                {
                    if (p.Is(CustomRoles.AgiTater)) winner.Add(p);
                    if (p.Is(CustomRoles.AgiTater)) p.RpcSetRole(AmongUs.GameOptions.RoleTypes.ImpostorGhost);
                    if (!p.Is(CustomRoles.AgiTater)) p.RpcSetRole(AmongUs.GameOptions.RoleTypes.CrewmateGhost);
                }
            }
            if (Main.currentWinner == CustomWinner.Marksman)
            {
                winner.Clear();
                foreach (var p in PlayerControl.AllPlayerControls)
                {
                    if (p.Is(CustomRoles.Marksman) | p.Is(CustomRoles.MMSchrodingerCat) | p.Is(CustomRoles.MMCat)) winner.Add(p);
                    if (p.Is(CustomRoles.Marksman) | p.Is(CustomRoles.MMSchrodingerCat) | p.Is(CustomRoles.MMCat)) p.RpcSetRole(AmongUs.GameOptions.RoleTypes.ImpostorGhost);
                    if (!p.Is(CustomRoles.Marksman) | !p.Is(CustomRoles.MMSchrodingerCat) | !p.Is(CustomRoles.MMCat)) p.RpcSetRole(AmongUs.GameOptions.RoleTypes.CrewmateGhost);
                }
            }
            if (Main.currentWinner == CustomWinner.Phantom)
            {
                winner.Clear();
                foreach (var p in PlayerControl.AllPlayerControls)
                {
                    if (p.PlayerId == Main.WonTrollID) winner.Add(p);
                }
            }
            if (Main.currentWinner == CustomWinner.Postman)
            {
                winner.Clear();
                foreach (var p in PlayerControl.AllPlayerControls)
                {
                    if (p.PlayerId == Main.WonTrollID) winner.Add(p);
                }
            }
            if (Main.currentWinner == CustomWinner.TheGlitch)
            {
                winner.Clear();
                foreach (var p in PlayerControl.AllPlayerControls)
                {
                    if (p.Is(CustomRoles.TheGlitch) | p.Is(CustomRoles.TGSchrodingerCat) | p.Is(CustomRoles.TGCat)) winner.Add(p);
                    if (p.Is(CustomRoles.TheGlitch) | p.Is(CustomRoles.TGSchrodingerCat) | p.Is(CustomRoles.TGCat)) p.RpcSetRole(AmongUs.GameOptions.RoleTypes.ImpostorGhost);
                    if (!p.Is(CustomRoles.TheGlitch) | !p.Is(CustomRoles.TGSchrodingerCat) | !p.Is(CustomRoles.TGCat)) p.RpcSetRole(AmongUs.GameOptions.RoleTypes.CrewmateGhost);
                }
            }
            if (Main.currentWinner == CustomWinner.Vulture)
            {
                winner.Clear();
                foreach (var p in PlayerControl.AllPlayerControls)
                {
                    if (p.Is(CustomRoles.Vulture)) winner.Add(p);
                    if (p.Is(CustomRoles.Vulture)) p.RpcSetRole(AmongUs.GameOptions.RoleTypes.ImpostorGhost);
                    if (!p.Is(CustomRoles.Vulture)) p.RpcSetRole(AmongUs.GameOptions.RoleTypes.CrewmateGhost);
                }
            }
            if (Main.currentWinner == CustomWinner.Dracula)
            {
                winner.Clear();
                foreach (var p in PlayerControl.AllPlayerControls)
                {
                    if (p.Is(CustomRoles.Dracula) | p.Is(CustomRoles.DRSchrodingerCat) | p.Is(CustomRoles.DRCat)) winner.Add(p);
                    if (p.Is(CustomRoles.Dracula) | p.Is(CustomRoles.DRSchrodingerCat) | p.Is(CustomRoles.DRCat)) p.RpcSetRole(AmongUs.GameOptions.RoleTypes.ImpostorGhost);
                    if (!p.Is(CustomRoles.Dracula) | !p.Is(CustomRoles.DRSchrodingerCat) | !p.Is(CustomRoles.DRCat)) p.RpcSetRole(AmongUs.GameOptions.RoleTypes.CrewmateGhost);
                }
            }
            if (Main.currentWinner == CustomWinner.Wraith)
            {
                winner.Clear();
                foreach (var p in PlayerControl.AllPlayerControls)
                {
                    if (p.Is(CustomRoles.Wraith) | p.Is(CustomRoles.WRASchrodingerCat) | p.Is(CustomRoles.WRACat)) winner.Add(p);
                    if (p.Is(CustomRoles.Wraith) | p.Is(CustomRoles.WRASchrodingerCat) | p.Is(CustomRoles.WRACat)) p.RpcSetRole(AmongUs.GameOptions.RoleTypes.ImpostorGhost);
                    if (!p.Is(CustomRoles.Wraith) | !p.Is(CustomRoles.WRASchrodingerCat) | !p.Is(CustomRoles.WRACat)) p.RpcSetRole(AmongUs.GameOptions.RoleTypes.CrewmateGhost);
                }
            }
            if (Main.currentWinner == CustomWinner.SerialNeutKiller)
            {
                winner.Clear();
                foreach (var p in PlayerControl.AllPlayerControls)
                {
                    if (p.Is(CustomRoles.SerialNeutKiller) | p.Is(CustomRoles.SNKSchrodingerCat) | p.Is(CustomRoles.SnkCat)) winner.Add(p);
                    if (p.Is(CustomRoles.SerialNeutKiller) | p.Is(CustomRoles.SNKSchrodingerCat) | p.Is(CustomRoles.SnkCat)) p.RpcSetRole(AmongUs.GameOptions.RoleTypes.ImpostorGhost);
                    if (!p.Is(CustomRoles.SerialNeutKiller) | !p.Is(CustomRoles.SNKSchrodingerCat) | !p.Is(CustomRoles.SnkCat)) p.RpcSetRole(AmongUs.GameOptions.RoleTypes.CrewmateGhost);
                }
            }
            if (Main.currentWinner == CustomWinner.TemplateRole)
            {
                winner.Clear();
                foreach (var p in PlayerControl.AllPlayerControls)
                {
                    if (p.Is(CustomRoles.TemplateRole) | p.Is(CustomRoles.TEMSchrodingerCat) | p.Is(CustomRoles.TEMCat)) winner.Add(p);
                    if (p.Is(CustomRoles.TemplateRole) | p.Is(CustomRoles.TEMSchrodingerCat) | p.Is(CustomRoles.TEMCat)) p.RpcSetRole(AmongUs.GameOptions.RoleTypes.ImpostorGhost);
                    if (!p.Is(CustomRoles.TemplateRole) | !p.Is(CustomRoles.TEMSchrodingerCat) | !p.Is(CustomRoles.TEMCat)) p.RpcSetRole(AmongUs.GameOptions.RoleTypes.CrewmateGhost);
                }
            }
            if (Main.currentWinner == CustomWinner.Retributionist)
            {
                winner.Clear();
                foreach (var p in PlayerControl.AllPlayerControls)
                {
                    if (p.Is(CustomRoles.Retributionist) | p.Is(CustomRoles.RETSchrodingerCat) | p.Is(CustomRoles.ResurectedCREW) | p.Is(CustomRoles.ResurectedIMP) | p.Is(CustomRoles.ResurectedNEU) | p.Is(CustomRoles.RETCat)) winner.Add(p);
                    if (p.Is(CustomRoles.Retributionist) | p.Is(CustomRoles.RETSchrodingerCat) | p.Is(CustomRoles.ResurectedCREW) | p.Is(CustomRoles.ResurectedIMP) | p.Is(CustomRoles.ResurectedNEU) | p.Is(CustomRoles.RETCat)) p.RpcSetRole(AmongUs.GameOptions.RoleTypes.ImpostorGhost);
                    if (!p.Is(CustomRoles.Retributionist) | !p.Is(CustomRoles.RETSchrodingerCat) | !p.Is(CustomRoles.ResurectedCREW) | !p.Is(CustomRoles.ResurectedIMP) | !p.Is(CustomRoles.ResurectedNEU) | !p.Is(CustomRoles.RETCat)) p.RpcSetRole(AmongUs.GameOptions.RoleTypes.CrewmateGhost);
                }
            }
            if (Main.currentWinner == CustomWinner.Occultist)
            {
                winner.Clear();
                foreach (var p in PlayerControl.AllPlayerControls)
                {
                    if (p.Is(CustomRoles.Occultist) | p.Is(CustomRoles.OCCSchrodingerCat) | p.Is(CustomRoles.OCCCat)) winner.Add(p);
                    if (p.Is(CustomRoles.Occultist) | p.Is(CustomRoles.OCCSchrodingerCat) | p.Is(CustomRoles.OCCCat)) p.RpcSetRole(AmongUs.GameOptions.RoleTypes.ImpostorGhost);
                    if (!p.Is(CustomRoles.Occultist) | !p.Is(CustomRoles.OCCSchrodingerCat) | !p.Is(CustomRoles.OCCCat)) p.RpcSetRole(AmongUs.GameOptions.RoleTypes.CrewmateGhost);
                }
            }
            if (Main.currentWinner == CustomWinner.Magician)
            {
                winner.Clear();
                foreach (var p in PlayerControl.AllPlayerControls)
                {
                    if (p.Is(CustomRoles.Magician) | p.Is(CustomRoles.MAGSchrodingerCat)) winner.Add(p);
                    if (p.Is(CustomRoles.Magician) | p.Is(CustomRoles.MAGSchrodingerCat)) p.RpcSetRole(AmongUs.GameOptions.RoleTypes.ImpostorGhost);
                    if (!p.Is(CustomRoles.Magician) | !p.Is(CustomRoles.MAGSchrodingerCat)) p.RpcSetRole(AmongUs.GameOptions.RoleTypes.CrewmateGhost);
                }
            }
            if (Main.currentWinner == CustomWinner.Unseeable)
            {
                winner.Clear();
                foreach (var p in PlayerControl.AllPlayerControls)
                {
                    if (p.Is(CustomRoles.Unseeable) | p.Is(CustomRoles.UNSchrodingerCat) | p.Is(CustomRoles.UNCat)) winner.Add(p);
                    if (p.Is(CustomRoles.Unseeable) | p.Is(CustomRoles.UNSchrodingerCat) | p.Is(CustomRoles.UNCat)) p.RpcSetRole(AmongUs.GameOptions.RoleTypes.ImpostorGhost);
                    if (!p.Is(CustomRoles.Unseeable) | !p.Is(CustomRoles.UNSchrodingerCat) | !p.Is(CustomRoles.UNCat)) p.RpcSetRole(AmongUs.GameOptions.RoleTypes.CrewmateGhost);
                }
            }
            if (Main.currentWinner == CustomWinner.BloodKnight)
            {
                winner.Clear();
                foreach (var p in PlayerControl.AllPlayerControls)
                {
                    if (p.Is(CustomRoles.BloodKnight) | p.Is(CustomRoles.BKSchrodingerCat) | p.Is(CustomRoles.BKCat)) winner.Add(p);
                    if (p.Is(CustomRoles.BloodKnight) | p.Is(CustomRoles.BKSchrodingerCat) | p.Is(CustomRoles.BKCat)) p.RpcSetRole(AmongUs.GameOptions.RoleTypes.ImpostorGhost);
                    if (!p.Is(CustomRoles.BloodKnight) | !p.Is(CustomRoles.BKSchrodingerCat) | !p.Is(CustomRoles.BKCat)) p.RpcSetRole(AmongUs.GameOptions.RoleTypes.CrewmateGhost);
                }
            }
            if (Main.currentWinner == CustomWinner.Pestilence)
            {
                winner.Clear();
                foreach (var p in PlayerControl.AllPlayerControls)
                {
                    if (p.Is(CustomRoles.Pestilence) || p.Is(CustomRoles.PlagueBearer) | p.Is(CustomRoles.PesSchrodingerCat) | p.Is(CustomRoles.PesCat)) winner.Add(p);
                    if (p.Is(CustomRoles.Pestilence) | p.Is(CustomRoles.PesSchrodingerCat) | p.Is(CustomRoles.PesCat)) p.RpcSetRole(AmongUs.GameOptions.RoleTypes.ImpostorGhost);
                    if (!p.Is(CustomRoles.Pestilence) | !p.Is(CustomRoles.PesSchrodingerCat) | !p.Is(CustomRoles.PesCat)) p.RpcSetRole(AmongUs.GameOptions.RoleTypes.CrewmateGhost);
                }
            }
            if (Main.currentWinner == CustomWinner.Pirate && CustomRoles.Pirate.IsEnable())
            {
                winner = new();
                foreach (var p in PlayerControl.AllPlayerControls)
                {
                    if (p.PlayerId == Main.WonPirateID)
                    {
                        winner.Add(p);
                        if (p.Is(CustomRoles.Pirate)) p.RpcSetRole(AmongUs.GameOptions.RoleTypes.ImpostorGhost);
                        if (!p.Is(CustomRoles.Pirate)) p.RpcSetRole(AmongUs.GameOptions.RoleTypes.CrewmateGhost);
                    }
                }
            }
            if (Main.currentWinner == CustomWinner.Juggernaut)
            {
                winner.Clear();
                foreach (var p in PlayerControl.AllPlayerControls)
                {
                    if (p.Is(CustomRoles.Juggernaut) | p.Is(CustomRoles.JugSchrodingerCat) | p.Is(CustomRoles.JugCat)) winner.Add(p);
                    if (p.Is(CustomRoles.Juggernaut) | p.Is(CustomRoles.JugSchrodingerCat) | p.Is(CustomRoles.JugCat)) p.RpcSetRole(AmongUs.GameOptions.RoleTypes.ImpostorGhost);
                    if (!p.Is(CustomRoles.Juggernaut) | !p.Is(CustomRoles.JugSchrodingerCat) | !p.Is(CustomRoles.JugCat)) p.RpcSetRole(AmongUs.GameOptions.RoleTypes.CrewmateGhost);
                }
            }
            if (Main.currentWinner == CustomWinner.Coven)
            {
                winner.Clear();
                foreach (var p in PlayerControl.AllPlayerControls)
                {
                    if (CustomRolesHelper.IsCoven(p.GetCustomRole())) winner.Add(p);
                }
            }
            if (Main.currentWinner == CustomWinner.DisconnectError)
            {
                winner = new List<PlayerControl>();
                foreach (var p in PlayerControl.AllPlayerControls)
                {
                    winner.Add(p);
                }
            }

            //廃村時の処理など
            if (endGameResult.GameOverReason == GameOverReason.HumansDisconnect ||
            endGameResult.GameOverReason == GameOverReason.ImpostorDisconnect ||
            Main.currentWinner == CustomWinner.Draw)
            {
                winner = new List<PlayerControl>();
                foreach (var p in PlayerControl.AllPlayerControls)
                {
                    winner.Add(p);
                }
            }

            //単独勝利
            if (Main.currentWinner == CustomWinner.Jester && CustomRoles.Jester.IsEnable())
            { //Jester単独勝利
                winner = new();
                foreach (var p in PlayerControl.AllPlayerControls)
                {
                    if (p.PlayerId == Main.ExiledJesterID)
                    {
                        winner.Add(p);
                        if (p.Is(CustomRoles.Jester)) p.RpcSetRole(AmongUs.GameOptions.RoleTypes.ImpostorGhost);
                        if (!p.Is(CustomRoles.Jester)) p.RpcSetRole(AmongUs.GameOptions.RoleTypes.CrewmateGhost);
                    }
                }
            }

            if (Main.currentWinner == CustomWinner.Terrorist && CustomRoles.Terrorist.IsEnable())
            { //Terrorist単独勝利
                winner = new();
                foreach (var p in PlayerControl.AllPlayerControls)
                {
                    if (p.PlayerId == Main.WonTerroristID)
                    {
                        winner.Add(p);
                        if (p.Is(CustomRoles.Terrorist)) p.RpcSetRole(AmongUs.GameOptions.RoleTypes.ImpostorGhost);
                        if (!p.Is(CustomRoles.Terrorist)) p.RpcSetRole(AmongUs.GameOptions.RoleTypes.CrewmateGhost);
                    }
                }
            }
            /* if (CustomRoles.LoversRecode.IsEnable() && Options.CurrentGameMode() == CustomGameMode.Standard && Main.LoversPlayers.Count > 0 && Main.LoversPlayers.ToArray().All(p => !p.Data.IsDead) //ラバーズが生きていて
             && (Main.currentWinner == CustomWinner.Impostor || Main.currentWinner == CustomWinner.Jackal || Main.currentWinner == CustomWinner.Vulture
             || (Main.currentWinner == CustomWinner.Crewmate && !endGameResult.GameOverReason.Equals(GameOverReason.HumansByTask))))   //クルー勝利でタスク勝ちじゃなければ
             { //Loversの単独勝利
                 winner = new();
                 Main.currentWinner = CustomWinner.Lovers;
                 foreach (var lp in Main.LoversPlayers)
                 {
                     winner.Add(lp);
                 }
             }*/
            if (CustomRoles.LoversRecode.IsEnable() && Main.currentWinner == CustomWinner.Lovers)
            {
                winner = new();
                Main.currentWinner = CustomWinner.Lovers;
                foreach (var lp in Main.LoversPlayers)
                {
                    winner.Add(lp);
                }
            }
            if (Main.currentWinner == CustomWinner.Executioner | Main.currentWinner == CustomWinner.Swapper && CustomRoles.Executioner.IsEnable() | CustomRoles.Swapper.IsEnable())
            { //Executioner単独勝利
                winner = new();
                foreach (var p in PlayerControl.AllPlayerControls)
                {
                    if (p.PlayerId == Main.WonExecutionerID)
                    {
                        winner.Add(p);
                        if (p.Is(CustomRoles.Executioner)) p.RpcSetRole(AmongUs.GameOptions.RoleTypes.ImpostorGhost);
                        if (!p.Is(CustomRoles.Executioner)) p.RpcSetRole(AmongUs.GameOptions.RoleTypes.CrewmateGhost);
                    }
                }
            }
            if (Main.currentWinner == CustomWinner.Hacker && CustomRoles.Hacker.IsEnable())
            { // HACKER //
                winner = new();
                foreach (var p in PlayerControl.AllPlayerControls)
                {
                    if (p.PlayerId == Main.WonHackerID)
                    {
                        winner.Add(p);
                        if (p.Is(CustomRoles.Hacker)) p.RpcSetRole(AmongUs.GameOptions.RoleTypes.ImpostorGhost);
                        if (!p.Is(CustomRoles.Hacker)) p.RpcSetRole(AmongUs.GameOptions.RoleTypes.CrewmateGhost);
                    }
                }
            }
            if (Main.currentWinner == CustomWinner.Child && CustomRoles.Child.IsEnable())
            {
                winner = new();
                foreach (var p in PlayerControl.AllPlayerControls)
                {
                    if (p.PlayerId == Main.WonChildID)
                    {
                        winner.Add(p);
                    }
                }
            }
            if (Main.currentWinner == CustomWinner.CrewmateDisconnected)
            {
                winner = new();
                foreach (var p in PlayerControl.AllPlayerControls)
                {
                    if (p.Is(RoleType.Crewmate) | p.Is(RoleType.Impostor) | p.Is(RoleType.Madmate) | p.Is(RoleType.Neutral) | p.Is(RoleType.Coven)) winner.Add(p);

                }
            }
            if (Main.currentWinner == CustomWinner.Dead)
            {
                winner = new();
                foreach (var p in PlayerControl.AllPlayerControls)
                {

                }
            }
            if (Main.currentWinner == CustomWinner.Troll && CustomRoles.Troll.IsEnable())
            {
                winner = new();
                foreach (var p in PlayerControl.AllPlayerControls)
                {
                    if (p.PlayerId == Main.WonTrollID)
                    {
                        winner.Add(p);
                    }
                }
            }
            if (Main.currentWinner == CustomWinner.Arsonist && CustomRoles.Arsonist.IsEnable())
            {
                winner = new();
                foreach (var p in PlayerControl.AllPlayerControls)
                {
                    if (p.Is(CustomRoles.Arsonist))
                    {
                        winner.Add(p);
                        if (p.Is(CustomRoles.Arsonist)) p.RpcSetRole(AmongUs.GameOptions.RoleTypes.ImpostorGhost);
                        if (!p.Is(CustomRoles.Arsonist)) p.RpcSetRole(AmongUs.GameOptions.RoleTypes.CrewmateGhost);
                    }
                }
            }
            if (TeamEgoist.EgoistWin)
                TeamEgoist.SoloWin(winner);
            ///以降追加勝利陣営 (winnerリセット無し)
            //Opportunist
            var winnerIDs = new List<byte>();
            foreach (var pc in PlayerControl.AllPlayerControls)
            {
                if (pc.Is(CustomRoles.Opportunist) && !pc.Data.IsDead | Main.AliveAtTheEndOfTheRound.Contains(pc.PlayerId) && Main.currentWinner != CustomWinner.Draw && Main.currentWinner != CustomWinner.Terrorist && Main.currentWinner != CustomWinner.Child && Main.currentWinner != CustomWinner.Troll && Main.currentWinner != CustomWinner.Jester && Main.currentWinner != CustomWinner.Executioner && Main.currentWinner != CustomWinner.Swapper)
                {
                    winner.Add(pc);
                    Main.additionalwinners.Add(AdditionalWinners.Opportunist);
                }
                if (pc.Is(CustomRoles.NeutWitch) && !pc.Data.IsDead | Main.AliveAtTheEndOfTheRound.Contains(pc.PlayerId) && Main.currentWinner != CustomWinner.Draw && Main.currentWinner != CustomWinner.Terrorist && Main.currentWinner != CustomWinner.Child && Main.currentWinner != CustomWinner.Jester && Main.currentWinner != CustomWinner.Executioner && Main.currentWinner != CustomWinner.Swapper && Main.currentWinner != CustomWinner.Crewmate)
                {
                    winner.Add(pc);
                    Main.additionalwinners.Add(AdditionalWinners.Witch);
                }
                //SchrodingerCat
                if (Options.CanBeforeSchrodingerCatWinTheCrewmate.GetBool())
                    if (pc.Is(CustomRoles.SchrodingerCat) && Main.currentWinner == CustomWinner.Crewmate)
                    {
                        winner.Add(pc);
                        Main.additionalwinners.Add(AdditionalWinners.SchrodingerCat);
                    }
                if (Main.currentWinner == CustomWinner.Jester)
                    foreach (var ExecutionerTarget in Main.ExecutionerTarget)
                    {
                        if (Main.ExiledJesterID == ExecutionerTarget.Value && pc.PlayerId == ExecutionerTarget.Key)
                        {
                            winner.Add(pc);
                            Main.additionalwinners.Add(AdditionalWinners.Executioner);
                        }
                    }
                if (pc.Is(CustomRoles.NeutWitch) && Main.currentWinner != CustomWinner.Crewmate)
                {

                }
            }

            //Undecided
            var winnerIDs2 = new List<byte>();
            foreach (var pc in PlayerControl.AllPlayerControls)
            {
                if (pc.Is(CustomRoles.Undecided) && !pc.Data.IsDead | Main.AliveAtTheEndOfTheRound.Contains(pc.PlayerId) && Main.currentWinner != CustomWinner.Draw && Main.currentWinner != CustomWinner.Terrorist && Main.currentWinner != CustomWinner.Child && Main.currentWinner != CustomWinner.Troll && Main.currentWinner != CustomWinner.Jester && Main.currentWinner != CustomWinner.Executioner && Main.currentWinner != CustomWinner.Swapper)
                {
                    winner.Add(pc);
                    Main.additionalwinners.Add(AdditionalWinners.Undecided);
                }
                if (pc.Is(CustomRoles.NeutWitch) && !pc.Data.IsDead | Main.AliveAtTheEndOfTheRound.Contains(pc.PlayerId) && Main.currentWinner != CustomWinner.Draw && Main.currentWinner != CustomWinner.Terrorist && Main.currentWinner != CustomWinner.Child && Main.currentWinner != CustomWinner.Jester && Main.currentWinner != CustomWinner.Executioner && Main.currentWinner != CustomWinner.Swapper && Main.currentWinner != CustomWinner.Crewmate)
                {
                    winner.Add(pc);
                    Main.additionalwinners.Add(AdditionalWinners.Witch);
                }
                //SchrodingerCat
                if (Options.CanBeforeSchrodingerCatWinTheCrewmate.GetBool())
                    if (pc.Is(CustomRoles.SchrodingerCat) && Main.currentWinner == CustomWinner.Crewmate)
                    {
                        winner.Add(pc);
                        Main.additionalwinners.Add(AdditionalWinners.SchrodingerCat);
                    }
                if (Main.currentWinner == CustomWinner.Jester)
                    foreach (var ExecutionerTarget in Main.ExecutionerTarget)
                    {
                        if (Main.ExiledJesterID == ExecutionerTarget.Value && pc.PlayerId == ExecutionerTarget.Key)
                        {
                            winner.Add(pc);
                            Main.additionalwinners.Add(AdditionalWinners.Executioner);
                        }
                    }
                if (pc.Is(CustomRoles.NeutWitch) && Main.currentWinner != CustomWinner.Crewmate)
                {

                }
            }

            foreach (var p in winner)
            {
                winnerIDs.Add(p.PlayerId);
            }

            foreach (var pc in PlayerControl.AllPlayerControls)
            {
                // if (Main.currentWinner == CustomWinner.None) break;
                if (pc.Is(CustomRoles.GuardianAngelTOU))
                {
                    foreach (var protect in Main.GuardianAngelTarget)
                    {
                        if (winnerIDs.Contains(protect.Value))
                        {
                            winner.Add(pc);
                            Main.additionalwinners.Add(AdditionalWinners.GuardianAngelTOU);
                        }
                    }
                }
                if (pc.Is(CustomRoles.Lawyer))
                {
                    foreach (var protect in Main.LawyerTarget)
                    {
                        if (winnerIDs.Contains(protect.Value))
                        {
                            winner.Add(pc);
                            Main.additionalwinners.Add(AdditionalWinners.Lawyer);
                        }
                    }
                }
                if (pc.Is(CustomRoles.Survivor) && !pc.Data.IsDead | Main.AliveAtTheEndOfTheRound.Contains(pc.PlayerId) && Main.currentWinner != CustomWinner.Draw && Main.currentWinner != CustomWinner.Terrorist && Main.currentWinner != CustomWinner.Child && Main.currentWinner != CustomWinner.Jester && Main.currentWinner != CustomWinner.Executioner && Main.currentWinner != CustomWinner.Swapper)
                {
                    winner.Add(pc);
                    Main.additionalwinners.Add(AdditionalWinners.Survivor);
                }
                if (pc.Is(CustomRoles.Hitman) && !pc.Data.IsDead | Main.AliveAtTheEndOfTheRound.Contains(pc.PlayerId) && Main.currentWinner != CustomWinner.Draw && Main.currentWinner != CustomWinner.Terrorist && Main.currentWinner != CustomWinner.Child)
                {
                    if (Main.currentWinner == CustomWinner.Jester && !Options.HitmanCanWinWithExeJes.GetBool()) continue;
                    if (Main.currentWinner == CustomWinner.Executioner && !Options.HitmanCanWinWithExeJes.GetBool()) continue;
                    if (Main.currentWinner == CustomWinner.Swapper && !Options.HitmanCanWinWithExeJes.GetBool()) continue;
                    if (Main.currentWinner == CustomWinner.Lovers && !Options.HitmanCanWinWithExeJes.GetBool()) continue;
                    winner.Add(pc);
                    Main.additionalwinners.Add(AdditionalWinners.Hitman);
                }
            }

            //HideAndSeek専用
            if (Options.CurrentGameMode() == CustomGameMode.HideAndSeek &&
                Main.currentWinner != CustomWinner.Draw && /*Main.currentWinner != CustomWinner.None &&*/ Main.currentWinner != CustomWinner.Jackal)
            {
                winner = new();
                foreach (var pc in PlayerControl.AllPlayerControls)
                {
                    var hasRole = Main.AllPlayerCustomRoles.TryGetValue(pc.PlayerId, out var role);
                    if (!hasRole) continue;
                    if (role.GetRoleType() == RoleType.Impostor)
                    {
                        if (GameManager.Instance.DidImpostorsWin(endGameResult.GameOverReason))
                            winner.Add(pc);
                    }
                    else if (role.GetRoleType() == RoleType.Crewmate)
                    {
                        if (GameManager.Instance.DidHumansWin(endGameResult.GameOverReason))
                            winner.Add(pc);
                    }
                    else if (role == CustomRoles.HASTroll && pc.Data.IsDead)
                    {
                        //トロールが殺されていれば単独勝ち
                        winner = new()
                        {
                            pc
                        };
                        break;
                    }
                    else if (Main.currentWinner == CustomWinner.Painter && pc.Is(CustomRoles.Painter))
                    {
                        winner.Add(pc);
                    }
                    else if (role == CustomRoles.HASFox && Main.currentWinner != CustomWinner.HASTroll && !pc.Data.IsDead)
                    {
                        winner.Add(pc);
                        Main.additionalwinners.Add(AdditionalWinners.HASFox);
                    }
                    if (Options.SpeedrunGamemode.GetBool())
                    {
                        if (Main.currentWinner == CustomWinner.Tasker)
                        {
                            winner.Clear();
                            foreach (var p in PlayerControl.AllPlayerControls)
                            {
                                if (p.PlayerId == Main.WonFFAid) winner.Add(p);
                            }
                        }
                    }
                }
            }
            Main.winnerList = new List<byte>();
            foreach (var pc in winner)
            {
                if (Main.currentWinner is not CustomWinner.Draw && pc.Is(CustomRoles.GM)) continue;
                TempData.winners.Add(new WinningPlayerData(pc.Data));
                Main.winnerList.Add(pc.PlayerId);
            }

            BountyHunter.ChangeTimer = new();
            Main.BitPlayers = new Dictionary<byte, (byte, float)>();
            Main.isDoused = new Dictionary<(byte, byte), bool>();
            Main.isInfected = new Dictionary<(byte, byte), bool>();
            Main.SilencedPlayer.Clear();
            Main.KillingSpree.Clear();
            Main.ColliderPlayers.Clear();
            Main.KilledDemo.Clear();
            Main.PuppeteerList.Clear();
            Main.WitchList.Clear();
            Main.DeadPlayersThisRound.Clear();
            Main.WitchedList.Clear();
            Main.SilencedPlayer = new List<PlayerControl>();

            NameColorManager.Instance.RpcReset();
            Main.VisibleTasksCount = false;
            if (AmongUsClient.Instance.AmHost)
            {
                Main.RealOptionsData.AsNormalOptions()!.KillCooldown = Options.DefaultKillCooldown;
                PlayerControl.LocalPlayer.RpcSyncSettings(Main.RealOptionsData.ToBytes());
            }
        }
    }
    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.SetEverythingUp))]
    class SetEverythingUpPatch
    {
        public static string LastWinsText = "";

        public static void Postfix(EndGameManager __instance)
        {
            if (!Main.playerVersion.ContainsKey(0)) return;
            if (Main.showEjections)
                GameOptionsManager.Instance.currentNormalGameOptions.ConfirmImpostor = true;
            GameOptionsManager.Instance.CurrentGameOptions = Main.RealOptionsData;

            Main.LastPlayerCustomRoles = new(Main.AllPlayerCustomRoles);
            Main.LastPlayerCustomSubRoles = new(Main.AllPlayerCustomSubRoles);

            GameObject bonusText = UnityEngine.Object.Instantiate(__instance.WinText.gameObject);
            bonusText.transform.position = new Vector3(__instance.WinText.transform.position.x, __instance.WinText.transform.position.y - 0.5f, __instance.WinText.transform.position.z);
            bonusText.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
            TMPro.TMP_Text textRenderer = bonusText.GetComponent<TMPro.TMP_Text>();
            textRenderer.text = "";

            string CustomWinnerText = "";
            string AdditionalWinnerText = "";
            string CustomWinnerColor = Utils.GetRoleColorCode(CustomRoles.Crewmate);

            var winnerRole = (CustomRoles)Main.currentWinner;
            if (winnerRole >= 0)
            {
                CustomWinnerText = Utils.GetRoleName(winnerRole);
                CustomWinnerColor = Utils.GetRoleColorCode(winnerRole);
                if (winnerRole.IsNeutral())
                {
                    __instance.BackgroundBar.material.color = Utils.GetRoleColor(winnerRole);
                }
            }
            if (AmongUsClient.Instance.AmHost && Main.AllPlayerCustomRoles[0] == CustomRoles.GM)
            {
                __instance.WinText.text = "Game Over";
                __instance.WinText.color = Utils.GetRoleColor(CustomRoles.GM);
                __instance.BackgroundBar.material.color = Utils.GetRoleColor(CustomRoles.GM);
            }
            switch (Main.currentWinner)
            {
                //通常勝利
                case CustomWinner.Crewmate:
                    CustomWinnerColor = Utils.GetRoleColorCode(CustomRoles.Engineer);
                    break;
                case CustomWinner.Terrorist:
                    __instance.Foreground.material.color = Color.red;
                    break;
                case CustomWinner.Draw:
                    __instance.WinText.text = GetString("ForceEnd");
                    __instance.WinText.color = Color.white;
                    __instance.BackgroundBar.material.color = Color.gray;
                    textRenderer.text = GetString("ForceEndText");
                    textRenderer.color = Color.gray;
                    break;
                case CustomWinner.Error:
                    __instance.WinText.text = GetString("ErrorEndText");
                    __instance.WinText.color = Color.red;
                    __instance.BackgroundBar.material.color = Color.red;
                    textRenderer.text = GetString("ErrorEndTextDescription");
                    textRenderer.color = Color.white;
                    break;
                case CustomWinner.DisconnectError:
                    __instance.WinText.text = $"Player Disconnected\nBy Error";
                    //    __instance.WinText.color = Color.white;
                    __instance.BackgroundBar.material.color = Utils.GetRoleColor(CustomRoles.Impostor);
                    textRenderer.text = "Game force ended to prevent black screens";
                    //    textRenderer.color = Utils.GetRoleColor(CustomRoles.Marshall);
                    break;
                case CustomWinner.CrewmateDisconnected:
                    __instance.WinText.text = $"Most Crewmates\nDisconnected";
                    //    __instance.WinText.color = Color.white;
                    __instance.BackgroundBar.material.color = Utils.GetRoleColor(CustomRoles.Impostor);
                    //    textRenderer.text = "Game force ended to prevent black screens";
                    //    textRenderer.color = Utils.GetRoleColor(CustomRoles.Marshall);
                    break;
                case CustomWinner.Dead:
                    //    __instance.WinText.text = $"Player Disconnected\nBy Error";
                    //    __instance.WinText.color = Color.white;
                    __instance.BackgroundBar.material.color = Utils.GetRoleColor(CustomRoles.Impostor);
                    textRenderer.text = "Everyone died";
                    textRenderer.color = Utils.GetRoleColor(CustomRoles.Child);
                    break;
            }

            foreach (var additionalwinners in Main.additionalwinners)
            {
                var addWinnerRole = (CustomRoles)additionalwinners;
                AdditionalWinnerText += "＆" + Helpers.ColorString(Utils.GetRoleColor(addWinnerRole), Utils.GetRoleName(addWinnerRole));
            }
            if (Main.currentWinner != CustomWinner.Draw && Main.currentWinner != CustomWinner.DisconnectError && Main.currentWinner != CustomWinner.Dead && Main.currentWinner != CustomWinner.CrewmateDisconnected)
            {
                textRenderer.text = $"<color={CustomWinnerColor}>{CustomWinnerText}{AdditionalWinnerText}{GetString("Win")}</color>";
            }
            Main.LastWinner = textRenderer.text.RemoveHtmlTags();
            LastWinsText = textRenderer.text.RemoveHtmlTags();

            var position = Camera.main.ViewportToWorldPoint(new Vector3(0f, 1f, Camera.main.nearClipPlane));
            GameObject roleSummary = UnityEngine.Object.Instantiate(__instance.WinText.gameObject);
            roleSummary.transform.position = new Vector3(__instance.Navigation.ExitButton.transform.position.x + 0.1f, position.y - 0.1f, -14f);
            roleSummary.transform.localScale = new Vector3(1f, 1f, 1f);

            string roleSummaryText = $"{GetString("RoleSummaryText")}";
            foreach (var key in Main.AllPlayerCustomRoles)
            {
                try
                {
                    var pc = Utils.GetPlayerById(key.Key);
                    roleSummaryText += $"\n{Main.AllPlayerSkin[key.Key].Item6} - ";
                    roleSummaryText += $"{Helpers.ColorString(Utils.GetRoleColor(key.Value), Utils.GetRoleName(key.Value))}";
                    var cSubRoleFound = Main.AllPlayerCustomSubRoles.TryGetValue(key.Key, out var cSubRole);
                    if (cSubRoleFound)
                        if (cSubRole != CustomRoles.NoSubRoleAssigned)
                            roleSummaryText += $"{Helpers.ColorString(Color.white, " (")} {Helpers.ColorString(Utils.GetRoleColor(cSubRole), Utils.GetRoleName(cSubRole))} {Helpers.ColorString(Color.white, ")")}";
                    var deathReasonFound = PlayerState.deathReasons.TryGetValue(key.Key, out var deathReason);
                    if (deathReasonFound && deathReason != PlayerState.DeathReason.etc)
                        roleSummaryText += $" | {GetString("DeathReason." + deathReason.ToString())}";
                    var killCountFound = Main.KillCount.TryGetValue(key.Key, out var killAmt);
                    if (killCountFound && killAmt != 0 && key.Value != CustomRoles.VoteStealer)
                        roleSummaryText += $" [Kill Count: {killAmt}]";
                }
                catch
                {
                    Logger.Error("Error loading last roles.", "Outro Patch (non-winner)");
                    var deathReasonFound = PlayerState.deathReasons.TryGetValue(key.Key, out var deathReason);
                    string more = "";
                    if (deathReasonFound)
                        if (deathReason != PlayerState.DeathReason.etc)
                        {
                            more = "Death Reason: ";
                            more += deathReason.ToString();
                        }
                    roleSummaryText += $"\nError getting some of this player's info. {more}";
                }
            }
            TMPro.TMP_Text roleSummaryTextMesh = roleSummary.GetComponent<TMPro.TMP_Text>();
            roleSummaryTextMesh.alignment = TMPro.TextAlignmentOptions.TopLeft;
            roleSummaryTextMesh.color = Color.white;
            roleSummaryTextMesh.outlineWidth *= 1.2f;
            roleSummaryTextMesh.fontSizeMin = 1.25f;
            roleSummaryTextMesh.fontSizeMax = 1.25f;
            roleSummaryTextMesh.fontSize = 1.25f;

            var roleSummaryTextMeshRectTransform = roleSummaryTextMesh.GetComponent<RectTransform>();
            roleSummaryTextMeshRectTransform.anchoredPosition = new Vector2(position.x + 3.5f, position.y - 0.1f);
            roleSummaryTextMesh.text = roleSummaryText;
        }
    }
}
