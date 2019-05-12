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

using System.Reflection;

namespace ResponseV.Callouts
{
    public class RVCallout : Callout
    {
        public Logger g_Logger = new Logger();

        public Vector3 g_SpawnPoint;

        public Blip g_CallBlip;

        public bool g_bOnScene;
        public bool g_bIsPursuit;

        public List<Ped> g_Victims = new List<Ped>();
        public List<Ped> g_Suspects = new List<Ped>();

        public Model[] g_PedModels = Model.PedModels;
        public Model[] g_Vehicles = Model.VehicleModels.Where(v => v.IsCar && !v.IsLawEnforcementVehicle && !v.IsEmergencyVehicle && !v.IsBigVehicle).ToArray();

        public Model[] g_PoliceVehicleModels = { "police", "police2", "police3", "police4", "sheriff", "sheriff2" };

        public WeaponHash[] g_WeaponList = {
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
            g_SpawnPoint = World.GetNextPositionOnStreet(
                Game.LocalPlayer.Character.Position.Around(
                Utils.GetRandInt(Configuration.config.Callouts.MinRadius, 
                Configuration.config.Callouts.MinRadius)));

            //if (Functions.GetCalloutName(Functions.GetCurrentCallout()) == "UnionDepository")
            //{
            //    g_SpawnPoint = new Vector3(17.51f, -655.52f, 31.48f);
            //}

            ShowCalloutAreaBlipBeforeAccepting(g_SpawnPoint, 25f);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            g_CallBlip = new Blip(g_SpawnPoint)
            {
                IsRouteEnabled = true
            };

            g_CallBlip.EnableRoute(System.Drawing.Color.Blue);
            g_CallBlip.Color = System.Drawing.Color.Blue;

            g_Logger.Log("Callout was accepted");

            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            base.OnCalloutNotAccepted();
        }

        public override void Process()
        {
            base.Process();

            //LHandle callout = Functions.GetCurrentCallout();

            //CalloutInfoAttribute ScriptInfo = typeof(Callout).GetField("ScriptInfo", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(callout) as CalloutInfoAttribute;

            //if (ScriptInfo != null)
            //{
            //    //Callout st = ScriptInfo.GetValue(callout) as Callout;

            //    Utils.Notify("Field: " + ScriptInfo?.Name);
            //}
            //else
            //{
            //    Utils.Notify("field is null");
            //}

            Utils.Notify(Functions.GetCalloutName(Functions.GetCurrentCallout()));

            if (Game.LocalPlayer.Character.Position.DistanceTo(g_SpawnPoint) < 35)
            {
                g_bOnScene = true;
                g_CallBlip.IsRouteEnabled = false;
                g_Logger.Log("RVCallout: DistanceTo(g_SpawnPoint) < 35 so hide g_CallBlip");
            }

            if (Game.LocalPlayer.IsDead)
            {
                Functions.PlayScannerAudio($"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.OFFICER_DOWN)} OFFICER_NEEDS_IMMEDIATE_ASSISTANCE");
                End();
                g_Logger.Log("Player is dead so force end call.");
            }
        }

        public override void End()
        {
            base.End();

            try
            {
                g_CallBlip.Delete();

                g_Victims.ForEach(v => v.Dismiss());
                g_Suspects.ForEach(s => s.Dismiss());

                g_bOnScene = false;
                g_bIsPursuit = false;
            }
            catch (Exception e)
            {
                Utils.CrashNotify();
                g_Logger.Log(e.Message, Logger.ELogLevel.LL_TRACE);
            }
        }
    }
}
