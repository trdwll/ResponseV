using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rage;
using LSPD_First_Response;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using LSPD_First_Response.Engine.Scripting;

namespace ResponseV.Callouts.Any
{
    [CalloutInfo("DeadBody", CalloutProbability.VeryHigh)]
    internal class DeadBody : CalloutBase
    {
        public override bool OnBeforeCalloutDisplayed()
        {
            CalloutMessage = $"Reports of a " + (Utils.GetRandBool() ? "Deceased Person" : "Dead Body");
            CalloutPosition = g_SpawnPoint;

            Functions.PlayScannerAudioUsingPosition($"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.WE_HAVE)} {LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.DEADBODY)} IN_OR_ON_POSITION", g_SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            g_Logger.Log("DeadBody: Callout accepted");

            g_Victims.Add(new Ped(Utils.GetRandValue(g_PedModels), g_SpawnPoint, Utils.GetRandInt(1, 360)));
            g_Victims.ForEach(v =>
            {
                if (Native.GetSafeCoordForPed(g_SpawnPoint, out Vector3 pos))
                {
                    v.Position = pos;
                }

                v.ApplyDamagePack(Utils.GetRandValue(
                        DamagePack.BigHitByVehicle, DamagePack.SCR_Torture,
                        DamagePack.Explosion_Large, DamagePack.Burnt_Ped_0,
                        DamagePack.Car_Crash_Light, DamagePack.Car_Crash_Heavy,
                        DamagePack.Fall_Low, DamagePack.Fall,
                        DamagePack.HitByVehicle, DamagePack.BigRunOverByVehicle,
                        DamagePack.RunOverByVehicle
                    ), 100, 1);

                v.Kill();
            });

            GameFiber.StartNew(delegate
            {
                GameFiber.Sleep(3000);
                LSPDFR.RequestEMS(g_SpawnPoint);
                LSPDFR.RequestBackup(g_SpawnPoint, 1);
            });

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();
            
            if (g_bOnScene)
            {
                g_Logger.Log("DeadBody: On scene and requesting coroner (if Arrest Manager is installed) then end call");
                LSPDFR.RequestCoroner(g_SpawnPoint);

                End();
            }
        }
    }
}
