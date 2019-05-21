using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;

namespace ResponseV.Callouts.Any
{
    [CalloutInfo("PedWithWeapon", CalloutProbability.Medium)]
    internal sealed class PedWithWeapon : CalloutBase
    {
        private WeaponHash m_Weapon;
        private LHandle m_Pursuit;

        private bool m_bMultiple = Utils.GetRandBool();
        private bool m_bOpenCarry = Utils.GetRandBool();

        private WeaponHash[] m_OpenCarryList = { WeaponHash.AssaultRifle, WeaponHash.AssaultSMG, WeaponHash.Pistol };

        public override bool OnBeforeCalloutDisplayed()
        {
            m_Weapon = m_bOpenCarry ? Utils.GetRandValue(m_OpenCarryList) : Utils.GetRandValue(g_WeaponList);

            if (m_bMultiple)
            {
                CalloutMessage = "Reports of multiple people with weapons";
            }
            else
            {
                CalloutMessage = "Reports of a person with a " + (Utils.GetRandBool() ? m_Weapon.ToString() : "weapon");
            }
            CalloutPosition = g_SpawnPoint;

            Functions.PlayScannerAudioUsingPosition($"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.WE_HAVE)} {LSPDFR.Radio.GetWeaponSound(m_Weapon)} IN_OR_ON_POSITION", g_SpawnPoint);
            
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            g_Logger.Log("PedWithWeapon: Callout accepted");

            if (m_bMultiple)
            {
                for (int i = 0; i < Utils.GetRandInt(1, 3); i++)
                {
                    g_Suspects.Add(new Ped(Utils.GetRandValue(g_PedModels), g_SpawnPoint, 360));
                }
            }

            g_Suspects.Add(new Ped(Utils.GetRandValue(g_PedModels), g_SpawnPoint, 360));

            g_Suspects.ForEach(s =>
            {
                s.Tasks.Wander();
                s.IsPersistent = true;
                if (!m_bOpenCarry)
                {
                    s.CanAttackFriendlies = true;
                }

                s.RelationshipGroup = !m_bOpenCarry ? RelationshipGroup.HatesPlayer : RelationshipGroup.AmbientFriendEmpty;
                s.Inventory.GiveNewWeapon(m_Weapon, (short)Utils.GetRandInt(30, 90), true);
            });

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            if (g_bOnScene)
            {
                if (!g_bIsPursuit && !m_bOpenCarry)
                {
                    Pursuit();
                }

                if (g_bIsPursuit && !Functions.IsPursuitStillRunning(m_Pursuit))
                {
                    g_Logger.Log("PedWithWeapon: Pursuit over, end call");
                    End();
                }

                if (m_bOpenCarry)
                {
                    End();
                }
            }
        }

        void Pursuit()
        {
            g_Logger.Log("PedWithWeapon: Create pursuit");
            g_bIsPursuit = true;
            GameFiber fiber = GameFiber.StartNew(delegate
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
            }, "PedWithWeaponPursuitFiber");

            Main.g_GameFibers.Add(fiber);
        }
    }
}