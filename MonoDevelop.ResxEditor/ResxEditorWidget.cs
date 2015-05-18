using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Linq;
using Gdk;
using Gtk;
using MonoDevelop.Ide.Gui.Dialogs;
using IOPath = System.IO.Path;

namespace MonoDevelop.ResxEditor
{
	public partial class ResxEditorWidget : Bin
	{
		static AssemblyName[] assemblyNames = new AssemblyName[] {
			typeof(string).Assembly.GetName(),
			typeof(Icon).Assembly.GetName()
		};

		Dictionary<string, ResXDataNode> resxNodeList;
		ListStore audioListStore;
		ListStore fileListStore;
		ListStore iconListStore;
		ListStore imageListStore;
		ListStore otherListStore;
		ListStore stringListStore;
		Menu typeMenu;
		MenuItem typeMenuAudioItem;
		MenuItem typeMenuFileItem;
		MenuItem typeMenuIconItem;
		MenuItem typeMenuImageItem;
		MenuItem typeMenuOtherItem;
		MenuItem typeMenuStringItem;
		MenuToolButton typeToolButton;
		OpenFileDialog openDialog;
		EditStringDialog stringDialog;
        ResxEditorView editorView;
        ToolButton addButton;
        ToolButton editButton;
		ToolButton removeButton;
		Toolbar mainToolbar;
		TreeView audioTreeView;
		TreeView fileTreeView;
		TreeView iconTreeView;
		TreeView imageTreeView;
		TreeView otherTreeView;
		TreeView stringTreeView;
		string resxBaseName;

