using System;
using Boxes;
using TShockAPI;
using Terraria;

namespace BoxesQuota
{
	[APIVersion( 1, 12 )]
	public class BoxesQuotaPlugin : TerrariaPlugin
	{
		public const string version = "0.0.0.0";

		public static ConfigFile Config;
		private BoxManager BoxMan;

		public BoxesQuotaPlugin( Main game) : base( game )
		{
			Config = new ConfigFile();
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
			// read/write config
			Util.SetupConfig();

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
			args.Player.SendMessage("Yes. (NOT IMPLEMENTED)");
			return true;
		}

		bool HandleOnDefine (CommandArgs args)
		{
			args.Player.SendMessage("Yes. (NOT IMPLEMENTED)");
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

