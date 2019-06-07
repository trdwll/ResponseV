using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;

using ResponseVLib;

using World = Rage.World;
using Ped = Rage.Ped;

namespace ResponseV.Callouts.Any
{
    [CalloutInfo("PedOnFire", CalloutProbability.Medium)]
    internal sealed class PedOnFire : CalloutBase
    {
        private bool m_bCallPursuit = ResponseVLib.Utils.GetRandBool();
        private bool m_bSpawnedFire;

        private LHandle m_Pursuit;

        public override bool OnBeforeCalloutDisplayed()
        {
            CalloutMessage = $"Reports of {LSPDFR.Radio.GetCallStringFromEnum(Enums.ECallType.CT_PEDONFIRE)}";

            Functions.PlayScannerAudioUsingPosition($"{LSPDFR.Radio.GetCalloutAudio(Enums.ECallType.CT_PEDONFIRE, Enums.EResponse.R_CODE3)}", g_SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            g_Logger.Log("PedOnFire: Callout accepted");

            g_Victims.Add(new Ped(ResponseVLib.Utils.GetRandValue(g_PedModels), g_SpawnPoint, 0f));

            // Spawn suspect
            if (m_bCallPursuit)
            {
                g_Suspects.Add(new Ped(ResponseVLib.Utils.GetRandValue(g_PedModels), g_SpawnPoint, 0f));
                g_Logger.Log("PedOnFire: Suspect created");
            }

            g_Victims.ForEach(v =>
            {
                v.ApplyDamagePack(ResponseVLib.Utils.GetRandValue(
                    DamagePacks.Burnt_Ped_0, DamagePacks.Burnt_Ped_Head_Torso,
                    DamagePacks.Burnt_Ped_Left_Arm, DamagePacks.Burnt_Ped_Limbs,
                    DamagePacks.Burnt_Ped_Right_Arm), 100f, 1f);

                if (ResponseVLib.Utils.GetRandBool())
                {
                    v.Kill();
                }

                if (Main.s_bBetterEMS)
                {
                    BetterEMS.API.EMSFunctions.OverridePedDeathDetails(v, "", "Fire", Game.GameTime, (float)MathHelper.GetRandomDouble(0.0, 1.0) - .05f);
                }
            });

            GameFiber fiber = GameFiber.StartNew(delegate
            {
                GameFiber.Sleep(6000);
                g_Logger.Log("PedOnFire: Calling for EMS and Fire");
                LSPDFR.RequestFire(g_SpawnPoint);
                GameFiber.Sleep(3000);
                LSPDFR.RequestEMS(g_SpawnPoint);
            }, "PedOnFireRequestEMSFireFiber");

            Main.s_GameFibers.Add(fiber);

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            if (g_bOnScene)
            {
                if (!m_bSpawnedFire)
                {
                    m_bSpawnedFire = true;
                    World.SpawnExplosion(g_SpawnPoint, 3, 10.0f, false, false, 0.0f);
                    World.SpawnExplosion(g_SpawnPoint, 3, 10.0f, false, false, 0.0f);
                }

                // Start checking if EMS is on scene
                if (!Utils.m_bCheckingEMS)
                {
                    Utils.CheckEMSOnScene(g_SpawnPoint, "PedOnFire");
                }

                // EMS is on scene and no pursuit is active so lets end the call
                if (Utils.m_bEMSOnScene && !g_bIsPursuit)
                {
                    End();
                }

                // Call in the pursuit if no pursuit is active and if we have a suspect
                if (!g_bIsPursuit && m_bCallPursuit)
                {
                    Pursuit();
                }

                // If the pursuit is over then just end the call
                if (g_bIsPursuit && !Functions.IsPursuitStillRunning(m_Pursuit))
                {
                    g_Logger.Log("PedOnFire: Pursuit has finished so End()");
                    End();
                }
            }

        }

        void Pursuit()
        {
            g_bIsPursuit = true;

            g_Logger.Log("PedOnFire: Starting pursuit");

            GameFiber fiber = GameFiber.StartNew(delegate
            {
                m_Pursuit = Functions.CreatePursuit();

                g_Suspects.ForEach(s =>
                {
                    Functions.AddPedToPursuit(m_Pursuit, s);
                    s.Inventory.GiveNewWeapon(WeaponHash.Pistol, 200, false);
                    s.Inventory.GiveNewWeapon(WeaponHash.Molotov, 5, true);

                    if (ResponseVLib.Utils.GetRandBool())
                    {
                        s.Inventory.GiveNewWeapon(WeaponHash.PetrolCan, 1, true);
                    }
                });

                Functions.SetPursuitIsActiveForPlayer(m_Pursuit, true);
            }, "PedOnFirePursuitFiber");

            Main.s_GameFibers.Add(fiber);
        }
    }
}