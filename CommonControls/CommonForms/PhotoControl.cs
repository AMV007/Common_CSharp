using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace CommonControls.CommonForms
{
    public partial class PhotoControl : UserControl
    {   

        public event EventHandler EventDeleteImage;
        public event EventHandler EventAddImage;
        public event EventHandler EventGetImage; 

        public PhotoControl()
        {
            InitializeComponent();                   
        }        

        public Image createImage(byte[] image_data)
        {
            return Image.FromStream(new System.IO.MemoryStream(image_data));
        }

        public void ClearList()
        {
            listViewImages.Items.Clear();
            listViewImages.LargeImageList.Images.Clear();
        }

        public void AddNewPhoto(Image NewImage, string ImageIDNew)
        {
            listViewImages.LargeImageList.Images.Add(ImageIDNew, NewImage);
            listViewImages.Items.Add(ImageIDNew, ImageIDNew, ImageIDNew);            
        }

        public void AddNewPhoto(byte[] NewImage, string ImageIDNew)
        {
            listViewImages.LargeImageList.Images.Add(ImageIDNew, createImage(NewImage));            
            listViewImages.Items.Add("", ImageIDNew);
        }

        public void AddNewPhoto(byte[] NewImage, string ImageIDNew, string PhotoName)
        {
            listViewImages.LargeImageList.Images.Add(ImageIDNew, createImage(NewImage));
            listViewImages.Items.Add(ImageIDNew, PhotoName, ImageIDNew);
            listViewImages.Items[listViewImages.Items.Count-1].Text = PhotoName;            
        }

        void DeleteImage(int DelNum)
        {
            string ImageIDLast = listViewImages.LargeImageList.Images.Keys[DelNum];            
            listViewImages.LargeImageList.Images.RemoveAt(DelNum);
            listViewImages.Items.RemoveAt(DelNum);            
            if (EventDeleteImage.Target != null)
            {
                EventDeleteImage.Invoke(ImageIDLast, EventArgs.Empty);
            }            
        }

        void DeleteImage(string ImageKey)
        {
            listViewImages.LargeImageList.Images.RemoveByKey(ImageKey);
            listViewImages.Items.RemoveByKey(ImageKey);                 
            if (EventDeleteImage.Target != null)
            {
                EventDeleteImage.Invoke(ImageKey, EventArgs.Empty);
            }            
        }

        private void óäàëèòüToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteImage(listViewImages.SelectedItems[0].ImageKey);
            listViewImages.Items.Remove(listViewImages.SelectedItems[0]);
        }

        private void äîáàâèòüToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();            
            openFileDialog1.Filter = "JPG files (*.jpg)|*.jpg| All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                System.IO.Stream FileOpened;
                if ((FileOpened = openFileDialog1.OpenFile()) != null)
                {
                    byte[] FileData = new byte[FileOpened.Length];
                    FileOpened.Read(FileData, 0, FileData.Length);
                    FileOpened.Close();
                    
                    if (EventAddImage.Target != null)
                    {
                        EventAddImage.Invoke(FileData, null);
                    }                    
                };
            };            
        }

        private void ñîõğàíèòüToolStripMenuItem_Click(object sender, EventArgs e)
        {   
            SaveFileDialog SaveFileDialog1 = new SaveFileDialog();            
            SaveFileDialog1.Filter = "JPG files (*.jpg)|*.jpg| All files (*.*)|*.*";
            SaveFileDialog1.FilterIndex = 1;
            SaveFileDialog1.RestoreDirectory = true;
            if (SaveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                System.IO.Stream FileOpened;
                if ((FileOpened = SaveFileDialog1.OpenFile()) != null)
                {
                    FileOpened.Close();                    
                    if (EventGetImage.Target != null)
                    {                        
                        EventGetImage.Invoke(new string[] {listViewImages.SelectedItems[0].ImageKey, SaveFileDialog1.FileName }, EventArgs.Empty);
                    }
                    else
                    {
                        listViewImages.LargeImageList.Images[listViewImages.LargeImageList.Images.IndexOfKey(listViewImages.SelectedItems[0].ImageKey)].Save(SaveFileDialog1.FileName);
                    }
                }
            }
        }

        public void SaveImage(byte[] Data, string FilePath)
        {
            createImage(Data).Save(FilePath);
        }

        private void listViewImages_SizeChanged(object sender, EventArgs e)
        {
            if (this.Width != 0 && this.Height != 0)
            {
                int SizeMin = Math.Min(this.Width, this.Height);
                int NewValueSize = Math.Min(256, SizeMin);
                listViewImages.TileSize = new Size(NewValueSize, NewValueSize);

                if (listViewImages.LargeImageList == null)
                {
                    listViewImages.LargeImageList = new ImageList();
                    listViewImages.LargeImageList.ColorDepth = ColorDepth.Depth16Bit;
                    listViewImages.LargeImageList.ImageSize = new Size(NewValueSize, NewValueSize);
                }
            }
        }

        private void listViewImages_Click(object sender, EventArgs e)
        {
            if (listViewImages.Items.Count > 0)
            {
                óäàëèòüToolStripMenuItem.Enabled = true;
                ñîõğàíèòüToolStripMenuItem.Enabled = true;
            }
            else
            {
                óäàëèòüToolStripMenuItem.Enabled = false;
                ñîõğàíèòüToolStripMenuItem.Enabled = false;
            }

            if (EventAddImage != null) äîáàâèòüToolStripMenuItem.Enabled = true;
            else äîáàâèòüToolStripMenuItem.Enabled = false;

            if (EventDeleteImage != null) óäàëèòüToolStripMenuItem.Enabled = true;
            else óäàëèòüToolStripMenuItem.Enabled = false;

            if (EventGetImage != null) ñîõğàíèòüToolStripMenuItem.Enabled = true;
            else ñîõğàíèòüToolStripMenuItem.Enabled = false;
        }
    }
}
