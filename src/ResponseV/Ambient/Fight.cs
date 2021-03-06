﻿using Rage;

namespace ResponseV.Ambient
{
    internal class Fight
    {
        public static void Initialize()
        {
            Main.MainLogger.Log("AmbientEvents: Fight started");
            Ped Player = Game.LocalPlayer.Character;

            // PedModels returns animals also so maybe filter this later?
            Ped ped1 = new Ped(ResponseVLib.Utils.GetRandValue(Model.PedModels), Player.Position.Around(25f, 30f), MathHelper.GetRandomInteger(1, 360));
            Ped ped2 = new Ped(ResponseVLib.Utils.GetRandValue(Model.PedModels), Player.Position.Around(25f, 30f), MathHelper.GetRandomInteger(1, 360));

            ped1.Tasks.FightAgainst(ped2);
            ped2.Tasks.FightAgainst(ped1);
        }
    }
}
