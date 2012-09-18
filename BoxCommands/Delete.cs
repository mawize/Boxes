using System;
using TShockAPI;

namespace Boxes.BoxCommands
{
	public class Delete : BoxCommand
	{
		private BoxManager BoxMan;

		public Delete (BoxManager BM)
		{
			BoxMan = BM;
		}

		public override void Execute(CommandArgs args){
			
			if (args.Parameters.Count > 1)
			{
				string boxName = String.Join(" ", args.Parameters.GetRange(1, args.Parameters.Count - 1));
				if(isOwner(args.Player,BoxMan.GetBoxByName(boxName)))
					if (BoxMan.DeleteBox(boxName))
						ChatHandler.communicate(ChatHandler.CustomSuccess, args.Player, "Deleted box " + boxName);
				else
					ChatHandler.communicate(ChatHandler.BoxNotFound, args.Player, boxName);
				else 
					ChatHandler.communicate(ChatHandler.NoPermission, args.Player, boxName);
			}
			else
				ChatHandler.communicate(ChatHandler.InvalidSyntax, args.Player, "delete [name]");
		}
	}
}

