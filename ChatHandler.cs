using System;
using TShockAPI;

namespace Boxes
{
	public class ChatHandler
	{
		public const int InvalidSyntax = 0;
		public const int NoPermission = 1;
		public const int BoxNotFound = 2;
		public const int PlayerNotFound = 3;
		public const int GroupNotFound = 4;
		public const int CustomError = 5;
		public const int CustomSuccess = 6;
		public const int CustomInfo = 7;
		public const int CustomWarning = 8;

		public ChatHandler ()
		{
		}

		public static void communicate (int what, TSPlayer to, string parameter)
		{
			switch(what)
			{
			case InvalidSyntax:
				to.SendMessage("Invalid syntax. Proper syntax: " + parameter, Color.Red);
				break;
			case NoPermission:
				to.SendMessage("No permissions for box: " + parameter, Color.Red);
				break;
			case BoxNotFound:
				to.SendMessage("Box not found: " + parameter, Color.Red);
				break;
			case PlayerNotFound:
				to.SendMessage("Player not found: " + parameter, Color.Red);
				break;
			case GroupNotFound:
				to.SendMessage("Group not found: " + parameter, Color.Red);
				break;
			case CustomError:
				to.SendMessage(parameter, Color.Red);
				break;
			case CustomSuccess:
				to.SendMessage(parameter, Color.Green);
				break;
			case CustomInfo:
				to.SendMessage(parameter);
				break;
			case CustomWarning:
				to.SendMessage(parameter, Color.Yellow);
				break;
			}
		}
	}
}

