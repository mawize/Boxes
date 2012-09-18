using System;
using TShockAPI;

namespace Boxes.BoxCommands
{
	public class Clear : BoxCommand
	{
		public Clear ()
		{
		}
		
		public override void Execute(CommandArgs args)
		{
			args.Player.TempPoints[0] = Point.Zero;
			args.Player.TempPoints[1] = Point.Zero;
			ChatHandler.communicate(ChatHandler.CustomSuccess, args.Player, "Cleared temp area");
			args.Player.AwaitingTempPoint = 0;
		}
	}
}

