using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace CommonControls.CommonForms
{
    public partial class FileSaveControl : UserControl
    {
        private string fileFilter = "All files (*.*)|*.*";

        [Category("Cntrl param"),
        Description("Label Text")]
        public string LabelText
        {
            set
            {
                labelMain.Text = value;
                CalcVarControlPos();
            }
            get
            {
                return labelMain.Text;
            }
        }

        [Category("Cntrl param"),
        Description("Path Text")]
        public string PathValue
        {
            set
            {
                textBoxPath.Text = value;
                CalcVarControlPos();
            }
            get
            {
                return textBoxPath.Text;
            }
        }

        [Category("Cntrl param"),
        Description("EditBox Enabled")]
        public bool EnableEditBox
        {
            set
            {
                textBoxPath.Enabled = value;
            }
            get
            {
                return textBoxPath.Enabled;
            }
        }

        [Category("Cntrl param"),
        Description("File Filter")]
        public string FileFilter
        {
            set
            {
                fileFilter = value;
            }
            get
            {
                return fileFilter;
            }
        }       

        public event EventHandler OnFileDirSave = null;
        private void onFileDirSave(string Path)
        {
            if (OnFileDirSave != null)
            {
                OnFileDirSave.Invoke(Path, EventArgs.Empty);
            }
        }

        public event EventHandler OnPathChanged = null;
        private void onPathChanged(string Path)
        {
            if (OnPathChanged != null)
            {
                OnPathChanged.Invoke(Path, EventArgs.Empty);
            }
        }

        public FileSaveControl()
        {
            InitializeComponent();
        }

        void CalcVarControlPos()
        {
            textBoxPath.Location = new Point(labelMain.Bounds.Right, textBoxPath.Location.Y);
            textBoxPath.Width = this.Width - textBoxPath.Location.X - 4 - buttonSave.Width;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = fileFilter;
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.CheckFileExists = true;
            saveFileDialog1.CheckPathExists = true;
            saveFileDialog1.RestoreDirectory = true;
            if (textBoxPath.Text != "") saveFileDialog1.InitialDirectory = textBoxPath.Text;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (System.IO.File.Exists(saveFileDialog1.FileName))
                {
                    textBoxPath.Text = saveFileDialog1.FileName;
                    onFileDirSave(saveFileDialog1.FileName);
                }
            };
        }

        private void textBoxPath_TextChanged(object sender, EventArgs e)
        {
            onPathChanged(textBoxPath.Text);
        }      
    }
}
