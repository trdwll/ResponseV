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

        public override bool OnBeforeCalloutDisplayed()
        {
            m_Weapon = Utils.GetRandValue(g_WeaponList);

            CalloutMessage = "Reports of a Person with a " + (Utils.GetRandBool() ? m_Weapon.ToString() : "Weapon");
            CalloutPosition = g_SpawnPoint;

            Functions.PlayScannerAudioUsingPosition(
                $"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.WE_HAVE)} " +
                $"{LSPDFR.Radio.GetWeaponSound(m_Weapon)} IN_OR_ON_POSITION", g_SpawnPoint);
            
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            g_Suspects.Add(new Ped(Utils.GetRandValue(RageMethods.GetPedModels()), g_SpawnPoint, 360));
            g_Suspects.ForEach(s =>
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
            base.Process();

            if (Game.LocalPlayer.Character.Position.DistanceTo(g_SpawnPoint) < 50)
            {
                g_CallBlip.IsRouteEnabled = false;

                Pursuit();
            }

            if (g_bOnScene && !Functions.IsPursuitStillRunning(m_Pursuit))
            {
                End();
            }
        }

        void Pursuit()
        {
            GameFiber.StartNew(delegate
            {
                m_Pursuit = Functions.CreatePursuit();
                g_Suspects.ForEach(s =>
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
                    LSPDFR.RequestBackup(g_SpawnPoint, Utils.GetRandInt(1, 3), LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.LocalUnit);
                }
            });
        }
    }
}