using MonoDevelop.Core;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Projects;

namespace MonoDevelop.ResxEditor
{
	public class ResxEditorDisplayBinding : IViewDisplayBinding
	{
		public bool CanUseAsDefault 
		{ 
			get 
			{ 
				return true; 
			} 
		}

		public string Name 
		{ 
			get 
			{ 
				return GettextCatalog.GetString("Resx Editor"); 
			} 
		}

		public ViewContent CreateContent(FilePath fileName, string mimeType, Project ownerProject)
		{
			return new ResxEditorView();
		}

		public bool CanHandle(FilePath fileName, string mimeType, Project ownerProject)
		{
			return mimeType == "text/microsoft-resx" || fileName.Extension.ToUpper() == ".RESX";
		}
	}
}