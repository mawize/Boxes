using System;
using System.IO;
using TShockAPI;

namespace BoxesQuota
{
	public class Util
	{
		internal static string BoxesConfigPath { get { return Path.Combine(TShock.SavePath, "PluginConfigs\\BoxesQuotaConfig.json"); } }

		private Util ()
		{        
		}

		public static void SetupConfig()
		{
			try
			{
				if (File.Exists(BoxesConfigPath))
				{
					BoxesQuotaPlugin.Config = ConfigFile.Read(BoxesConfigPath);
					// Add all the missing config properties in the json file
				}
				BoxesQuotaPlugin.Config.Write(BoxesConfigPath);
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Error in config file");
				Console.ForegroundColor = ConsoleColor.Gray;
				Log.Error("Config Exception");
				Log.Error(ex.ToString());
			}
		}
	}
}

