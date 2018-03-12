namespace CommonControls
{
    partial class CircularIndicator
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
            this.labelIndicator = new System.Windows.Forms.Label();
            this.panelIndicator = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // labelIndicator
            // 
            this.labelIndicator.AutoSize = true;
            this.labelIndicator.Location = new System.Drawing.Point(25, 0);
            this.labelIndicator.Name = "labelIndicator";
            this.labelIndicator.Size = new System.Drawing.Size(0, 17);
            this.labelIndicator.TabIndex = 0;
            this.labelIndicator.SizeChanged += new System.EventHandler(this.labelIndicator_SizeChanged);
            // 
            // panelIndicator
            // 
            this.panelIndicator.Location = new System.Drawing.Point(0, 0);
            this.panelIndicator.Name = "panelIndicator";
            this.panelIndicator.Size = new System.Drawing.Size(20, 20);
            this.panelIndicator.TabIndex = 1;
            this.panelIndicator.Paint += new System.Windows.Forms.PaintEventHandler(this.panelIndicator_Paint);
            // 
            // CircularIndicator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelIndicator);
            this.Controls.Add(this.labelIndicator);
            this.Name = "CircularIndicator";
            this.Size = new System.Drawing.Size(31, 20);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelIndicator;
        private System.Windows.Forms.Panel panelIndicator;
    }
}
