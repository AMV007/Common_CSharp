namespace CommonControls.CommonForms
{
    partial class PhotoControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.listViewImages = new System.Windows.Forms.ListView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.äîáàâèòüToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.óäàëèòüToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ñîõğàíèòüToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listViewImages
            // 
            this.listViewImages.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.listViewImages.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listViewImages.ContextMenuStrip = this.contextMenuStrip1;
            this.listViewImages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewImages.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.listViewImages.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewImages.HotTracking = true;
            this.listViewImages.HoverSelection = true;
            this.listViewImages.Location = new System.Drawing.Point(0, 0);
            this.listViewImages.MultiSelect = false;
            this.listViewImages.Name = "listViewImages";
            this.listViewImages.ShowItemToolTips = true;
            this.listViewImages.Size = new System.Drawing.Size(429, 241);
            this.listViewImages.TabIndex = 1;
            this.listViewImages.TileSize = new System.Drawing.Size(1, 1);
            this.listViewImages.UseCompatibleStateImageBehavior = false;
            this.listViewImages.View = System.Windows.Forms.View.Tile;
            this.listViewImages.SizeChanged += new System.EventHandler(this.listViewImages_SizeChanged);
            this.listViewImages.Click += new System.EventHandler(this.listViewImages_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.äîáàâèòüToolStripMenuItem,
            this.óäàëèòüToolStripMenuItem,
            this.ñîõğàíèòüToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(165, 70);
            // 
            // äîáàâèòüToolStripMenuItem
            // 
            this.äîáàâèòüToolStripMenuItem.Name = "äîáàâèòüToolStripMenuItem";
            this.äîáàâèòüToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.äîáàâèòüToolStripMenuItem.Text = "Äîáàâèòü";
            this.äîáàâèòüToolStripMenuItem.Click += new System.EventHandler(this.äîáàâèòüToolStripMenuItem_Click);
            // 
            // óäàëèòüToolStripMenuItem
            // 
            this.óäàëèòüToolStripMenuItem.Enabled = false;
            this.óäàëèòüToolStripMenuItem.Name = "óäàëèòüToolStripMenuItem";
            this.óäàëèòüToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.óäàëèòüToolStripMenuItem.Text = "Óäàëèòü";
            this.óäàëèòüToolStripMenuItem.Click += new System.EventHandler(this.óäàëèòüToolStripMenuItem_Click);
            // 
            // ñîõğàíèòüToolStripMenuItem
            // 
            this.ñîõğàíèòüToolStripMenuItem.Enabled = false;
            this.ñîõğàíèòüToolStripMenuItem.Name = "ñîõğàíèòüToolStripMenuItem";
            this.ñîõğàíèòüToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.ñîõğàíèòüToolStripMenuItem.Text = "Ñîõğàíèòü";
            this.ñîõğàíèòüToolStripMenuItem.Click += new System.EventHandler(this.ñîõğàíèòüToolStripMenuItem_Click);
            // 
            // PhotoControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.listViewImages);
            this.Name = "PhotoControl";
            this.Size = new System.Drawing.Size(429, 241);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listViewImages;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem óäàëèòüToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem äîáàâèòüToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ñîõğàíèòüToolStripMenuItem;

    }
}
