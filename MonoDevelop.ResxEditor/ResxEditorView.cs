using System.Resources;
using Gtk;
using MonoDevelop.Ide.Gui;
using System.IO;
using System.Xml;
using MonoDevelop.Components;
using System;
using System.Threading.Tasks;

namespace MonoDevelop.ResxEditor
{
	public class ResxEditorView : ViewContent
	{
		readonly ResxEditorWidget widget;

		public override Control Control
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

		public override Task Load(FileOpenInformation info)
		{
			widget.SetResxInfo(info.FileName);
			ContentName = info.FileName;
			IsDirty = false;
			return Task.FromResult(true);
		}

		public override Task Save(FileSaveInformation info)
		{
			ResXDataNode[] nodes = widget.GetResxInfo(info.FileName);

			using (var stream = new MemoryStream())
			{
				var resxWriter = new ResXResourceWriter(stream);
				foreach (var node in nodes)
				{
					resxWriter.AddResource(node);
				}
				resxWriter.Generate();
				stream.Flush();

				stream.Position = 0;
				//pretty xml
				var document = new XmlDocument();
				document.Load(stream);
				document.Save(info.FileName);
			}

			ContentName = info.FileName;
			IsDirty = false;
			return Task.FromResult(true);
		}
	}
}