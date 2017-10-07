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
        private Model[] pedList = new Model[] { "csb_ballasog", "g_f_y_ballas_01", "g_m_y_ballasout_01" };
        private WeaponHash[] weaponList = new WeaponHash[] { WeaponHash.AssaultRifle, WeaponHash.Pistol, WeaponHash.SawnOffShotgun,
            WeaponHash.Golfclub, WeaponHash.Crowbar, WeaponHash.Hammer };

        public override bool OnBeforeCalloutDisplayed()
        {
            SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(/*Utils.getRandInt(300, 350)*/25f));

            ShowCalloutAreaBlipBeforeAccepting(SpawnPoint, 25f);

            CalloutMessage = "Reports of gang activity";
            CalloutPosition = SpawnPoint;

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Game.DisplaySubtitle("Spawned Peds!!!");
            for (int i = 0; i < Utils.getRandInt(4, 10); i++)
            {
                members.Add(new Ped(pedList[new Random().Next(0, pedList.Length)], SpawnPoint, Utils.getRandInt(1, 100)));
                blips.Add(members[i].AttachBlip());
                members[i].IsPersistent = true;
                members[i].RelationshipGroup = RelationshipGroup.AmbientGangBallas;
                members[i].Inventory.GiveNewWeapon(weaponList[new Random().Next(0, weaponList.Length)], (short)Utils.getRandInt(10, 60), true);
            }

            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            base.OnCalloutNotAccepted();
        }

        public override void Process()
        {
            base.Process();
            
            if (Game.LocalPlayer.Character.Position.DistanceTo(SpawnPoint) <= 15)
            {
                // init call here
            }
        }

        public override void End()
        {
            if (members.Count() > 0)
                foreach (Ped ped in members) ped.Dismiss();

            if (blips.Count() > 0)
                foreach (Blip blip in blips) blip.Delete();

            base.End();
        }
    }
}
