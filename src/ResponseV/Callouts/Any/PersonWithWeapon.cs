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
    [CalloutInfo("PersonWithWeapon", CalloutProbability.Low)]
    public class PersonWithWeapon : Callout
    {
        private Vector3 m_SpawnPoint;
        private Ped m_Suspect;
        private WeaponHash m_Weapon;
        private Blip m_Blip;
        private LHandle m_Pursuit;
        private Enums.Callout m_State;

        private WeaponHash[] m_WeaponList = {
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
            m_SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(Utils.GetRandInt(500, 600)));

            m_Weapon = Utils.GetRandValue(m_WeaponList);

            ShowCalloutAreaBlipBeforeAccepting(m_SpawnPoint, 25f);

            CalloutMessage = "Reports of a Person with a " + (Utils.GetRandBool() ? m_Weapon.ToString() : "Weapon");
            CalloutPosition = m_SpawnPoint;

            Functions.PlayScannerAudioUsingPosition(
                $"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.WE_HAVE)} " +
                $"{LSPDFR.Radio.GetWeaponSound(m_Weapon)} IN_OR_ON_POSITION", m_SpawnPoint);
            
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            m_State = Enums.Callout.EnRoute;

            m_Suspect = new Ped(Utils.GetRandValue(RageMethods.GetPedModels()), m_SpawnPoint, 360);

            m_Suspect.Tasks.Wander();
            m_Suspect.IsPersistent = true;
            m_Suspect.CanAttackFriendlies = true;
            if (Utils.GetRandBool()) m_Suspect.RelationshipGroup = RelationshipGroup.HatesPlayer;
            else m_Suspect.RelationshipGroup = RelationshipGroup.AmbientFriendEmpty;

            m_Suspect.Inventory.GiveNewWeapon(m_Weapon, (short)Utils.GetRandInt(30, 90), true);

            m_Blip = new Blip(m_SpawnPoint)
            {
                IsRouteEnabled = true,
                IsFriendly = false
            };
            m_Blip.EnableRoute(System.Drawing.Color.Red);

            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            base.OnCalloutNotAccepted();
        }

        public override void Process()
        {
            if (m_State == Enums.Callout.EnRoute && Game.LocalPlayer.Character.Position.DistanceTo(m_SpawnPoint) < 50)
            {
                m_Blip.IsRouteEnabled = false;
                m_State = Enums.Callout.OnScene;

                Pursuit();
            }

            if (m_State == Enums.Callout.InPursuit)
            {
                if (!Functions.IsPursuitStillRunning(m_Pursuit))
                {
                    m_State = Enums.Callout.Done;
                    End();
                }
            }
        }

        void Pursuit()
        {
            GameFiber.StartNew(delegate
            {
                m_Pursuit = Functions.CreatePursuit();
                Functions.AddPedToPursuit(m_Pursuit, m_Suspect);
                m_Blip.Delete();
                
                if (Utils.GetRandBool()) 
                {
                    Functions.SetPursuitDisableAI(m_Pursuit, true);
                }

                m_Suspect.BlockPermanentEvents = Utils.GetRandBool();
                m_Suspect.Tasks.FightAgainstClosestHatedTarget(50f);

                m_State = Enums.Callout.InPursuit;

                GameFiber.Sleep(5000);
                LSPDFR.RequestBackup(Game.LocalPlayer.Character.Position, 1, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.LocalUnit);
            });
        }

        public override void End()
        {
            m_Blip.Delete();
            m_Suspect.Dismiss();

            base.End();
        }
    }
}