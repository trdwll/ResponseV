// Police patrol the city
namespace ResponseV.Ambient
{
    internal class Patrol
    {
        public static void Initialize()
        {
            Main.MainLogger.Log("AmbientEvent: Patrol started");
            Rage.Game.DisplayNotification("Patrol event");
        }
    }
}
