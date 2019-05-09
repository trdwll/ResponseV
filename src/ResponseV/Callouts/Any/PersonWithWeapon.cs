using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;

namespace ResponseV.Callouts.Any
{
    [CalloutInfo("PersonWithWeapon", CalloutProbability.Medium)]
    public class PersonWithWeapon : RVCallout
    {
        private WeaponHash m_Weapon;
        private LHandle m_Pursuit;
        private bool m_bOnScene;

        public override bool OnBeforeCalloutDisplayed()
        {
            m_Weapon = Utils.GetRandValue(m_WeaponList);

            CalloutMessage = "Reports of a Person with a " + (Utils.GetRandBool() ? m_Weapon.ToString() : "Weapon");
            CalloutPosition = m_SpawnPoint;

            Functions.PlayScannerAudioUsingPosition(
                $"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.WE_HAVE)} " +
                $"{LSPDFR.Radio.GetWeaponSound(m_Weapon)} IN_OR_ON_POSITION", m_SpawnPoint);
            
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            m_Suspects.Add(new Ped(Utils.GetRandValue(RageMethods.GetPedModels()), m_SpawnPoint, 360));
            m_Suspects.ForEach(s =>
            {
                s.Tasks.Wander();
                s.IsPersistent = true;
                s.CanAttackFriendlies = true;

                s.RelationshipGroup = Utils.GetRandBool() ? RelationshipGroup.HatesPlayer : RelationshipGroup.AmbientFriendEmpty;
                s.Inventory.GiveNewWeapon(m_Weapon, (short)Utils.GetRandInt(30, 90), true);
            });

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            if (Game.LocalPlayer.Character.Position.DistanceTo(m_SpawnPoint) < 50)
            {
                m_bOnScene = true;
                m_CallBlip.IsRouteEnabled = false;

                Pursuit();
            }

            if (m_bOnScene && !Functions.IsPursuitStillRunning(m_Pursuit))
            {
                End();
            }
        }

        void Pursuit()
        {
            GameFiber.StartNew(delegate
            {
                m_Pursuit = Functions.CreatePursuit();
                m_Suspects.ForEach(s =>
                {
                    // TODO: Add peds to pursuit doesn't show the blip 
                    Functions.AddPedToPursuit(m_Pursuit, s);
                    s.BlockPermanentEvents = Utils.GetRandBool();

                    // We don't want all suspects to attack lol
                    if (Utils.GetRandBool())
                    {
                        s.Tasks.FightAgainstClosestHatedTarget(50f);
                    }
                });

                if (Utils.GetRandBool()) 
                {
                    Functions.SetPursuitDisableAI(m_Pursuit, true);
                }

                Functions.SetPursuitIsActiveForPlayer(m_Pursuit, true);

                if (Utils.GetRandBool())
                {
                    GameFiber.Sleep(5000);
                    LSPDFR.RequestBackup(m_SpawnPoint, Utils.GetRandInt(1, 3), LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.LocalUnit);
                }
            });
        }
    }
}