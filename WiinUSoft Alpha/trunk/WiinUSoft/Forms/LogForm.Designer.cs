namespace WiinUSoft
{
    partial class LogForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogForm));
            this.logBox = new System.Windows.Forms.RichTextBox();
            this.buttonListGames = new System.Windows.Forms.Button();
            this.buttonAddProcess = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // logBox
            // 
            this.logBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logBox.Location = new System.Drawing.Point(0, 0);
            this.logBox.Name = "logBox";
            this.logBox.ReadOnly = true;
            this.logBox.Size = new System.Drawing.Size(540, 197);
            this.logBox.TabIndex = 0;
            this.logBox.Text = "";
            // 
            // buttonListGames
            // 
            this.buttonListGames.Location = new System.Drawing.Point(3, 201);
            this.buttonListGames.Name = "buttonListGames";
            this.buttonListGames.Size = new System.Drawing.Size(108, 23);
            this.buttonListGames.TabIndex = 1;
            this.buttonListGames.Text = "List Test Processes";
            this.buttonListGames.UseVisualStyleBackColor = true;
            this.buttonListGames.Click += new System.EventHandler(this.buttonListGames_Click);
            // 
            // buttonAddProcess
            // 
            this.buttonAddProcess.Location = new System.Drawing.Point(117, 201);
            this.buttonAddProcess.Name = "buttonAddProcess";
            this.buttonAddProcess.Size = new System.Drawing.Size(111, 23);
            this.buttonAddProcess.TabIndex = 2;
            this.buttonAddProcess.Text = "Add Process to List";
            this.buttonAddProcess.UseVisualStyleBackColor = true;
            this.buttonAddProcess.Click += new System.EventHandler(this.buttonAddProcess_Click);
            // 
            // LogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 228);
            this.Controls.Add(this.buttonAddProcess);
            this.Controls.Add(this.buttonListGames);
            this.Controls.Add(this.logBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "LogForm";
            this.Text = "Log";
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.RichTextBox logBox;
        private System.Windows.Forms.Button buttonListGames;
        private System.Windows.Forms.Button buttonAddProcess;


    }
}