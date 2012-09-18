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
		public const string version = "1.1.0.0";

		private BoxManager BoxMan;
		private IDbConnection DB;
		private Commands comms;
		
		public BoxesPlugin( Main game) : base( game )
		{
		}
		
		public override string Author
		{
			get { return "mawize"; }
		}
		
		public override string Description
		{
			get { return "Region/Area Control"; }
		}

		public override string Name
		{
			get { return "Boxes"; }
		}
		
		public override Version Version
		{
			get { return new System.Version(version); }
		}
		
		public override void Initialize()
		{
			// init DB
			DB = TShock.DB;
			BoxMan = BoxManager.GetInstance();
			BoxMan.EnsureTableExists(DB);
			comms = new Commands( BoxMan );

			// hook setup
			Hooks.GameHooks.PostInitialize += OnPostInit;
			GetDataHandlers.TileEdit += OnTileEdit;

			// register our own hooks to ensure handling when no Plugin is loaded
			Hooks.BoxesHooks.onDefine += Hooks.BoxesHooks.defaultDefine;
			Hooks.BoxesHooks.onResize += Hooks.BoxesHooks.defaultResize;

			// permission Setup
			bool managebox = false;
			bool adminbox = false;
			
			// look if there are already set up permissions
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

			// if not we set this for default user
			if (!managebox)
				perm.Add("boxes.manage");
			TShock.Groups.AddPermissions("default", perm);

			if (!adminbox)
				perm.Add("boxes.admin");
			TShock.Groups.AddPermissions("trustedadmin", perm);

			// register chatcommand
			TShockAPI.Commands.ChatCommands.Add(new Command("boxes.manage", comms.Boxes, Commands.CommandName));
		}
		
		private void OnPostInit()
		{
			BoxMan.ReloadAllBoxes();
		}
		
		private void OnTileEdit(object sender, GetDataHandlers.TileEditEventArgs args)
		{
			if (args.Player.AwaitingName)
			{
				var protectedboxes = BoxMan.InAreaBoxName(args.X, args.Y);
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
			
			Box box = BoxMan.GetTopBox(BoxMan.InAreaBox(args.X, args.Y));
			if (!BoxMan.CanBuild(args.X, args.Y, args.Player))
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
				
				// unregister our own hooks just cause i can
				Hooks.BoxesHooks.onDefine -= Hooks.BoxesHooks.defaultDefine;
				Hooks.BoxesHooks.onResize -= Hooks.BoxesHooks.defaultResize;
			}
			
			base.Dispose(disposing);
		}
		
/*		public BoxManager GetBoxManager()
		{
			return BoxManager;
		}
*/
	}
}