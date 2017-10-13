
using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using System.Collections.Generic;

namespace ResponseV.Callouts.Any
{
    [CalloutInfo("OfficerDown", CalloutProbability.Low)]
    public class OfficerDown : Callout
    {
        private Vector3 SpawnPoint;
        private LHandle pursuit;
        private Blip blip;

        private List<Vehicle> officers = new List<Vehicle>();
        private List<Ped> suspects = new List<Ped>();

        private Enums.Callout state;
        private Enums.CalloutType type;
        
        private bool multiple;

        private Model[] vehicleModels = { "police", "police2", "police3", "police4", "sheriff", "sheriff2" };
        private WeaponHash[] weaponList = { WeaponHash.AssaultRifle, WeaponHash.Pistol, WeaponHash.SawnOffShotgun };

        public override bool OnBeforeCalloutDisplayed()
        {
            SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(Utils.GetRandInt(400, 550)));

            ShowCalloutAreaBlipBeforeAccepting(SpawnPoint, 25f);

            multiple = Utils.GetRandomBool();

            CalloutMessage = "Reports of " + (multiple ? "Multiple Officers": "an Officer") + " Down";
            CalloutPosition = SpawnPoint;

            if (!multiple)
            {
                type = Utils.GetRandValue(Enums.CalloutType.Shooting, Enums.CalloutType.Stabbing, Enums.CalloutType.Unknown);

                switch (type)
                {
                    default:
                    case Enums.CalloutType.Unknown:
                        Functions.PlayScannerAudioUsingPosition(
                            $"{LSPDFR.Radio.getRandomSound(LSPDFR.Radio.WE_HAVE)} " +
                            $"{LSPDFR.Radio.getRandomSound(LSPDFR.Radio.OFFICER_DOWN)} IN_OR_ON_POSITION", SpawnPoint);
                        break;
                    case Enums.CalloutType.Shooting:
                        Functions.PlayScannerAudioUsingPosition(
                            $"{LSPDFR.Radio.getRandomSound(LSPDFR.Radio.WE_HAVE)} " +
                            $"{LSPDFR.Radio.getRandomSound(LSPDFR.Radio.OFFICER_SHOT)} IN_OR_ON_POSITION", SpawnPoint);
                        break;
                    case Enums.CalloutType.Stabbing:
                        Functions.PlayScannerAudioUsingPosition(
                            $"{LSPDFR.Radio.getRandomSound(LSPDFR.Radio.WE_HAVE)} CRIME_OFFICER_STABBED IN_OR_ON_POSITION", SpawnPoint);
                        break;
                }
            }
            else
            {
                type = Enums.CalloutType.Shooting;
                Functions.PlayScannerAudioUsingPosition(
                    $"{LSPDFR.Radio.getRandomSound(LSPDFR.Radio.WE_HAVE)} " +
                    $"{LSPDFR.Radio.getRandomSound(LSPDFR.Radio.OFFICERS_DOWN)} IN_OR_ON_POSITION", SpawnPoint);
            }
            
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            state = Enums.Callout.EnRoute;

            Ped suspect;
            Vehicle veh;
            switch (type)
            {
                default:
                case Enums.CalloutType.Unknown:
                    veh = new Vehicle(Utils.GetRandValue(vehicleModels), SpawnPoint);
                    veh.CreateRandomDriver();
                    veh.Driver.Kill();

                    officers.Add(veh);

                    suspect = new Ped(Utils.GetRandValue(Model.PedModels), SpawnPoint, Utils.GetRandInt(1, 360));
                    suspects.Add(suspect);
                    break;
                case Enums.CalloutType.Stabbing:
                    suspect = new Ped(Utils.GetRandValue(Model.PedModels), SpawnPoint, Utils.GetRandInt(1, 360));
                    suspect.Inventory.GiveNewWeapon(WeaponHash.Knife, 1, true);
                    suspect.Tasks.FightAgainstClosestHatedTarget(30f);
                    suspects.Add(suspect);

                    Vehicle veh2 = new Vehicle(Utils.GetRandValue(vehicleModels), SpawnPoint);
                    veh2.CreateRandomDriver();
                    veh2.Driver.Kill();

                    officers.Add(veh2);
                    break;
                case Enums.CalloutType.Shooting:
                    if (multiple)
                    {
                        // Spawn suspects
                        for (int i = 0; i < Utils.GetRandInt(2, 5); i++)
                        {
                            Ped sus = new Ped(Utils.GetRandValue(Model.PedModels), SpawnPoint, Utils.GetRandInt(1, 360));
                            sus.Inventory.GiveNewWeapon(Utils.GetRandValue(weaponList), (short)Utils.GetRandInt(10, 60), true);
                            sus.Tasks.FightAgainstClosestHatedTarget(30f);
                            suspects.Add(sus);
                        }

                        // Spawn police
                        for (int j = 0; j < Utils.GetRandInt(2, 5); j++)
                        {
                            Vehicle veh3 = new Vehicle(Utils.GetRandValue(vehicleModels), SpawnPoint.Around(10f));
                            veh3.CreateRandomDriver();
                            veh3.Driver.Kill();

                            officers.Add(veh3);
                        }
                    }
                    else
                    {
                        suspect = new Ped(Utils.GetRandValue(Model.PedModels), SpawnPoint, Utils.GetRandInt(1, 360));
                        suspect.Inventory.GiveNewWeapon(Utils.GetRandValue(weaponList), (short)Utils.GetRandInt(10, 60), true);
                        suspects.Add(suspect);
                    }
                    break;
            }

            // Turn on police car lighting
            officers.ForEach(v =>
            {
                v.IsSirenOn = true;
                v.IsSirenSilent = true;
                v.IsEngineOn = true;
            });

            blip = new Blip(SpawnPoint)
            {
                IsRouteEnabled = true
            };
            blip.EnableRoute(System.Drawing.Color.Blue);

            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            base.OnCalloutNotAccepted();
        }

        public override void Process()
        {
            base.Process();
  
            if (state== Enums.Callout.EnRoute && Game.LocalPlayer.Character.Position.DistanceTo(SpawnPoint) < 70)
            {
                state = Enums.Callout.OnScene;

                blip.IsRouteEnabled = false;

                OfcDown();
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

        void OfcDown()
        {
            GameFiber.StartNew(delegate
            {
                pursuit = Functions.CreatePursuit();
                suspects.ForEach(suspect =>
                {
                    Functions.AddPedToPursuit(pursuit, suspect);
                });
                blip.Delete();

                state = Enums.Callout.InPursuit;

                if (multiple)
                {
                    LSPDFR.RequestBackup(Game.LocalPlayer.Character.Position, 3, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.LocalUnit);
                    LSPDFR.RequestBackup(Game.LocalPlayer.Character.Position, 1, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.SwatTeam);
                    LSPDFR.RequestBackup(Game.LocalPlayer.Character.Position, 1, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.AirUnit);
                }
                else
                {
                    LSPDFR.RequestBackup(Game.LocalPlayer.Character.Position, 3, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.LocalUnit);
                }
            });
        }

        public override void End()
        {
            blip.Delete();
            officers.ForEach(veh => veh.Dismiss());
            suspects.ForEach(suspect => suspect.Dismiss());

            base.End();
        }
    }
}