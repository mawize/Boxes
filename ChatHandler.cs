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
		public const int TypeForMore = 9;
		public const int CustomWarning = 8;

		public ChatHandler ()
		{
		}

		public static void communicate (int what, TSPlayer to, string parameter)
		{
			switch(what)
			{
			case InvalidSyntax:
				to.SendMessage("Invalid syntax. Proper syntax: " + Commands.SLASH_COMMAND + " " + parameter, Color.Red);
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
			case TypeForMore:
				to.SendMessage("Type " + Commands.SLASH_COMMAND + " list " + parameter + " for more boxes.");
				break;
			case CustomError:
				to.SendMessage(parameter, Color.Red);
				break;
			case CustomSuccess:
				to.SendMessage(parameter, Color.LightGreen);
				break;
			case CustomInfo:
				to.SendMessage(parameter, Color.Azure);
				break;
			case CustomWarning:
				to.SendMessage(parameter, Color.Yellow);
				break;
			}
		}
	}
}

