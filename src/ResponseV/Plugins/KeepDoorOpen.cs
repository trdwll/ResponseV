using System;
using Rage;

namespace ResponseV.Plugins
{
    internal class KeepDoorOpen
    {
        public static void KeepDoorOpenImpl()
        {
            Main.MainLogger.Log("KeepDoorOpen: Initialized");
            GameFiber fiber = GameFiber.StartNew(delegate
            {
                for (;;)
                {
                    GameFiber.Yield();
                    try
                    {
                        bool bIsInPoliceVehicle = Game.LocalPlayer.Character.IsInAnyPoliceVehicle;
                        bool bIsControlPressed = Game.IsControlPressed(2, GameControl.VehicleExit);
                        if (bIsInPoliceVehicle && bIsControlPressed)
                        {
                            GameFiber.Sleep(150);
                            if (bIsInPoliceVehicle && bIsControlPressed)
                            {
                                Game.LocalPlayer.Character.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Main.MainLogger.Log($"TurnWheels: {ex.StackTrace}");
                        Utils.CrashNotify();
                    }
                }
            }, "KeepDoorOpenFiber");

            Main.g_GameFibers.Add(fiber);
        }
    }
}
