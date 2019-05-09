using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rage;
using Rage.Native;
using LSPD_First_Response;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using LSPD_First_Response.Engine.Scripting;

namespace ResponseV.Callouts
{
    public class RVCallout : Callout
    {
        public Logger m_Logger = new Logger();

        public Vector3 m_SpawnPoint;

        public Blip m_CallBlip;

        public List<Ped> m_Victims = new List<Ped>();
        public List<Ped> m_Suspects = new List<Ped>();

        public Model[] m_PedModels = Model.PedModels;
        public Model[] m_Vehicles = Model.VehicleModels.Where(v => v.IsCar && !v.IsLawEnforcementVehicle && !v.IsEmergencyVehicle).ToArray();

        public Model[] m_PoliceVehicleModels = { "police", "police2", "police3", "police4", "sheriff", "sheriff2" };

        public WeaponHash[] m_WeaponList = {
            WeaponHash.AdvancedRifle, WeaponHash.AssaultRifle, WeaponHash.CarbineRifle, // Assault Rifles
            WeaponHash.AssaultSMG, WeaponHash.MicroSMG, // SMGs
            WeaponHash.MG, WeaponHash.CombatMG, // LMGs
            WeaponHash.Bat, WeaponHash.Knife, // Melee
            // WeaponHash.RPG, WeaponHash.Minigun, WeaponHash.GrenadeLauncher, WeaponHash.Grenade, WeaponHash.SmokeGrenade, // Special
            WeaponHash.SawnOffShotgun, WeaponHash.PumpShotgun, WeaponHash.AssaultShotgun, WeaponHash.BullpupShotgun, // Shotguns
            WeaponHash.SniperRifle, WeaponHash.HeavySniper, // Sniper Rifles
            WeaponHash.APPistol, WeaponHash.Pistol, WeaponHash.CombatPistol, WeaponHash.Pistol50 // Pistols
        };

        public override bool OnBeforeCalloutDisplayed()
        {
            m_SpawnPoint = World.GetNextPositionOnStreet(
                Game.LocalPlayer.Character.Position.Around(
                Utils.GetRandInt(Configuration.config.Callouts.MinRadius, 
                Configuration.config.Callouts.MinRadius)));

            ShowCalloutAreaBlipBeforeAccepting(m_SpawnPoint, 25f);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            m_CallBlip = new Blip(m_SpawnPoint)
            {
                IsRouteEnabled = true
            };

            m_CallBlip.EnableRoute(System.Drawing.Color.Blue);
            m_CallBlip.Color = System.Drawing.Color.Blue;

            m_Logger.Log("Callout was accepted");

            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            base.OnCalloutNotAccepted();
        }

        public override void Process()
        {
            base.Process();

            if (Game.LocalPlayer.Character.Position.DistanceTo(m_SpawnPoint) < 35)
            {
                m_CallBlip.IsRouteEnabled = false;
            }
        }

        public override void End()
        {
            base.End();

            try
            {
                m_CallBlip.Delete();
                m_Victims.ForEach(v => v.Dismiss());
                m_Suspects.ForEach(s => s.Dismiss());
            }
            catch (Exception e)
            {
                Rage.Game.DisplayNotification($"Response~y~V~w~ has had an error. Please report to developer.");
                m_Logger.Log(e.ToString(), Logger.ELogLevel.LL_TRACE);
            }
        }
    }
}
