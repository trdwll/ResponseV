using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using LSPD_First_Response.Mod.API;
using Rage;

using RAGENativeUI;
using RAGENativeUI.Elements;

namespace ResponseV.Plugins
{
    internal class BaitCar 
    {
        private static bool s_bShowInfoPanel;
        
        public static Blip s_BaitCarBlip;
        private static Vehicle s_BaitCar;

        private static GameFiber s_BaitCarFiber;

        private static bool s_bCanBeStolen;
        private static bool s_bIsStolen;
        private static bool s_bPursuit;
        private static bool s_bCarDisabled;

        private static LHandle s_Pursuit;

        public static void BaitCarImpl()
        {
            Main.MainLogger.Log("BaitCar: Initialized");
            BaitCarMenu.Menu();
        }
        static void StartBaitCar()
        {
            s_BaitCarFiber = GameFiber.StartNew(delegate
            {
                Game.RawFrameRender += Game_RawFrameRender;

                Ped ped = null;
                for (;;)
                {
                    GameFiber.Yield();
                    try
                    {
                        if (!s_BaitCar.Exists()) return;

                        if (Game.LocalPlayer.Character.Position.DistanceTo(s_BaitCar) > 50 && (s_BaitCar.Driver == null || ped == null)) // TODO: check if police are not nearby
                        {
                            s_bCanBeStolen = true;

                            List<Vehicle> vehicles = World.GetAllVehicles().Where(v => v.DistanceTo(s_BaitCar) <= 25 && v.IsPoliceVehicle).Take(10).ToList();

                            bool bIsCopNear = ResponseVLib.Native.IsCopNear(s_BaitCar.Position, 25.0f) || vehicles.Count > 0;

                            if (!bIsCopNear)
                            {
                                // if only we could use GetNearbyPeds(16)
                                List<Ped> peds = World.GetAllPeds().Where(p => p.DistanceTo(s_BaitCar) <= 60 && p.IsHuman && p.IsAlive && !p.IsInAnyVehicle(true) && p != Game.LocalPlayer.Character).Take(30).ToList();
                                Main.MainLogger.Log($"BaitCar: Found {peds.Count} possible suspects.");

                                if (peds.Count > 0)
                                {
                                    ped = peds[new Random().Next(peds.Count)];
                                    peds.Remove(ped);
                                    ped.BlockPermanentEvents = true;
                                    ped.IsPersistent = true;

                                    Main.MainLogger.Log("BaitCar: Selected a suspect, assigning a task to GoToOffsetFromEntity");

                                    // Sleep for some time so the ped doesn't immediately go towards the car before we get back to ours.
                                    GameFiber.Sleep(10000);

                                    ped.Tasks.GoToOffsetFromEntity(s_BaitCar, 5.0f, 3.0f, 1.2f).WaitForCompletion(MathHelper.GetRandomInteger(10000, 25000));

                                    Main.MainLogger.Log("BaitCar: GoToOffsetFromEntity complete");

                                    if (ResponseVLib.Utils.GetRandBool()) // && !bIsCopNear)
                                    {
                                        ped.Tasks.EnterVehicle(s_BaitCar, -1).WaitForCompletion();
                                        ped.Tasks.CruiseWithVehicle(MathHelper.GetRandomInteger(30, 60), VehicleDrivingFlags.Normal);
                                        Main.MainLogger.Log("BaitCar: suspect is stealing the vehicle");
                                        s_bIsStolen = true;

                                        break;
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

                            //vehicles.Clear();
                        }
                    }
                    catch (Exception ex)
                    {
                        Main.MainLogger.Log($"BaitCar: {ex.StackTrace}");
                        Utils.CrashNotify();
                    }
                }

                for (;;)
                {
                    GameFiber.Yield();
                    try
                    {
                        if (s_bIsStolen && s_BaitCarBlip.Exists())
                        {
                            s_BaitCarBlip.Color = Color.Red;
                            s_BaitCarBlip.Name = "Stolen Bait Car";
                        }

                        if (Functions.IsPlayerPerformingPullover() && Functions.GetPulloverSuspect(Functions.GetCurrentPullover()) == ped && s_Pursuit == null)
                        {
                            if (ResponseVLib.Utils.GetRandBool() && !s_bPursuit && !s_bCarDisabled)
                            {
                                StartPursuit();
                            }
                        }

                        if ((s_bPursuit && !Functions.IsPursuitStillRunning(s_Pursuit)) || (Functions.IsPedArrested(ped) || ped.IsDead))
                        {
                            if (s_BaitCarBlip.Exists())
                            {
                                s_BaitCarBlip.Color = Color.Yellow;
                                s_BaitCarBlip.Name = "Bait Car";
                            }
                            s_bIsStolen = false;

                            Main.MainLogger.Log("BaitCar: Pursuit is over or the ped was arrested.");

                            break;
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

        private static void Game_RawFrameRender(object sender, GraphicsEventArgs e)
        {
            if (s_bShowInfoPanel)
            {
                Utils.Notify("Showing Info Panel");
            }
            /*Rectangle drawRect = new Rectangle(1, 250, 230, 117);
            e.Graphics.DrawRectangle(drawRect, Color.FromArgb(200, Color.Black));
            e.Graphics.DrawText($"Distance {Game.LocalPlayer.Character.DistanceTo(s_BaitCar)}ft", "Arial", 20.0f, new PointF(3f, 253f), Color.White, drawRect);
            e.Graphics.DrawText("", "Arial", 20.0f, new PointF(3f, 278f), Color.White, drawRect);
            e.Graphics.DrawText("", "Arial", 20.0f, new PointF(3f, 303f), Color.White, drawRect);
            e.Graphics.DrawText("", "Arial", 20.0f, new PointF(3f, 328f), Color.White, drawRect);*/
        }

        static void StartPursuit()
        {
            s_bPursuit = true;
            Main.MainLogger.Log("Bait Car: Starting pursuit fiber");
            GameFiber.StartNew(delegate
            {
                s_Pursuit = Functions.CreatePursuit();

                Functions.AddPedToPursuit(s_Pursuit, s_BaitCar.Driver);

                Functions.SetPursuitIsActiveForPlayer(s_Pursuit, true);
                Functions.SetPursuitCopsCanJoin(s_Pursuit, true);

                GameFiber.Sleep(10000);
                LSPDFR.RequestBackup(Game.LocalPlayer.Character.Position, 2, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.LocalUnit);

            }, "BaitCarPursuitFiber");
        }

        static void EndBaitCar()
        {
            /*
            if (s_BaitCar.IsAlive)
            {
                Utils.Notify("Bait Car: Cleared the ~r~Bait Car ~s~from the world!");
                s_BaitCar?.GetAttachedBlip()?.Delete();
                s_BaitCar?.Delete();
            }*/

            if (s_BaitCarFiber.IsAlive)
            {
                s_BaitCarFiber?.Abort();
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
            private static UIMenuCheckboxItem s_ShowInfoPanelCheckbox;

            private static UIMenuItem s_CleanupBaitCarsItem;
            private static UIMenuItem s_PreviewCarItem;
            private static UIMenuItem s_SpawnCarItem;
            private static UIMenuItem s_ShutOffEngineItem;
            private static UIMenuItem s_LockDoorsItem;
            private static UIMenuItem s_UnlockDoorsItem;

            private static readonly List<UIMenuListItem> s_CarsList = new List<UIMenuListItem>()
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

            private static Vehicle s_PreviewCar;

            public static void Menu()
            {
                // Create a fiber to process our menus
                s_MenusProcessFiber = new GameFiber(ProcessLoop);

                // Create the MenuPool to easily process our menus
                s_MenuPool = new MenuPool();

                // Create our main menu
                s_MainMenu = new UIMenu($"{Utils.PluginPrefix}", "~r~Bait Car");

                // Add our main menu to the MenuPool
                s_MenuPool.Add(s_MainMenu);

                s_MainMenu.RefreshIndex();
                s_MainMenu.OnItemSelect += OnItemSelect;
                s_MainMenu.OnCheckboxChange += MainMenu_OnCheckboxChange;

                s_MainMenu.AddItem(s_ShowInfoPanelCheckbox = new UIMenuCheckboxItem("Show Info Panel", false, "Do you want to show the info panel?"));


                /** Bait Car Menu */
                UIMenuItem BaitCarMenu = new UIMenuItem("Bait Car Settings");
                s_MainMenu.AddItem(BaitCarMenu);

                s_BaitCarMenu = new UIMenu($"{Utils.PluginPrefix}", "~r~Bait Car ~s~Settings");
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

                s_VehicleMenu = new UIMenu($"{Utils.PluginPrefix}", "~r~Bait Car ~s~Options");
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

            private static void MainMenu_OnCheckboxChange(UIMenu sender, UIMenuCheckboxItem checkboxItem, bool Checked)
            {
                if (sender != s_MainMenu || checkboxItem != s_ShowInfoPanelCheckbox) return;

                s_bShowInfoPanel = Checked;
                Game.DisplayNotification("~r~s_bShowInfoPanel: ~b~" + s_bShowInfoPanel);
                Main.MainLogger.Log($"BaitCar: OnCheckboxChange - {s_bShowInfoPanel}");
            }

            private static void BaitCarMenu_OnItemSelect(UIMenu sender, UIMenuItem selectedItem, int index)
            {
                if (sender != s_BaitCarMenu || s_BaitCar == null) return;

                if (selectedItem == s_ShutOffEngineItem)
                {
                    if (s_BaitCar.IsEngineOn)
                    {
                        s_BaitCar.IsEngineOn = false;
                        s_BaitCar.IsDriveable = false;
                        s_BaitCar.EngineHealth = 0.0f;
                        s_BaitCar.AlarmTimeLeft = new TimeSpan(0, 1, 0);
                        s_BaitCar.IndicatorLightsStatus = VehicleIndicatorLightsStatus.Both;
                        s_bCarDisabled = true;
                    }
                    else
                    {
                        s_BaitCar.IsEngineOn = true;
                        s_BaitCar.IsDriveable = true;
                        s_BaitCar.EngineHealth = 1000.0f;
                        s_BaitCar.AlarmTimeLeft = new TimeSpan(0, 0, 0);
                        s_BaitCar.IndicatorLightsStatus = VehicleIndicatorLightsStatus.Off;
                        s_bCarDisabled = false;
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
                        // TODO: Make sure no vehicles are in the location of spawning
                        if (s_BaitCar == null)
                        {
                            UIMenuListItem vehicle = s_CarsList.Where(list => list.Index != 0).First();
                            s_BaitCar = new Vehicle(vehicle.SelectedValue.ToString(), Game.LocalPlayer.Character.GetOffsetPositionFront(6f))
                            {
                                IsPersistent = true
                            };

                            Functions.SetVehicleOwnerName(s_BaitCar, Functions.GetCurrentAgencyScriptName().ToUpper());

                            ResponseVLib.Vehicle.RandomizeLicensePlate(s_BaitCar);

                            Utils.Notify($"Spawned a {vehicle.SelectedValue.ToString()} as a Bait Car");
                            StartBaitCar();
                        }
                    }
                    catch (System.ArgumentNullException ex)
                    {
                        Main.MainLogger.Log($"{ex.StackTrace}", ResponseVLib.Logger.ELogLevel.LL_TRACE);
                        Utils.CrashNotify();
                    }

                    if (s_TrackerCheckbox.Checked && s_BaitCar != null)
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
                            // TODO: Make sure no vehicles are in the location of spawning
                            // TODO: FIX if you press preview car without choosing one then it crashes
                            if (s_PreviewCar == null)
                            {
                                UIMenuListItem vehicle = s_CarsList.Where(list => list.Index != 0).First();
                                s_PreviewCar = new Vehicle(vehicle.SelectedValue.ToString(), Game.LocalPlayer.Character.GetOffsetPositionFront(6f));
                                ResponseVLib.Vehicle.RandomizeLicensePlate(s_PreviewCar);

                                Utils.Notify($"Spawned a {vehicle.SelectedValue.ToString()} as a Preview Bait Car");

                                GameFiber.Sleep(3000);
                                s_PreviewCar?.Delete();
                                s_PreviewCar = null;
                            }
                        }
                        catch (System.ArgumentNullException ex)
                        {
                            Main.MainLogger.Log($"{ex.StackTrace}", ResponseVLib.Logger.ELogLevel.LL_TRACE);
                            Utils.CrashNotify();
                        }
                    }, "BaitCarPreviewVehicle");
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
                    if (s_BaitCar.Exists())
                    {
                        Utils.Notify("Bait Car: Cleared the ~r~Bait Car ~s~from the world!");
                        s_BaitCar.Delete();
                        s_BaitCar = null;
                    }

                    if (s_BaitCarBlip.Exists())
                    {
                        s_BaitCarBlip.Delete();
                    }
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
