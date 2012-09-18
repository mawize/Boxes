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

		public const string CommandName = "box";
		public const string SlashCommand = "/" + CommandName;

        private BoxManager BoxMan;

        public Commands( BoxManager r )
        {
            BoxMan = r;
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
					new Info(BoxMan).Execute(args);
					break;
                case "set":
					new Set().Execute(args);
					break;
				case "clear":
					new Clear().Execute(args);
					break;
                case "define":
					new Define(BoxMan).Execute(args);
					break;
				case "delete":
					new Delete(BoxMan).Execute(args);
					break;
				case "protect":
					new Protect(BoxMan).Execute(args);
					break;
                case "allow":
					new Allow(BoxMan).Execute(args);
					break;
                case "remove":
					new Remove(BoxMan).Execute(args);
					break;
                case "allowg":
					new AllowG(BoxMan).Execute(args);
					break;
                case "removeg":
					new RemoveG(BoxMan).Execute(args);
					break;
                case "list":
					new List(BoxMan).Execute(args);
					break; 
				case "resize":
				case "expand":
					new Resize(BoxMan).Execute(args);
					break;
                case "z":
                    {
                        if (args.Parameters.Count == 3)
                        {
                            string boxName = args.Parameters[1];
							bool isOwner = (args.Player.UserAccountName ==  BoxMan.GetBoxByName(boxName).Owner || args.Player.Group.HasPermission("boxes.admin") || args.Player.Group.Name == "superadmin");
							if(isOwner){
	                            int z = 0;
	                            if (int.TryParse(args.Parameters[2], out z))
	                                if (BoxMan.SetZ(boxName, z))
										ChatHandler.communicate(ChatHandler.CustomSuccess, args.Player, "Box's z is now " + z);
	                                else
										ChatHandler.communicate(ChatHandler.BoxNotFound, args.Player, boxName);
	                            else
									ChatHandler.communicate(ChatHandler.InvalidSyntax, args.Player, SlashCommand + " z [name] [#]");
							} else 
								ChatHandler.communicate(ChatHandler.NoPermission, args.Player, boxName);
                        }
                        else
							ChatHandler.communicate(ChatHandler.InvalidSyntax, args.Player,  SlashCommand + " z [name] [#]");
                        break;
                    }
				case "help":
				default:
				{
					args.Player.SendMessage("Avialable Boxes commands:");
					args.Player.SendMessage("" +  SlashCommand + " set [1/2] " +  SlashCommand + " define [boxName] " +  SlashCommand + " protect [boxName] [true/false]");
					args.Player.SendMessage("" +  SlashCommand + " name (provides boxName)");
					args.Player.SendMessage("" +  SlashCommand + " delete [boxName] " +  SlashCommand + " clear (temporary box)");
					args.Player.SendMessage("" +  SlashCommand + " allow [playerName] [boxName]");
					args.Player.SendMessage("" +  SlashCommand + " resize [boxname] [u/d/l/r] [amount]");
					break;
				}
			}
        }
    }
}
