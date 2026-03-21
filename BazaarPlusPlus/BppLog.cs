using System;
using System.Collections.Generic;
using BepInEx.Logging;

namespace BazaarPlusPlus
{
	// Token: 0x0200003A RID: 58
	internal static class BppLog
	{
		// Token: 0x06000238 RID: 568 RVA: 0x0000E480 File Offset: 0x0000C680
		public static string Format(string component, string message)
		{
			return "[BPP][" + component + "] " + message;
		}

		// Token: 0x06000239 RID: 569 RVA: 0x0000E493 File Offset: 0x0000C693
		public static string FormatError(string component, string message, Exception ex)
		{
			return string.Format("{0}{1}{2}", BppLog.Format(component, message), Environment.NewLine, ex);
		}

		// Token: 0x0600023A RID: 570 RVA: 0x0000E4AC File Offset: 0x0000C6AC
		public static void Debug(string component, string message)
		{
			if (ModState.IsDebug)
			{
				BppLog.Write(32, BppLog.Format(component, message));
			}
		}

		// Token: 0x0600023B RID: 571 RVA: 0x0000E4C3 File Offset: 0x0000C6C3
		public static void Info(string component, string message)
		{
			BppLog.Write(16, BppLog.Format(component, message));
		}

		// Token: 0x0600023C RID: 572 RVA: 0x0000E4D3 File Offset: 0x0000C6D3
		public static void Warn(string component, string message)
		{
			BppLog.Write(4, BppLog.Format(component, message));
		}

		// Token: 0x0600023D RID: 573 RVA: 0x0000E4E2 File Offset: 0x0000C6E2
		public static void Error(string component, string message)
		{
			BppLog.Write(2, BppLog.Format(component, message));
		}

		// Token: 0x0600023E RID: 574 RVA: 0x0000E4F1 File Offset: 0x0000C6F1
		public static void Error(string component, string message, Exception ex)
		{
			BppLog.Write(2, BppLog.FormatError(component, message, ex));
		}

		// Token: 0x0600023F RID: 575 RVA: 0x0000E504 File Offset: 0x0000C704
		public static void Flush()
		{
			ManualLogSource logger = ModState.Logger;
			if (logger == null)
			{
				return;
			}
			object syncRoot = BppLog.SyncRoot;
			lock (syncRoot)
			{
				BppLog.FlushPendingState(logger);
				BppLog.RecentEntries.Clear();
			}
		}

		// Token: 0x06000240 RID: 576 RVA: 0x0000E558 File Offset: 0x0000C758
		private static void Write(LogLevel level, string message)
		{
			ManualLogSource logger = ModState.Logger;
			if (logger == null)
			{
				return;
			}
			level = BppLog.NormalizeLevel(level);
			object syncRoot = BppLog.SyncRoot;
			lock (syncRoot)
			{
				bool flag2 = false;
				if (!BppLog.TryConsumeActiveSequence(logger, level, message, ref flag2))
				{
					if (flag2)
					{
						BppLog.Log(logger, level, message);
						BppLog.RememberEntry(new BppLog.BufferedLogEntry(level, message));
					}
					else if (!BppLog.TryStartRepeatedSequence(level, message))
					{
						BppLog.Log(logger, level, message);
						BppLog.RememberEntry(new BppLog.BufferedLogEntry(level, message));
					}
				}
			}
		}

		// Token: 0x06000241 RID: 577 RVA: 0x0000E5EC File Offset: 0x0000C7EC
		private static LogLevel NormalizeLevel(LogLevel level)
		{
			return level;
		}

		// Token: 0x06000242 RID: 578 RVA: 0x0000E5F0 File Offset: 0x0000C7F0
		private static bool TryConsumeActiveSequence(ManualLogSource logger, LogLevel level, string message, ref bool flushedActiveSequence)
		{
			if (BppLog._activeSequence == null || BppLog._activeSequence.Count == 0)
			{
				return false;
			}
			if (!BppLog._activeSequence[BppLog._activeSequenceIndex].Matches(level, message))
			{
				BppLog.FlushPendingState(logger);
				flushedActiveSequence = true;
				return false;
			}
			if (BppLog._activeSequence.Count > 1)
			{
				BppLog.ActiveSequenceBuffer.Add(new BppLog.BufferedLogEntry(level, message));
			}
			BppLog._activeSequenceIndex++;
			if (BppLog._activeSequenceIndex < BppLog._activeSequence.Count)
			{
				return true;
			}
			BppLog._activeSequenceIndex = 0;
			BppLog._activeSequenceRepeatCount++;
			BppLog.ActiveSequenceBuffer.Clear();
			return true;
		}

		// Token: 0x06000243 RID: 579 RVA: 0x0000E694 File Offset: 0x0000C894
		private static bool TryStartRepeatedSequence(LogLevel level, string message)
		{
			int i = Math.Min(BppLog.GetRepeatDetectionMaxLength(), BppLog.RecentEntries.Count);
			while (i >= 1)
			{
				int num = BppLog.RecentEntries.Count - i;
				if (BppLog.RecentEntries[num].Matches(level, message))
				{
					BppLog._activeSequence = new List<BppLog.BufferedLogEntry>(i);
					for (int j = num; j < BppLog.RecentEntries.Count; j++)
					{
						BppLog._activeSequence.Add(BppLog.RecentEntries[j]);
					}
					BppLog._activeSequenceRepeatCount = 0;
					BppLog._activeSequenceIndex = 0;
					BppLog.ActiveSequenceBuffer.Clear();
					if (i == 1)
					{
						BppLog._activeSequenceRepeatCount = 1;
						return true;
					}
					BppLog._activeSequenceIndex = 1;
					BppLog.ActiveSequenceBuffer.Add(new BppLog.BufferedLogEntry(level, message));
					return true;
				}
				else
				{
					i--;
				}
			}
			return false;
		}

