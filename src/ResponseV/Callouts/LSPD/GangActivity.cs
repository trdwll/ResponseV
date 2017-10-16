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
        private Vector3 SpawnPoint;
 
        private List<Ped> members = new List<Ped>();
        private List<Blip> blips = new List<Blip>();
    
        private Model[] models = { "csb_ballasog", "g_f_y_ballas_01", "g_m_y_ballasout_01" };
        private WeaponHash[] weaponList = { WeaponHash.AssaultRifle, WeaponHash.Pistol, WeaponHash.SawnOffShotgun, WeaponHash.Crowbar };

        public override bool OnBeforeCalloutDisplayed()
        {
            SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(/*Utils.getRandInt(300, 350)*/150f));

            ShowCalloutAreaBlipBeforeAccepting(SpawnPoint, 25f);

            CalloutMessage = "Reports of Gang Activity";
            CalloutPosition = SpawnPoint;
            
            Functions.PlayScannerAudioUsingPosition(
                $"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.WE_HAVE)} " +
                $"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.GANG)} IN_OR_ON_POSITION", SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            LSPDFR.RequestBackup(SpawnPoint, Utils.GetRandInt(2, 3), LSPD_First_Response.EBackupResponseType.Code3, LSPD_First_Response.EBackupUnitType.LocalUnit);

            for (int i = 0; i < Utils.GetRandInt(4, 10); i++)
            {
                Ped ped = new Ped(Utils.GetRandValue(models), SpawnPoint, Utils.GetRandInt(1, 360));
                Blip blip = new Blip(ped)
                {
                    IsFriendly = false
                };

                members.Add(ped);
                blips.Add(blip);
                ped.Tasks.Wander();

                ped.IsPersistent = true;
                ped.RelationshipGroup = RelationshipGroup.AmbientGangBallas;
                ped.CanAttackFriendlies = true;

                ped.Inventory.GiveNewWeapon(Utils.GetRandValue(weaponList), (short)Utils.GetRandInt(10, 60), true);
            }

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            // ped == null for some reason...
            members.ForEach(ped =>
            {
                if (ped && ped.GetAttachedBlip() && (ped.IsDead || Functions.IsPedArrested(ped)))
                {
                    ped.GetAttachedBlip().Delete();
                }
            });

            if (Game.LocalPlayer.Character.Position.DistanceTo(SpawnPoint) < 50)
            {

            }
        }

        public override void End()
        {
            members.ForEach(ped => ped.Dismiss());
            blips.ForEach(blip => blip.Delete());

            base.End();
        }
    }
}
