using System;
using TShockAPI;

namespace Boxes.BoxCommands
{
	public class Set : BoxCommand
	{
		public Set ()
		{
		}
		
		public override void Execute(CommandArgs args)
		{
			int choice = 0;
			if (args.Parameters.Count == 2 && int.TryParse(args.Parameters[1], out choice) && choice >= 1 && choice <= 2)
			{
				ChatHandler.communicate(ChatHandler.CustomWarning, args.Player, "Hit a block to Set Point " + choice);
				args.Player.AwaitingTempPoint = choice;
			}
			else
				ChatHandler.communicate(ChatHandler.InvalidSyntax, args.Player, "set [1/2]");
		}

	}
}

