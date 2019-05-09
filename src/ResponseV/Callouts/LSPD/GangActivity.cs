using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using LSPD_First_Response.Engine.Scripting;

namespace ResponseV.Callouts.LSPD
{
    [CalloutInfo("GangActivity", CalloutProbability.VeryHigh)]
    public class GangActivity : Callout
    {
        private Vector3 m_SpawnPoint;
 
        private List<Ped> m_Members = new List<Ped>();
        private List<Blip> m_Blips = new List<Blip>();
    
        private Model[] m_Models = { "csb_ballasog", "g_f_y_ballas_01", "g_m_y_ballasout_01" };
        private WeaponHash[] m_WeaponList = { WeaponHash.AssaultRifle, WeaponHash.Pistol, WeaponHash.SawnOffShotgun, WeaponHash.Crowbar };

        public override bool OnBeforeCalloutDisplayed()
        {
            m_SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(/*Utils.getRandInt(300, 350)*/150f));

            ShowCalloutAreaBlipBeforeAccepting(m_SpawnPoint, 25f);

            CalloutMessage = "Reports of Gang Activity";
            CalloutPosition = m_SpawnPoint;
            
            Functions.PlayScannerAudioUsingPosition(
                $"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.WE_HAVE)} " +
                $"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.GANG)} IN_OR_ON_POSITION", m_SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            LSPDFR.RequestBackup(m_SpawnPoint, Utils.GetRandInt(2, 3), LSPD_First_Response.EBackupResponseType.Code3, LSPD_First_Response.EBackupUnitType.LocalUnit);

            for (int i = 0; i < Utils.GetRandInt(4, 10); i++)
            {
                Ped ped = new Ped(Utils.GetRandValue(m_Models), m_SpawnPoint, Utils.GetRandInt(1, 360));
                Blip blip = new Blip(ped)
                {
                    IsFriendly = false
                };

                m_Members.Add(ped);
                m_Blips.Add(blip);
                ped.Tasks.Wander();

                ped.IsPersistent = true;
                ped.RelationshipGroup = RelationshipGroup.AmbientGangBallas;
                ped.CanAttackFriendlies = true;

                ped.Inventory.GiveNewWeapon(Utils.GetRandValue(m_WeaponList), (short)Utils.GetRandInt(10, 60), true);
            }

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            // ped == null for some reason...
            m_Members.ForEach(ped =>
            {
                if (ped && ped.GetAttachedBlip() && (ped.IsDead || Functions.IsPedArrested(ped)))
                {
                    ped.GetAttachedBlip().Delete();
                }
            });

            if (Game.LocalPlayer.Character.Position.DistanceTo(m_SpawnPoint) < 50)
            {

            }
        }

        public override void End()
        {
            m_Members.ForEach(ped => ped.Dismiss());
            m_Blips.ForEach(blip => blip.Delete());

            base.End();
        }
    }
}
