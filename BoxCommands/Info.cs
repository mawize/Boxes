using System;
using TShockAPI;

namespace Boxes.BoxCommands
{
	public class Info : BoxCommand
	{
		private BoxManager boxman = BoxManager.GetInstance();

		public Info ()
		{
		}

		public override void Execute(CommandArgs args)
		{
			if (args.Parameters.Count > 1)
			{
				string boxName = String.Join(" ", args.Parameters.GetRange(1, args.Parameters.Count - 1));
				Box r = boxman.GetBoxByName(boxName);
				if (r == null)
				{
					ChatHandler.communicate(ChatHandler.BoxNotFound, args.Player, boxName);
					return;
				}
				
				ChatHandler.communicate(ChatHandler.CustomInfo, args.Player, r.Name + ": P: " + r.DisableBuild + " X: " + r.Area.X + " Y: " + r.Area.Y + " W: " +
				                        r.Area.Width + " H: " + r.Area.Height);
				foreach (int s in r.AllowedIDs)
				{
					var user = TShock.Users.GetUserByID(s);
					ChatHandler.communicate(ChatHandler.CustomInfo, args.Player, r.Name + ": " + (user != null ? user.Name : "Unknown"));
				}
			}
			else
				ChatHandler.communicate(ChatHandler.InvalidSyntax, args.Player, "info [name]");
		}
	}
}

