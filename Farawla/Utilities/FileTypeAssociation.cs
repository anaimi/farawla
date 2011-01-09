using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;

namespace Farawla.Utilities
{
	class FileTypeAssociation
	{
		
		private static RegistryKey GetRegistryPath(bool readOnly)
		{
			if (readOnly)
			{
				return Registry.CurrentUser.OpenSubKey(@"Software\Classes");
			}
			
			return Registry.CurrentUser.CreateSubKey(@"Software\Classes");
		}

		// Associate file extension with progID, description, icon and application
		public static void Associate(string extension, string progID, string description, string icon, string application)
		{
			GetRegistryPath(false).CreateSubKey(extension).SetValue("", progID);
			
			if (!string.IsNullOrEmpty(progID))
			{
				using (var key = GetRegistryPath(false).CreateSubKey(progID))
				{
					if (description != null)
						key.SetValue("", description);
						
					if (icon != null)
						key.CreateSubKey("DefaultIcon").SetValue("", ToShortPathName(icon));
						
					if (application != null)
						key.CreateSubKey(@"Shell\Open\Command").SetValue("", "\"" + ToShortPathName(application) + "\" \"%1\"");
				}
			}
		}
		
		public static void RemoveAssociation(string extesion)
		{
			GetRegistryPath(false).DeleteSubKey(extesion);
		}

		// Return true if extension already associated in registry
		public static bool IsAssociated(string extension)
		{
			return (GetRegistryPath(true).OpenSubKey(extension, false) != null);
		}
		
		public static string GetAssociationProgId(string extension)
		{
			var key = GetRegistryPath(true).OpenSubKey(extension, false);
			
			if (key == null)
				return string.Empty;
			
			return key.GetValue("").ToString();
		}

		[DllImport("Kernel32.dll")]
		private static extern uint GetShortPathName(string lpszLongPath, [Out] StringBuilder lpszShortPath, uint cchBuffer);

		// Return short path format of a file name
		private static string ToShortPathName(string longName)
		{
			var str = new StringBuilder(1000);
			
			GetShortPathName(longName, str, (uint)str.Capacity);
			
			return str.ToString();
		}
	}
}
