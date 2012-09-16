using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TShockAPI;
using Terraria;

namespace Boxes
{
    internal class Commands
    {

		public const string Command = "box";
		public const string CommandName = "/" + Command;

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
                    {
                        ChatHandler.communicate(ChatHandler.CustomWarning, args.Player, "Hit a block to get the name of the box");
                        args.Player.AwaitingName = true;
                        break;
                    }
                case "set":
                    {
                        int choice = 0;
                        if (args.Parameters.Count == 2 && int.TryParse(args.Parameters[1], out choice) && choice >= 1 && choice <= 2)
                        {
							ChatHandler.communicate(ChatHandler.CustomWarning, args.Player, "Hit a block to Set Point " + choice);
                            args.Player.AwaitingTempPoint = choice;
                        }
                        else
							ChatHandler.communicate(ChatHandler.InvalidSyntax, args.Player, CommandName + " set [1/2]");
                        break;
                    }
                case "define":
                    {
                        if (args.Parameters.Count > 1)
                            if (!args.Player.TempPoints.Any(p => p == Point.Zero))
                            {
                                string boxName = String.Join(" ", args.Parameters.GetRange(1, args.Parameters.Count - 1));
                                var x = Math.Min(args.Player.TempPoints[0].X, args.Player.TempPoints[1].X);
                                var y = Math.Min(args.Player.TempPoints[0].Y, args.Player.TempPoints[1].Y);
                                var width = Math.Abs(args.Player.TempPoints[0].X - args.Player.TempPoints[1].X);
                                var height = Math.Abs(args.Player.TempPoints[0].Y - args.Player.TempPoints[1].Y);
								if(BoxesPlugin.Config.MaxTilesPerUser==0 || ((width * height)+BoxMan.GetUsersBoxedTiles(args.Player.Name)) <= BoxesPlugin.Config.MaxTilesPerUser)
	                                if (BoxMan.AddBox(x, y, width, height, boxName, args.Player.UserAccountName, Main.worldID.ToString()))
	                                {
	                                    args.Player.TempPoints[0] = Point.Zero;
	                                    args.Player.TempPoints[1] = Point.Zero;
										ChatHandler.communicate(ChatHandler.CustomSuccess, args.Player, "Box " + boxName + " successfully created");
										ChatHandler.communicate(ChatHandler.CustomSuccess, args.Player, "Box " + boxName + " protected");
	                                }
	                                else
										ChatHandler.communicate(ChatHandler.CustomError, args.Player, "Box " + boxName + " already exists");
								else
									ChatHandler.communicate(ChatHandler.CustomError, args.Player, "Box " + boxName + " would exceed MaxTilesPerUser limit");

                            }
                            else
                            {
								ChatHandler.communicate(ChatHandler.CustomWarning, args.Player, "Points not set up yet.");
                            }
                        else
							ChatHandler.communicate(ChatHandler.InvalidSyntax, args.Player, CommandName + " define [name]");
                        break;
                    }
			case "protect":
                    {
                        if (args.Parameters.Count == 3)
                        {
                            string boxName = args.Parameters[1];
							bool isOwner = (args.Player.UserAccountName ==  BoxMan.GetBoxByName(boxName).Owner || args.Player.Group.HasPermission("boxes.admin") || args.Player.Group.Name == "superadmin");
							if(isOwner)
	                            if (args.Parameters[2].ToLower() == "true")
	                            {
	                                if (isOwner && BoxMan.SetBoxProtected(boxName, true))
										ChatHandler.communicate(ChatHandler.CustomSuccess, args.Player, "Box " + boxName + " protected");
	                                else
										ChatHandler.communicate(ChatHandler.BoxNotFound, args.Player, boxName);
	                            }
	                            else if (args.Parameters[2].ToLower() == "false")
	                            {
									if (isOwner && BoxMan.SetBoxProtected(boxName, false))
										ChatHandler.communicate(ChatHandler.CustomWarning, args.Player, "Box " + boxName + " unprotected");
	                                else
										ChatHandler.communicate(ChatHandler.BoxNotFound, args.Player, boxName);
	                            }
	                            else
									ChatHandler.communicate(ChatHandler.InvalidSyntax, args.Player, CommandName + " protect [name] [true/false]");
							else
								ChatHandler.communicate(ChatHandler.NoPermission, args.Player, boxName);
                        }
                        else
							ChatHandler.communicate(ChatHandler.InvalidSyntax, args.Player,  CommandName + " protect [name] [true/false]");
                        break;
                    }
                case "delete":
                    {
		
                        if (args.Parameters.Count > 1)
                        {
                            string boxName = String.Join(" ", args.Parameters.GetRange(1, args.Parameters.Count - 1));
							bool isOwner = (args.Player.UserAccountName ==  BoxMan.GetBoxByName(boxName).Owner || args.Player.Group.HasPermission("boxes.admin") || args.Player.Group.Name == "superadmin");

							if(isOwner)
	                            if (BoxMan.DeleteBox(boxName))
									ChatHandler.communicate(ChatHandler.CustomSuccess, args.Player, "Deleted box " + boxName);
	                            else
									ChatHandler.communicate(ChatHandler.BoxNotFound, args.Player, boxName);
							else 
								ChatHandler.communicate(ChatHandler.NoPermission, args.Player, boxName);
						}
                        else
							ChatHandler.communicate(ChatHandler.InvalidSyntax, args.Player, CommandName + " delete [name]");
                        break;
                    }
                case "clear":
                    {
                        args.Player.TempPoints[0] = Point.Zero;
                        args.Player.TempPoints[1] = Point.Zero;
						ChatHandler.communicate(ChatHandler.CustomSuccess, args.Player, "Cleared temp area");
                        args.Player.AwaitingTempPoint = 0;
                        break;
                    }
                case "allow":
                    {
                        if (args.Parameters.Count > 2)
                        {
                            string playerName = args.Parameters[1];
                            string boxName = args.Parameters[2];
                                
							bool isOwner = (args.Player.UserAccountName ==  BoxMan.GetBoxByName(boxName).Owner || args.Player.Group.HasPermission("boxes.admin") || args.Player.Group.Name == "superadmin");

							if(isOwner)
	                            if (TShock.Users.GetUserByName(playerName) != null)
	                                if (BoxMan.AddNewUser(boxName, playerName))
										ChatHandler.communicate(ChatHandler.CustomSuccess, args.Player, "Added user " + playerName + " to " + boxName);
	                                else
										ChatHandler.communicate(ChatHandler.BoxNotFound, args.Player, boxName);
	                            else
									ChatHandler.communicate(ChatHandler.PlayerNotFound, args.Player, playerName);
							else 
								ChatHandler.communicate(ChatHandler.NoPermission, args.Player, boxName);
                        }
                        else
							ChatHandler.communicate(ChatHandler.InvalidSyntax, args.Player, CommandName + " allow [name] [box]");
                        break;
                    }
                case "remove":
                    if (args.Parameters.Count > 2)
                    {
                        string playerName = args.Parameters[1];
                        string boxName = args.Parameters[2];
                            
						bool isOwner = (args.Player.UserAccountName ==  BoxMan.GetBoxByName(boxName).Owner || args.Player.Group.HasPermission("boxes.admin") || args.Player.Group.Name == "superadmin");
						if(isOwner)
	                        if (TShock.Users.GetUserByName(playerName) != null)
	                            if (BoxMan.RemoveUser(boxName, playerName))
									ChatHandler.communicate(ChatHandler.CustomSuccess, args.Player, "Removed user " + playerName + " from " + boxName);
	                            else
									ChatHandler.communicate(ChatHandler.BoxNotFound, args.Player, boxName);
	                        else
								ChatHandler.communicate(ChatHandler.PlayerNotFound, args.Player, playerName);
						else
							ChatHandler.communicate(ChatHandler.NoPermission, args.Player, boxName);
                    }
                    else
						ChatHandler.communicate(ChatHandler.InvalidSyntax, args.Player, CommandName + " remove [name] [box]");
                    break;
                case "allowg":
                    {
                        if (args.Parameters.Count > 2)
                        {
                            string group = args.Parameters[1];
                            string boxName = args.Parameters[2];

							bool isOwner = (args.Player.UserAccountName ==  BoxMan.GetBoxByName(boxName).Owner || args.Player.Group.HasPermission("boxes.admin") || args.Player.Group.Name == "superadmin");

							if(isOwner)
	                            if (TShock.Groups.GroupExists(group))
	                                if (BoxMan.AllowGroup(boxName, group))
										ChatHandler.communicate(ChatHandler.CustomSuccess, args.Player, "Added group " + group + " to " + boxName);
	                                else
										ChatHandler.communicate(ChatHandler.BoxNotFound, args.Player, boxName);
	                            else
									ChatHandler.communicate(ChatHandler.GroupNotFound, args.Player, group);
							else
								ChatHandler.communicate(ChatHandler.NoPermission, args.Player, boxName);
                        }
                        else
							ChatHandler.communicate(ChatHandler.InvalidSyntax, args.Player, CommandName + " allow [group] [box]");
                        break;
                    }
                case "removeg":
                    if (args.Parameters.Count > 2)
                    {
                        string group = args.Parameters[1];
                        string boxName = args.Parameters[2];

						bool isOwner = (args.Player.UserAccountName ==  BoxMan.GetBoxByName(boxName).Owner || args.Player.Group.HasPermission("boxes.admin") || args.Player.Group.Name == "superadmin");
						if(isOwner)
	                        if (TShock.Groups.GroupExists(group))
	                            if (BoxMan.RemoveGroup(boxName, group))
									ChatHandler.communicate(ChatHandler.CustomSuccess, args.Player, "Removed group " + group + " from " + boxName);
	                            else
									ChatHandler.communicate(ChatHandler.BoxNotFound, args.Player, boxName);
	                        else
								ChatHandler.communicate(ChatHandler.GroupNotFound, args.Player, group);
						else
							ChatHandler.communicate(ChatHandler.NoPermission, args.Player, boxName);
                    }
                    else
						ChatHandler.communicate(ChatHandler.InvalidSyntax, args.Player, CommandName + " removeg [group] [box])");
                    break;
                case "list":
                    {
                        //How many boxes per page
                        const int pagelimit = 15;
                        //How many boxes per line
                        const int perline = 5;
                        //Pages start at 0 but are displayed and parsed at 1
                        int page = 0;


                        if (args.Parameters.Count > 1)
                        {
                            if (!int.TryParse(args.Parameters[1], out page) || page < 1)
                            {
								ChatHandler.communicate(ChatHandler.CustomError, args.Player, string.Format("Invalid page number ({0})", page));
                                return;
                            }
                            page--; //Substract 1 as pages are parsed starting at 1 and not 0
                        }

                        var boxes = BoxMan.ListAllBoxes(Main.worldID.ToString());

                        // Are there even any boxes to display?
                        if (boxes.Count == 0)
                        {
							ChatHandler.communicate(ChatHandler.CustomInfo, args.Player, "There are currently no boxes defined.");
                            return;
                        }

                        //Check if they are trying to access a page that doesn't exist.
                        int pagecount = boxes.Count / pagelimit;
                        if (page > pagecount)
                        {
							ChatHandler.communicate(ChatHandler.CustomError, args.Player, string.Format("Page number exceeds pages ({0}/{1})", page + 1, pagecount + 1));
                            return;
                        }

                        //Display the current page and the number of pages.
						ChatHandler.communicate(ChatHandler.CustomInfo, args.Player, string.Format("Current Boxes ({0}/{1}):", page + 1, pagecount + 1));

                        //Add up to pagelimit names to a list
                        var nameslist = new List<string>();
                        for (int i = (page * pagelimit); (i < ((page * pagelimit) + pagelimit)) && i < boxes.Count; i++)
                            nameslist.Add(boxes[i].Name);

                        //convert the list to an array for joining
                        var names = nameslist.ToArray();
                        for (int i = 0; i < names.Length; i += perline)
							ChatHandler.communicate(ChatHandler.CustomInfo, args.Player, string.Join(", ", names, i, Math.Min(names.Length - i, perline)));

                        if (page < pagecount)
							ChatHandler.communicate(ChatHandler.CustomWarning, args.Player, string.Format("Type " +  CommandName + " list {0} for more boxes.", (page + 2)));

                        break;
                    }
                case "info":
                    {
                        if (args.Parameters.Count > 1)
                        {
                            string boxName = String.Join(" ", args.Parameters.GetRange(1, args.Parameters.Count - 1));
                            Box r = BoxMan.GetBoxByName(boxName);

                            if (r == null)
                            {
								ChatHandler.communicate(ChatHandler.BoxNotFound, args.Player, boxName);
                                break;
                            }

							ChatHandler.communicate(ChatHandler.CustomInfo, args.Player, r.Name + ": P: " + r.DisableBuild + " X: " + r.Area.X + " Y: " + r.Area.Y + " W: " +
                                                    r.Area.Width + " H: " + r.Area.Height);
                            foreach (int s in r.AllowedIDs)
                            {
                                var user = TShock.Users.GetUserByID(s);
								ChatHandler.communicate(ChatHandler.CustomInfo, args.Player, r.Name + ": " + (user != null ? user.Name : "Unknown"));
                            }
                        }
                        else
							ChatHandler.communicate(ChatHandler.InvalidSyntax, args.Player, CommandName + " info [name]");
                        break;
                    }
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
									ChatHandler.communicate(ChatHandler.InvalidSyntax, args.Player, CommandName + " z [name] [#]");
							} else 
								ChatHandler.communicate(ChatHandler.NoPermission, args.Player, boxName);
                        }
                        else
							ChatHandler.communicate(ChatHandler.InvalidSyntax, args.Player,  CommandName + " z [name] [#]");
                        break;
                    }
                case "resize":
                case "expand":
                    {
                        if (args.Parameters.Count == 4)
                        {
                            int direction;
                            switch (args.Parameters[2])
                            {
                                case "u":
                                case "up":
                                    {
                                        direction = 0;
                                        break;
                                    }
                                case "r":
                                case "right":
                                    {
                                        direction = 1;
                                        break;
                                    }
                                case "d":
                                case "down":
                                    {
                                        direction = 2;
                                        break;
                                    }
                                case "l":
                                case "left":
                                    {
                                        direction = 3;
                                        break;
                                    }
                                default:
                                    {
                                        direction = -1;
                                        break;
                                    }
                            }
                            int addAmount;
                            int.TryParse(args.Parameters[3], out addAmount);
							string boxName = args.Parameters[1];
							bool isOwner = (args.Player.UserAccountName ==  BoxMan.GetBoxByName(boxName).Owner || args.Player.Group.HasPermission("boxes.admin") || args.Player.Group.Name == "superadmin");
							if(isOwner){
	                            if (BoxMan.resizeBox(boxName, addAmount, direction))
	                            {
									ChatHandler.communicate(ChatHandler.CustomSuccess, args.Player, "Box Resized Successfully!");
	                                BoxMan.ReloadAllBoxes();
	                            }
	                            else
									ChatHandler.communicate(ChatHandler.CustomSuccess, args.Player, "Syntax? Does box exist?");
							} else 
								ChatHandler.communicate(ChatHandler.NoPermission, args.Player, boxName);
                        }
                        else
							ChatHandler.communicate(ChatHandler.InvalidSyntax, args.Player, CommandName + " resize [boxname] [u/d/l/r] [amount]1");
                        break;
                    }
                case "help":
                default:
                    {
                        args.Player.SendMessage("Avialable Boxes commands:");
						args.Player.SendMessage("" +  CommandName + " set [1/2] " +  CommandName + " define [boxName] " +  CommandName + " protect [boxName] [true/false]");
						args.Player.SendMessage("" +  CommandName + " name (provides boxName)");
						args.Player.SendMessage("" +  CommandName + " delete [boxName] " +  CommandName + " clear (temporary box)");
                        args.Player.SendMessage("" +  CommandName + " allow [playerName] [boxName]");
                        args.Player.SendMessage("" +  CommandName + " resize [boxname] [u/d/l/r] [amount]");
                        break;
                    }
            }
        }
    }
}
