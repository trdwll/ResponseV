using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using System.Collections.Generic;
using ResponseV.GTAV;

namespace ResponseV.Callouts.Any
{
    [CalloutInfo("OfficerDown", CalloutProbability.Low)]
    internal sealed class OfficerDown : CalloutBase
    {
        private LHandle m_Pursuit;

        private List<Vehicle> m_Officers = new List<Vehicle>();

        private Enums.CalloutType m_CalloutType;
        
        private bool m_bMultiple;

        public override bool OnBeforeCalloutDisplayed()
        {
            m_bMultiple = Utils.GetRandBool();

            CalloutMessage = "Reports of " + (m_bMultiple ? "Multiple Officers": "an Officer") + " Down";
            CalloutPosition = g_SpawnPoint;

            string aud;
            if (!m_bMultiple)
            {
                m_CalloutType = Utils.GetRandValue(Enums.CalloutType.Shooting, Enums.CalloutType.Stabbing, Enums.CalloutType.Unknown);

                switch (m_CalloutType)
                {
                    default:
                    case Enums.CalloutType.Unknown: aud = LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.OFFICER_DOWN); break;
                    case Enums.CalloutType.Shooting: aud = LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.OFFICER_SHOT); break;
                    case Enums.CalloutType.Stabbing: aud = "CRIME_OFFICER_STABBED";  break;
                }
            }
            else
            {
                m_CalloutType = Enums.CalloutType.Shooting;
                aud = LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.OFFICERS_DOWN);
            }

            Functions.PlayScannerAudioUsingPosition($"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.OFFICERS_REPORT)} {aud} IN_OR_ON_POSITION", g_SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            g_Logger.Log("OfficerDown: Callout accepted");

            Ped suspect;
            Vehicle veh;
            switch (m_CalloutType)
            {
                default:
                case Enums.CalloutType.Unknown:
                case Enums.CalloutType.Stabbing:
                    veh = new Vehicle(Utils.GetRandValue(g_PoliceVehicleModels), g_SpawnPoint);
                    veh.CreateRandomDriver();
                    veh.Driver.Kill();

                    m_Officers.Add(veh);

                    suspect = new Ped(Utils.GetRandValue(g_PedModels), g_SpawnPoint, Utils.GetRandInt(1, 360));

                    if (m_CalloutType == Enums.CalloutType.Stabbing)
                    {
                        suspect.Inventory.GiveNewWeapon(WeaponHash.Knife, 1, true);
                        suspect.Tasks.FightAgainstClosestHatedTarget(30f);
                    }
                    g_Suspects.Add(suspect);
                    break;
                case Enums.CalloutType.Shooting:
                    if (m_bMultiple)
                    {
                        // Spawn suspects
                        for (int i = 0; i < Utils.GetRandInt(1, 5); i++)
                        {
                            Ped sus = new Ped(Utils.GetRandValue(g_PedModels), g_SpawnPoint, Utils.GetRandInt(1, 360));
                            sus.Inventory.GiveNewWeapon(Utils.GetRandValue(g_WeaponList), (short)Utils.GetRandInt(10, 60), true);
                            sus.Tasks.FightAgainstClosestHatedTarget(30f);
                            g_Suspects.Add(sus);
                        }

                        // Spawn police
                        for (int j = 0; j < Utils.GetRandInt(2, 5); j++)
                        {
                            Vehicle veh2 = new Vehicle(Utils.GetRandValue(g_PoliceVehicleModels), g_SpawnPoint.Around(10f));
                            veh2.CreateRandomDriver();
                            if (Utils.GetRandBool())
                            {
                                veh2.Driver.Kill();
                            }

                            m_Officers.Add(veh2);
                        }
                    }
                    else
                    {
                        veh = new Vehicle(Utils.GetRandValue(g_PoliceVehicleModels), g_SpawnPoint);
                        veh.CreateRandomDriver();
                        veh.Driver.Kill();

                        m_Officers.Add(veh);

                        suspect = new Ped(Utils.GetRandValue(g_PedModels), g_SpawnPoint, Utils.GetRandInt(1, 360));
                        suspect.Inventory.GiveNewWeapon(Utils.GetRandValue(g_WeaponList), (short)Utils.GetRandInt(10, 60), true);
                        g_Suspects.Add(suspect);
                    }
                    break;
            }

            // TODO: Fix the lighting, it doesn't work atm.
            m_Officers.ForEach(o =>
            {
                if (o.VehicleModelIsELS())
                {
                    o.IsSirenOn = false;
                    o.IsSirenSilent = false;
                }
                else
                {
                    o.IsSirenOn = true;
                    o.IsSirenSilent = true;
                }
            });

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();
  
            if (g_bOnScene && !g_bIsPursuit)
            {
                OfcDown();
            }

            if (g_bIsPursuit && !Functions.IsPursuitStillRunning(m_Pursuit))
            {
                End();
            }
        }

        void OfcDown()
        {
            g_Logger.Log("OfficerDown: Starting pursuit");

            g_bIsPursuit = true;
            GameFiber fiber = GameFiber.StartNew(delegate
            {
                m_Pursuit = Functions.CreatePursuit();
                g_Suspects.ForEach(suspect =>
                {
                    Functions.AddPedToPursuit(m_Pursuit, suspect);
                });

                Functions.SetPursuitIsActiveForPlayer(m_Pursuit, true);

                if (m_bMultiple)
                {
                    LSPDFR.RequestBackup(Game.LocalPlayer.Character.Position, 3, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.LocalUnit);
                    LSPDFR.RequestBackup(Game.LocalPlayer.Character.Position, 1, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.SwatTeam);
                    LSPDFR.RequestBackup(Game.LocalPlayer.Character.Position, 1, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.AirUnit);
                }
                else
                {
                    LSPDFR.RequestBackup(Game.LocalPlayer.Character.Position, 2, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.LocalUnit);
                }
            }, "OfficerDownPursuitFiber");

            Main.g_GameFibers.Add(fiber);
        }

        public override void End()
        {
            m_Officers?.ForEach(veh => veh.Dismiss());
            g_Suspects?.ForEach(suspect => suspect.Dismiss());

            g_Logger.Log("OfficerDown: End call");

            base.End();
        }
    }
}