using System;
using System.IO;
using System.Collections.Generic;
using TShockAPI;
using Terraria;
using MySql.Data.MySqlClient;

namespace BoxesQuota
{
	[APIVersion( 1, 12 )]
	public class BoxesQuotaPlugin : TerrariaPlugin
	{
		internal const string version = "1.0.0.0";
		internal static string PluginConfigsPath { get { return Path.Combine(TShock.SavePath, "PluginConfigs"); } }
		internal static string ConfigPath { get { return Path.Combine(PluginConfigsPath, "BoxesQuotaConfig.json"); } }

		public static BQConfigFile getConfig { get; set; }
		private Boxes.BoxManager boxman;
		private Commands cmds;

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

			// permission Setup
			bool viewquota = false;
			bool editquota = false;
			
			// look if there are already set up permissions
			foreach (Group group in TShock.Groups.groups)
			{
				if (group.Name != "superadmin")
				{
					if (group.HasPermission("boxes.viewquota"))
						viewquota = true;
					if (group.HasPermission("boxes.editquota"))
						editquota = true;
				}
			}
			List<string> perm = new List<string>();
			
			// if not we set this for default user
			if (!viewquota)
				perm.Add("boxes.viewquota");
			TShock.Groups.AddPermissions("default", perm);
			
			if (!editquota)
				perm.Add("boxes.editquota");
			TShock.Groups.AddPermissions("trustedadmin", perm);

			// register chatcommand
			cmds = new Commands();
			TShockAPI.Commands.ChatCommands.Add(new Command("boxes.viewquota", cmds.BoxesQuota, Commands.COMMAND_NAME));
		}

		private void OnPostInit ()
		{
			// hook setup for Boxes
			boxman = Boxes.BoxManager.GetInstance();
			Hooks.BoxesHooks.onDefine += HandleOnDefine;
			Hooks.BoxesHooks.onResize += HandleOnResize;
		}


		bool HandleOnResize (CommandArgs args)
		{
			int boxed = boxman.GetUsersBoxedTiles (args.Player.Name);
			int limit;
			int.TryParse(getConfig.UserTilesQuota, out limit);

			int wantToBox;
			int.TryParse (args.Parameters [3], out wantToBox);
			Boxes.Box box = boxman.GetBoxByName (args.Parameters [1]);
			if (args.Parameters[2][0] == 'u' || args.Parameters[2][0] == 'd') {
				wantToBox *= box.Area.Width;
			} else {
				wantToBox *= box.Area.Height;
			}
			args.Player.SendMessage(string.Format ("WantToBox: {0}, AlreadyBoxed: {1}, Quota: {2}", wantToBox, boxed, limit));
			return (boxed+wantToBox)<limit;
		}

		bool HandleOnDefine (CommandArgs args)
		{			
			int boxed = boxman.GetUsersBoxedTiles (args.Player.Name);
			int limit;
			int.TryParse(getConfig.UserTilesQuota, out limit);

			int width = Math.Abs(args.Player.TempPoints[0].X - args.Player.TempPoints[1].X);
			int height = Math.Abs(args.Player.TempPoints[0].Y - args.Player.TempPoints[1].Y);
			
			int wantToBox = width * height;
			args.Player.SendMessage(string.Format ("WantToBox: {0}, AlreadyBoxed: {1}, Quota: {2}", wantToBox, boxed, limit));
			return (boxed+wantToBox)<limit;
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

