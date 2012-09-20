using System;
using System.Collections.Generic;
using TShockAPI;
using Terraria;

namespace Boxes.BoxCommands
{
	public class List : BoxCommand
	{
		private BoxManager boxman = BoxManager.GetInstance();
		
		public List ()
		{
		}

		public override void Execute(CommandArgs args){
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
			
			var boxes = boxman.ListAllBoxes(Main.worldID.ToString());
			
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
				ChatHandler.communicate(ChatHandler.TypeForMore, args.Player,  string.Format("{0}", (page + 2)));

		}
	}
}

