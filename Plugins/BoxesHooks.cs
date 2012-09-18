using System;
using TShockAPI;
using Boxes;

namespace Hooks
{
	public class BoxesHooks
	{
		public static event Func<CommandArgs, bool> onDefine;
		public static event Func<CommandArgs, bool> onResize;

		private BoxesHooks ()
		{
		}

		public static bool AskOnDefineHooks (CommandArgs args)
		{
			return InvokeHook(args, onDefine);
		}

		public static bool AskOnResizeHooks (CommandArgs args)
		{
			return InvokeHook(args, onResize);
		}

		public static TResult InvokeHook<TResult>(CommandArgs args, Func<CommandArgs, TResult> action)
		{
				return action(args);
		}
		
		public static bool defaultDefine(CommandArgs args)
		{
			return true;
		}
		
		public static bool defaultResize(CommandArgs args)
		{
			return true;
		}
	}
}

