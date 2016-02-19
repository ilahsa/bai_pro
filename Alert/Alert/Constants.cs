using System;
using System.Collections.Generic;
using System.Text;

namespace Alert
{
	public static class Constants
	{
		/// <summary>
		/// ��������
		/// </summary>
		public static bool IsHourAlert = true;

		/// <summary>
		/// �������
		/// </summary>
		public static bool IsHalfHourAlert = true;

		/// <summary>
		/// �Ƿ���������ʾ
		/// </summary>
		public static bool IsSound = true;

		/// <summary>
		/// �����б�
		/// </summary>
		public static List<TimeAlert> AlertList = new List<TimeAlert>();
	}

	public class TimeAlert
	{
		public TimeAlert()
		{ 
			
		}

		//ָ��ʱ������
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
		/// ������Ϣ
		/// </summary>
		public string Message;

		/// <summary>
		/// �������
		/// </summary>
		public SplitTypeEnum SplitType;

		/// <summary>
		/// ����ʱ��
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
		/// �������
		/// </summary>
		public enum SplitTypeEnum
		{ 
			/// <summary>
			/// ����ʾһ��
			/// </summary>
			OnlyOne,

			/// <summary>
			/// ÿ��
			/// </summary>
			Day,

			/// <summary>
			/// ÿ��
			/// </summary>
			Month,

			/// <summary>
			/// ÿ��
			/// </summary>
			Year
		}
	}
}
