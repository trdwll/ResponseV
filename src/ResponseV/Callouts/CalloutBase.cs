﻿using System;
using System.Collections.Generic;
using System.Linq;

using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;


namespace ResponseV.Callouts
{
    internal abstract class CalloutBase : Callout
    {
        protected ResponseVLib.Logger g_Logger = new ResponseVLib.Logger(Main.s_AppVersion);

        protected Vector3 g_SpawnPoint;

        protected Blip g_CallBlip;

        protected bool g_bOnScene;
        protected bool g_bIsPursuit;
        protected bool g_bCustomSpawn;

        protected List<Ped> g_Victims = new List<Ped>();
        protected List<Ped> g_Suspects = new List<Ped>();

        protected Model[] g_PedModels = Model.PedModels.Where(p => !p.Name.StartsWith("A_C_")).ToArray(); // TODO: filter out EMS/FD/PD/MIL/etc
        protected Model[] g_Vehicles = Model.VehicleModels.Where(v => v.IsCar && !v.IsLawEnforcementVehicle && !v.IsEmergencyVehicle && !v.IsBigVehicle).ToArray();

        protected Model[] g_LosSantosPoliceVehicles = { "police", "police2", "police3", "police4" };
        protected Model[] g_BlaineCountyPoliceVehicles = { "sheriff", "sheriff2" };

        protected WeaponHash[] g_WeaponList = {
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
            if (!g_bCustomSpawn)
            {
                g_SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(MathHelper.GetRandomInteger(ResponseVLib.Configuration.config.Callouts.MinRadius, ResponseVLib.Configuration.config.Callouts.MaxRadius)));
            }

            CalloutPosition = g_SpawnPoint;

            // if (g_SpawnPoint.DistanceTo(Game.LocalPlayer.Character.Position) <= 500)
            {
                ShowCalloutAreaBlipBeforeAccepting(g_SpawnPoint, 25f);
            }

            g_Logger.Log($"CalloutBase: Spawn Point {g_SpawnPoint.ToString()}");

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

            g_Logger.Log("CalloutBase: Callout was accepted");

            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            base.OnCalloutNotAccepted();
        }

        public override void Process()
        {
            base.Process();

            if (Game.LocalPlayer.Character.Position.DistanceTo(g_SpawnPoint) < 35 && !g_bOnScene)
            {
                g_bOnScene = true;
                g_CallBlip.IsRouteEnabled = false;
                g_Logger.Log("CalloutBase: DistanceTo(g_SpawnPoint) < 35 so hide g_CallBlip");

                Utils.NotifyPlayerTo("Dispatch", ResponseVLib.Utils.GetRandValue("I'm on scene.", "on scene"));
            }

            if (Game.LocalPlayer.IsDead)
            {
                Functions.PlayScannerAudio($"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.OFFICER_DOWN)} OFFICER_NEEDS_IMMEDIATE_ASSISTANCE");
                End();
                g_Logger.Log("CalloutBase: Player is dead so force end call.");
            }
        }

        public override void End()
        {
            base.End();

            try
            {
                Utils.NotifyPlayerTo("Dispatch", ResponseVLib.Utils.GetRandValue("I'll be code 4.", "I'm code 4 dispatch.", "I'm code 4 and back on patrol."));
                g_Logger.Log("CalloutBase: Callout ended");

                g_CallBlip?.Delete();

                g_Victims?.ForEach(v => v.Dismiss());
                g_Suspects?.ForEach(s => s.Dismiss());

                g_bOnScene = false;
                g_bIsPursuit = false;
                g_bCustomSpawn = false;

                g_SpawnPoint = new Vector3();
            }
            catch (Exception e)
            {
                Utils.CrashNotify();
                g_Logger.Log(e.StackTrace, ResponseVLib.Logger.ELogLevel.LL_TRACE);
            }
        }
    }
}
