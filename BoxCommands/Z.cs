using System;

namespace Boxes
{
	public class Z : BoxCommand
	{
		private BoxManager boxman = BoxManager.GetInstance();
		
		public Z ()
		{
		}
		public override void Execute (TShockAPI.CommandArgs args)
		{
			if (args.Parameters.Count == 3)
			{
				string boxName = args.Parameters[1];
				if(args.Player.Group.HasPermission("boxes.admin")|| args.Player.Group.Name == "superadmin")
				{
					int z = 0;
					if (int.TryParse(args.Parameters[2], out z))
					{
						if (boxman.SetZ(boxName, z))
							ChatHandler.communicate(ChatHandler.CustomSuccess, args.Player, "Box's z is now " + z);
						else
							ChatHandler.communicate(ChatHandler.BoxNotFound, args.Player, boxName);
					}
					else
						ChatHandler.communicate(ChatHandler.InvalidSyntax, args.Player, "z [name] [#]");
				}
				else 
					ChatHandler.communicate(ChatHandler.NoPermission, args.Player, boxName);
			}
			else
				ChatHandler.communicate(ChatHandler.InvalidSyntax, args.Player, "z [name] [#]");
		}
	}
}

