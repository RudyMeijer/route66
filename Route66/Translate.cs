using System;
using System.Collections.Generic;
using MyLib;
namespace Route66
{
	internal class Translate
	{
		private static string[] navigationMessages;
		public static string[] NavigationMessages
		{
			get
			{
				var list = new List<string>();
				if (navigationMessages == null)
				{
					foreach (var item in Enum.GetNames(typeof(Route66.NavigationMessages)))
					{
						list.Add(My.PascalCase(item));
						//list.Add("Turn left");
						//list.Add("Turn right");
						// TODO list.Add(dictionary<string, string>(item));
					}
					navigationMessages = list.ToArray();
				}
				return navigationMessages;
			}
		}
	}
}