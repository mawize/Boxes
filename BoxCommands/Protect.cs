using System;
using TShockAPI;

namespace Boxes.BoxCommands
{
	public class Protect : BoxCommand
	{
		private BoxManager boxman = BoxManager.GetInstance();

		public Protect ()
		{
		}

		public override void Execute(CommandArgs args){
			if (args.Parameters.Count == 3)
			{
				string boxName = args.Parameters[1];
				if(IsOwner(args.Player,boxman.GetBoxByName(boxName)))
				{
					if (args.Parameters[2].ToLower() == "true")
					{
						if (boxman.SetBoxProtected(boxName, true))
							ChatHandler.communicate(ChatHandler.CustomSuccess, args.Player, "Box " + boxName + " protected");
						else
							ChatHandler.communicate(ChatHandler.BoxNotFound, args.Player, boxName);
					}
					else if (args.Parameters[2].ToLower() == "false")
					{
						if (boxman.SetBoxProtected(boxName, false))
							ChatHandler.communicate(ChatHandler.CustomWarning, args.Player, "Box " + boxName + " unprotected");
						else
							ChatHandler.communicate(ChatHandler.BoxNotFound, args.Player, boxName);
					}
					else
						ChatHandler.communicate(ChatHandler.InvalidSyntax, args.Player, "protect [name] [true/false]");
				}
				else
					ChatHandler.communicate(ChatHandler.NoPermission, args.Player, boxName);
			}
			else
				ChatHandler.communicate(ChatHandler.InvalidSyntax, args.Player, "protect [name] [true/false]");
		}
	}
}

