using Rage;
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
            CalloutPosition = m_SpawnPoint;

            Functions.PlayScannerAudioUsingPosition(
                $"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.WE_HAVE)} " +
                $"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.VEHICLE_FIRE)} IN_OR_ON_POSITION", m_SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            m_Vehicle = new Vehicle(Utils.GetRandValue(m_Vehicles), m_SpawnPoint)
            {
                EngineHealth = 350.0f
            };

            m_Victims.Add(m_Vehicle.CreateRandomDriver());
            m_Victims.ForEach(v => v.Tasks.LeaveVehicle(m_Vehicle, LeaveVehicleFlags.LeaveDoorOpen));

            GameFiber.StartNew(delegate
            {
                GameFiber.Sleep(5000);
                LSPDFR.RequestEMS(m_SpawnPoint);
                LSPDFR.RequestFire(m_SpawnPoint);
            });

            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            base.OnCalloutNotAccepted();
        }

        public override void Process()
        {
            if (Game.LocalPlayer.Character.Position.DistanceTo(m_SpawnPoint) < 75)
            {
                bool bCool = Utils.GetRandBool();
                m_Victims.ForEach(v =>
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