		public ResxEditorWidget(ResxEditorView view)
		{
			openDialog = new OpenFileDialog();
			resxNodeList = new Dictionary<string, ResXDataNode>();
			editorView = view;

			stringDialog = new EditStringDialog();

			typeMenuAudioItem = new MenuItem("Audio");
			typeMenuAudioItem.Activated += DropDownEventHandler;

			typeMenuFileItem = new MenuItem("Files");
			typeMenuFileItem.Activated += DropDownEventHandler;

			typeMenuIconItem = new MenuItem("Icons");
			typeMenuIconItem.Activated += DropDownEventHandler;

			typeMenuImageItem = new MenuItem("Images");
			typeMenuImageItem.Activated += DropDownEventHandler;

			typeMenuOtherItem = new MenuItem("Other");
			typeMenuOtherItem.Activated += DropDownEventHandler;

			typeMenuStringItem = new MenuItem("Strings");
			typeMenuStringItem.Activated += DropDownEventHandler;

			// Menus
            typeMenu = new Menu() {
                typeMenuStringItem,
				typeMenuAudioItem,
				typeMenuFileItem,
				typeMenuIconItem,
				typeMenuImageItem,
				typeMenuOtherItem
			};

			// MenuToolButtons
			typeToolButton = new MenuToolButton(null, "Strings"){ Menu = typeMenu };

            addButton = new ToolButton(null, "Add");
			addButton.Clicked += AddButtonEventHandler;

            editButton = new ToolButton(null, "Edit");
            editButton.Clicked += EditButtonEventHandler;

			removeButton = new ToolButton(null, "Remove");
			removeButton.Clicked += RemoveButtonEventHandler;

			// Toolbars
            mainToolbar = new Toolbar(){ addButton, editButton, removeButton, typeToolButton };

			// ListStores
			audioListStore = new ListStore(typeof(string), typeof(string), typeof(string));
			fileListStore = new ListStore(typeof(string), typeof(string), typeof(string), typeof(string));
			iconListStore = new ListStore(typeof(Pixbuf), typeof(string), typeof(string));
			imageListStore = new ListStore(typeof(Pixbuf), typeof(string), typeof(string), typeof(string));
			otherListStore = new ListStore(typeof(string), typeof(string), typeof(string));
			stringListStore = new ListStore(typeof(string), typeof(string), typeof(string), typeof(string));

			// TreeViews
			audioTreeView = new TreeView(audioListStore);
			audioTreeView.Selection.Mode = SelectionMode.Multiple;
			audioTreeView.AppendColumn("ResourceName", new CellRendererText(), "text", 0).Resizable = true;  
			audioTreeView.AppendColumn("FileSize", new CellRendererText(), "text", 1).Resizable = true;
			audioTreeView.AppendColumn("Comment", new CellRendererText(), "text", 2).Resizable = true;

			fileTreeView = new TreeView(fileListStore);
			fileTreeView.Selection.Mode = SelectionMode.Multiple;
			fileTreeView.AppendColumn("ResourceName", new CellRendererText(), "text", 0).Resizable = true;
			fileTreeView.AppendColumn("FileSize", new CellRendererText(), "text", 1).Resizable = true;
			fileTreeView.AppendColumn("FileName/Path", new CellRendererText(), "text", 2).Resizable = true;
			fileTreeView.AppendColumn("Comment", new CellRendererText(), "text", 3).Resizable = true;

			iconTreeView = new TreeView(iconListStore);
			iconTreeView.Selection.Mode = SelectionMode.Multiple;
			iconTreeView.AppendColumn("Icon", new CellRendererPixbuf(), "pixbuf", 0).Resizable = true;  
			iconTreeView.AppendColumn("ResourceName", new CellRendererText(), "text", 1).Resizable = true;
			iconTreeView.AppendColumn("Comment", new CellRendererText(), "text", 2).Resizable = true;

			imageTreeView = new TreeView(imageListStore);
			imageTreeView.Selection.Mode = SelectionMode.Multiple;
			imageTreeView.AppendColumn("Image", new CellRendererPixbuf(), "pixbuf", 0).Resizable = true;  
			imageTreeView.AppendColumn("ResourceName", new CellRendererText(), "text", 1).Resizable = true;
			imageTreeView.AppendColumn("Size", new CellRendererText(), "text", 2).Resizable = true;
			imageTreeView.AppendColumn("Comment", new CellRendererText(), "text", 3).Resizable = true;

			otherTreeView = new TreeView(otherListStore);
			otherTreeView.Selection.Mode = SelectionMode.Multiple;
			otherTreeView.AppendColumn("ResourceName", new CellRendererText(), "text", 0).Resizable = true;  
			otherTreeView.AppendColumn("Type", new CellRendererText(), "text", 1).Resizable = true;
			otherTreeView.AppendColumn("Comment", new CellRendererText(), "text", 2).Resizable = true;

			stringTreeView = new TreeView(stringListStore);
			stringTreeView.Selection.Mode = SelectionMode.Multiple;
            stringTreeView.AppendColumn("ResourceName", new CellRendererText(), "text", 0).Resizable = true;
            stringTreeView.AppendColumn("Length", new CellRendererText(), "text", 1).Resizable = true;
            stringTreeView.AppendColumn("Value", new CellRendererText(), "text", 2).Resizable = true;
			stringTreeView.AppendColumn("Comment", new CellRendererText(), "text", 3).Resizable = true;

			// SetUp
			Build();
            scrolledWindow.Add(stringTreeView);
			verticalBox.Add(mainToolbar);
			Box.BoxChild boxChild = ((Box.BoxChild)verticalBox[mainToolbar]);
			boxChild.Position = 0;
			boxChild.Expand = false;
			typeMenu.ShowAll();
			ShowAll();
        }

        private void Clear()
        {
            resxBaseName = String.Empty;
            resxNodeList.Clear();
            audioListStore.Clear();
            fileListStore.Clear();
            iconListStore.Clear();
            imageListStore.Clear();
            otherListStore.Clear();
            stringListStore.Clear();
        }

        #region ResxInfo get/set

		public ResXDataNode[] GetResxInfo(string fileName)
		{
            return resxNodeList.Values.OrderBy(value => value.Name).ToArray();
		}

		public void SetResxInfo(string fileName)
		{
			Clear();

			using (ResXResourceReader reader = new ResXResourceReader(fileName))
			{
				reader.UseResXDataNodes = true;
				reader.BasePath = IOPath.GetDirectoryName(fileName);
				resxBaseName = reader.BasePath;

				try
				{
                    foreach (var value in reader
                        .OfType<DictionaryEntry>()
                        .Select(en => (ResXDataNode)en.Value)
                        .OrderBy(value => value.Name))
					{		
                        AddItem(value);
					}
				}
				catch (ArgumentException)
				{
				}
			}
		}

        #endregion

        #region AddItem

