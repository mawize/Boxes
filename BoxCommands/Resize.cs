using System;
using TShockAPI;

namespace Boxes.BoxCommands
{
	public class Resize : BoxCommand
	{
		private BoxManager boxman = BoxManager.GetInstance();
		
		public Resize ()
		{
		}

		public override void Execute(CommandArgs args){
			if (args.Parameters.Count == 4)
			{
				int direction;
				switch (args.Parameters[2])
				{
				case "u":
				case "up":
				{
					direction = 0;
					break;
				}
				case "r":
				case "right":
				{
					direction = 1;
					break;
				}
				case "d":
				case "down":
				{
					direction = 2;
					break;
				}
				case "l":
				case "left":
				{
					direction = 3;
					break;
				}
				default:
				{
					direction = -1;
					break;
				}
				}
				int addAmount;
				int.TryParse(args.Parameters[3], out addAmount);
				string boxName = args.Parameters[1];
				if(IsOwner(args.Player, boxman.GetBoxByName(boxName)))
				{
					if(args.Player.Group.HasPermission("boxes.admin") || Hooks.BoxesHooks.AskOnDefineHooks(args))
					{
						if (boxman.resizeBox(boxName, addAmount, direction))
						{
							ChatHandler.communicate(ChatHandler.CustomSuccess, args.Player, "Box Resized Successfully!");
							boxman.ReloadAllBoxes();
						}
						else
							ChatHandler.communicate(ChatHandler.CustomError, args.Player, "Syntax? Does box exist?");
					}
					else
						ChatHandler.communicate(ChatHandler.CustomError, args.Player, "Box " + boxName + " could not be resized");
				}
				else 
					ChatHandler.communicate(ChatHandler.NoPermission, args.Player, boxName);
			}
			else
				ChatHandler.communicate(ChatHandler.InvalidSyntax, args.Player, "resize [boxname] [u/d/l/r] [amount]1");
		}
	}
}

