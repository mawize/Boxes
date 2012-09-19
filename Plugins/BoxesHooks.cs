using System;
using System.Collections.Generic;
using TShockAPI;
using Boxes;

namespace Hooks
{
	public class BoxesHooks
	{
		private static IList<Func<CommandArgs, bool>> onDefineHooks = new List<Func<CommandArgs, bool>>();
		public static event Func<CommandArgs, bool> onDefine {
			add {
				onDefineHooks.Add(value);
			}
			remove {
				onDefineHooks.Remove(value);
			}
		}
		
		private static IList<Func<CommandArgs, bool>> onResizeHooks = new List<Func<CommandArgs, bool>>();
		public static event Func<CommandArgs, bool> onResize {
			add {
				onResizeHooks.Add(value);
			}
			remove {
				onResizeHooks.Remove(value);
			}
		}

		private BoxesHooks ()
		{
		}

		public static bool AskOnDefineHooks (CommandArgs args)
		{
			bool ret = true;
			onDefineHooks.ForEach(delegate(Func<CommandArgs, bool> a){ 
				ret &= a(args); 
			});
			return ret;
		}

		public static bool AskOnResizeHooks (CommandArgs args)
		{
			bool ret = true;
			onResizeHooks.ForEach(delegate(Func<CommandArgs, bool> a){ 
				ret &= a(args);
			});
			return ret;
		}
	}
}

