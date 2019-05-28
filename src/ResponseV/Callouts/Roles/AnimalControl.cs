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
        private static Model[] s_VehicleModels = { "DUBSTA3", "SPEEDO" };
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
            Utils.NotifyDispatchTo("Player", "Animal Control en-route.");

            Vector3 SpawnPoint = World.GetNextPositionOnStreet(location.AroundPosition(300.0f));
            s_Vehicle = new Vehicle(ResponseVLib.Utils.GetRandValue(s_VehicleModels), SpawnPoint, SpawnPoint.GetClosestVehicleNodeHeading())
            {
                LicensePlateStyle = LicensePlateStyle.BlueOnWhite3
            };

            if (ResponseVLib.Utils.GetRandBool())
            {
                s_Vehicle.SetColors(EPaint.Ice_White, EPaint.Ice_White);
            }
            else
            {
                s_Vehicle.SetColors(EPaint.Dew_Yellow, EPaint.Dew_Yellow);
            }

            // turn on lights
            if (s_Vehicle.HasSiren)
            {
                s_Vehicle.IsSirenOn = true;
                s_Vehicle.IsSirenSilent = true;
            }

            Ped s_Officer = new Ped(ResponseVLib.Utils.GetRandValue(s_OfficerModels), SpawnPoint, 0.0f)
            {
                IsPersistent = true,
                BlockPermanentEvents = true
            };

            s_Officer.WarpIntoVehicle(s_Vehicle, -1);

            s_Officer.RelationshipGroup = new RelationshipGroup("AnimalControl");
            Game.SetRelationshipBetweenRelationshipGroups("AnimalControl", "PLAYER", Relationship.Companion);
            Game.SetRelationshipBetweenRelationshipGroups("COP", "AnimalControl", Relationship.Companion);

            // just to prevent our officer from being killed...
            /*s_Officer.MaxHealth = int.MaxValue;
            s_Officer.Health = int.MaxValue;*/

            s_VehicleBlip = s_Vehicle.AttachBlip();
            s_VehicleBlip.Color = System.Drawing.Color.Orange;
            // s_VehicleBlip.Scale = 0.5f;



            GameFiber fiber = GameFiber.StartNew(delegate
            {
                s_bIsEnroute = true;

                s_Officer.Tasks.DriveToPosition(location.Around(10.0f), 20.0f, VehicleDrivingFlags.Normal, 20.0f).WaitForCompletion(90000);
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
                    // TODO: remove in prod?
                    animal.BlockPermanentEvents = true;

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
                    Blip b = animal.AttachBlip();
                    b.Sprite = BlipSprite.Chop;

                    if (animal.Exists() && animal.Position.DistanceTo(s_Officer) < 100)
                    {
                        // Aim weapon 
                        s_Officer.Inventory.GiveNewWeapon(WeaponHash.SniperRifle, 1, true);

                        // chase after the animal
                        if (animal.Position.DistanceTo(s_Officer) > 10)
                        {
                            Main.MainLogger.Log("AnimalControl: Officer is > 10 away from the animal so go towards the animal");
                            s_Officer.Tasks.AimWeaponAt(animal, -1);
                            s_Officer.Tasks.FollowNavigationMeshToPosition(animal.Position, animal.GetHeadingTowards(animal), 3.0f, 25.0f).WaitForCompletion(20000);
                        }
                        else
                        {
                            GameFiber.Sleep(500);
                        }

                        Main.MainLogger.Log("AnimalControl: Fire the weapon at the animal");

                        // s_Officer.Tasks.FireWeaponAt(animal, -1, FiringPattern.SingleShot).WaitForCompletion(3000);

                        GameFiber.Sleep(2000);
                        // TODO: Play sound here
                        Utils.Notify("Play sound for tranquilizer");
                        animal.Kill();
                        animal.ClearBlood();

                        GameFiber.Sleep(250);
                        
                        // remove weapons
                        s_Officer.RemoveAllWeapons();
                        Main.MainLogger.Log("AnimalControl: Cleared the Officer's weapons");

                        // We kill the animal to act like it was tranquilized
                        if (animal.IsDead)
                        {
                            TranquilizedAnimals.Add(animal);
                            Main.MainLogger.Log("AnimalControl: Animal has been tranquilized");

                            s_Officer.Tasks.FollowNavigationMeshToPosition(animal.Position, animal.GetHeadingTowards(animal), 2.0f, 1.0f).WaitForCompletion();
                            s_Officer.Tasks.PlayAnimation(new AnimationDictionary("amb@medic@standing@tendtodead@idle_a"), "idle_a", 1.0f, AnimationFlags.None);
                            Main.MainLogger.Log("AnimalControl: Animal is tranquilized so lets pick it up");
                            GameFiber.Sleep(3000);
                        }
                    }

                    b.Delete();
                    animal.Delete();
                    Main.MainLogger.Log("AnimalControl: Deleted the animal (AliveAnimals)");
                });

                // Wait before going to get the dead animals
                GameFiber.Sleep(2000);

                DeadAnimals.ForEach(animal =>
                {
                    if (animal.Exists() && animal.Position.DistanceTo(s_Officer) < 100)
                    {
                        Main.MainLogger.Log("AnimalControl: Officer walks towards animal");
                        s_Officer.Tasks.FollowNavigationMeshToPosition(animal.Position, animal.GetHeadingTowards(animal), 2.0f, 1.0f).WaitForCompletion();

                        // Kind of redundant to check if they're dead or not, but might as well be thorough
                        if (animal.IsDead)
                        {
                            Main.MainLogger.Log("AnimalControl: Animal is dead so lets pick it up");
                            s_Officer.Tasks.PlayAnimation(new AnimationDictionary("amb@medic@standing@tendtodead@idle_a"), "idle_a", 1.0f, AnimationFlags.None);
                            GameFiber.Sleep(3000);
                        }

                        animal.Delete();
                        Main.MainLogger.Log("AnimalControl: Deleted the animal (DeadAnimals)");
                    }
                });

                GameFiber.Sleep(1000);



                Rage.Native.NativeFunction.Natives.TASK_GO_TO_ENTITY(s_Officer, s_Vehicle, -1, 5.0f, 2.5f, 0, 0);
                Main.MainLogger.Log("AnimalControl: Officer is going to the vehicle.");

                s_Officer.Tasks.EnterVehicle(s_Vehicle, -1).WaitForCompletion(10000);

                if (!s_Officer.IsInVehicle(s_Vehicle, false))
                {
                    s_Officer.WarpIntoVehicle(s_Vehicle, -1);
                }
                Main.MainLogger.Log("AnimalControl: Officer entered the vehicle.");

                s_Vehicle.Driver.Tasks.CruiseWithVehicle(25.0f, VehicleDrivingFlags.Normal);
                Main.MainLogger.Log("AnimalControl: Officer is cruising with the vehicle.");


                s_VehicleBlip.Delete();

                // Cleanup if the player position > 250 from the s_Officer
                if (Game.LocalPlayer.Character.Position.DistanceTo(s_Officer) > 250)
                {
                    animals.ToList().ForEach(animal =>
                    {
                        if (animal.Exists())
                        {
                            animal.Delete();
                        }
                    });

                    s_Officer.Delete();
                    s_Vehicle.Delete();

                    Main.MainLogger.Log("AnimalControl: Cleaned up");
                }

            }, "AnimalControlFiber");

            Main.s_GameFibers.Add(fiber);
        }
    }
}
