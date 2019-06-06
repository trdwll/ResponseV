using LSPD_First_Response.Engine.Scripting;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ResponseVLib
{
    public static class Utils
    {
        public static Vector3[] GetClosestVector3(Vector3[] ArrayOfVectors, Vector3 Position, float Radius = 100.0f, int MaxCount = 5)
        {
            List<Tuple<float, Vector3>> tmpVectors = new List<Tuple<float, Vector3>>();
            foreach (Vector3 vec in ArrayOfVectors)
            {
                float Distance = (Position - vec).Length();

                if (Distance <= Radius)
                {
                    tmpVectors.Add(new Tuple<float, Vector3>(Distance, vec));
                }
            }

            return tmpVectors.OrderBy(v => v.Item1).Select(v => v.Item2).Take(MaxCount).ToArray();
        }

        public static T GetRandValue<T>(params T[] args)
        {
            return args[new Random().Next(0, args.Length)];
        }

        public static bool GetRandBool()
        {
            return new Random().Next(0, 1) == 1;
        }

        public static T[] MergeArrays<T>(T[] array1, T[] array2)
        {
            T[] tmp = new T[array1.Length + array2.Length];
            array1.CopyTo(tmp, 0);
            array2.CopyTo(tmp, array1.Length);

            return tmp;
        }

        public static void PlaySound(string SoundFile)
        {
            SoundPlayer player = new SoundPlayer($"{Configuration.CustomAudioPath}{SoundFile}");
            player.Play();
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
