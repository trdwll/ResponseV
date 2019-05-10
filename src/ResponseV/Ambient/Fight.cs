using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rage;

namespace ResponseV.Ambient
{
    public class Fight
    {
        public static void Initialize()
        {
            Main.MainLogger.Log("AmbientEvent: Fight started");

            Ped Player = Game.LocalPlayer.Character;

            List<Ped> Peds = new List<Ped>();
            for (int i = 0; i < Utils.GetRandInt(2, 4); i++)
            {
                // PedModels returns animals also so maybe filter this later?
                // TODO: Fix peds spawning in the air also
                Peds.Add(new Ped(Utils.GetRandValue(Model.PedModels), Player.Position.Around(25f, 30f), Utils.GetRandInt(1, 360)));
            }

            Peds.ForEach(p =>
            {
                Ped randPed = Peds.Take(1).First();

                if (randPed == p)
                {
                    randPed = Peds.Take(1).First();
                }

                p.Tasks.FightAgainst(randPed);

                // lol this is annoying
                //if (Utils.GetRandBool())
                //{
                //    p.Tasks.FightAgainst(Player);
                //}
            });
        }
    }
}
