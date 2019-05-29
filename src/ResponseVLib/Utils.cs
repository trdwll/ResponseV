using LSPD_First_Response.Engine.Scripting;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ResponseVLib
{
    public static class Utils
    {
        public static T GetRandValue<T>(params T[] args)
        {
            return args[MathHelper.GetRandomInteger(args.Length - 1)];
        }

        public static bool GetRandBool()
        {
            return MathHelper.GetRandomDouble(0.0, 1.0) >= 0.5;
        }

        public static T[] MergeArrays<T>(T[] array1, T[] array2)
        {
            T[] tmp = new T[array1.Length + array2.Length];
            array1.CopyTo(tmp, 0);
            array2.CopyTo(tmp, array1.Length);

            return tmp;
        }

        public static bool IsLSPDFRPluginRunning(string PluginName)
        {
            foreach (Assembly assembly in Functions.GetAllUserPlugins())
            {
                AssemblyName an = assembly.GetName();
                if (an.Name.ToLower() == PluginName.ToLower())
                {
                    return true;
                }
                // for some reason this fucking return doesn't return true when it should
                //return assembly.GetName().Name.ToLower() == PluginName.ToLower();
            }

            return false;
        }

        public static Callout GetCurrentRunningCallout()
        {
            foreach (Callout callout in ScriptComponent.GetAllByType<Callout>())
            {
                if (callout != null && callout.AcceptanceState == CalloutAcceptanceState.Running)
                {
                    return callout;
                }
            }
            return null;
        }

        public static bool IsKeyDown(System.Windows.Forms.Keys Key, System.Windows.Forms.Keys Modifier)
        {
            return (Rage.Game.IsKeyDownRightNow(Key) || Modifier == System.Windows.Forms.Keys.None) && Rage.Game.IsKeyDown(Key);
        }
    }
}
