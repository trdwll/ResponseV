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
    [CalloutInfo("PersonWithWeapon", CalloutProbability.VeryHigh)]
    public class PersonWithWeapon : Callout
    {
        private Vector3 SpawnPoint;
        private Ped suspect;
        private WeaponHash weapon;
        private Blip blip;
        private LHandle pursuit;
        private Enums.Callout state;

        private WeaponHash[] weaponList = {
            WeaponHash.AdvancedRifle, WeaponHash.AssaultRifle, WeaponHash.CarbineRifle, // Assault Rifles
            WeaponHash.AssaultSMG, WeaponHash.MicroSMG, // SMGs
            WeaponHash.MG, WeaponHash.CombatMG, // LMGs
            WeaponHash.Bat, WeaponHash.Knife, // Melee
            WeaponHash.RPG, WeaponHash.Minigun, WeaponHash.GrenadeLauncher, WeaponHash.Grenade, WeaponHash.SmokeGrenade, // Special
            WeaponHash.SawnOffShotgun, WeaponHash.PumpShotgun, WeaponHash.AssaultShotgun, WeaponHash.BullpupShotgun, // Shotguns
            WeaponHash.SniperRifle, WeaponHash.HeavySniper, // Sniper Rifles
            WeaponHash.APPistol, WeaponHash.Pistol, WeaponHash.CombatPistol, WeaponHash.Pistol50 // Pistols
        };

        public override bool OnBeforeCalloutDisplayed()
        {
            SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(Utils.GetRandInt(500, 600)));

            weapon = Utils.GetRandValue(weaponList);

            ShowCalloutAreaBlipBeforeAccepting(SpawnPoint, 25f);

            CalloutMessage = "Reports of a Person with a " + (Utils.GetRandBool() ? weapon.ToString() : "Weapon");
            CalloutPosition = SpawnPoint;

            Functions.PlayScannerAudioUsingPosition(
                $"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.WE_HAVE)} " +
                $"{LSPDFR.Radio.GetWeaponSound(weapon)} IN_OR_ON_POSITION", SpawnPoint);
            
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            state = Enums.Callout.EnRoute;

            suspect = new Ped(Utils.GetRandValue(RageMethods.GetPedModels()), SpawnPoint, 360);

            suspect.Tasks.Wander();
            suspect.IsPersistent = true;
            suspect.CanAttackFriendlies = true;
            if (Utils.GetRandBool()) suspect.RelationshipGroup = RelationshipGroup.HatesPlayer;
            else suspect.RelationshipGroup = RelationshipGroup.AmbientFriendEmpty;

            suspect.Inventory.GiveNewWeapon(weapon, (short)Utils.GetRandInt(30, 90), true);

            blip = new Blip(SpawnPoint)
            {
                IsRouteEnabled = true,
                IsFriendly = false
            };
            blip.EnableRoute(System.Drawing.Color.Red);

            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            base.OnCalloutNotAccepted();
        }

        public override void Process()
        {
            if (state == Enums.Callout.EnRoute && Game.LocalPlayer.Character.Position.DistanceTo(SpawnPoint) < 50)
            {
                blip.IsRouteEnabled = false;
                state = Enums.Callout.OnScene;

                Pursuit();
            }

            if (state == Enums.Callout.InPursuit)
            {
                if (!Functions.IsPursuitStillRunning(pursuit))
                {
                    state = Enums.Callout.Done;
                    End();
                }
            }
        }

        void Pursuit()
        {
            GameFiber.StartNew(delegate
            {
                pursuit = Functions.CreatePursuit();
                Functions.AddPedToPursuit(pursuit, suspect);
                blip.Delete();
                
                if (Utils.GetRandBool()) Functions.SetPursuitDisableAI(pursuit, true);

                suspect.BlockPermanentEvents = Utils.GetRandBool();
                suspect.Tasks.FightAgainstClosestHatedTarget(50f);

                state = Enums.Callout.InPursuit;

                GameFiber.Sleep(5000);
                LSPDFR.RequestBackup(Game.LocalPlayer.Character.Position, 1, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.LocalUnit);
            });
        }

        public override void End()
        {
            blip.Delete();
            suspect.Dismiss();

            base.End();
        }
    }
}