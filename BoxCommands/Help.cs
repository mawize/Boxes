using System;

namespace Boxes
{
	public class Help : BoxCommand
	{
		public Help ()
		{
		}

		public override void Execute (TShockAPI.CommandArgs args)
		{
			args.Player.SendMessage("Avialable Boxes commands:");
			args.Player.SendMessage("" +  Commands.SLASH_COMMAND + " set [1/2] " +  Commands.SLASH_COMMAND + " define [boxName] " +  Commands.SLASH_COMMAND + " protect [boxName] [true/false]");
			args.Player.SendMessage("" +  Commands.SLASH_COMMAND + " name (provides boxName)");
			args.Player.SendMessage("" +  Commands.SLASH_COMMAND + " delete [boxName] " +  Commands.SLASH_COMMAND + " clear (temporary box)");
			args.Player.SendMessage("" +  Commands.SLASH_COMMAND + " allow [playerName] [boxName]");
			args.Player.SendMessage("" +  Commands.SLASH_COMMAND + " resize [boxname] [u/d/l/r] [amount]");
		}
	}
}

