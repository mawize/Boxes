using System;
using System.IO;
using Newtonsoft.Json;
using TShockAPI;

namespace BoxesQuota
{
	public class BQConfigFile
	{
		public string UserTilesQuota = "1000";
		
		public static BQConfigFile Read(string path)
		{
			if (!File.Exists(path))
				return new BQConfigFile();
			using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				return Read(fs);
			}
		}
		
		public static BQConfigFile Read(Stream stream)
		{
			using (var sr = new StreamReader(stream))
			{
				var cf = JsonConvert.DeserializeObject<BQConfigFile>(sr.ReadToEnd());
				if (ConfigRead != null)
					ConfigRead(cf);
				return cf;
			}
		}
		
		public void Write(string path)
		{
			using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write))
			{
				Write(fs);
			}
		}
		
		public void Write(Stream stream)
		{
			var str = JsonConvert.SerializeObject(this, Formatting.Indented);
			using (var sw = new StreamWriter(stream))
			{
				sw.Write(str);
			}
		}
		
		public static Action<BQConfigFile> ConfigRead;
		
		public static void LoadConfig()
		{
			try
			{
				if (File.Exists(BoxesQuotaPlugin.ConfigPath))
				{
					BoxesQuotaPlugin.getConfig = BQConfigFile.Read(BoxesQuotaPlugin.ConfigPath);
				}
				BoxesQuotaPlugin.getConfig.Write(BoxesQuotaPlugin.ConfigPath);
			}
			catch (Exception ex)
			{
				Log.ConsoleError("Exception in BoxesQuota ConfigFile");
				Log.Error(ex.ToString());
			}
		}
		
		public static void ReloadConfig(CommandArgs args)
		{
			try
			{
				if (File.Exists(BoxesQuotaPlugin.ConfigPath))
				{
					BoxesQuotaPlugin.getConfig = BQConfigFile.Read(BoxesQuotaPlugin.ConfigPath);
				}
				BoxesQuotaPlugin.getConfig.Write(BoxesQuotaPlugin.ConfigPath);
				args.Player.SendMessage("BoxesQuota Config Reloaded Successfully.", Color.MediumSeaGreen);
			}
			catch (Exception ex)
			{
				args.Player.SendMessage("Error: Could not reload BoxesQuota config, Check log for more details.", Color.OrangeRed);
				Log.Error("Exception in BoxesQuota ConfigFile");
				Log.Error(ex.ToString());
			}
		}
		
		public static void CreateExample()
		{
			new BQConfigFile().Write(BoxesQuotaPlugin.ConfigPath);
			/*File.WriteAllText(BoxesQuotaPlugin.ConfigPath,
			                  "{" + Environment.NewLine +
			                  "  \"UserTilesQuota\": \"10000\"" + Environment.NewLine +
			                  "}");
*/
		}
	}
}

