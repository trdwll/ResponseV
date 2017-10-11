using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rage;
using LSPD_First_Response;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using LSPD_First_Response.Engine.Scripting;

namespace ResponseV.Callouts.Any
{
    [CalloutInfo("Overdose", CalloutProbability.VeryHigh)]
    public class Overdose : Callout
    {
        private Vector3 SpawnPoint;
        private Ped vic;
        private Blip blip;

        private Model[] models = { "a_f_y_rurmeth_01", "a_m_m_rurmeth_01", "a_m_y_methhead_01" };

        public override bool OnBeforeCalloutDisplayed()
        {
            SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(/*Utils.getRandInt(300, 350)*/150f));

            ShowCalloutAreaBlipBeforeAccepting(SpawnPoint, 25f);

            CalloutMessage = "Reports of an Overdose";
            CalloutPosition = SpawnPoint;

            Functions.PlayScannerAudioUsingPosition(
                $"{Utils.Radio.getRandomSound(Utils.Radio.WE_HAVE)} " +
                $"{Utils.Radio.getRandomSound(Utils.Radio.OVERDOSE)} IN_OR_ON_POSITION", SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {

            vic = new Ped(Utils.getRandValue(models), SpawnPoint, Utils.getRandInt(1, 360));
            blip = new Blip(vic);
            blip.EnableRoute(System.Drawing.Color.Orange);
            
            //blip.EnableRoute(System.Drawing.Color.Orange);

            if (Utils.getRandInt(0, 1) == 1)
            {
                vic.Kill();
            }
            else
            {
                vic.Health = 5;
            }

            Utils.RequestBackup(SpawnPoint, 1, EBackupResponseType.Code3, EBackupUnitType.LocalUnit);
            Utils.RequestBackup(SpawnPoint, 1, EBackupResponseType.Code3, EBackupUnitType.Ambulance);

            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            base.OnCalloutNotAccepted();
        }

        public override void Process()
        {
            base.Process();

            if (Game.LocalPlayer.Character.Position.DistanceTo(SpawnPoint) < 50)
            {

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