using System;
using TShockAPI;

namespace Boxes
{
	public abstract class BoxCommand
	{
		public abstract void Execute(CommandArgs args);

		protected bool IsOwner (TSPlayer player, Box box)
		{
			return (player.UserAccountName ==  box.Owner || player.Group.HasPermission("boxes.admin") || player.Group.Name == "superadmin");
		}
	}
}

