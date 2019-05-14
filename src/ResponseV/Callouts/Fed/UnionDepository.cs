using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/**
 * This class isn't implementing the RVCallout abstract class due to issues with Functions.GetCalloutName(Functions.GetCurrentCallout())
 */
namespace ResponseV.Callouts.Fed
{
    [CalloutInfo("UnionDepository", CalloutProbability.VeryHigh)]
    public class UnionDepository : Callout
    {
        private readonly Logger m_Logger = new Logger();

        private readonly Vector3 m_SpawnPoint = new Vector3(-76.98f, -677.95f, 33.95f);//new Vector3(62.77f, -625.39f, 31.71f);//(17.51f, -655.52f, 31.48f);

        private Vehicle m_Vehicle1;
        private Vehicle m_Vehicle2;
        private Vehicle m_Vehicle3;

        private LHandle m_Pursuit;
        private bool m_bIsPursuit;

        private bool m_bOnScene;
        private bool m_bBackupCalled;

        private List<Ped> m_Suspects = new List<Ped>();

        private Blip m_CallBlip;

        private readonly Model[] m_PedModels = Model.PedModels; // TODO: Come up with some good peds to choose from (only skilled people or *cough* dumb *cough* people would go after the UD)
        private readonly Model[] m_Vehicles = { "SPEEDO", "BOXVILLE" };

