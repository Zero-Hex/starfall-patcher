using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System.Windows.Forms;

namespace EQEmu_Patcher
{
    class IniLibrary
    {
        // Starfall's own names: every server's eqemupatcher writes
        // eqemupatcher.yml and filelist.yml, and a client folder another one
        // has patched holds its settings (Project Quarm's clientVersion: Quarm)
        public const string ConfigFile = "starfall.yml";
        public const string FilelistFile = "starfall_filelist.yml";

        public static IniLibrary instance;
        public string AutoPatch { get; set; }
        public string AutoPlay { get; set; }
        public VersionTypes ClientVersion { get; set; }
        public string LastPatchedVersion { get; set; }


        public static void Save()
        {
            var serializerBuilder = new SerializerBuilder().WithNamingConvention(new CamelCaseNamingConvention());
            var serializer = serializerBuilder.Build();
            string body = serializer.Serialize(instance);

            Console.WriteLine(body);
            File.WriteAllText($"{System.IO.Path.GetDirectoryName(Application.ExecutablePath)}\\{ConfigFile}", body);
        }

        public static void Load()
        {
            try {
                using (var input = File.OpenText($"{System.IO.Path.GetDirectoryName(Application.ExecutablePath)}\\{ConfigFile}"))
                {
                    var deserializerBuilder = new DeserializerBuilder().WithNamingConvention(new CamelCaseNamingConvention());

                    var deserializer = deserializerBuilder.Build();

                    instance = deserializer.Deserialize<IniLibrary>(input);
                }

                if (instance == null) {
                    ResetDefaults();
                    Save();
                }
            } catch (FileNotFoundException e) {
                Console.WriteLine($"Failed loading config: {e.Message}");
                ResetDefaults();
                Save();
            } catch (Exception e) {
                // a settings file this patcher cannot read starts over with
                // defaults rather than stopping it: the client is told from
                // eqgame.exe anyway
                Console.WriteLine($"Ignoring unreadable config: {e.Message}");
                ResetDefaults();
                Save();
            }

            if (instance.AutoPatch == null) instance.AutoPatch = "false";
            if (instance.AutoPlay == null) instance.AutoPlay = "false";
        }

        public static void ResetDefaults()
        {
            instance = new IniLibrary();
            instance.AutoPlay = "false";
            instance.AutoPatch = "false";
        }
    }
}
