using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Police patrol the city
namespace ResponseV.Ambient
{
    public class Patrol
    {
        public static void Initialize()
        {
            Main.MainLogger.Log("AmbientEvent: Patrol started");
            Rage.Game.DisplayNotification("Patrol event");
        }
    }
}