        public override bool OnBeforeCalloutDisplayed()
        {
            // If the player is close then we don't want to create this callout
            if (Game.LocalPlayer.Character.Position.DistanceTo(m_SpawnPoint) <= 200)
            {
                return false;
            }

            CalloutMessage = "Reports of a " + (Utils.GetRandBool() ? "Robbery" : "Heist" ) + " at the Union Depository";
            CalloutPosition = m_SpawnPoint;

            ShowCalloutAreaBlipBeforeAccepting(m_SpawnPoint, 25f);

            Functions.PlayScannerAudioUsingPosition($"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.WE_HAVE)} UNION_DEPOSITORY_ROBBERY IN_OR_ON_POSITION", m_SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            m_Logger.Log("UnionDepository: Callout accepted");

            m_CallBlip = new Blip(m_SpawnPoint)
            {
                IsRouteEnabled = true
            };

            m_CallBlip.EnableRoute(System.Drawing.Color.Blue);
            m_CallBlip.Color = System.Drawing.Color.Blue;

            m_Vehicle1 = new Vehicle("STOCKADE", new Vector3(-94.28f, -671.04f, 35.05f), 69.04f);//new Vector3(29.37f, -658.82f, 31.23f), 313.50f);
            Ped suspect = new Ped(Utils.GetRandValue(m_PedModels), m_SpawnPoint, Utils.GetRandInt(1, 360));
            Ped suspect2 = new Ped(Utils.GetRandValue(m_PedModels), m_SpawnPoint, Utils.GetRandInt(1, 360));
            m_Suspects.Add(suspect);
            m_Suspects.Add(suspect2);

            suspect.WarpIntoVehicle(m_Vehicle1, -1);
            suspect2.WarpIntoVehicle(m_Vehicle1, 0);

            // Spawn a possible second STOCKADE
            if (Utils.GetRandBool())
            {
                m_Vehicle2 = new Vehicle("STOCKADE", new Vector3(-85.70f, -674.37f, 34.94f), 67.71f);//new Vector3(38.77f, -649.90f, 31.23f), 313.50f);
                Ped sus = new Ped(Utils.GetRandValue(m_PedModels), m_SpawnPoint, Utils.GetRandInt(1, 360));
                Ped sus2 = new Ped(Utils.GetRandValue(m_PedModels), m_SpawnPoint, Utils.GetRandInt(1, 360));
                m_Suspects.Add(sus);
                m_Suspects.Add(sus2);

                sus.WarpIntoVehicle(m_Vehicle2, -1);
                sus2.WarpIntoVehicle(m_Vehicle2, 0);
            }

            // Spawn a possible third STOCKADE
            if (Utils.GetRandBool())
            {
                m_Vehicle3 = new Vehicle("STOCKADE", new Vector3(-76.98f, -677.95f, 33.95f), 67.48f);//new Vector3(52.94f, -632.16f, 31.25f), 331.61f);
                Ped sus = new Ped(Utils.GetRandValue(m_PedModels), m_SpawnPoint, Utils.GetRandInt(1, 360));
                Ped sus2 = new Ped(Utils.GetRandValue(m_PedModels), m_SpawnPoint, Utils.GetRandInt(1, 360));
                m_Suspects.Add(sus);
                m_Suspects.Add(sus2);

                sus.WarpIntoVehicle(m_Vehicle3, -1);
                sus2.WarpIntoVehicle(m_Vehicle3, 0);
            }

            // Spawn a possible group of people that shoot police on sight
            if (Utils.GetRandBool())
            {
                for (int i = 0; i < Utils.GetRandInt(2, 6); i++)
                {
                    Ped p = new Ped(Utils.GetRandValue(m_PedModels), new Vector3(-84.27f, -661.55f, 35.94f), Utils.GetRandInt(1, 360));
                    m_Suspects.Add(p);
                }

                Vehicle v = new Vehicle(Utils.GetRandValue(m_Vehicles), new Vector3(-87.38f, -658.41f, 35.57f), 160.89f);
                v.Dismiss();
            }

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            float DistanceTo = Game.LocalPlayer.Character.Position.DistanceTo(m_SpawnPoint);

            if (DistanceTo <= 250 && !m_bBackupCalled)
            {
                // Standard procedure to respond nearby units
                Utils.Notify($"Dispatch to any available units in the vicinity of the Union Depository; respond code 3 for a possible robbery.");
                LSPDFR.RequestBackup(Game.LocalPlayer.Character.Position, 2);
                m_bBackupCalled = true;
            }

            if (DistanceTo < 100 && !m_bOnScene)
            {
                m_bOnScene = true;
                m_CallBlip.IsRouteEnabled = false;
                m_Logger.Log("UnionDepository: DistanceTo(m_SpawnPoint) < 100 so hide m_CallBlip");

                m_Vehicle1.Driver.Tasks.CruiseWithVehicle(Utils.GetRandInt(45, 70), VehicleDrivingFlags.IgnorePathFinding);
                m_Vehicle2?.Driver.Tasks.CruiseWithVehicle(Utils.GetRandInt(45, 70), VehicleDrivingFlags.IgnorePathFinding);
                m_Vehicle3?.Driver.Tasks.CruiseWithVehicle(Utils.GetRandInt(45, 70), VehicleDrivingFlags.IgnorePathFinding);

                m_Logger.Log("UnionDepository: Making vehicles start to cruise.");
            }

            if (DistanceTo < 50 && !m_bIsPursuit)
            {
                m_Logger.Log("UnionDepository: OnScene and start pursuit");
                m_bOnScene = true;

                // Notify dispatch that the heist is real.
                Pursuit();
            }

            if (Game.LocalPlayer.IsDead)
            {
                Functions.PlayScannerAudio($"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.OFFICER_DOWN)} OFFICER_NEEDS_IMMEDIATE_ASSISTANCE");
                End();
                m_Logger.Log("UnionDepository: Player is dead so force end call.");
            }

            if (m_bIsPursuit && !Functions.IsPursuitStillRunning(m_Pursuit))
            {
                // TODO: Check if suspects are dead or arrested
                // TODO: Check if vehicles are not dead 
                // then Utils.Notify("Dispatch all suspects are dead or in custody, start fire and ems.");
                m_Logger.Log("UnionDepository: Pursuit has finished so End()");
                End();
            }
        }

        void Pursuit()
        {
            m_bIsPursuit = true;
            GameFiber.StartNew(delegate
            {
                m_Logger.Log("UnionDepository: Started pursuit");
                m_Pursuit = Functions.CreatePursuit();

                m_Suspects.ForEach(s =>
                {
                    Functions.AddPedToPursuit(m_Pursuit, s);
                    s.Inventory.GiveNewWeapon(Utils.GetRandValue(WeaponHash.AssaultRifle, WeaponHash.CombatMG, WeaponHash.AssaultSMG), 180, true);
                    s.Armor = 100;
                    s.Health = 100;

                    s.RelationshipGroup = RelationshipGroup.HatesPlayer;

                    s.Tasks.FightAgainstClosestHatedTarget(50f);
                });


                Functions.SetPursuitIsActiveForPlayer(m_Pursuit, true);

                GameFiber.Sleep(3000);
                LSPDFR.RequestBackup(Game.LocalPlayer.Character.Position, 2, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.LocalUnit);

                GameFiber.Sleep(1000);
                LSPDFR.RequestBackup(Game.LocalPlayer.Character.Position, 1, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.AirUnit);

                GameFiber.Sleep(5000);
                LSPDFR.RequestBackup(Game.LocalPlayer.Character.Position, 2, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.NooseTeam);
                LSPDFR.RequestBackup(Game.LocalPlayer.Character.Position, 2, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.NooseAirUnit);

            }, "UnionDepositoryPursuitFiber");


            //GameFiber.StartNew(delegate
            //{
            //    m_Logger.Log("UnionDepository: Checking nearby vehicles and turning on their lighting");
            //    for (;;)
            //    {
            //        GameFiber.Yield();
            //        List<Vehicle> NearbyVehicles1 = m_Vehicle1.Driver.GetNearbyVehicles(16).ToList();
            //        List<Vehicle> NearbyVehicles2 = m_Vehicle2?.Driver.GetNearbyVehicles(16).ToList();
            //        List<Vehicle> NearbyVehicles3 = m_Vehicle3?.Driver.GetNearbyVehicles(16).ToList();

            //        NearbyVehicles1.ForEach(v =>
            //        {
            //            if (v.IsPoliceVehicle)
            //            {
            //                v.IsSirenOn = true;
            //            }
            //        });

            //        NearbyVehicles2?.ForEach(v =>
            //        {
            //            if (v.IsPoliceVehicle)
            //            {
            //                v.IsSirenOn = true;
            //            }
            //        });

            //        NearbyVehicles3?.ForEach(v =>
            //        {
            //            if (v.IsPoliceVehicle)
            //            {
            //                v.IsSirenOn = true;
            //            }
            //        });

            //        // Clear all lists before we get them again
            //        NearbyVehicles1.Clear();
            //        NearbyVehicles2?.Clear();
            //        NearbyVehicles3?.Clear();

            //        GameFiber.Sleep(10000);
            //    }
            //}, "UnionDepositoryPoliceLightingFiber");
        }

        public override void End()
        {
            base.End();

            m_Vehicle1.Dismiss();
            m_Vehicle2?.Dismiss();
            m_Vehicle3?.Dismiss();

            m_CallBlip.Delete();

            m_Suspects.ForEach(s => s.Dismiss());
            m_bIsPursuit = false;
            m_bOnScene = false;

            m_Logger.Log("UnionDepository: Ended");
        }
    }
}
