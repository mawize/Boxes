using System;
using TShockAPI;

namespace Boxes.BoxCommands
{
	public class Name : BoxCommand
	{
		public Name ()
		{
		}

		public override void Execute(CommandArgs args)
		{
			ChatHandler.communicate(ChatHandler.CustomWarning, args.Player, "Hit a block to get the name of the box");
			args.Player.AwaitingName = true;
		}
	}
}

