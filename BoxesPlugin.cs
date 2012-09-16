using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TShockAPI;
using Terraria;

namespace Boxes
{
	[APIVersion( 1, 12 )]
	public class BoxesPlugin : TerrariaPlugin
	{
		public static ConfigFile Config;
		private BoxManager BoxManager;
		private IDbConnection DB;
		private Commands comms;
		
		public BoxesPlugin( Main game) : base( game )
		{
			Config = new ConfigFile();
		}
		
		public override string Author
		{
			get { return "mawize"; }
		}
		
		public override string Description
		{
			get { return "Region Control"; }
		}
		
		public override string Name
		{
			get { return "Boxes"; }
		}
		
		public override Version Version
		{
			get { return TShock.VersionNum; }
		}
		
		public override void Initialize()
		{
			// read/write config
			Util.SetupConfig();

			// init DB
			DB = TShock.DB;
			BoxManager = new BoxManager(DB);
			comms = new Commands( BoxManager );

			// hook setup
			Hooks.GameHooks.PostInitialize += OnPostInit;
			GetDataHandlers.TileEdit += OnTileEdit;

			// permission Setup
			bool managebox = false;
			bool adminbox = false;
			
			foreach (Group group in TShock.Groups.groups)
			{
				if (group.Name != "superadmin")
				{
					if (group.HasPermission("boxes.manage"))
						managebox = true;
					if (group.HasPermission("boxes.admin"))
						adminbox = true;
				}
			}
			List<string> perm = new List<string>();
			if (!managebox)
				perm.Add("boxes.manage");
			TShock.Groups.AddPermissions("default", perm);

			if (!adminbox)
				perm.Add("boxes.admin");
			TShock.Groups.AddPermissions("trustedadmin", perm);

			// register chatcommand
			TShockAPI.Commands.ChatCommands.Add(new Command("boxes.manage", comms.Boxes, Commands.Command));

		}
		
		private void OnPostInit()
		{
			BoxManager.ReloadAllBoxes();
		}
		
		private void OnTileEdit(object sender, GetDataHandlers.TileEditEventArgs args)
		{
			if (args.Player.AwaitingName)
			{
				var protectedboxes = BoxManager.InAreaBoxName(args.X, args.Y);
				if (protectedboxes.Count == 0)
				{
					args.Player.SendMessage("Box is not protected", Color.Yellow);
				}
				else
				{
					string boxlist = string.Join(",", protectedboxes.ToArray());
					args.Player.SendMessage("Box Name(s): " + boxlist, Color.Yellow);
				}
				args.Player.SendTileSquare(args.X, args.Y);
				args.Player.AwaitingName = false;
				args.Handled = true;
			}
			
			if (args.Handled)
			{
				return;
			}
			
			Box box = BoxManager.GetTopBox(BoxManager.InAreaBox(args.X, args.Y));
			if (!BoxManager.CanBuild(args.X, args.Y, args.Player))
			{
				if (((DateTime.Now.Ticks/TimeSpan.TicksPerMillisecond) - args.Player.RPm) > 2000)
				{
					args.Player.SendMessage("Box protected from changes.", Color.Red);
					args.Player.RPm = DateTime.Now.Ticks/TimeSpan.TicksPerMillisecond;
				}
				args.Player.SendTileSquare(args.X, args.Y);
				args.Handled = true;
			}
		}
		
		protected override void Dispose(bool disposing)
		{
			if( disposing )
			{
				Hooks.GameHooks.PostInitialize -= OnPostInit;
			}
			
			base.Dispose(disposing);
		}
		
		public BoxManager GetBoxManager()
		{
			return BoxManager;
		}
	}
}