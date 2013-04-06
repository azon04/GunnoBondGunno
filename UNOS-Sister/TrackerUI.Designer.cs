namespace UNOS_Sister
{
    partial class TrackerUI
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
            this.label1 = new System.Windows.Forms.Label();
            this.IPText = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.MaxPeerNumber = new System.Windows.Forms.NumericUpDown();
            this.MaxRoomNumber = new System.Windows.Forms.NumericUpDown();
            this.LogCheck = new System.Windows.Forms.CheckBox();
            this.LogText = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.MaxPeerNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MaxRoomNumber)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "IP";
            // 
            // IPText
            // 
            this.IPText.Location = new System.Drawing.Point(50, 13);
            this.IPText.Name = "IPText";
            this.IPText.ReadOnly = true;
            this.IPText.Size = new System.Drawing.Size(143, 20);
            this.IPText.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(108, 134);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Shutdown";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 134);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "Start";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Max Peer";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Max Room";
            // 
            // MaxPeerNumber
            // 
            this.MaxPeerNumber.Location = new System.Drawing.Point(77, 44);
            this.MaxPeerNumber.Name = "MaxPeerNumber";
            this.MaxPeerNumber.Size = new System.Drawing.Size(83, 20);
            this.MaxPeerNumber.TabIndex = 6;
            this.MaxPeerNumber.ValueChanged += new System.EventHandler(this.MaxPeerNumber_ValueChanged);
            // 
            // MaxRoomNumber
            // 
            this.MaxRoomNumber.Location = new System.Drawing.Point(77, 72);
            this.MaxRoomNumber.Name = "MaxRoomNumber";
            this.MaxRoomNumber.Size = new System.Drawing.Size(83, 20);
            this.MaxRoomNumber.TabIndex = 7;
            this.MaxRoomNumber.ValueChanged += new System.EventHandler(this.MaxRoomNumber_ValueChanged);
            // 
            // LogCheck
            // 
            this.LogCheck.AutoSize = true;
            this.LogCheck.Location = new System.Drawing.Point(77, 99);
            this.LogCheck.Name = "LogCheck";
            this.LogCheck.Size = new System.Drawing.Size(44, 17);
            this.LogCheck.TabIndex = 8;
            this.LogCheck.Text = "Log";
            this.LogCheck.UseVisualStyleBackColor = true;
            this.LogCheck.CheckedChanged += new System.EventHandler(this.LogCheck_CheckedChanged);
            // 
            // LogText
            // 
            this.LogText.Location = new System.Drawing.Point(12, 163);
            this.LogText.Name = "LogText";
            this.LogText.Size = new System.Drawing.Size(442, 138);
            this.LogText.TabIndex = 10;
            this.LogText.Text = "";
            // 
            // TrackerUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(466, 313);
            this.Controls.Add(this.LogText);
            this.Controls.Add(this.LogCheck);
            this.Controls.Add(this.MaxRoomNumber);
            this.Controls.Add(this.MaxPeerNumber);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.IPText);
            this.Controls.Add(this.label1);
            this.Name = "TrackerUI";
            this.Text = "TrackerUI";
            ((System.ComponentModel.ISupportInitialize)(this.MaxPeerNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MaxRoomNumber)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox IPText;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown MaxPeerNumber;
        private System.Windows.Forms.NumericUpDown MaxRoomNumber;
        private System.Windows.Forms.CheckBox LogCheck;
        public System.Windows.Forms.RichTextBox LogText;
    }
}