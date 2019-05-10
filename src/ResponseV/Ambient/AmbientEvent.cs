using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rage;

namespace ResponseV.Ambient
{
    public class AmbientEvent
    {
        private enum EAmbientEvent
        {
            AE_FIGHT,
            AE_EMS,
            AE_PATROL,
            AE_CALL
        }

        private static EAmbientEvent m_AmbientEvent;

        public static void Initialize()
        {
            for (;;)
            {
                GameFiber.Yield();
                m_AmbientEvent = EAmbientEvent.AE_FIGHT;// Utils.GetRandValue(EAmbientEvent.AE_CALL, EAmbientEvent.AE_PATROL);

                switch (m_AmbientEvent)
                {
                case EAmbientEvent.AE_CALL: Call.Initialize(); break;
                case EAmbientEvent.AE_PATROL: Patrol.Initialize(); break;
                case EAmbientEvent.AE_FIGHT: Fight.Initialize(); break;
                }

                GameFiber.Sleep(100000);
                // GameFiber.Sleep(Utils.GetRandInt(60000, 300000));
            }
        }
    }
}
