namespace CommonControls.Display
{
    partial class SimplePlot
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panelPlot = new System.Windows.Forms.Panel();
            this.contextMenuStripPlot = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.SaveToFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panelScale = new System.Windows.Forms.Panel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.DataToReporttxtToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripPlot.SuspendLayout();
            this.panelScale.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelPlot
            // 
            this.panelPlot.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panelPlot.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelPlot.ContextMenuStrip = this.contextMenuStripPlot;
            this.panelPlot.Location = new System.Drawing.Point(80, 0);
            this.panelPlot.Name = "panelPlot";
            this.panelPlot.Size = new System.Drawing.Size(681, 310);
            this.panelPlot.TabIndex = 0;
            this.panelPlot.MouseLeave += new System.EventHandler(this.panelPlot_MouseLeave);
            this.panelPlot.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelPlot_MouseMove);
            this.panelPlot.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelPlot_MouseDown);
            this.panelPlot.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelPlot_MouseUp);
            this.panelPlot.MouseEnter += new System.EventHandler(this.panelPlot_MouseEnter);
            // 
            // contextMenuStripPlot
            // 
            this.contextMenuStripPlot.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SaveToFileToolStripMenuItem});
            this.contextMenuStripPlot.Name = "contextMenuStrip1";
            this.contextMenuStripPlot.Size = new System.Drawing.Size(219, 48);
            // 
            // SaveToFileToolStripMenuItem
            // 
            this.SaveToFileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ImageToolStripMenuItem,
            this.DataToolStripMenuItem,
            this.DataToReporttxtToolStripMenuItem});
            this.SaveToFileToolStripMenuItem.Name = "SaveToFileToolStripMenuItem";
            this.SaveToFileToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.SaveToFileToolStripMenuItem.Text = "Сохранить в файл";
            // 
            // ImageToolStripMenuItem
            // 
            this.ImageToolStripMenuItem.Name = "ImageToolStripMenuItem";
            this.ImageToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.ImageToolStripMenuItem.Text = "Рисунок";
            this.ImageToolStripMenuItem.Click += new System.EventHandler(this.ImageToolStripMenuItem_Click);
            // 
            // DataToolStripMenuItem
            // 
            this.DataToolStripMenuItem.Name = "DataToolStripMenuItem";
            this.DataToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.DataToolStripMenuItem.Text = "Данные";
            this.DataToolStripMenuItem.Click += new System.EventHandler(this.DataToolStripMenuItem_Click);
            // 
            // panelScale
            // 
            this.panelScale.Controls.Add(this.panelPlot);
            this.panelScale.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelScale.Location = new System.Drawing.Point(0, 0);
            this.panelScale.Name = "panelScale";
            this.panelScale.Size = new System.Drawing.Size(761, 341);
            this.panelScale.TabIndex = 1;
            this.panelScale.SizeChanged += new System.EventHandler(this.panelScale_SizeChanged);
            // 
            // DataToReporttxtToolStripMenuItem
            // 
            this.DataToReporttxtToolStripMenuItem.Name = "DataToReporttxtToolStripMenuItem";
            this.DataToReporttxtToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.DataToReporttxtToolStripMenuItem.Text = "Данные в report.txt";
            this.DataToReporttxtToolStripMenuItem.Click += new System.EventHandler(this.DataToReporttxtToolStripMenuItem_Click);
            // 
            // SimplePlot
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(761, 341);
            this.Controls.Add(this.panelScale);
            this.Name = "SimplePlot";
            this.Text = "SimplePlot";
            this.contextMenuStripPlot.ResumeLayout(false);
            this.panelScale.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelPlot;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripPlot;
        private System.Windows.Forms.ToolStripMenuItem SaveToFileToolStripMenuItem;
        private System.Windows.Forms.Panel panelScale;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStripMenuItem ImageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem DataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem DataToReporttxtToolStripMenuItem;
    }
}