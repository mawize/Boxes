using System;
using TShockAPI;

namespace Boxes
{
	public interface BoxCommand
	{
		void Execute(CommandArgs args);
	}
}

