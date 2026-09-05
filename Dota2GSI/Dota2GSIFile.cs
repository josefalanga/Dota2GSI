using Dota2GSI.Utils;
using System;
using System.IO;

namespace Dota2GSI
{
    /// <summary>
    /// Class handling Game State Integration configuration file generation.
    /// </summary>
    public class Dota2GSIFile
    {
        /// <summary>
        /// Attempts to create a Game State Integration configuraion file.<br/>
        /// The configuration will target <c>http://localhost:{port}/</c> address.<br/>
        /// Returns true on success, false otherwise.
        /// </summary>
        /// <param name="name">The name of your integration.</param>
        /// <param name="port">The port for your integration.</param>
        /// <returns>Returns true on success, false otherwise.</returns>
        public static bool CreateFile(string name, int port)
        {
            return CreateFile(name, $"http://localhost:{port}/");
        }

        /// <summary>
        /// Attempts to create a Game State Integration configuraion file.<br/>
        /// The configuration will target the specified URI address.<br/>
        /// Returns true on success, false otherwise.
        /// </summary>
        /// <param name="name">The name of your integration.</param>
        /// <param name="uri">The URI for your integration.</param>
        /// <returns>Returns true on success, false otherwise.</returns>
        public static bool CreateFile(string name, string uri)
        {
            return CreateFile(name, uri, false);
        }

        /// <summary>
        /// Attempts to create a Game State Integration configuraion file with optional experimental block.<br/>
        /// When <paramref name="experimental"/> is true the generated cfg includes Valve's experimental
        /// GSI flags (full_units, full_hero_kills, output precision). This materially increases the volume
        /// of data sent per tick; left opt-in to preserve current behavior. A one-time warning is emitted
        /// to the console when enabled. When false, output is byte-identical to the legacy generator.
        /// Returns true on success, false otherwise.
        /// </summary>
        /// <param name="name">The name of your integration.</param>
        /// <param name="uri">The URI for your integration.</param>
        /// <param name="experimental">When true, adds the experimental GSI block.</param>
        /// <returns>Returns true on success, false otherwise.</returns>
        public static bool CreateFile(string name, string uri, bool experimental)
        {
            string game_path = SteamUtils.GetGamePath(570);

            try
            {
                if (!string.IsNullOrWhiteSpace(game_path))
                {
                    string gsifolder = game_path + @"\game\dota\cfg\gamestate_integration\";
                    Directory.CreateDirectory(gsifolder);
                    string gsifile = gsifolder + @$"gamestate_integration_{name}.cfg";

                    ACF provider_configuration = new ACF();
                    provider_configuration.Items["auth"] = "1";
                    provider_configuration.Items["provider"] = "1";
                    provider_configuration.Items["map"] = "1";
                    provider_configuration.Items["player"] = "1";
                    provider_configuration.Items["hero"] = "1";
                    provider_configuration.Items["abilities"] = "1";
                    provider_configuration.Items["items"] = "1";
                    provider_configuration.Items["events"] = "1";
                    provider_configuration.Items["buildings"] = "1";
                    provider_configuration.Items["league"] = "1";
                    provider_configuration.Items["draft"] = "1";
                    provider_configuration.Items["wearables"] = "1";
                    provider_configuration.Items["minimap"] = "1";
                    provider_configuration.Items["roshan"] = "1";
                    provider_configuration.Items["couriers"] = "1";
                    provider_configuration.Items["neutralitems"] = "1";

                    ACF gsi_configuration = new ACF();
                    gsi_configuration.Items["uri"] = uri;
                    gsi_configuration.Items["timeout"] = "5.0";
                    gsi_configuration.Items["buffer"] = "0.1";
                    gsi_configuration.Items["throttle"] = "0.1";
                    gsi_configuration.Items["heartbeat"] = "10.0";
                    gsi_configuration.Children["data"] = provider_configuration;

                    if (experimental)
                    {
                        Console.WriteLine("[Dota2GSI] Experimental GSI block enabled: tick data volume will increase substantially.");

                        ACF experimental_output = new ACF();
                        experimental_output.Items["precision"] = "1";

                        ACF experimental_block = new ACF();
                        experimental_block.Items["full_units"] = "1";
                        experimental_block.Items["full_hero_kills"] = "1";
                        experimental_block.Children["output"] = experimental_output;

                        gsi_configuration.Children["experimental"] = experimental_block;
                    }

                    ACF gsi = new ACF();
                    gsi.Children[$"{name} Integration Configuration"] = gsi_configuration;

                    File.WriteAllText(gsifile, gsi.ToString());

                    return true;
                }
            }
            catch (Exception)
            {
            }

            return false;
        }
    }
}
