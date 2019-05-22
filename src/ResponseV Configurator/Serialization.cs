using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResponseV_Configurator
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
                    System.IO.File.WriteAllText(jsonFile, Pretty(SerializeObject(obj)));
                }
            }

            // https://stackoverflow.com/a/23828858
            public static string Pretty(string jsonString)
            {
                StringBuilder stringBuilder = new StringBuilder();

                bool escaping = false;
                bool inQuotes = false;
                int indentation = 0;

                foreach (char character in jsonString)
                {
                    if (escaping)
                    {
                        escaping = false;
                        stringBuilder.Append(character);
                    }
                    else
                    {
                        if (character == '\\')
                        {
                            escaping = true;
                            stringBuilder.Append(character);
                        }
                        else if (character == '\"')
                        {
                            inQuotes = !inQuotes;
                            stringBuilder.Append(character);
                        }
                        else if (!inQuotes)
                        {
                            if (character == ',')
                            {
                                stringBuilder.Append(character);
                                stringBuilder.Append("\r\n");
                                stringBuilder.Append('\t', indentation);
                            }
                            else if (character == '[' || character == '{')
                            {
                                stringBuilder.Append(character);
                                stringBuilder.Append("\r\n");
                                stringBuilder.Append('\t', ++indentation);
                            }
                            else if (character == ']' || character == '}')
                            {
                                stringBuilder.Append("\r\n");
                                stringBuilder.Append('\t', --indentation);
                                stringBuilder.Append(character);
                            }
                            else if (character == ':')
                            {
                                stringBuilder.Append(character);
                                stringBuilder.Append('\t');
                            }
                            else
                            {
                                stringBuilder.Append(character);
                            }
                        }
                        else
                        {
                            stringBuilder.Append(character);
                        }
                    }
                }

                return stringBuilder.ToString();
            }
        }
    }
}
