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
using ResponseV.GTAV;

namespace ResponseV.Callouts.Any
{
    [CalloutInfo("AircraftCrash", CalloutProbability.VeryHigh)]
    internal class AircraftCrash : CalloutBase
    {
        private Vehicle m_Vehicle;

        private Model[] m_AirplaneModels = Model.VehicleModels.Where(v => v.IsPlane).ToArray();
        private Model[] m_HelicopterModels = Model.VehicleModels.Where(v => v.IsHelicopter).ToArray();

        private bool m_bIsAirplane = Utils.GetRandBool();
        private bool m_bIsExploded = Utils.GetRandBool();
        private bool m_bSceneSetup;

        private GameFiber m_ExplosionFiber;

        public override bool OnBeforeCalloutDisplayed()
        {
            g_bCustomSpawn = true;

            Vector3[] HeliSpawns = Utils.MergeArrays(SpawnPoints.m_AirplaneCrashSpawnPoints, SpawnPoints.m_HelicopterCrashSpawnPoints);

            // TODO: if in blaine then choose from blaine points
            g_SpawnPoint = m_bIsAirplane ? Utils.GetRandValue(SpawnPoints.m_AirplaneCrashSpawnPoints) : Utils.GetRandValue(HeliSpawns);

            CalloutMessage = "Reports of an " + (m_bIsAirplane ? Utils.GetRandValue("Aircraft", "Airplane") : "Helicopter") + " Crash";
            CalloutPosition = g_SpawnPoint;

            Functions.PlayScannerAudioUsingPosition($"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.WE_HAVE)} " + 
                (m_bIsAirplane ? 
                "{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.AIRCRAFT_CRASH)}" : 
                "{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.HELICOPTER_CRASH)}") + 
                " IN_OR_ON_POSITION", g_SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            m_Vehicle = new Vehicle(m_bIsAirplane ? Utils.GetRandValue(m_AirplaneModels) : Utils.GetRandValue(m_HelicopterModels), g_SpawnPoint);
            m_Vehicle.IsPersistent = true;

            int max_vics = MathHelper.GetRandomInteger(4, 10);
            for (int i = 0; i < max_vics; i++)
            {
                Vector3 pos = Extensions.AroundPosition(g_SpawnPoint, Utils.GetRandInt(25, 60));
                g_Victims.Add(new Ped(Utils.GetRandValue(g_PedModels), pos, Utils.GetRandInt(0, 360)));
            }

            g_Victims.ForEach(v =>
            {
                v.ApplyDamagePack(DamagePack.BigHitByVehicle, Utils.GetRandInt(50, 100), 0.0f);
                v.IsPersistent = true;

                if (Utils.GetRandBool())
                {
                    v.Kill();
                }
            });

            GameFiber.StartNew(delegate
            {
                LSPDFR.RequestBackup(g_SpawnPoint, 2);

                GameFiber.Sleep(10000);
                LSPDFR.RequestEMS(g_SpawnPoint);

                GameFiber.Sleep(1000);
                LSPDFR.RequestFire(g_SpawnPoint);

            }, "AircraftCrashBackupFiber");

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            if (g_bOnScene)
            {
                if (m_bIsExploded && !m_bSceneSetup)
                {
                    SceneCreation();
                    m_Vehicle.Explode(true);
                }

                g_Suspects.ForEach(v =>
                {
                    if (v.IsAlive)
                    {
                        v.Tasks.Flee(v, Utils.GetRandInt(10, 25), 6);
                        v.KeepTasks = true;
                    }
                });

                if (!Utils.m_bCheckingEMS)
                {
                    g_Logger.Log("AircraftCrash: Checking for EMS");
                    Utils.CheckEMSOnScene(g_SpawnPoint, "AircraftCrash");
                }

                if (Utils.m_bEMSOnScene)
                {
                    g_Logger.Log("AircraftCrash: EMS on Scene, end call.");
                    if (g_Victims.Exists(v => v.IsDead))
                    {
                        LSPDFR.RequestCoroner(g_SpawnPoint);
                    }

                    End();
                }
            }
        }

        void SceneCreation()
        {
            g_Logger.Log("AircraftCrash: SceneCreation");
            m_bSceneSetup = true;
            m_ExplosionFiber = GameFiber.StartNew(delegate
            {
                for (int i = 0; i < Utils.GetRandInt(5, 12); i++)
                {
                    World.SpawnExplosion(Extensions.AroundPosition(g_SpawnPoint, Utils.GetRandInt(5, 15)), Utils.GetRandValue(3, 6, 9), Utils.GetRandInt(5, 12), false, false, 0.0f);
                }

            }, "AircraftCrashFireFiber");
        }

        public override void End()
        {
            GameFiber.StartNew(delegate
            {
                GameFiber.Sleep(30000);
                m_Vehicle.Delete();
            });

            m_ExplosionFiber?.Abort();

            base.End();
        }
    }
}