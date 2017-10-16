using System.Linq;

using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;

namespace ResponseV.Callouts.Any
{
    [CalloutInfo("Speeding", CalloutProbability.VeryHigh)]
    public class Speeding : Callout
    {
        private Vector3 SpawnPoint;
        private Vehicle veh;
        private Ped suspect;
        private Blip blip;
        private LHandle pursuit;
        private Enums.Callout state;

        public override bool OnBeforeCalloutDisplayed()
        {
            SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(Utils.GetRandInt(500, 600)));

            ShowCalloutAreaBlipBeforeAccepting(SpawnPoint, 25f);

            CalloutMessage = "Reports of a Speeding Vehicle";
            CalloutPosition = SpawnPoint;

            Functions.PlayScannerAudioUsingPosition(
                $"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.WE_HAVE)} "+
                $"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.SPEEDING)} IN_OR_ON_POSITION", SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            state = Enums.Callout.EnRoute;

            veh = new Vehicle(Utils.GetRandValue(Model.VehicleModels.Where(v => v.IsCar && !v.IsLawEnforcementVehicle).ToArray()), SpawnPoint);
            suspect = veh.CreateRandomDriver();
            veh.Driver.Tasks.CruiseWithVehicle(Utils.GetRandInt(40, 80));

            blip = new Blip(SpawnPoint);

            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            base.OnCalloutNotAccepted();
        }

        public override void Process()
        {
            if (state == Enums.Callout.EnRoute && Game.LocalPlayer.Character.Position.DistanceTo(SpawnPoint) < 50)
            {
                state = Enums.Callout.OnScene;

                Pursuit();
            }

            if (state == Enums.Callout.InPursuit)
            {
                if (!Functions.IsPursuitStillRunning(pursuit))
                {
                    state = Enums.Callout.Done;
                    End();
                }
            }
        }

        void Pursuit()
        {
            GameFiber.StartNew(delegate
            {
                pursuit = Functions.CreatePursuit();
                Functions.AddPedToPursuit(pursuit, suspect);
                blip.Delete();

                state = Enums.Callout.InPursuit;

                LSPDFR.RequestBackup(Game.LocalPlayer.Character.Position, 1, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.LocalUnit);
            });
        }

        public override void End()
        {
            blip.Delete();
            veh.Dismiss();

            base.End();
        }
    }
}