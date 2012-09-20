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
			return AskAndBoolReturnHooks(args, onDefineHooks);
		}
		
		public static bool AskOnResizeHooks (CommandArgs args)
		{
			return AskAndBoolReturnHooks(args, onResizeHooks);
		}
		
		private static bool AskAndBoolReturnHooks (CommandArgs args, IList<Func<CommandArgs, bool>> hooks)
		{
			bool ret = true;
			hooks.ForEach(delegate(Func<CommandArgs, bool> a){ 
				ret &= a(args);
			});
			return ret;
		}
		
		private static bool AskOrBoolReturnHooks (CommandArgs args, IList<Func<CommandArgs, bool>> hooks)
		{
			bool ret = false;
			hooks.ForEach(delegate(Func<CommandArgs, bool> a){ 
				ret |= a(args);
			});
			return ret;
		}
	}
}

