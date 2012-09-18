using System;
using System.Linq;
using TShockAPI;
using Terraria;

namespace Boxes.BoxCommands
{
	public class Define : BoxCommand
	{
		private BoxManager BoxMan;

		public Define ( BoxManager BM)
		{
			BoxMan = BM;
		}

		public override void Execute(CommandArgs args){
			if (args.Parameters.Count > 1)
				if (!args.Player.TempPoints.Any(p => p == Point.Zero))
			{
				string boxName = String.Join(" ", args.Parameters.GetRange(1, args.Parameters.Count - 1));
				var x = Math.Min(args.Player.TempPoints[0].X, args.Player.TempPoints[1].X);
				var y = Math.Min(args.Player.TempPoints[0].Y, args.Player.TempPoints[1].Y);
				var width = Math.Abs(args.Player.TempPoints[0].X - args.Player.TempPoints[1].X);
				var height = Math.Abs(args.Player.TempPoints[0].Y - args.Player.TempPoints[1].Y);

				if(args.Player.Group.HasPermission("boxes.admin") || Hooks.BoxesHooks.AskOnDefineHooks(args))
					if (BoxMan.AddBox(x, y, width, height, boxName, args.Player.UserAccountName, Main.worldID.ToString()))
					{
						args.Player.TempPoints[0] = Point.Zero;
						args.Player.TempPoints[1] = Point.Zero;
						ChatHandler.communicate(ChatHandler.CustomSuccess, args.Player, "Box " + boxName + " successfully created");
						ChatHandler.communicate(ChatHandler.CustomSuccess, args.Player, "Box " + boxName + " protected");
					}
					else
						ChatHandler.communicate(ChatHandler.CustomError, args.Player, "Box " + boxName + " already exists");
				else
					ChatHandler.communicate(ChatHandler.CustomError, args.Player, "Box " + boxName + " NOT created");
			}
			else
			{
				ChatHandler.communicate(ChatHandler.CustomError, args.Player, "Points not set up yet.");
			}
			else
				ChatHandler.communicate(ChatHandler.InvalidSyntax, args.Player, "define [name]");
		}
	}
}

