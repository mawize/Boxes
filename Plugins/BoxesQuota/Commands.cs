using System;
using TShockAPI;

namespace BoxesQuota 
{
	internal class Commands 
	{
		
		public const string COMMAND_NAME = "boxq";
		public const string SLASH_COMMAND = "/" + COMMAND_NAME;

		private Boxes.BoxManager boxman = Boxes.BoxManager.GetInstance();
		
		public Commands()
		{
		}
		
		public void BoxesQuota(CommandArgs args)
		{
			string cmd = "help";
			if (args.Parameters.Count > 0)
			{
				cmd = args.Parameters[0].ToLower();
			}
			
			switch (cmd)
			{
			case "show":
				// show quota usage of a specific player
			case "extend":
				// extend quota limit of a specific player
			case "shrink":
				// reduce quota limit of a specific player
			case "help":
			default:
				int boxed = boxman.GetUsersBoxedTiles(args.Player.Name);
				int limit;
				int.TryParse(BoxesQuotaPlugin.getConfig.UserTilesQuota, out limit);
				args.Player.SendMessage(string.Format ("AlreadyBoxed: {1}, Limit: {2}", boxed, limit));
				break;
			}
		}
	}
}