		private void AddItem(ResXDataNode node)
		{
			if (resxNodeList.ContainsKey(node.Name))
			{
				node.Name = node.Name + "_Copy";
				AddItem(node);
				return;
			}

			Type itemType;

			if (node.FileRef != null)
			{
				itemType = Type.GetType(node.FileRef.TypeName);
				resxNodeList.Add(node.Name, new ResXDataNode(node.Name, new ResXFileRef(node.FileRef.FileName.Replace(resxBaseName + "\\", ""), node.FileRef.TypeName)));

				if (itemType != typeof(System.Drawing.Icon) && itemType != typeof(System.Drawing.Bitmap) && itemType != typeof(MemoryStream))
				{
					FileInfo fileInfo = new FileInfo(node.FileRef.FileName);
					fileListStore.AppendValues(node.Name, fileInfo.Length + " KB", fileInfo.FullName, node.Comment);
					return;
				}
			}
			else
			{
				resxNodeList.Add(node.Name, node);
			}

			object item = node.GetValue(assemblyNames);
			itemType = item.GetType();

			if (itemType == typeof(string))
			{
				stringListStore.AppendValues(node.Name, ((string)item).Length.ToString(), item, node.Comment);
				return;
			}

			if (itemType == typeof(MemoryStream))
			{
				MemoryStream stream = (MemoryStream)item;
				audioListStore.AppendValues(node.Name, Math.Round(stream.Length / 1024f, 1) + " KB", node.Comment);
				stream.Dispose();
				return;
			}

			if (itemType == typeof(System.Drawing.Icon))
			{
				Pixbuf buff = ImageToPixbuf(((System.Drawing.Icon)item).ToBitmap());
				iconListStore.AppendValues(buff, node.Name, node.Comment);
				return;
			}

			if (itemType == typeof(System.Drawing.Bitmap))
			{
				Pixbuf buff = ImageToPixbuf((System.Drawing.Bitmap)item);
				double magnitude = Math.Sqrt(buff.Width * buff.Width + buff.Height * buff.Height);
				imageListStore.AppendValues(buff.ScaleSimple((int)(buff.Width / magnitude * 150), (int)(buff.Height / magnitude * 150), InterpType.Tiles), node.Name, (buff.Height + "x" + buff.Width), node.Comment);
				return;
			}

			otherListStore.AppendValues(node.Name, itemType.Name, node.Comment);
		}

		void AddFileAsItem(string filePath)
		{
			string resourceType = FileExtension.FileExtensionToType(IOPath.GetExtension(filePath)).AssemblyQualifiedName;
			string resourceName = IOPath.GetFileNameWithoutExtension(filePath);
			AddItem(new ResXDataNode(resourceName, new ResXFileRef(filePath, resourceType)));
		}

        #endregion

        #region Event Handlers

		void DropDownEventHandler(object sender, EventArgs e)
		{
            editButton.Hide();
			if (sender == typeMenuAudioItem)
			{
				SwapTreeView(audioTreeView, sender);
				return;
			}

			if (sender == typeMenuFileItem)
			{
				SwapTreeView(fileTreeView, sender);
				return;
			}

			if (sender == typeMenuIconItem)
			{
				SwapTreeView(iconTreeView, sender);
				return;
			}

			if (sender == typeMenuImageItem)
			{
				SwapTreeView(imageTreeView, sender);
				return;
			}

			if (sender == typeMenuOtherItem)
			{
				SwapTreeView(otherTreeView, sender);
				return;
			}

			if (sender == typeMenuStringItem)
            {
                editButton.Show();
				SwapTreeView(stringTreeView, sender);
				return;
			}
		}

		void AddButtonEventHandler(object sender, EventArgs e)
		{
            if (scrolledWindow.Children[0] == stringTreeView)
            {
                ResponseType response = (ResponseType)stringDialog.Run();
                stringDialog.Hide();

                if (response == ResponseType.Ok && !string.IsNullOrWhiteSpace(stringDialog.NodeName))
                {
                    var note = new ResXDataNode(stringDialog.NodeName, stringDialog.NodeText) {
                        Comment = stringDialog.NodeComment
                    };
                    this.AddItem(note);
                    editorView.IsDirty = true;
                }

                stringDialog.NodeText = "";
                stringDialog.NodeName = "";
                stringDialog.NodeComment = "";
				
                return;
            }
            else
            {
                //add existing item
                if (openDialog.Run())
                {
                    AddFileAsItem(openDialog.SelectedFile);
                    return;
                }
            }
		}

