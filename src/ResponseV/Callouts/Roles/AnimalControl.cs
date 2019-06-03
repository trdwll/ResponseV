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
        private static bool s_bOnScene;

        private static GameFiber s_Fiber1;
        private static GameFiber s_Fiber2;

        private static Model[] s_VehicleModels = { "DUBSTA3", "SPEEDO" };
        private static Model[] s_OfficerModels = { "s_m_m_paramedic_01", "s_m_m_scientist_01" };

        public static bool Request(Vector3 location)
        {
            Ped[] animals = World.GetAllPeds().Where(p => p.DistanceTo(location) <= 200 && !p.IsHuman).ToArray();
            if (animals.Length > 0)
            {
                Request_Impl(location, animals);
                return true;
            }

            Main.MainLogger.Log("AnimalControl: Unable to request Animal Control, no animals are nearby.");

            return false;
        }

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

        private static void Request_Impl(Vector3 location, params Ped[] animals)
        {
            if (s_Vehicle.Exists() || s_bIsEnroute)
            {
                // Main.MainLogger.Log("AnimalControl: Exists or they're already enroute.");
                return;
            }

            string UnitPrefix = $"{ResponseVLib.Utils.GetRandValue("AC", "Animal Control")}-{MathHelper.GetRandomInteger(100)}";

            Main.MainLogger.Log("AnimalControl: Requested");
            Utils.NotifyDispatchTo("Player", ResponseVLib.Utils.GetRandValue("Animal Control en-route.", "Animal Control has been notified."));

            GameFiber.Sleep(2500);
            Game.DisplayNotification($"{UnitPrefix} to Dispatch: I'll be en-route to the scene.");

            Vector3 SpawnPoint = World.GetNextPositionOnStreet(location.AroundPosition(200.0f));
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

            s_Officer = new Ped(ResponseVLib.Utils.GetRandValue(s_OfficerModels), SpawnPoint, 0.0f)
            {
                IsPersistent = true,
                BlockPermanentEvents = true
            };

            s_Officer.WarpIntoVehicle(s_Vehicle, -1);

            s_Officer.RelationshipGroup = new RelationshipGroup("AnimalControl");
            Game.SetRelationshipBetweenRelationshipGroups("AnimalControl", "PLAYER", Relationship.Companion);
            Game.SetRelationshipBetweenRelationshipGroups("COP", "AnimalControl", Relationship.Companion);

            // TODO: Check if the officer is dead

            // just to prevent our officer from being killed...
            s_Officer.MaxHealth = int.MaxValue;
            s_Officer.Health = int.MaxValue;

            s_VehicleBlip = s_Vehicle.AttachBlip();
            s_VehicleBlip.Color = System.Drawing.Color.Orange;
            // s_VehicleBlip.Scale = 0.5f;

            s_Fiber1 = GameFiber.StartNew(delegate
            {
                s_bIsEnroute = true;

                s_Officer.Tasks.DriveToPosition(World.GetNextPositionOnStreet(location.Around(10.0f, 20.0f)), 20.0f, VehicleDrivingFlags.Normal, 20.0f).WaitForCompletion(120000);
                Main.MainLogger.Log("AnimalControl: Driving to the location");

                if (Vector3.Distance(s_Vehicle.Position, location) > 30.0f)
                {
                    s_Vehicle.Position = location.Around(8.0f);
                }

                animals.ToList().ForEach(animal =>
                {
                    if (animal.IsAlive)
                    {
                        animal.Tasks.Flee(animal, 50.0f, 10);
                    }
                });

                s_Officer.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen).WaitForCompletion(10000);

                s_bOnScene = true;
                Main.MainLogger.Log("AnimalControl: On Scene");
                Game.DisplayNotification($"{UnitPrefix} to Dispatch: I'll be on scene.");

                /** Sort through the animals. */
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


                /** Iterate over the alive animals so we can tranquilize them first as they pose a threat. */
                AliveAnimals.ForEach(animal =>
                {
                    if (animal.Exists() && animal.IsVisible)
                    {
                        Blip b = animal.AttachBlip();
                        //b.Color = System.Drawing.Color.Yellow;
                        b.Sprite = BlipSprite.Chop;

                        if (animal.Position.DistanceTo(SpawnPoint) <= 200)
                        {
                            // Aim weapon 
                            s_Officer.Inventory.GiveNewWeapon(WeaponHash.SniperRifle, 1, true);

                            // chase after the animal if it's <= 200
                            if (animal.Position.DistanceTo(s_Officer) <= 200)
                            {
                                Main.MainLogger.Log("AnimalControl: Officer is <= 200 away from the animal so go towards the animal (AliveAnimals)");
                                s_Officer.Tasks.FollowNavigationMeshToPosition(animal.Position, animal.GetHeadingTowards(animal), 3.0f, 25.0f).WaitForCompletion(20000);
                                s_Officer.Tasks.AimWeaponAt(animal, 500);
                            }

                            Main.MainLogger.Log("AnimalControl: Fire the weapon at the animal (AliveAnimals)");

                            // s_Officer.Tasks.FireWeaponAt(animal, -1, FiringPattern.SingleShot).WaitForCompletion(3000);

                            GameFiber.Sleep(500);
                            ResponseVLib.Utils.PlaySound("dart.03.wav");

                            GameFiber.Sleep(250);
                            animal.Kill();
                            animal.ClearBlood();

                            GameFiber.Sleep(250);

                            s_Officer.RemoveAllWeapons();

                            // We kill the animal to act like it was tranquilized
                            if (animal.IsDead)
                            {
                                Main.MainLogger.Log("AnimalControl: Animal has been tranquilized (AliveAnimals)");

                                s_Officer.Tasks.FollowNavigationMeshToPosition(animal.Position, animal.GetHeadingTowards(animal), 2.0f, 1.0f).WaitForCompletion();
                                s_Officer.Tasks.PlayAnimation(new AnimationDictionary("amb@medic@standing@tendtodead@idle_a"), "idle_a", 1.0f, AnimationFlags.None);
                                Main.MainLogger.Log("AnimalControl: Animal is tranquilized so lets pick it up (AliveAnimals)");
                                GameFiber.Sleep(3000);
                            }
                        }

                        b.Delete();
                        animal.IsVisible = false;
                        animal.Position = new Vector3(0f, 0f, 0f);
                        Main.MainLogger.Log("AnimalControl: Hid the animal (AliveAnimals)");
                    }
                });

                // Wait before going to get the dead animals
                GameFiber.Sleep(2000);

                /** Iterate over the dead animals since they don't pose a risk we do them last. */
                DeadAnimals.ForEach(animal =>
                {
                    if (animal.Exists() && animal.Position.DistanceTo(SpawnPoint) <= 200 && animal.IsVisible)
                    {
                        Main.MainLogger.Log("AnimalControl: Officer walks towards animal (DeadAnimals)");
                        s_Officer.Tasks.FollowNavigationMeshToPosition(animal.Position, animal.GetHeadingTowards(animal), 2.0f, 1.0f).WaitForCompletion();

                        Main.MainLogger.Log("AnimalControl: Animal is dead so lets pick it up");
                        s_Officer.Tasks.PlayAnimation(new AnimationDictionary("amb@medic@standing@tendtodead@idle_a"), "idle_a", 1.0f, AnimationFlags.None);
                        GameFiber.Sleep(3000);

                        animal.IsVisible = false;
                        animal.Position = new Vector3(0f, 0f, 0f);
                        Main.MainLogger.Log("AnimalControl: Hid the animal (DeadAnimals)");
                    }
                });

                GameFiber.Sleep(1000);

                Game.DisplaySubtitle("Animal Control Officer: See you later.", 2000);
                GameFiber.Sleep(500);
                Game.DisplayNotification($"{UnitPrefix} to Dispatch: I'll be clear and en-route back to the station.");

                Rage.Native.NativeFunction.Natives.TASK_GO_TO_ENTITY(s_Officer, s_Vehicle, 30000, 5.0f, 2.5f, 0, 0);
                Main.MainLogger.Log("AnimalControl: Officer is going to the vehicle.");

                s_Officer.Tasks.EnterVehicle(s_Vehicle, -1).WaitForCompletion(10000);

                if (!s_Officer.IsInVehicle(s_Vehicle, false))
                {
                    s_Officer.WarpIntoVehicle(s_Vehicle, -1);
                }
                Main.MainLogger.Log("AnimalControl: Officer entered the vehicle.");

                s_Vehicle.Driver.Tasks.CruiseWithVehicle(30.0f, VehicleDrivingFlags.Normal);
                Main.MainLogger.Log("AnimalControl: Officer is cruising with the vehicle.");

                s_VehicleBlip.Delete();

                GameFiber.Sleep(20000);

                Main.MainLogger.Log("AnimalControl: Deleting all animals");

                s_Officer.Delete();
                s_Vehicle.Delete();

                s_Officer = null;
                s_Vehicle = null;

                s_bIsEnroute = false;
                s_bOnScene = false;

                animals.ToList().ForEach(animal =>
                {
                    if (animal.Exists())
                    {
                        animal.Delete();
                    }
                });

            }, "AnimalControlFiber");


            {
            //    s_Fiber2 = GameFiber.StartNew(delegate
            //    {
            //        for (;;)
            //        {
            //            GameFiber.Yield();

            //            // Cleanup if the s_Vehicle position >= 150 from the SpawnPoint
            //            if (s_Vehicle.DistanceTo(SpawnPoint) >= 150 && s_bOnScene)
            //            {
            //                // s_Officer?.Delete();
            //                // s_Vehicle?.Delete();
            //                Main.MainLogger.Log("Cleaned this shti up");
            //            }
            //        }
            //    }, "AnimalControlCheckFiber");
            }
        }

        public static void CleanupAnimalControl()
        {
            if (s_bIsEnroute && s_bOnScene)
            {
                s_Fiber1.Abort();
                // s_Fiber2.Abort();

                s_Officer?.Delete();
                s_Vehicle?.Delete();

                s_bIsEnroute = false;
                s_bOnScene = false;

                Main.MainLogger.Log("AnimalControl: Cleaned up");
            }
        }
    }
}
