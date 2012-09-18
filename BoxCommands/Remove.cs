using System;
using TShockAPI;

namespace Boxes.BoxCommands
{
	public class Remove : BoxCommand
	{
		private BoxManager BoxMan;

		public Remove (BoxManager BM)
		{
			BoxMan = BM;
		}
		
		public override void Execute(CommandArgs args)
		{
			if (args.Parameters.Count > 2)
			{
				string playerName = args.Parameters[1];
				string boxName = args.Parameters[2];
				
				if(isOwner(args.Player, BoxMan.GetBoxByName(boxName)))
					if (TShock.Users.GetUserByName(playerName) != null)
						if (BoxMan.RemoveUser(boxName, playerName))
							ChatHandler.communicate(ChatHandler.CustomSuccess, args.Player, "Removed user " + playerName + " from " + boxName);
				else
					ChatHandler.communicate(ChatHandler.BoxNotFound, args.Player, boxName);
				else
					ChatHandler.communicate(ChatHandler.PlayerNotFound, args.Player, playerName);
				else
					ChatHandler.communicate(ChatHandler.NoPermission, args.Player, boxName);
			}
			else
				ChatHandler.communicate(ChatHandler.InvalidSyntax, args.Player, "remove [playerName] [boxName]");
		}
	}
}

