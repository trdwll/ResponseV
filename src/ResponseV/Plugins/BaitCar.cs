using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Rage;

using RAGENativeUI;
using RAGENativeUI.Elements;

namespace ResponseV.Plugins
{
    internal class BaitCar 
    {
        public static Blip s_BaitCarBlip;
        private static Vehicle s_BaitCar;

        private static GameFiber s_BaitCarFiber;

        private static bool s_bCanBeStolen;

        public static void BaitCarImpl()
        {
            Main.MainLogger.Log("BaitCar: Initialized");
            BaitCarMenu.Menu();
        }

        static void StartBaitCar()
        {
            s_BaitCarFiber = GameFiber.StartNew(delegate
            {
                for (;;)
                {
                    GameFiber.Yield();
                    try
                    {
                        if (Game.LocalPlayer.Character.Position.DistanceTo(s_BaitCar) > 50 && s_BaitCar.Driver == null) // TODO: check if police are not nearby
                        {
                            s_bCanBeStolen = true;

                            // if only we could use GetNearbyPeds(16)
                            List<Ped> peds = World.GetAllPeds().Where(p => p.DistanceTo(s_BaitCar) <= 20 && p.IsHuman && p.IsAlive).Take(30).ToList();
                            Main.MainLogger.Log($"BaitCar: Found {peds.Count} possible suspects.");

                            Ped ped = peds[new Random().Next(peds.Count)];
                            peds.Remove(ped);
                            ped.BlockPermanentEvents = true;
                            ped.IsPersistent = true;

                            Main.MainLogger.Log("BaitCar: Selected a suspect, assigning a task to GoToOffsetFromEntity");

                            ped.Tasks.GoToOffsetFromEntity(s_BaitCar, 5.0f, 3.0f, 1.2f).WaitForCompletion(MathHelper.GetRandomInteger(10000, 25000));

                            Main.MainLogger.Log("BaitCar: GoToOffsetFromEntity complete");

                            if (Utils.GetRandBool())
                            {
                                ped.Tasks.EnterVehicle(s_BaitCar, -1).WaitForCompletion();
                            }
                            else
                            {
                                // Ped didn't enter the vehicle so let's just dismiss them and get another ped nearby
                                GameFiber.Sleep(MathHelper.GetRandomInteger(3000, 8000));
                                ped.Dismiss();
                                Main.MainLogger.Log("BaitCar: Suspect didn't enter the car");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Main.MainLogger.Log($"BaitCar: {ex.StackTrace}");
                        Utils.CrashNotify();
                    }
                }
            }, "BaitCarFiber");
        }

        static void EndBaitCar()
        {
            Utils.Notify("Bait Car: Cleared the ~r~Bait Car ~s~from the world!");
            if (s_BaitCar.IsAlive)
            {
                s_BaitCar?.GetAttachedBlip()?.Delete();
                s_BaitCar?.Delete();
            }

            if (s_BaitCarFiber.IsAlive)
            {
                s_BaitCarFiber.Abort();
                s_BaitCarFiber = null;
            }
        }

        class BaitCarMenu
        {
            private static GameFiber s_MenusProcessFiber;

            private static UIMenu s_MainMenu;
            private static UIMenu s_VehicleMenu;
            private static UIMenu s_BaitCarMenu;

            private static UIMenuCheckboxItem s_TrackerCheckbox;

            private static UIMenuItem s_CleanupBaitCarsItem;
            private static UIMenuItem s_PreviewCarItem;
            private static UIMenuItem s_SpawnCarItem;
            private static UIMenuItem s_ShutOffEngineItem;
            private static UIMenuItem s_LockDoorsItem;
            private static UIMenuItem s_UnlockDoorsItem;

            private static List<UIMenuListItem> s_CarsList = new List<UIMenuListItem>()
            {
                new UIMenuListItem("Cars Models (Super)", "Choose a vehicle that you want to spawn as the bait car.",
                    "EMPTY",
                    "ADDER",
                    "AUTARCH",
                    "BULLET",
                    "CHEETAH",
                    "CYCLONE",
                    "ENTITYXF",
                    "ENTITYXF2",
                    "FMJ",
                    "GP1",
                    "INFERNUS",
                    "LE7B",
                    "NERO",
                    "NERO2",
                    "OSIRIS",
                    "PENETRATOR",
                    "PROTOTIPO",
                    "REAPER",
                    "SC1",
                    "SHEAVA",
                    "TEZERACT",
                    "TYRANT",
                    "ZENTORNO"
                ),
                new UIMenuListItem("Cars Models (Sports)", "Choose a vehicle that you want to spawn as the bait car.",
                    "EMPTY",
                    "BUFFALO", "BUFFALO2",
                    "CARBONIZZARE",
                    "COMET2", "COMET3", "COMET5",
                    "COQUETTE",
                    "ELEGY",
                    "FUROREGT",
                    "FUSILADE",
                    "JESTER",
                    "LYNX",
                    "MASSACRO",
                    "NEON",
                    "PARIAH",
                    "REVOLTER",
                    "SEVEN70",
                    "SURANO"
                ),
                new UIMenuListItem("Cars Models (Muscle)", "Choose a vehicle that you want to spawn as the bait car.",
                    "EMPTY",
                    "BUCCANEER", "BUCCANEER2",
                    "DOMINATOR", "DOMINATOR3",
                    "ELLIE",
                    "GAUNTLET",
                    "SABREGT", "SABREGT2"
                )
            };

            private static MenuPool s_MenuPool;

            public static void Menu()
            {
                // Create a fiber to process our menus
                s_MenusProcessFiber = new GameFiber(ProcessLoop);

                // Create the MenuPool to easily process our menus
                s_MenuPool = new MenuPool();

                // Create our main menu
                s_MainMenu = new UIMenu("Response~y~V~w~", "~r~Bait Car");

                // Add our main menu to the MenuPool
                s_MenuPool.Add(s_MainMenu);

                s_MainMenu.RefreshIndex();
                s_MainMenu.OnItemSelect += OnItemSelect;


                /** Bait Car Menu */
                UIMenuItem BaitCarMenu = new UIMenuItem("Bait Car Settings");
                s_MainMenu.AddItem(BaitCarMenu);

                s_BaitCarMenu = new UIMenu("Response~y~V~w~", "~r~Bait Car ~s~Settings");
                s_BaitCarMenu.OnItemSelect += BaitCarMenu_OnItemSelect;

                s_MenuPool.Add(s_BaitCarMenu);

                s_ShutOffEngineItem = new UIMenuItem("Turn off engine", "Shut off the engine of the ~r~Bait Car.");
                s_BaitCarMenu.AddItem(s_ShutOffEngineItem);

                s_LockDoorsItem = new UIMenuItem("Lock doors", "Lock the doors on the ~r~Bait Car.");
                s_BaitCarMenu.AddItem(s_LockDoorsItem);

                s_UnlockDoorsItem = new UIMenuItem("Unlock doors", "Unlock the doors on the ~r~Bait Car.");
                s_BaitCarMenu.AddItem(s_UnlockDoorsItem);

                s_BaitCarMenu.RefreshIndex();

                s_MainMenu.BindMenuToItem(s_BaitCarMenu, BaitCarMenu);
                /** End Bait Car Menu */


                /** Vehicle Menu */
                UIMenuItem menuItem = new UIMenuItem("Vehicle Menu");
                s_MainMenu.AddItem(menuItem);

                s_VehicleMenu = new UIMenu("Response~y~V~w~", "~r~Bait Car ~s~Options");
                s_VehicleMenu.OnListChange += VehicleMenu_OnListChange;
                s_VehicleMenu.OnItemSelect += VehicleMenu_OnItemSelect;

                s_MenuPool.Add(s_VehicleMenu); 
                
                s_VehicleMenu.AddItem(s_TrackerCheckbox = new UIMenuCheckboxItem("Tracker", true, "Do you want to add a tracking device on the vehicle?"));

                foreach (UIMenuListItem c in s_CarsList)
                {
                    s_VehicleMenu.AddItem(c);
                }

                s_VehicleMenu.RefreshIndex();

                s_PreviewCarItem = new UIMenuItem("Preview Car", "Don't know what car this is? Spawn it for 3 seconds.");
                s_VehicleMenu.AddItem(s_PreviewCarItem);

                s_SpawnCarItem = new UIMenuItem("Spawn Car", "Spawn the car in front of you.");
                s_VehicleMenu.AddItem(s_SpawnCarItem);

                s_MainMenu.BindMenuToItem(s_VehicleMenu, menuItem); 
                /** End Vehicle Menu*/

                s_CleanupBaitCarsItem = new UIMenuItem("Delete Bait Car", "Do you want to delete the Bait Car that you've spawned in the world?");
                s_MainMenu.AddItem(s_CleanupBaitCarsItem);


                s_MenusProcessFiber.Start();

                BaitCarMenu.Enabled = s_BaitCar.IsAlive;

                GameFiber.Hibernate();
            }

            private static void BaitCarMenu_OnItemSelect(UIMenu sender, UIMenuItem selectedItem, int index)
            {
                if (sender != s_BaitCarMenu) return;

                if (selectedItem == s_ShutOffEngineItem)
                {
                    s_BaitCar.IsEngineOn = !true;
                    s_BaitCar.IsDriveable = !true;
                    if (!s_BaitCar.IsEngineOn)
                    {
                        s_BaitCar.AlarmTimeLeft = new TimeSpan(0, 1, 0);
                        s_BaitCar.IndicatorLightsStatus = VehicleIndicatorLightsStatus.Both;
                    }
                    else
                    {
                        s_BaitCar.AlarmTimeLeft = new TimeSpan(0, 0, 0);
                        s_BaitCar.IndicatorLightsStatus = VehicleIndicatorLightsStatus.Off;
                    }
                }
                else if (selectedItem == s_LockDoorsItem)
                {
                    s_BaitCar.LockStatus = VehicleLockStatus.Locked;
                }
                else if (selectedItem == s_UnlockDoorsItem)
                {
                    s_BaitCar.LockStatus = VehicleLockStatus.Unlocked;
                }
            }

            private static void VehicleMenu_OnItemSelect(UIMenu sender, UIMenuItem selectedItem, int index)
            {
                if (sender != s_VehicleMenu) return; 

                if (selectedItem == s_SpawnCarItem)
                {
                    /*GameFiber.StartNew(delegate
                    {*/
                    try
                    {
                        UIMenuListItem vehicle = s_CarsList.Where(list => list.Index != 0).First();
                        s_BaitCar = new Vehicle(vehicle.SelectedValue.ToString(), Game.LocalPlayer.Character.GetOffsetPositionFront(6f));
                        s_BaitCar.IsPersistent = true;
                        
                        GTAV.VehicleExtensions.RandomizeLicensePlate(s_BaitCar);

                        Utils.Notify($"Spawned a {vehicle.SelectedValue.ToString()} as a Bait Car");
                    }
                    catch (System.ArgumentNullException ex)
                    {
                        Main.MainLogger.Log($"{ex.StackTrace}", Logger.ELogLevel.LL_TRACE);
                        Utils.CrashNotify();
                    }

                    if (s_TrackerCheckbox.Checked)
                    {
                        s_BaitCarBlip = s_BaitCar?.AttachBlip();
                        s_BaitCarBlip.Color = Color.Yellow;
                        s_BaitCarBlip.Scale = 0.5f;
                        s_BaitCarBlip.Name = "Bait Car";
                    }
                    /*});*/
                }
                else if (selectedItem == s_PreviewCarItem)
                {
                    GameFiber.StartNew(delegate
                    {
                        try
                        {
                            UIMenuListItem vehicle = s_CarsList.Where(list => list.Index != 0).First();
                            Vehicle BaitCar = new Vehicle(vehicle.SelectedValue.ToString(), Game.LocalPlayer.Character.GetOffsetPositionFront(6f));
                            GTAV.VehicleExtensions.RandomizeLicensePlate(BaitCar);

                            Utils.Notify($"Spawned a {vehicle.SelectedValue.ToString()} as a Preview Bait Car");

                            GameFiber.Sleep(3000);
                            BaitCar?.Delete();
                        }
                        catch (System.ArgumentNullException ex)
                        {
                            Main.MainLogger.Log($"{ex.StackTrace}", Logger.ELogLevel.LL_TRACE);
                            Utils.CrashNotify();
                        }
                    });
                }
            }

            private static void VehicleMenu_OnListChange(UIMenu sender, UIMenuListItem listItem, int newIndex)
            {
                foreach (UIMenuListItem c in s_CarsList)
                {
                    if (c != listItem)
                    {
                        c.Index = 0;
                    }
                }
            }

            public static void OnItemSelect(UIMenu sender, UIMenuItem selectedItem, int index)
            {
                if (sender != s_MainMenu) return; 

                if (selectedItem == s_CleanupBaitCarsItem)
                {
                    EndBaitCar();
                }
            }

            // The method that contains a loop to handle our menus
            public static void ProcessLoop()
            {
                while (true)
                {
                    GameFiber.Yield();

                    if (Game.IsKeyDown(Keys.F5) && !s_MenuPool.IsAnyMenuOpen()) // Our menu on/off switch.
                    {
                        s_MainMenu.Visible = !s_MainMenu.Visible;
                    }

                    s_MenuPool.ProcessMenus();
                }
            }
        }
    }
}