		// Token: 0x06000244 RID: 580 RVA: 0x0000E75C File Offset: 0x0000C95C
		private static void FlushPendingState(ManualLogSource logger)
		{
			if (BppLog._activeSequence != null && BppLog._activeSequenceRepeatCount > 0)
			{
				BppLog.Log(logger, BppLog.GetSummaryLevel(), BppLog.Format("Logger", BppLog.BuildRepeatSummary()));
			}
			if (BppLog.ActiveSequenceBuffer.Count > 0)
			{
				foreach (BppLog.BufferedLogEntry entry in BppLog.ActiveSequenceBuffer)
				{
					BppLog.Log(logger, entry.Level, entry.Message);
					BppLog.RememberEntry(entry);
				}
			}
			BppLog.ActiveSequenceBuffer.Clear();
			BppLog._activeSequence = null;
			BppLog._activeSequenceIndex = 0;
			BppLog._activeSequenceRepeatCount = 0;
		}

		// Token: 0x06000245 RID: 581 RVA: 0x0000E814 File Offset: 0x0000CA14
		private static string BuildRepeatSummary()
		{
			if (BppLog._activeSequence == null || BppLog._activeSequence.Count == 0)
			{
				return "Repeated log sequence suppressed";
			}
			if (BppLog._activeSequence.Count == 1)
			{
				return string.Format("Previous message repeated {0} additional time(s)", BppLog._activeSequenceRepeatCount);
			}
			return string.Format("Previous {0}-message sequence repeated {1} additional time(s)", BppLog._activeSequence.Count, BppLog._activeSequenceRepeatCount);
		}

		// Token: 0x06000246 RID: 582 RVA: 0x0000E880 File Offset: 0x0000CA80
		private static LogLevel GetSummaryLevel()
		{
			if (BppLog._activeSequence == null || BppLog._activeSequence.Count == 0)
			{
				return 16;
			}
			return BppLog._activeSequence[0].Level;
		}

		// Token: 0x06000247 RID: 583 RVA: 0x0000E8B6 File Offset: 0x0000CAB6
		private static int GetRepeatDetectionMaxLength()
		{
			return 3;
		}

		// Token: 0x06000248 RID: 584 RVA: 0x0000E8BC File Offset: 0x0000CABC
		private static void RememberEntry(BppLog.BufferedLogEntry entry)
		{
			BppLog.RecentEntries.Add(entry);
			int repeatDetectionMaxLength = BppLog.GetRepeatDetectionMaxLength();
			while (BppLog.RecentEntries.Count > repeatDetectionMaxLength)
			{
				BppLog.RecentEntries.RemoveAt(0);
			}
		}

		// Token: 0x06000249 RID: 585 RVA: 0x0000E8F4 File Offset: 0x0000CAF4
		private static void Log(ManualLogSource logger, LogLevel level, string message)
		{
			if (level <= 4)
			{
				if (level == 2)
				{
					logger.LogError(message);
					return;
				}
				if (level == 4)
				{
					logger.LogWarning(message);
					return;
				}
			}
			else
			{
				if (level == 16)
				{
					logger.LogInfo(message);
					return;
				}
				if (level == 32)
				{
					logger.LogDebug(message);
					return;
				}
			}
			logger.Log(level, message);
		}

		// Token: 0x04000137 RID: 311
		private const string Prefix = "[BPP]";

		// Token: 0x04000138 RID: 312
		private static readonly object SyncRoot = new object();

		// Token: 0x04000139 RID: 313
		private static readonly List<BppLog.BufferedLogEntry> RecentEntries = new List<BppLog.BufferedLogEntry>();

		// Token: 0x0400013A RID: 314
		private static readonly List<BppLog.BufferedLogEntry> ActiveSequenceBuffer = new List<BppLog.BufferedLogEntry>();

		// Token: 0x0400013B RID: 315
		private static List<BppLog.BufferedLogEntry> _activeSequence;

		// Token: 0x0400013C RID: 316
		private static int _activeSequenceIndex;

		// Token: 0x0400013D RID: 317
		private static int _activeSequenceRepeatCount;

		// Token: 0x02000086 RID: 134
		private readonly struct BufferedLogEntry
		{
			// Token: 0x0600032B RID: 811 RVA: 0x00011C66 File Offset: 0x0000FE66
			public BufferedLogEntry(LogLevel level, string message)
			{
				this.Level = level;
				this.Message = message;
			}

			// Token: 0x17000081 RID: 129
			// (get) Token: 0x0600032C RID: 812 RVA: 0x00011C76 File Offset: 0x0000FE76
			public LogLevel Level { get; }

			// Token: 0x17000082 RID: 130
			// (get) Token: 0x0600032D RID: 813 RVA: 0x00011C7E File Offset: 0x0000FE7E
			public string Message { get; }

			// Token: 0x0600032E RID: 814 RVA: 0x00011C86 File Offset: 0x0000FE86
			public bool Matches(LogLevel level, string message)
			{
				return this.Level == level && string.Equals(this.Message, message, StringComparison.Ordinal);
			}
		}
	}
}
