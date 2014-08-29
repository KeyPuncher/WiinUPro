namespace WiinUSoft
{
    partial class DeviceControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DeviceControl));
            this.cIcon = new System.Windows.Forms.PictureBox();
            this.cPlayerLabel = new System.Windows.Forms.Label();
            this.cPlayerNum = new System.Windows.Forms.Label();
            this.cUpButton = new System.Windows.Forms.Button();
            this.cDownButton = new System.Windows.Forms.Button();
            this.cConfigButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.cIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // cIcon
            // 
            this.cIcon.Image = ((System.Drawing.Image)(resources.GetObject("cIcon.Image")));
            this.cIcon.Location = new System.Drawing.Point(3, 2);
            this.cIcon.Name = "cIcon";
            this.cIcon.Size = new System.Drawing.Size(52, 47);
            this.cIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.cIcon.TabIndex = 0;
            this.cIcon.TabStop = false;
            // 
            // cPlayerLabel
            // 
            this.cPlayerLabel.AutoSize = true;
            this.cPlayerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cPlayerLabel.Location = new System.Drawing.Point(61, 3);
            this.cPlayerLabel.Name = "cPlayerLabel";
            this.cPlayerLabel.Size = new System.Drawing.Size(107, 20);
            this.cPlayerLabel.TabIndex = 5;
            this.cPlayerLabel.Text = "Disconnected";
            // 
            // cPlayerNum
            // 
            this.cPlayerNum.AutoSize = true;
            this.cPlayerNum.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cPlayerNum.Location = new System.Drawing.Point(117, 3);
            this.cPlayerNum.Name = "cPlayerNum";
            this.cPlayerNum.Size = new System.Drawing.Size(0, 20);
            this.cPlayerNum.TabIndex = 6;
            // 
            // cUpButton
            // 
            this.cUpButton.Enabled = false;
            this.cUpButton.Image = ((System.Drawing.Image)(resources.GetObject("cUpButton.Image")));
            this.cUpButton.Location = new System.Drawing.Point(61, 26);
            this.cUpButton.Name = "cUpButton";
            this.cUpButton.Size = new System.Drawing.Size(48, 23);
            this.cUpButton.TabIndex = 7;
            this.cUpButton.UseVisualStyleBackColor = true;
            this.cUpButton.Click += new System.EventHandler(this.cUpButton_Click);
            // 
            // cDownButton
            // 
            this.cDownButton.Enabled = false;
            this.cDownButton.Image = ((System.Drawing.Image)(resources.GetObject("cDownButton.Image")));
            this.cDownButton.Location = new System.Drawing.Point(115, 26);
            this.cDownButton.Name = "cDownButton";
            this.cDownButton.Size = new System.Drawing.Size(48, 23);
            this.cDownButton.TabIndex = 8;
            this.cDownButton.UseVisualStyleBackColor = true;
            this.cDownButton.Click += new System.EventHandler(this.cDownButton_Click);
            // 
            // cConfigButton
            // 
            this.cConfigButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cConfigButton.Location = new System.Drawing.Point(169, 3);
            this.cConfigButton.Name = "cConfigButton";
            this.cConfigButton.Size = new System.Drawing.Size(88, 46);
            this.cConfigButton.TabIndex = 9;
            this.cConfigButton.Text = "Configure";
            this.cConfigButton.UseVisualStyleBackColor = true;
            this.cConfigButton.Click += new System.EventHandler(this.cConfigButton_Click);
            // 
            // DeviceControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.Controls.Add(this.cConfigButton);
            this.Controls.Add(this.cDownButton);
            this.Controls.Add(this.cUpButton);
            this.Controls.Add(this.cPlayerNum);
            this.Controls.Add(this.cPlayerLabel);
            this.Controls.Add(this.cIcon);
            this.Name = "DeviceControl";
            this.Size = new System.Drawing.Size(259, 52);
            ((System.ComponentModel.ISupportInitialize)(this.cIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label cPlayerLabel;
        public System.Windows.Forms.PictureBox cIcon;
        public System.Windows.Forms.Label cPlayerNum;
        public System.Windows.Forms.Button cUpButton;
        public System.Windows.Forms.Button cDownButton;
        public System.Windows.Forms.Button cConfigButton;
    }
}
