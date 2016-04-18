namespace ServerForm {
    partial class MainForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.label5 = new System.Windows.Forms.Label();
            this.serverDebugListBox = new System.Windows.Forms.ListBox();
            this.serverStartBTN = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.label5.Location = new System.Drawing.Point(29, 14);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(205, 25);
            this.label5.TabIndex = 32;
            this.label5.Text = "Server Information";
            // 
            // serverDebugListBox
            // 
            this.serverDebugListBox.FormattingEnabled = true;
            this.serverDebugListBox.HorizontalScrollbar = true;
            this.serverDebugListBox.Location = new System.Drawing.Point(34, 42);
            this.serverDebugListBox.Name = "serverDebugListBox";
            this.serverDebugListBox.Size = new System.Drawing.Size(568, 407);
            this.serverDebugListBox.TabIndex = 31;
            // 
            // serverStartBTN
            // 
            this.serverStartBTN.Location = new System.Drawing.Point(34, 463);
            this.serverStartBTN.Name = "serverStartBTN";
            this.serverStartBTN.Size = new System.Drawing.Size(116, 23);
            this.serverStartBTN.TabIndex = 30;
            this.serverStartBTN.Text = "Start Server Test";
            this.serverStartBTN.UseVisualStyleBackColor = true;
            this.serverStartBTN.Click += new System.EventHandler(this.serverStartBTN_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 498);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.serverDebugListBox);
            this.Controls.Add(this.serverStartBTN);
            this.Name = "MainForm";
            this.Text = "ServerForm";
            this.Load += new System.EventHandler(this.ServerForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ListBox serverDebugListBox;
        private System.Windows.Forms.Button serverStartBTN;
    }
}

