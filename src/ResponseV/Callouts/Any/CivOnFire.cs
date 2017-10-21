using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using LSPD_First_Response.Engine.Scripting;

namespace ResponseV.Callouts.Any
{
    [CalloutInfo("CivOnFire", CalloutProbability.VeryLow)]
    public class CivOnFire : Callout
    {
        private Vector3 SpawnPoint;
        private Ped vic;
        private Blip blip;

        public override bool OnBeforeCalloutDisplayed()
        {
            SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(Utils.GetRandInt(500, 600)));

            ShowCalloutAreaBlipBeforeAccepting(SpawnPoint, 25f);

            CalloutMessage = "Reports of a Civilian on Fire";
            CalloutPosition = SpawnPoint;

            Functions.PlayScannerAudioUsingPosition(
                $"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.WE_HAVE)} CRIME_CIV_ON_FIRE IN_OR_ON_POSITION", SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            vic = new Ped(SpawnPoint)
            {
                IsOnFire = true
            };
            // vic.Kill();

            blip = new Blip(SpawnPoint)
            {
                IsRouteEnabled = true
            };
            blip.EnableRoute(System.Drawing.Color.Yellow);

            DamagePack.ApplyDamagePack(vic, Utils.GetRandValue(
                DamagePack.Burnt_Ped_0, DamagePack.Burnt_Ped_Head_Torso,
                DamagePack.Burnt_Ped_Left_Arm, DamagePack.Burnt_Ped_Limbs,
                DamagePack.Burnt_Ped_Right_Arm), 100, 1);

            BetterEMS.API.EMSFunctions.OverridePedDeathDetails(vic, "", "Fire", Game.GameTime, (float)Utils.GetRandDouble() + .1f);

            LSPDFR.RequestEMS(vic.Position);

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            if (Game.LocalPlayer.Character.Position.DistanceTo(SpawnPoint) < 20)
            {
                blip.IsRouteEnabled = false;
                if (BetterEMS.API.EMSFunctions.DidEMSRevivePed(vic) == true)
                {
                    End();
                }

                if (BetterEMS.API.EMSFunctions.DidEMSRevivePed(vic) == false)
                {
                    Arrest_Manager.API.Functions.CallCoroner(true);
                    End();
                }
            }
        }

        public override void End()
        {
            blip.Delete();
            vic.Dismiss();
            base.End();
        }
    }
}