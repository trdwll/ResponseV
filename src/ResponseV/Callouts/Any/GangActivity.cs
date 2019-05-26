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
    [CalloutInfo("GangActivity", CalloutProbability.VeryHigh)]
    internal sealed class GangActivity : Callout
    {
        private Vector3 g_SpawnPoint;
 
        private List<Ped> m_Members = new List<Ped>();
        private List<Blip> m_Blips = new List<Blip>();
    
        private Model[] m_Models = { "csb_ballasog", "g_f_y_ballas_01", "g_m_y_ballasout_01" };
        private WeaponHash[] g_WeaponList = { WeaponHash.AssaultRifle, WeaponHash.Pistol, WeaponHash.SawnOffShotgun, WeaponHash.Crowbar };

        public override bool OnBeforeCalloutDisplayed()
        {
            g_SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(/*MathHelper.GetRandomInteger(300, 350)*/150f));

            ShowCalloutAreaBlipBeforeAccepting(g_SpawnPoint, 25f);

            string str = "";
            string radio = "";
            int rand = MathHelper.GetRandomInteger(3);
            if (rand == 1)
            {
                str = "Gang Activity";
                radio = LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.GANG_ACTIVITY);
            }
            else if (rand == 2)
            {
                str = "Gang Disturbance";
                radio = LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.GANG_DISTURBANCE);
            }
            else
            {
                str = "Gang Violence";
                radio = LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.GANG_VIOLENCE);
            }

            CalloutMessage = $"Reports of {str}";
            CalloutPosition = g_SpawnPoint;
            
            Functions.PlayScannerAudioUsingPosition($"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.WE_HAVE)} {radio} IN_OR_ON_POSITION", g_SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            LSPDFR.RequestBackup(g_SpawnPoint, MathHelper.GetRandomInteger(2, 3), LSPD_First_Response.EBackupResponseType.Code3, LSPD_First_Response.EBackupUnitType.LocalUnit);

            for (int i = 0; i < MathHelper.GetRandomInteger(4, 10); i++)
            {
                Ped ped = new Ped(ResponseVLib.Utils.GetRandValue(m_Models), g_SpawnPoint, MathHelper.GetRandomInteger(1, 360));
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

                ped.Inventory.GiveNewWeapon(ResponseVLib.Utils.GetRandValue(g_WeaponList), (short)MathHelper.GetRandomInteger(10, 60), true);
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

            if (Game.LocalPlayer.Character.Position.DistanceTo(g_SpawnPoint) < 50)
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
