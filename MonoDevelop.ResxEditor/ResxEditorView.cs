using System.Resources;
using Gtk;
using MonoDevelop.Ide.Gui;
using System.IO;
using System.Xml;

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

			using (var stream = new MemoryStream()) {
				var resxWriter = new ResXResourceWriter(stream);
				foreach (var node in nodes) {
					resxWriter.AddResource(node);
				}
				resxWriter.Generate();
				stream.Flush();

				stream.Position = 0;
				//pretty xml
				var document = new XmlDocument();
				document.Load(stream);
				document.Save(fileName);
			}

			ContentName = fileName;
			IsDirty = false;
		}
	}
}