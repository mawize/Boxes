using System;
using System.IO;
using Boxes;
using TShockAPI;
using Terraria;

namespace BoxesQuota
{
	[APIVersion( 1, 12 )]
	public class BoxesQuotaPlugin : TerrariaPlugin
	{
		internal const string version = "0.0.0.0";
		internal static string PluginConfigsPath { get { return Path.Combine(TShock.SavePath, "PluginConfigs"); } }
		internal static string ConfigPath { get { return Path.Combine(PluginConfigsPath, "BoxesQuotaConfig.json"); } }

		public static BQConfigFile getConfig { get; set; }
		private BoxManager BoxMan;

		public BoxesQuotaPlugin( Main game) : base( game )
		{
		}
		
		public override string Author
		{
			get { return "mawize"; }
		}
		
		public override string Description
		{
			get { return "BoxesQuota Plugin"; }
		}
		
		public override string Name
		{
			get { return "BoxesQuota"; }
		}

		public override Version Version
		{
			get { return new System.Version(version); }
		}

		public override void Initialize ()
		{			
			/* Load Config */
			if (!Directory.Exists(PluginConfigsPath)) //@"tshock/PluginConfigs/"
				Directory.CreateDirectory(PluginConfigsPath);
			
			if (!File.Exists(ConfigPath))
				BQConfigFile.CreateExample();
			
			BQConfigFile.LoadConfig();

			// hook setup
			Hooks.GameHooks.PostInitialize += OnPostInit;
			BoxMan = BoxManager.GetInstance();
			BoxMan.EnsureTableExists(TShock.DB);
		}

		private void OnPostInit ()
		{
			Hooks.BoxesHooks.onDefine += HandleOnDefine;
			Hooks.BoxesHooks.onResize += HandleOnResize;
		}


		bool HandleOnResize (CommandArgs args)
		{
			args.Player.SendMessage(string.Format ("QUOTA: {0} Yes. (NOT IMPLEMENTED)", getConfig.UserTilesQuota));
			return true;
		}

		bool HandleOnDefine (CommandArgs args)
		{
			args.Player.SendMessage(string.Format ("QUOTA: {0} Yes. (NOT IMPLEMENTED)", getConfig.UserTilesQuota));
			return true;
		}

		protected override void Dispose(bool disposing)
		{
			if( disposing )
			{
				Hooks.BoxesHooks.onDefine -= HandleOnDefine;
				Hooks.BoxesHooks.onResize -= HandleOnResize;
			}
			
			base.Dispose(disposing);
		}
	}
}

