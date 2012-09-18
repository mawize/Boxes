using System;
using TShockAPI;

namespace Boxes.BoxCommands
{
	public class RemoveG : BoxCommand
	{
		private BoxManager BoxMan;

		public RemoveG (BoxManager BM)
		{
			BoxMan = BM;
		}
		
		public override void Execute(CommandArgs args){
			if (args.Parameters.Count > 2)
			{
				string group = args.Parameters[1];
				string boxName = args.Parameters[2];
				
				if(isOwner(args.Player, BoxMan.GetBoxByName(boxName)))
					if (TShock.Groups.GroupExists(group))
						if (BoxMan.RemoveGroup(boxName, group))
							ChatHandler.communicate(ChatHandler.CustomSuccess, args.Player, "Removed group " + group + " from " + boxName);
				else
					ChatHandler.communicate(ChatHandler.BoxNotFound, args.Player, boxName);
				else
					ChatHandler.communicate(ChatHandler.GroupNotFound, args.Player, group);
				else
					ChatHandler.communicate(ChatHandler.NoPermission, args.Player, boxName);
			}
			else
				ChatHandler.communicate(ChatHandler.InvalidSyntax, args.Player, "removeg [group] [box])");
		}
	}
}

