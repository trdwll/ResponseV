using Rage;

namespace ResponseV.Ambient
{
    internal class AmbientEvent
    {
        private enum EAmbientEvent
        {
            AE_FIGHT,
            AE_EMS,
            AE_PATROL,
            AE_CALL
        }

        private static EAmbientEvent s_AmbientEvent;

        public readonly Model[] m_HunterPedModels = { "csb_cletus", "ig_clay", "ig_oneil", "a_m_m_tramp_01", "ig_old_man1a", "ig_hunter", "player_two", "mp_m_exarmy_01", "ig_clay", "ig_oneil", "ig_old_man2", "ig_old_man1a", "ig_hunter" , "ig_russiandrunk", "ig_clay" , "mp_m_exarmy_01", "ig_old_man2" , "ig_old_man1a", "ig_hunter", "s_m_y_armymech_01", "s_m_m_ammucountry" , "s_m_m_trucker_01" , "ig_ortega", "ig_russiandrunk", "a_m_m_tramp_01", "a_m_m_trampbeac_01", "a_m_m_rurmeth_01" , "mp_m_exarmy_01" , "g_m_y_pologoon_01", "g_m_y_mexgoon_03", "g_m_y_lost_03", "g_m_y_lost_02" , "g_m_y_pologoon_02" , "g_m_m_armboss_01" , "u_m_o_taphillbilly", "u_m_o_taphillbilly" };
        public readonly Model[] m_HunterAnimalModels = { "a_c_coyote", "a_c_boar", "a_c_deer", "a_c_cormorant", "a_c_pig", "a_c_deer", "a_c_coyote", "a_c_boar", "a_c_rhesus" };
        public readonly WeaponHash[] m_HunterWeapons = { WeaponHash.Pistol50, WeaponHash.HeavySniper, WeaponHash.PumpShotgun, WeaponHash.SawnOffShotgun, WeaponHash.SniperRifle };

        public static void Initialize()
        {
            for (;;)
            {
                GameFiber.Yield();
                s_AmbientEvent = EAmbientEvent.AE_FIGHT;// ResponseVLib.Utils.GetRandValue(EAmbientEvent.AE_CALL, EAmbientEvent.AE_PATROL);

                switch (s_AmbientEvent)
                {
                case EAmbientEvent.AE_CALL: Call.Initialize(); break;
                case EAmbientEvent.AE_PATROL: Patrol.Initialize(); break;
                case EAmbientEvent.AE_FIGHT: Fight.Initialize(); break;
                }

                GameFiber.Sleep(100000);
                // GameFiber.Sleep(MathHelper.GetRandomInteger(60000, 300000));
            }
        }
    }
}
