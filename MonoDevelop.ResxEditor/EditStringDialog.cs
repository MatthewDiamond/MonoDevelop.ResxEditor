using System;

namespace MonoDevelop.ResxEditor
{
	public partial class EditStringDialog : Gtk.Dialog
    {
        public string NodeName
        {
            get { return nameField.Buffer.Text; }
            set { nameField.Buffer.Text = value; }
        }

		public string NodeText
		{
			get { return textField.Buffer.Text; }
			set { textField.Buffer.Text = value; }
		}

        public string NodeComment
        {
            get { return commentField.Buffer.Text; }
            set { commentField.Buffer.Text = value; }
        }

		public EditStringDialog()
		{
            this.Build();
		}
	}
}

