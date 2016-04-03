namespace TCPTest {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.serverStartBTN = new System.Windows.Forms.Button();
            this.clientStartBTN = new System.Windows.Forms.Button();
            this.serverDebugListBox = new System.Windows.Forms.ListBox();
            this.clientDebugListBox = new System.Windows.Forms.ListBox();
            this.boardPB = new System.Windows.Forms.PictureBox();
            this.bBishop = new System.Windows.Forms.PictureBox();
            this.bKing = new System.Windows.Forms.PictureBox();
            this.bPawn = new System.Windows.Forms.PictureBox();
            this.bKnight = new System.Windows.Forms.PictureBox();
            this.bRook = new System.Windows.Forms.PictureBox();
            this.bQueen = new System.Windows.Forms.PictureBox();
            this.wRook = new System.Windows.Forms.PictureBox();
            this.wQueen = new System.Windows.Forms.PictureBox();
            this.wPawn = new System.Windows.Forms.PictureBox();
            this.wKnight = new System.Windows.Forms.PictureBox();
            this.wKing = new System.Windows.Forms.PictureBox();
            this.wBishop = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.boardPB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bBishop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bKing)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bPawn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bKnight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bRook)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bQueen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.wRook)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.wQueen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.wPawn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.wKnight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.wKing)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.wBishop)).BeginInit();
            this.SuspendLayout();
            // 
            // serverStartBTN
            // 
            this.serverStartBTN.Location = new System.Drawing.Point(12, 254);
            this.serverStartBTN.Name = "serverStartBTN";
            this.serverStartBTN.Size = new System.Drawing.Size(75, 23);
            this.serverStartBTN.TabIndex = 0;
            this.serverStartBTN.Text = "Start Server";
            this.serverStartBTN.UseVisualStyleBackColor = true;
            this.serverStartBTN.Click += new System.EventHandler(this.button1_Click);
            // 
            // clientStartBTN
            // 
            this.clientStartBTN.Location = new System.Drawing.Point(373, 258);
            this.clientStartBTN.Name = "clientStartBTN";
            this.clientStartBTN.Size = new System.Drawing.Size(75, 23);
            this.clientStartBTN.TabIndex = 1;
            this.clientStartBTN.Text = "Start Client";
            this.clientStartBTN.UseVisualStyleBackColor = true;
            this.clientStartBTN.Click += new System.EventHandler(this.button2_Click);
            // 
            // serverDebugListBox
            // 
            this.serverDebugListBox.FormattingEnabled = true;
            this.serverDebugListBox.HorizontalScrollbar = true;
            this.serverDebugListBox.Location = new System.Drawing.Point(12, 14);
            this.serverDebugListBox.Name = "serverDebugListBox";
            this.serverDebugListBox.Size = new System.Drawing.Size(345, 238);
            this.serverDebugListBox.TabIndex = 2;
            // 
            // clientDebugListBox
            // 
            this.clientDebugListBox.FormattingEnabled = true;
            this.clientDebugListBox.HorizontalScrollbar = true;
            this.clientDebugListBox.Location = new System.Drawing.Point(373, 14);
            this.clientDebugListBox.Name = "clientDebugListBox";
            this.clientDebugListBox.Size = new System.Drawing.Size(345, 238);
            this.clientDebugListBox.TabIndex = 3;
            // 
            // boardPB
            // 
            this.boardPB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.boardPB.Location = new System.Drawing.Point(761, 14);
            this.boardPB.Name = "boardPB";
            this.boardPB.Size = new System.Drawing.Size(420, 420);
            this.boardPB.TabIndex = 4;
            this.boardPB.TabStop = false;
            // 
            // bBishop
            // 
            this.bBishop.Image = ((System.Drawing.Image)(resources.GetObject("bBishop.Image")));
            this.bBishop.Location = new System.Drawing.Point(103, 296);
            this.bBishop.Name = "bBishop";
            this.bBishop.Size = new System.Drawing.Size(60, 60);
            this.bBishop.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.bBishop.TabIndex = 5;
            this.bBishop.TabStop = false;
            this.bBishop.Visible = false;
            // 
            // bKing
            // 
            this.bKing.Image = ((System.Drawing.Image)(resources.GetObject("bKing.Image")));
            this.bKing.Location = new System.Drawing.Point(182, 296);
            this.bKing.Name = "bKing";
            this.bKing.Size = new System.Drawing.Size(60, 60);
            this.bKing.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.bKing.TabIndex = 6;
            this.bKing.TabStop = false;
            this.bKing.Visible = false;
            // 
            // bPawn
            // 
            this.bPawn.Image = ((System.Drawing.Image)(resources.GetObject("bPawn.Image")));
            this.bPawn.Location = new System.Drawing.Point(337, 296);
            this.bPawn.Name = "bPawn";
            this.bPawn.Size = new System.Drawing.Size(60, 60);
            this.bPawn.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.bPawn.TabIndex = 8;
            this.bPawn.TabStop = false;
            this.bPawn.Visible = false;
            // 
            // bKnight
            // 
            this.bKnight.Image = ((System.Drawing.Image)(resources.GetObject("bKnight.Image")));
            this.bKnight.Location = new System.Drawing.Point(258, 296);
            this.bKnight.Name = "bKnight";
            this.bKnight.Size = new System.Drawing.Size(60, 60);
            this.bKnight.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.bKnight.TabIndex = 7;
            this.bKnight.TabStop = false;
            this.bKnight.Visible = false;
            // 
            // bRook
            // 
            this.bRook.Image = ((System.Drawing.Image)(resources.GetObject("bRook.Image")));
            this.bRook.Location = new System.Drawing.Point(491, 296);
            this.bRook.Name = "bRook";
            this.bRook.Size = new System.Drawing.Size(60, 60);
            this.bRook.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.bRook.TabIndex = 10;
            this.bRook.TabStop = false;
            this.bRook.Visible = false;
            // 
            // bQueen
            // 
            this.bQueen.Image = ((System.Drawing.Image)(resources.GetObject("bQueen.Image")));
            this.bQueen.Location = new System.Drawing.Point(412, 296);
            this.bQueen.Name = "bQueen";
            this.bQueen.Size = new System.Drawing.Size(60, 60);
            this.bQueen.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.bQueen.TabIndex = 9;
            this.bQueen.TabStop = false;
            this.bQueen.Visible = false;
            // 
            // wRook
            // 
            this.wRook.Image = ((System.Drawing.Image)(resources.GetObject("wRook.Image")));
            this.wRook.Location = new System.Drawing.Point(491, 362);
            this.wRook.Name = "wRook";
            this.wRook.Size = new System.Drawing.Size(60, 60);
            this.wRook.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.wRook.TabIndex = 16;
            this.wRook.TabStop = false;
            this.wRook.Visible = false;
            // 
            // wQueen
            // 
            this.wQueen.Image = ((System.Drawing.Image)(resources.GetObject("wQueen.Image")));
            this.wQueen.Location = new System.Drawing.Point(412, 362);
            this.wQueen.Name = "wQueen";
            this.wQueen.Size = new System.Drawing.Size(60, 60);
            this.wQueen.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.wQueen.TabIndex = 15;
            this.wQueen.TabStop = false;
            this.wQueen.Visible = false;
            // 
            // wPawn
            // 
            this.wPawn.Image = ((System.Drawing.Image)(resources.GetObject("wPawn.Image")));
            this.wPawn.Location = new System.Drawing.Point(337, 362);
            this.wPawn.Name = "wPawn";
            this.wPawn.Size = new System.Drawing.Size(60, 60);
            this.wPawn.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.wPawn.TabIndex = 14;
            this.wPawn.TabStop = false;
            this.wPawn.Visible = false;
            // 
            // wKnight
            // 
            this.wKnight.Image = ((System.Drawing.Image)(resources.GetObject("wKnight.Image")));
            this.wKnight.Location = new System.Drawing.Point(258, 362);
            this.wKnight.Name = "wKnight";
            this.wKnight.Size = new System.Drawing.Size(60, 60);
            this.wKnight.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.wKnight.TabIndex = 13;
            this.wKnight.TabStop = false;
            this.wKnight.Visible = false;
            // 
            // wKing
            // 
            this.wKing.Image = ((System.Drawing.Image)(resources.GetObject("wKing.Image")));
            this.wKing.Location = new System.Drawing.Point(182, 362);
            this.wKing.Name = "wKing";
            this.wKing.Size = new System.Drawing.Size(60, 60);
            this.wKing.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.wKing.TabIndex = 12;
            this.wKing.TabStop = false;
            this.wKing.Visible = false;
            // 
            // wBishop
            // 
            this.wBishop.Image = ((System.Drawing.Image)(resources.GetObject("wBishop.Image")));
            this.wBishop.Location = new System.Drawing.Point(103, 362);
            this.wBishop.Name = "wBishop";
            this.wBishop.Size = new System.Drawing.Size(60, 60);
            this.wBishop.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.wBishop.TabIndex = 11;
            this.wBishop.TabStop = false;
            this.wBishop.Visible = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1205, 487);
            this.Controls.Add(this.wRook);
            this.Controls.Add(this.wQueen);
            this.Controls.Add(this.wPawn);
            this.Controls.Add(this.wKnight);
            this.Controls.Add(this.wKing);
            this.Controls.Add(this.wBishop);
            this.Controls.Add(this.bRook);
            this.Controls.Add(this.bQueen);
            this.Controls.Add(this.bPawn);
            this.Controls.Add(this.bKnight);
            this.Controls.Add(this.bKing);
            this.Controls.Add(this.bBishop);
            this.Controls.Add(this.boardPB);
            this.Controls.Add(this.clientDebugListBox);
            this.Controls.Add(this.serverDebugListBox);
            this.Controls.Add(this.clientStartBTN);
            this.Controls.Add(this.serverStartBTN);
            this.Name = "MainForm";
            this.Text = "COSC636: TCP Chess";
            ((System.ComponentModel.ISupportInitialize)(this.boardPB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bBishop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bKing)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bPawn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bKnight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bRook)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bQueen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.wRook)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.wQueen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.wPawn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.wKnight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.wKing)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.wBishop)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button serverStartBTN;
        private System.Windows.Forms.Button clientStartBTN;
        private System.Windows.Forms.ListBox serverDebugListBox;
        private System.Windows.Forms.ListBox clientDebugListBox;
        private System.Windows.Forms.PictureBox boardPB;
        private System.Windows.Forms.PictureBox bBishop;
        private System.Windows.Forms.PictureBox bKing;
        private System.Windows.Forms.PictureBox bPawn;
        private System.Windows.Forms.PictureBox bKnight;
        private System.Windows.Forms.PictureBox bRook;
        private System.Windows.Forms.PictureBox bQueen;
        private System.Windows.Forms.PictureBox wRook;
        private System.Windows.Forms.PictureBox wQueen;
        private System.Windows.Forms.PictureBox wPawn;
        private System.Windows.Forms.PictureBox wKnight;
        private System.Windows.Forms.PictureBox wKing;
        private System.Windows.Forms.PictureBox wBishop;
    }
}

