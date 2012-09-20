using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TShockAPI;
using Boxes.BoxCommands;

namespace Boxes 
{
    internal class Commands 
	{

		public const string COMMAND_NAME = "box";
		public const string SLASH_COMMAND = "/" + COMMAND_NAME;

        public Commands()
        {
        }

        public void Boxes(CommandArgs args)
        {
            string cmd = "help";
            if (args.Parameters.Count > 0)
            {
                cmd = args.Parameters[0].ToLower();
            }

			switch (cmd)
            {
            case "name":
				new Name().Execute(args);
				break;
			case "info":
				new Info().Execute(args);
				break;
            case "set":
				new Set().Execute(args);
				break;
			case "clear":
				new Clear().Execute(args);
				break;
            case "define":
				new Define().Execute(args);
				break;
			case "delete":
				new Delete().Execute(args);
				break;
			case "protect":
				new Protect().Execute(args);
				break;
            case "allow":
				new Allow().Execute(args);
				break;
            case "remove":
				new Remove().Execute(args);
				break;
            case "allowg":
				new AllowG().Execute(args);
				break;
            case "removeg":
				new RemoveG().Execute(args);
				break;
            case "list":
				new List().Execute(args);
				break; 
			case "resize":
			case "expand":
				new Resize().Execute(args);
				break;
			case "z":
				new Z().Execute(args);
				break;
			case "help":
			default:
				new Help().Execute(args);
				break;
			}
        }
    }
}
