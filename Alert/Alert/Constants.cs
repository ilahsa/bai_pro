using System;
using System.Collections.Generic;
using System.Text;

namespace Alert
{
	public static class Constants
	{
		/// <summary>
		/// 整点提醒
		/// </summary>
		public static bool IsHourAlert = true;

		/// <summary>
		/// 半点提醒
		/// </summary>
		public static bool IsHalfHourAlert = true;

		/// <summary>
		/// 是否有声音提示
		/// </summary>
		public static bool IsSound = true;

		/// <summary>
		/// 提醒列表
		/// </summary>
		public static List<TimeAlert> AlertList = new List<TimeAlert>();
	}

	public class TimeAlert
	{
		public TimeAlert()
		{ 
			
		}

		//指定时间提醒
		public TimeAlert(string message, SplitTypeEnum splitType, DateTime alertTime)
		{
			if (alertTime < DateTime.Now)
			{
				return;
			}

			this.Message = message;
			this.AlertTime = alertTime;
			this.SplitType = splitType;
		}

		/// <summary>
		/// 提醒信息
		/// </summary>
		public string Message;

		/// <summary>
		/// 间隔类型
		/// </summary>
		public SplitTypeEnum SplitType;

		/// <summary>
		/// 提醒时间
		/// </summary>
		public DateTime AlertTime;

		private static string[] TypeName = {"once","everyday","everymonth","everyyear"};
		public static string GetSplitTypeName(SplitTypeEnum type)
		{
			return TypeName[(int)type];
		}

		public static SplitTypeEnum GetSplitType(string type)
		{
			int index = 0;
			for (int i = 0; i < TypeName.Length; i++)
			{
				if (type ==TypeName[i])
				{
					index = i;
					break;
				}
			}

			return (SplitTypeEnum)index;
		}

		/// <summary>
		/// 间隔类型
		/// </summary>
		public enum SplitTypeEnum
		{ 
			/// <summary>
			/// 仅提示一次
			/// </summary>
			OnlyOne,

			/// <summary>
			/// 每天
			/// </summary>
			Day,

			/// <summary>
			/// 每月
			/// </summary>
			Month,

			/// <summary>
			/// 每年
			/// </summary>
			Year
		}
	}
}
