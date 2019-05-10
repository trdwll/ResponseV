using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rage;

// Units responding to a call
namespace ResponseV.Ambient
{
    public class Call
    {
        private List<Vehicle> m_Vehicles = new List<Vehicle>();

        enum ECallType
        {
            CT_FIREEMSPOLICE,
            CT_FIREEMS,
            CT_FIREPOLICE,
            CT_FIRE,
            CT_EMSPOLICE,
            CT_EMS,
            CT_POLICE,
        }

        private ECallType m_CallType;

        public static void Initialize()
        {
            Rage.Game.DisplayNotification("Calls ambient event");

            //switch (m_CallType)
            //{

            //}
        }

        private void End()
        {
            m_Vehicles.ForEach(v => v.Delete());
            m_Vehicles.Clear();
        }
    }
}
