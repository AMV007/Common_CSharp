using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace CommonControls.CommonForms
{
    public partial class FileOpenControl : UserControl
    {
        public enum e_CntrlType
        {
            FolderBrowsing,
            FileBrowsing
        }

        private e_CntrlType cntrlType = e_CntrlType.FolderBrowsing;
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

        [Category("Cntrl type"),
        Description("Control Type")]
        public e_CntrlType ControlType
        {
            set
            {
                cntrlType = value;
            }
            get
            {
                return cntrlType;
            }
        }

        public event EventHandler OnFileDirOpen = null;
        private void onFileDirOpen(string Path)
        {
            if (OnFileDirOpen != null)
            {
                OnFileDirOpen.Invoke(Path, EventArgs.Empty);
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

        public FileOpenControl()
        {
            InitializeComponent();
        }

        void CalcVarControlPos()
        {
            textBoxPath.Location = new Point(labelMain.Bounds.Right, textBoxPath.Location.Y);
            textBoxPath.Width = this.Width - textBoxPath.Location.X - 4 - buttonOpen.Width;            
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            if (cntrlType == e_CntrlType.FileBrowsing)
            {
                FolderBrowserDialog FolderBrowserDialogthis = new FolderBrowserDialog();
                if (textBoxPath.Text != "") FolderBrowserDialogthis.SelectedPath = textBoxPath.Text;

                if (FolderBrowserDialogthis.ShowDialog() == DialogResult.OK)
                {
                    textBoxPath.Text = FolderBrowserDialogthis.SelectedPath;
                    onFileDirOpen(FolderBrowserDialogthis.SelectedPath);
                }
            }
            else
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.Filter = fileFilter;
                openFileDialog1.FilterIndex = 1;
                openFileDialog1.CheckFileExists = true;
                openFileDialog1.CheckPathExists = true;
                openFileDialog1.RestoreDirectory = true;
                if (textBoxPath.Text != "") openFileDialog1.InitialDirectory = textBoxPath.Text;

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    if (System.IO.File.Exists(openFileDialog1.FileName))
                    {
                        textBoxPath.Text = openFileDialog1.FileName;
                        onFileDirOpen(openFileDialog1.FileName);                        
                    }
                };
            }
        }

        private void textBoxPath_TextChanged(object sender, EventArgs e)
        {
            onPathChanged(textBoxPath.Text);
        }
    }
}
