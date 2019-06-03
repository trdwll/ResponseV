using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;

namespace ResponseV.Callouts.Any
{
    [CalloutInfo("AnimalAttack", CalloutProbability.Always)]
    internal sealed class AnimalAttack : CalloutBase
    {
        private Ped m_Animal, m_Animal2;
        private Ped m_Victim;

        public override bool OnBeforeCalloutDisplayed()
        {
            //g_bCustomSpawn = true;
            //g_SpawnPoint = new Vector3(1527.37f, -1541.09f, 76.02f);

            CalloutMessage = $"Reports of {LSPDFR.Radio.GetCallStringFromEnum(Enums.ECallType.CT_ANIMALATTACK)}";
            CalloutPosition = g_SpawnPoint;

            g_Logger.Log($"AnimalAttack: {g_SpawnPoint.ToString()}");

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

            m_Animal2 = new Ped(ResponseVLib.Utils.GetRandValue("a_c_husky", "a_c_rottweiler", "a_c_poodle", "a_c_shepherd", "a_c_westy", "a_c_retriever"), g_SpawnPoint, MathHelper.GetRandomInteger(1, 360))
            {
                IsPersistent = true
            };

            m_Victim = new Ped(ResponseVLib.Utils.GetRandValue(g_PedModels), g_SpawnPoint, MathHelper.GetRandomInteger(1, 360))
            {
                IsPersistent = true
            };

            // make the victim like us rather than attack us
            m_Victim.RelationshipGroup = new RelationshipGroup("VICTIM");
            Game.SetRelationshipBetweenRelationshipGroups("VICTIM", "PLAYER", Relationship.Neutral);

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            if (Game.LocalPlayer.Character.Position.DistanceTo(g_SpawnPoint) <= 70)
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
                // this isn't ideal as it's constantly running /shrug
                Roles.AnimalControl.Request(g_SpawnPoint, m_Animal, m_Animal2);

                if (!m_Animal.Exists() && !m_Animal2.Exists())
                {
                    End();
                }
            }
        }

        public override void End()
        {
            base.End();
            g_Logger.Log("AnimalAttack: Callout end");
            //Roles.AnimalControl.CleanupAnimalControl();
            m_Victim?.Dismiss();
            m_Animal?.Dismiss();
        }
    }
}