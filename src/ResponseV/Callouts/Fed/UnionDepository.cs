using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResponseV.Callouts.Fed
{
    [CalloutInfo("UnionDepository", CalloutProbability.VeryHigh)]
    public class UnionDepository : RVCallout
    {
        // private Vector3 m_SpawnPoint = new Vector3(17.51f, -655.52f, 31.48f);

        private Vehicle m_Vehicle1;
        private Vehicle m_Vehicle2;
        private Vehicle m_Vehicle3;

        private LHandle m_Pursuit;

        // TODO: Add more possible ped models
        private Model[] m_PedModels = { "mp_m_waremech_01", "s_m_y_blackops_01", "s_m_y_blackops_02"  };

        public override bool OnBeforeCalloutDisplayed()
        {
            CalloutMessage = "Reports of a Robbery at the Union Depository";
            CalloutPosition = g_SpawnPoint;

            Functions.PlayScannerAudioUsingPosition($"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.WE_HAVE)} UNION_DEPOSITORY_ROBBERY IN_OR_ON_POSITION", g_SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            g_Logger.Log("UnionDepository: Call accepted");

            m_Vehicle1 = new Vehicle("STOCKADE", new Vector3(29.37f, -658.82f, 31.23f), 313.50f);
            Ped suspect = new Ped(Utils.GetRandValue(m_PedModels), g_SpawnPoint, Utils.GetRandInt(1, 360));
            Ped suspect2 = new Ped(Utils.GetRandValue(m_PedModels), g_SpawnPoint, Utils.GetRandInt(1, 360));
            g_Suspects.Add(suspect);
            g_Suspects.Add(suspect2);

            suspect.WarpIntoVehicle(m_Vehicle1, -1);
            suspect2.WarpIntoVehicle(m_Vehicle1, 0);
            
            if (Utils.GetRandBool())
            {
                m_Vehicle2 = new Vehicle("STOCKADE", new Vector3(38.77f, -649.90f, 31.23f), 313.50f);
                Ped sus = new Ped(Utils.GetRandValue(m_PedModels), g_SpawnPoint, Utils.GetRandInt(1, 360));
                Ped sus2 = new Ped(Utils.GetRandValue(m_PedModels), g_SpawnPoint, Utils.GetRandInt(1, 360));
                g_Suspects.Add(sus);
                g_Suspects.Add(sus2);

                sus.WarpIntoVehicle(m_Vehicle2, -1);
                sus2.WarpIntoVehicle(m_Vehicle2, 0);
            }
            if (Utils.GetRandBool())
            {
                m_Vehicle3 = new Vehicle("STOCKADE", new Vector3(52.94f, -632.16f, 31.25f), 331.61f);
                Ped sus = new Ped(Utils.GetRandValue(m_PedModels), g_SpawnPoint, Utils.GetRandInt(1, 360));
                Ped sus2 = new Ped(Utils.GetRandValue(m_PedModels), g_SpawnPoint, Utils.GetRandInt(1, 360));
                g_Suspects.Add(sus);
                g_Suspects.Add(sus2);

                sus.WarpIntoVehicle(m_Vehicle3, -1);
                sus2.WarpIntoVehicle(m_Vehicle3, 0);
            }

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            if (Game.LocalPlayer.Character.Position.DistanceTo(g_SpawnPoint) < 100)
            {
                // Destroy the barriers in the area so the Stockades can drive out
                World.SpawnExplosion(new Vector3(62.77f, -625.39f, 31.71f), 1, 5f, false, true, 0f);

                m_Vehicle1.Driver.Tasks.CruiseWithVehicle(Utils.GetRandInt(45, 70));
                m_Vehicle2?.Driver.Tasks.CruiseWithVehicle(Utils.GetRandInt(45, 70));
                m_Vehicle3?.Driver.Tasks.CruiseWithVehicle(Utils.GetRandInt(45, 70));
            }

            if (Game.LocalPlayer.Character.Position.DistanceTo(g_SpawnPoint) < 50 && !g_bIsPursuit)
            {
                g_Logger.Log("UnionDepository: OnScene and start pursuit");
                g_bOnScene = true;

                Pursuit();
            }

            if (g_bIsPursuit && !Functions.IsPursuitStillRunning(m_Pursuit))
            {
                g_Logger.Log("UnionDepository: Pursuit has finished so End()");
                End();
            }
        }

        void Pursuit()
        {
            g_bIsPursuit = true;
            GameFiber.StartNew(delegate
            {
                g_Logger.Log("UnionDepository: Started pursuit");
                m_Pursuit = Functions.CreatePursuit();

                g_Suspects.ForEach(s =>
                {
                    Functions.AddPedToPursuit(m_Pursuit, s);
                    s.Inventory.GiveNewWeapon(Utils.GetRandValue(WeaponHash.AdvancedRifle, WeaponHash.CombatMG, WeaponHash.Smg), 200, true);

                    s.RelationshipGroup = RelationshipGroup.HatesPlayer;

                    s.Tasks.FightAgainstClosestHatedTarget(50f);
                });

                Functions.SetPursuitIsActiveForPlayer(m_Pursuit, true);

                GameFiber.Sleep(3000);
                LSPDFR.RequestBackup(Game.LocalPlayer.Character.Position, 5, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.LocalUnit);

                GameFiber.Sleep(1000);
                LSPDFR.RequestBackup(Game.LocalPlayer.Character.Position, 1, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.NooseTeam);
                LSPDFR.RequestBackup(Game.LocalPlayer.Character.Position, 1, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.SwatTeam);

                GameFiber.Sleep(1000);
                LSPDFR.RequestBackup(Game.LocalPlayer.Character.Position, 1, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.AirUnit);
            });


            GameFiber.StartNew(delegate
            {
                g_Logger.Log("UnionDepository: Checking nearby vehicles and turning on their lighting");
                for (;;)
                {
                    GameFiber.Yield();
                    List<Vehicle> NearbyVehicles1 = m_Vehicle1.Driver.GetNearbyVehicles(16).ToList();
                    List<Vehicle> NearbyVehicles2 = m_Vehicle2?.Driver.GetNearbyVehicles(16).ToList();
                    List<Vehicle> NearbyVehicles3 = m_Vehicle3?.Driver.GetNearbyVehicles(16).ToList();

                    NearbyVehicles1.ForEach(v =>
                    {
                        if (v.IsPoliceVehicle)
                        {
                            v.IsSirenOn = true;
                        }
                    });

                    NearbyVehicles2?.ForEach(v =>
                    {
                        if (v.IsPoliceVehicle)
                        {
                            v.IsSirenOn = true;
                        }
                    });

                    NearbyVehicles3?.ForEach(v =>
                    {
                        if (v.IsPoliceVehicle)
                        {
                            v.IsSirenOn = true;
                        }
                    });

                    // Clear all lists before we get them again
                    NearbyVehicles1.Clear();
                    NearbyVehicles2?.Clear();
                    NearbyVehicles3?.Clear();

                    GameFiber.Sleep(10000);
                }
            });
        }

        public override void End()
        {
            m_Vehicle1.Dismiss();
            m_Vehicle2?.Dismiss();
            m_Vehicle3?.Dismiss();

            base.End();
        }
    }
}
