using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rage;

// Units responding to a call
namespace ResponseV.Ambient
{
    internal class Call
    {
        private List<Vehicle> g_Vehicles = new List<Vehicle>();

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
            g_Vehicles.ForEach(v => v.Delete());
            g_Vehicles.Clear();
        }
    }
}
