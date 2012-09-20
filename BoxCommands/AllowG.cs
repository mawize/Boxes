using System;
using TShockAPI;

namespace Boxes.BoxCommands
{
	public class AllowG : BoxCommand
	{
		private BoxManager boxman = BoxManager.GetInstance();

		public AllowG ()
		{
		}

		public override void Execute(CommandArgs args){
			if (args.Parameters.Count > 2)
			{
				string group = args.Parameters[1];
				string boxName = args.Parameters[2];
				
				if(IsOwner(args.Player, boxman.GetBoxByName(boxName)))
					if (TShock.Groups.GroupExists(group))
						if (boxman.AllowGroup(boxName, group))
							ChatHandler.communicate(ChatHandler.CustomSuccess, args.Player, "Added group " + group + " to " + boxName);
				else
					ChatHandler.communicate(ChatHandler.BoxNotFound, args.Player, boxName);
				else
					ChatHandler.communicate(ChatHandler.GroupNotFound, args.Player, group);
				else
					ChatHandler.communicate(ChatHandler.NoPermission, args.Player, boxName);
			}
			else
				ChatHandler.communicate(ChatHandler.InvalidSyntax, args.Player, "allow [group] [box]");
		}
	}
}