        void EditButtonEventHandler(object sender, EventArgs e)
        {
            if (scrolledWindow.Children[0] == stringTreeView)
            {
                object[] keys = PeekSelectedRows((TreeView)scrolledWindow.Children[0]);
                if (keys.Length != 1)
                {
                    Ide.MessageService.ShowMessage("Choose only one row");
                    return;
                }

                var row = resxNodeList.Values.ToList().FirstOrDefault(value => value.Name == (string)keys[0]);
                stringDialog.NodeName = row.Name;
                stringDialog.NodeText = row.GetValue(assemblyNames).ToString();
                stringDialog.NodeComment = row.Comment;

                ResponseType response = (ResponseType)stringDialog.Run();
                stringDialog.Hide();

                if (response == ResponseType.Ok && !string.IsNullOrWhiteSpace(stringDialog.NodeName))
                {
                    RemoveButtonEventHandler(sender, e);
                    var note = new ResXDataNode(stringDialog.NodeName, stringDialog.NodeText) {
                        Comment = stringDialog.NodeComment
                    };
                    AddItem(note);
                    editorView.IsDirty = true;
                }

                stringDialog.NodeText = "";
                stringDialog.NodeName = "";
                stringDialog.NodeComment = "";

                return;
            }
        }

		void RemoveButtonEventHandler(object sender, EventArgs e)
		{
			object[] keys = DequeueSelectedRows((TreeView)scrolledWindow.Children[0]);

            if (keys.Length == 0)
                return;

			foreach (string key in keys)
			{
				resxNodeList.Remove(key);
			}

			editorView.IsDirty = true;
		}

        #endregion

        #region TreeView methods

		private void SwapTreeView(TreeView treeView, object sender)
		{
			Widget Child = scrolledWindow.Children[0];
			Child.HideAll();

			scrolledWindow.Remove(Child);
			scrolledWindow.Add(treeView);

			treeView.ShowAll();

			typeToolButton.Label = ((AccelLabel)((MenuItem)sender).Children[0]).Text;
		}

		private object[] DequeueSelectedRows(TreeView treeView)
		{
			ListStore listStore = (ListStore)treeView.Model;
			TreePath[] selectedRows = treeView.Selection.GetSelectedRows();
			TreeIter[] treeIterators = new TreeIter[selectedRows.Length];
			object[] reternValues = new object[selectedRows.Length];

			for (int index = 0; index < selectedRows.Length; index++)
			{
				listStore.GetIter(out treeIterators[index], selectedRows[index]);

				int i = 0;

				do
				{
					reternValues[index] = listStore.GetValue(treeIterators[index], i++);
				} while (reternValues[index].GetType() != typeof(string));
			}

			for (int index = 0; index < treeIterators.Length; index++)
			{
				listStore.Remove(ref treeIterators[index]);
			}

			return reternValues;
		}

        private object[] PeekSelectedRows(TreeView treeView)
        {
            ListStore listStore = (ListStore)treeView.Model;
            TreePath[] selectedRows = treeView.Selection.GetSelectedRows();
            TreeIter[] treeIterators = new TreeIter[selectedRows.Length];
            object[] reternValues = new object[selectedRows.Length];

            for (int index = 0; index < selectedRows.Length; index++)
            {
                listStore.GetIter(out treeIterators[index], selectedRows[index]);

                int i = 0;

                do
                {
                    reternValues[index] = listStore.GetValue(treeIterators[index], i++);
                } while (reternValues[index].GetType() != typeof(string));
            }

            return reternValues;
        }

        #endregion

        #region Helpers

		private static Pixbuf ImageToPixbuf(System.Drawing.Image image)
		{
			MemoryStream rawImage = new MemoryStream();
			image.Save(rawImage, System.Drawing.Imaging.ImageFormat.Png);
			rawImage.Position = 0;
			Pixbuf pixbufBuffer = new Pixbuf(rawImage);
			rawImage.Dispose();

			return pixbufBuffer;
		}

        #endregion
	}
}