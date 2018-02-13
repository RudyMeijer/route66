using System;
using System.Collections.Generic;
using MyLib;
namespace Route66
{
	internal static class Translate
	{
		private static string[] navigationMessages;
		public static string[] NavigationMessages
		{
			get
			{
				var list = new List<string>();
				if (navigationMessages == null)
				{
					foreach (var item in Enum.GetNames(typeof(NavigationTypes)))
					{
						list.Add(My.PascalCase(item));
						////list.Add("Turn left");
						////list.Add("Turn right");
						////list.Add(dictionary<string, string>(item));
					}
					navigationMessages = list.ToArray();
				}
				return navigationMessages;
			}
		}
	}
}