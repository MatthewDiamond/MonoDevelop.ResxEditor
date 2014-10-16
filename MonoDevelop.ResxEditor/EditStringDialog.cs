using System;

namespace MonoDevelop.ResxEditor
{
	public partial class EditStringDialog : Gtk.Dialog
	{
		public string NodeText
		{
			get { return textFeild.Buffer.Text; }
			set { textFeild.Buffer.Text = value; }
		}

		public string NodeName
		{
			get { return nameFeild.Buffer.Text; }
			set { nameFeild.Buffer.Text = value; }
		}

		public EditStringDialog()
		{
			this.Build();
		}
	}
}

