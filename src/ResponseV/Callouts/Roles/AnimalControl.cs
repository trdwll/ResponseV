using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rage;
using Vehicle = Rage.Vehicle;
using World = Rage.World;
using Ped = Rage.Ped;
using ResponseVLib;
using static ResponseVLib.Vehicle;

namespace ResponseV.Callouts.Roles
{
    internal sealed class AnimalControl : RoleBase
    {
        private static Vehicle s_Vehicle;
        private static Ped s_Officer;
        private static Blip s_VehicleBlip;
        private static bool s_bIsEnroute;
        private static Model[] s_VehicleModels = { "DUBSTA3" };
        private static Model[] s_OfficerModels = { "s_m_m_paramedic_01", "s_m_m_scientist_01" };

        public static bool Request(Vector3 location, params Ped[] animals)
        {
            if (animals.Length > 0)// && animals.Where(a => !a.IsHuman) != null)
            {
                Request_Impl(location, animals);
                return s_bIsEnroute;//s_Vehicle.Exists() && s_bIsEnroute;
            }

            Main.MainLogger.Log("AnimalControl: Unable to request Animal Control, the animals array was > 0 or contained humans.");

            return false;
        }

        public static bool Request(Vector3 location)
        {
            Request_Impl(location);
            return s_Vehicle.Exists() && s_bIsEnroute;
        }

        private static void Request_Impl(Vector3 location, params Ped[] animals)
        {
            if (s_Vehicle.Exists() || s_bIsEnroute)
            {
                // Main.MainLogger.Log("AnimalControl: Exists or they're already enroute.");
                return;
            }

            Main.MainLogger.Log("AnimalControl: Requested");

            Vector3 SpawnPoint = World.GetNextPositionOnStreet(location.AroundPosition(500.0f));
            s_Vehicle = new Vehicle(ResponseVLib.Utils.GetRandValue(s_VehicleModels), SpawnPoint, SpawnPoint.GetClosestVehicleNodeHeading());
            if (ResponseVLib.Utils.GetRandBool())
            {
                s_Vehicle.SetColors(EPaint.Ice_White, EPaint.Ice_White);
            }
            else
            {
                s_Vehicle.SetColors(EPaint.Dew_Yellow, EPaint.Dew_Yellow);
            }

            Ped s_Officer = new Ped(ResponseVLib.Utils.GetRandValue(s_OfficerModels), SpawnPoint, 0.0f)
            {
                IsPersistent = true,
                BlockPermanentEvents = true
            };

            s_Officer.WarpIntoVehicle(s_Vehicle, -1);
            s_Officer.Inventory.GiveFlashlight();
            s_Officer.Inventory.GiveNewWeapon(WeaponHash.SniperRifle, 10, false);
            s_Officer.Inventory.GiveNewWeapon(WeaponHash.Pistol, 30, false);

            s_VehicleBlip = s_Vehicle.AttachBlip();
            s_VehicleBlip.Color = System.Drawing.Color.Orange;
            // s_VehicleBlip.Scale = 0.5f;


            //Rage.Native.NativeFunction.Natives.TASK_GO_TO_ENTITY(s_Officer, s_Vehicle, -1, 5.0f, 1.0f, 0, 0);

            GameFiber fiber = GameFiber.StartNew(delegate
            {
                s_bIsEnroute = true;

                // s_Officer.Tasks.EnterVehicle(s_Vehicle, -1).WaitForCompletion(5000);

                s_Officer.Tasks.DriveToPosition(location, 20.0f, VehicleDrivingFlags.Normal, 20.0f).WaitForCompletion(90000);
                Main.MainLogger.Log("AnimalControl: Driving to the location");

                if (Vector3.Distance(s_Vehicle.Position, location) > 30.0f)
                {
                    s_Vehicle.Position = location.Around(8.0f);
                }

                s_Officer.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen);
                

                List<Ped> AliveAnimals = new List<Ped>();
                List<Ped> DeadAnimals = new List<Ped>();

                animals.ToList().ForEach(animal =>
                {
                    animal.IsPersistent = true;

                    if (animal.IsAlive)
                    {
                        AliveAnimals.Add(animal);
                    }
                    else
                    {
                        DeadAnimals.Add(animal);
                    }
                });

                // TODO: if animal is alive then tranquilize it
                List<Ped> TranquilizedAnimals = new List<Ped>();

                AliveAnimals.ForEach(animal =>
                {

                });


                DeadAnimals.ForEach(animal =>
                {
                    s_Officer.Tasks.FollowNavigationMeshToPosition(animal.Position, animal.GetHeadingTowards(animal), 2.0f, 1.0f).WaitForCompletion();

                    if (animal.Exists() && animal.IsDead)
                    {
                        s_Officer.Tasks.PlayAnimation(new AnimationDictionary("amb@medic@standing@tendtodead@idle_a"), "idle_a", 1.0f, AnimationFlags.Loop);
                        GameFiber.Sleep(5000);
                    }

                    animal.Delete();
                });


                // s_Vehicle.Driver.Tasks.CruiseWithVehicle(25.0f, VehicleDrivingFlags.Emergency);
            }, "AnimalControlFiber");

            Main.s_GameFibers.Add(fiber);

        }
    }
}
