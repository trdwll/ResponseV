using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;

namespace ResponseV.Callouts.Any
{
    [CalloutInfo("ParkingViolation", CalloutProbability.VeryLow)]
    public class ParkingViolation : RVCallout
    {
        private Vehicle m_Vehicle;

        public override bool OnBeforeCalloutDisplayed()
        {
            CalloutMessage = "Reports of an Illegally Parked Vehicle";
            CalloutPosition = g_SpawnPoint;

            Functions.PlayScannerAudioUsingPosition(
                $"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.WE_HAVE)} " +
                $"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.PARKING)} IN_OR_ON_POSITION", g_SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            m_Vehicle = new Vehicle(Utils.GetRandValue(g_Vehicles), g_SpawnPoint);
            
            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            if (Game.LocalPlayer.Character.Position.DistanceTo(g_SpawnPoint) < 20)
            {
                End();
            }
        }

        public override void End()
        {
            m_Vehicle.Dismiss();

            base.End();
        }
    }
}