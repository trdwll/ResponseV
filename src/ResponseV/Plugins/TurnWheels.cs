using System;
using Rage;

namespace ResponseV.Plugins
{
    internal class TurnWheels
    {
        public static void TurnWheelsImpl()
        {
            Main.MainLogger.Log("TurnWheels: Initialized");
            float SteeringAngle = 0f;
            GameFiber fiber = GameFiber.StartNew(delegate
            {
                for (;;)
                {
                    GameFiber.Yield();
                    try
                    {
                        Vehicle LastVehicle = Game.LocalPlayer.Character.LastVehicle;
                        if (LastVehicle && LastVehicle.Speed == 0f)
                        {
                            if (LastVehicle.SteeringAngle > 20f)
                            {
                                SteeringAngle = 40f;
                            }
                            else if (LastVehicle.SteeringAngle < -20f)
                            {
                                SteeringAngle = -40f;
                            }
                            else if (LastVehicle.SteeringAngle > 5f || LastVehicle.SteeringAngle < -5f)
                            {
                                SteeringAngle = LastVehicle.SteeringAngle;
                            }

                            LastVehicle.SteeringAngle = SteeringAngle;
                        }
                    }
                    catch (Exception ex)
                    {
                        Main.MainLogger.Log($"TurnWheels: {ex.StackTrace}");
                        Utils.CrashNotify();
                    }
                }
            }, "TurnWheelsFiber");

            Main.g_GameFibers.Add(fiber);
        }
    }
}
