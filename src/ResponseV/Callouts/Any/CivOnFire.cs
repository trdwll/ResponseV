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
        private Vector3 m_SpawnPoint;
        private Ped m_Victim;
        private Blip m_Blip;

        public override bool OnBeforeCalloutDisplayed()
        {
            m_SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(Utils.GetRandInt(500, 600)));

            ShowCalloutAreaBlipBeforeAccepting(m_SpawnPoint, 25f);

            CalloutMessage = "Reports of a Civilian on Fire";
            CalloutPosition = m_SpawnPoint;

            Functions.PlayScannerAudioUsingPosition(
                $"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.WE_HAVE)} CRIME_CIV_ON_FIRE IN_OR_ON_POSITION", m_SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            m_Victim = new Ped(m_SpawnPoint)
            {
                IsOnFire = true
            };
            // m_Victim.Kill();

            m_Blip = new Blip(m_SpawnPoint)
            {
                IsRouteEnabled = true
            };
            m_Blip.EnableRoute(System.Drawing.Color.Yellow);

            DamagePack.ApplyDamagePack(m_Victim, Utils.GetRandValue(
                DamagePack.Burnt_Ped_0, DamagePack.Burnt_Ped_Head_Torso,
                DamagePack.Burnt_Ped_Left_Arm, DamagePack.Burnt_Ped_Limbs,
                DamagePack.Burnt_Ped_Right_Arm), 100, 1);

            LSPDFR.RequestEMS(m_Victim.Position);

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            if (Game.LocalPlayer.Character.Position.DistanceTo(m_SpawnPoint) < 20)
            {
                m_Blip.IsRouteEnabled = false;

                End();
            }
        }

        public override void End()
        {
            m_Blip.Delete();
            m_Victim.Dismiss();
            base.End();
        }
    }
}