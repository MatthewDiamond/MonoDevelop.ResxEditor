using System;
using System.Drawing;
using System.IO;
using System.Linq;

namespace MonoDevelop.ResxEditor
{
	public class FileExtension
	{
		private static string[] TextExtensions = new string[]
		{
			".TXT",
			".XML",
			".HTML",
			".CS",
			".INI"
		};

		private static string[] IconExtensions = new string[]
		{
			".ICO",
			".CUR"
		};

		private static string[] ImageExtensions = new string[]
		{
			".BMP",
			".GIF",
			".JPG",
			".PNG",
			".BMP",
			".TIFF",
			".TIF"
		};

		private static string[] AudioExtensions = new string[]
		{
			".WAV",
			".OGG"
		};

		public static Type FileExtensionToType(string str)
		{
			str = str.ToUpper();

			if (TextExtensions.Contains(str)) 
			{
				return typeof(string);
			}

			if (IconExtensions.Contains(str)) 
			{
				return typeof(Icon);
			}

			if (ImageExtensions.Contains(str)) 
			{
				return typeof(Bitmap);
			}

			if (AudioExtensions.Contains(str)) 
			{
				return typeof(MemoryStream);
			}

			return typeof(byte[]);
		}
	}
}
