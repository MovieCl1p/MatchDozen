using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
	public enum MonoLogChannel
	{
		Services,
		UI,
		Data,
		Parse,
		Facebook,
		GameCenter,
		All,
		Exceptions,
		Splash,
		MiniGames,
		Pet,
		Core,
		Others,
		Controllers,
		MultiPlayer,
		Configs,
		AppController,
		Notifications,
		Sound,
		Quirks,
		Dances,
		Meter,
		etcetEtcetera,
		Storekit,
		Leaks,
		PathFind,
		Abilities,
		Map,
		Chat,
		NeedImplemented
	}
	
	
	public static class MonoLog
	{
		public static List<MonoLogChannel> Channels
		{
			get;
			private set;
		}
		
		static MonoLog()
		{
			Channels = new List<MonoLogChannel>();
			
//			Channels.Add(MonoLogChannel.UI);
//			Channels.Add(MonoLogChannel.Controllers);
//			Channels.Add(MonoLogChannel.Core);
//			Channels.Add(MonoLogChannel.All);
        }
		
		public static void Log(MonoLogChannel channel, object message)
		{			
			if (Channels.Contains(channel) || Channels.Contains(MonoLogChannel.All))
			{
				string details = "[" + System.DateTime.UtcNow.ToString("HH:mm:ss.fff") + "] [" + channel.ToString() + "]: " + message.ToString();
				
				//System.Diagnostics.Trace.TraceInformation(details);
				
				Debug.Log(details);
								
				//System.Diagnostics.Debug.WriteLine(details);
			}
		}
		
		public static void LogWarning(MonoLogChannel channel, object message)
		{
			if (Channels.Contains(channel) || Channels.Contains(MonoLogChannel.All))
			{
				Debug.LogWarning("MySide [" + channel.ToString() + "]: " + message.ToString());	
			}
		}
		
		public static void Log(MonoLogChannel channel, object message, Exception exception)
		{
			if (Channels.Contains(channel) || Channels.Contains(MonoLogChannel.Exceptions) || Channels.Contains(MonoLogChannel.All))
			{
				Debug.LogException(new Exception("MySide [" + channel.ToString() + "]: " + message.ToString(), exception));
			}
		}
	}
}

