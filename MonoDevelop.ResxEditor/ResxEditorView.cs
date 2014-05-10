using System.Resources;
using Gtk;
using MonoDevelop.Ide.Gui;


namespace MonoDevelop.ResxEditor
{
	public class ResxEditorView : AbstractViewContent
	{
		readonly ResxEditorWidget widget;

		public override Widget Control 
		{ 
			get 
			{ 
				return widget; 
			} 
		}

		public ResxEditorView()
		{
			widget = new ResxEditorWidget(this);
		}

		public override void Load(string fileName)
		{
			widget.SetResxInfo(fileName);
			ContentName = fileName;
			IsDirty = false;
		}

		public override void Save(string fileName)
		{
			ResXDataNode[] nodes = widget.GetResxInfo(fileName);

			using (ResXResourceWriter resxWriter = new ResXResourceWriter(fileName))
			{
				foreach (ResXDataNode node in nodes) 
				{
					resxWriter.AddResource(node);
				}

				resxWriter.Generate();
			}

			ContentName = fileName;
			IsDirty = false;
		}
	}
}