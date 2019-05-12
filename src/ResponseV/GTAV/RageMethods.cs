using System.Linq;

namespace ResponseV
{
    public class RageMethods
    {
        public static Rage.Model[] GetPedModels()
        {
            return Rage.Model.PedModels.Where(p => !p.Name.StartsWith("A_C_")).ToArray();
        }
    }
}
