using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResponseV.Callouts.Roles
{
    internal abstract class RoleBase
    {
        protected struct Ped
        {
            Rage.Model model;
        }

        protected List<Ped> m_RolePeds = new List<Ped>();

    }
}
