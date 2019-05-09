using System;

namespace ResponseV
{
    /*
     * Serialization class written by Jack 'OhYea777' Taylor (https://github.com/OhYea777) and is licensed under MIT
     * This class was written for an old project we worked on and is being recycled for this current project
     * Thanks Jack
     */
    public class Serialization
    {
        public class JSON
        {
            private static System.Web.Script.Serialization.JavaScriptSerializer jsonSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();

            public class Deserialize
            {
                public static T GetFromFile<T>(string jsonFile)
                {
                    T obj = default(T);

                    if (!System.IO.File.Exists(jsonFile))
                    {
                        obj = Activator.CreateInstance<T>();

                        System.IO.Directory.GetParent(jsonFile).Create();
                        System.IO.File.WriteAllText(jsonFile, jsonSerializer.Serialize(obj));
                    }
                    else
                    {
                        obj = GetFromJson<T>(System.IO.File.ReadAllText(jsonFile));
                    }

                    return obj;
                }

                public static T GetFromJson<T>(string json)
                {
                    return jsonSerializer.Deserialize<T>(json);
                }
            }

            public class Serialize
            {
                public static string SerializeObject(object obj)
                {
                    return jsonSerializer.Serialize(obj);
                }

                public static void SerializeToFile(object obj, string jsonFile)
                {
                    System.IO.File.WriteAllText(jsonFile, SerializeObject(obj));
                }
            }
        }
    }
}
