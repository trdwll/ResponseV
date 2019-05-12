﻿using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;

namespace ResponseV.Callouts.Any
{
    [CalloutInfo("VehicleFire", CalloutProbability.Medium)]
    public class VehicleFire : RVCallout
    {
        private Vehicle m_Vehicle;

        public override bool OnBeforeCalloutDisplayed()
        {
            CalloutMessage = "Reports of a Vehicle Fire";
            CalloutPosition = g_SpawnPoint;

            Functions.PlayScannerAudioUsingPosition(
                $"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.WE_HAVE)} " +
                $"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.VEHICLE_FIRE)} IN_OR_ON_POSITION", g_SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            m_Vehicle = new Vehicle(Utils.GetRandValue(g_Vehicles), g_SpawnPoint)
            {
                EngineHealth = 350.0f
            };

            g_Victims.Add(m_Vehicle.CreateRandomDriver());
            g_Victims.ForEach(v => v.Tasks.LeaveVehicle(m_Vehicle, LeaveVehicleFlags.LeaveDoorOpen));

            GameFiber.StartNew(delegate
            {
                GameFiber.Sleep(5000);
                LSPDFR.RequestEMS(g_SpawnPoint);
                LSPDFR.RequestFire(g_SpawnPoint);
            });

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            if (Game.LocalPlayer.Character.Position.DistanceTo(g_SpawnPoint) < 75)
            {
                bool bCool = Utils.GetRandBool();
                g_Victims.ForEach(v =>
                {
                    if (bCool)
                    {
                        v.Kill();
                    }
                    else
                    {
                        v.Tasks.Flee(v, Utils.GetRandInt(25, 50), 5);
                    }
                });
                
                m_Vehicle.Explode(bCool);


                End();

                //if (BetterEMS.API.EMSFunctions.DidEMSRevivePed(m_Victim) == true)
                //{
                //    End();
                //}

                //if (BetterEMS.API.EMSFunctions.DidEMSRevivePed(m_Victim) == false)
                //{
                //    Arrest_Manager.API.Functions.CallCoroner(true);
                //    End();
                //}
            }
        }
    }
}