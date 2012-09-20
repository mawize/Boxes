using System;
using TShockAPI;

namespace Boxes.BoxCommands
{
	public class Delete : BoxCommand
	{
		private BoxManager boxman = BoxManager.GetInstance();

		public Delete ()
		{
		}

		public override void Execute(CommandArgs args){
			
			if (args.Parameters.Count > 1)
			{
				string boxName = String.Join(" ", args.Parameters.GetRange(1, args.Parameters.Count - 1));
				if(IsOwner(args.Player,boxman.GetBoxByName(boxName)))
				{
					if (boxman.DeleteBox(boxName))
						ChatHandler.communicate(ChatHandler.CustomSuccess, args.Player, "Deleted box " + boxName);
					else
						ChatHandler.communicate(ChatHandler.BoxNotFound, args.Player, boxName);
				}
				else 
					ChatHandler.communicate(ChatHandler.NoPermission, args.Player, boxName);
			}
			else
				ChatHandler.communicate(ChatHandler.InvalidSyntax, args.Player, "delete [name]");
		}
	}
}

