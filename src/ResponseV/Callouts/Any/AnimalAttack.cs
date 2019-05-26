using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;

namespace ResponseV.Callouts.Any
{
    [CalloutInfo("AnimalAttack", CalloutProbability.VeryHigh)]
    internal sealed class AnimalAttack : CalloutBase
    {
        private Ped m_Animal;
        private Ped m_Victim;

        public override bool OnBeforeCalloutDisplayed()
        {
            CalloutMessage = $"Reports of {LSPDFR.Radio.GetCallStringFromEnum(Enums.ECallType.CT_ANIMALATTACK)}";
            CalloutPosition = g_SpawnPoint;

            Functions.PlayScannerAudioUsingPosition($"{LSPDFR.Radio.GetCalloutAudio(Enums.ECallType.CT_ANIMALATTACK, Enums.EResponse.R_CODE2OR3)}", g_SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            g_Logger.Log("AnimalAttack: Callout accepted");

            m_Animal = new Ped(ResponseVLib.Utils.GetRandValue("a_c_husky", "a_c_rottweiler", "a_c_poodle", "a_c_shepherd", "a_c_westy", "a_c_retriever"), g_SpawnPoint, MathHelper.GetRandomInteger(1, 360))
            {
                IsPersistent = true
            };

            m_Victim = new Ped(ResponseVLib.Utils.GetRandValue(g_PedModels), g_SpawnPoint, MathHelper.GetRandomInteger(1, 360))
            {
                IsPersistent = true
            };

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            if (Game.LocalPlayer.Character.Position.DistanceTo(g_SpawnPoint) <= 60)
            {
                m_Animal.Tasks.FightAgainst(m_Victim);
                if (ResponseVLib.Utils.GetRandBool())
                {
                    m_Victim.Tasks.FightAgainst(m_Animal);
                    m_Victim.Inventory.GiveNewWeapon(WeaponHash.Pistol, 31, ResponseVLib.Utils.GetRandBool());
                }
            }

            if (g_bOnScene)
            {
                if (m_Victim.IsInjured || m_Victim.IsDead)
                {
                    LSPDFR.RequestEMS(g_SpawnPoint);

                    g_Logger.Log("AnimalAttack: On scene and victim is injured/dead so call EMS");
                    if (!Utils.m_bCheckingEMS)
                    {
                        g_Logger.Log("AnimalAttack: Checking for EMS");
                        Utils.CheckEMSOnScene(g_SpawnPoint, "AnimalAttack");
                    }

                    if (Utils.m_bEMSOnScene)
                    {
                        g_Logger.Log("AnimalAttack: EMS on Scene, end call.");
                        if (m_Victim.IsDead)
                        {
                            LSPDFR.RequestCoroner(g_SpawnPoint);
                        }

                        End();
                    }
                }
                else
                {
                    // TODO: call for Animal Control (add this in or use a 3rd party plugin)
                    // Utils.NotifyPlayerTo("Dispatch", ResponseVLib.Utils.GetRandValue("notify animal control.", "I need animal control on scene.")); 
                    End();
                }
            }
        }

        public override void End()
        {
            g_Logger.Log("AnimalAttack: Callout end");
            m_Victim?.Dismiss();
            m_Animal?.Dismiss();

            base.End();
        }
    }
}