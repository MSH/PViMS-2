﻿using System;
using System.Threading;

namespace PVIMS.Core.Services
{
	public abstract class DateService
	{
		private const string slotName = "VPS.DateService.Current";

		public static DateService Current
		{
			get
			{
				var obj = (DateService) Thread.GetData(Thread.GetNamedDataSlot(slotName));

				if (obj == null)
				{
					obj = new DefaultDateService();
					Thread.SetData(Thread.GetNamedDataSlot(slotName), obj);
				}
				return obj;
			}

			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				Thread.SetData(Thread.GetNamedDataSlot(slotName), value);
			}
		}

		public abstract DateTime Now { get; }

		public abstract void PushDate(DateTime startDate);
		public abstract void PopDate();
	}
}