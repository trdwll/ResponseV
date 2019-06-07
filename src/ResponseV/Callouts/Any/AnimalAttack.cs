using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using WorldZone = ResponseVLib.WorldZone;

namespace ResponseV.Callouts.Any
{
    [CalloutInfo("AnimalAttack", CalloutProbability.Medium)]
    internal sealed class AnimalAttack : CalloutBase
    {
        private Ped m_Animal;
        private Ped m_Victim;
        private string[] AnimalModels = { "a_c_husky", "a_c_rottweiler", "a_c_poodle", "a_c_shepherd", "a_c_westy", "a_c_retriever" };

        public override bool OnBeforeCalloutDisplayed()
        {
            CalloutMessage = $"Reports of {LSPDFR.Radio.GetCallStringFromEnum(Enums.ECallType.CT_ANIMALATTACK)}";

            // Could check if the g_SpawnPoint is also in Blaine, but whatever
            if (WorldZone.GetArea(Game.LocalPlayer.Character.Position) == WorldZone.EWorldArea.Blaine_County)
            {
                AnimalModels = ResponseVLib.Utils.MergeArrays(AnimalModels, new string[] { "a_c_coyote", "a_c_deer", "a_c_mtlion", "a_c_boar" });
            }

            g_Logger.Log($"AnimalAttack: {g_SpawnPoint.ToString()}");

            Functions.PlayScannerAudioUsingPosition($"{LSPDFR.Radio.GetCalloutAudio(Enums.ECallType.CT_ANIMALATTACK, Enums.EResponse.R_CODE2OR3)}", g_SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            g_Logger.Log("AnimalAttack: Callout accepted");

            m_Animal = new Ped(ResponseVLib.Utils.GetRandValue(AnimalModels), g_SpawnPoint, MathHelper.GetRandomInteger(1, 360))
            {
                IsPersistent = true
            };

            m_Victim = new Ped(ResponseVLib.Utils.GetRandValue(g_PedModels), g_SpawnPoint, MathHelper.GetRandomInteger(1, 360))
            {
                IsPersistent = true
            };

            m_Victim.RelationshipGroup = new RelationshipGroup("VICTIM");
            
            if (ResponseVLib.Utils.GetRandBool())
            {
                // make the victim like us rather than attack us
                Game.SetRelationshipBetweenRelationshipGroups("VICTIM", "PLAYER", Relationship.Companion);
            }
            
            if (ResponseVLib.Utils.GetRandBool())
            {
                Game.SetRelationshipBetweenRelationshipGroups("VICTIM", "COP", Relationship.Like);
                Game.SetRelationshipBetweenRelationshipGroups("COP", "VICTIM", Relationship.Like);
            }

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
                Roles.AnimalControl.Request(g_SpawnPoint, m_Animal);

                if (!m_Animal.Exists())
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