namespace TCPChess {
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
            this.stopClientBTN = new System.Windows.Forms.Button();
            this.directionsLB = new System.Windows.Forms.Label();
            this.playersLB = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.colorPanel = new System.Windows.Forms.Panel();
            this.blackRB = new System.Windows.Forms.RadioButton();
            this.whiteRB = new System.Windows.Forms.RadioButton();
            this.gameLB = new System.Windows.Forms.Label();
            this.serverIPTB = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.portTB = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.requestsLB = new System.Windows.Forms.ListBox();
            this.label7 = new System.Windows.Forms.Label();
            this.playerNameTB = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
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
            this.colorPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // serverStartBTN
            // 
            this.serverStartBTN.Location = new System.Drawing.Point(1076, 217);
            this.serverStartBTN.Name = "serverStartBTN";
            this.serverStartBTN.Size = new System.Drawing.Size(116, 23);
            this.serverStartBTN.TabIndex = 0;
            this.serverStartBTN.Text = "Start Server Test";
            this.serverStartBTN.UseVisualStyleBackColor = true;
            this.serverStartBTN.Click += new System.EventHandler(this.serverStartBTN_Click);
            // 
            // clientStartBTN
            // 
            this.clientStartBTN.Location = new System.Drawing.Point(455, 366);
            this.clientStartBTN.Name = "clientStartBTN";
            this.clientStartBTN.Size = new System.Drawing.Size(111, 23);
            this.clientStartBTN.TabIndex = 1;
            this.clientStartBTN.Tag = "CONNECT";
            this.clientStartBTN.Text = "CONNECT";
            this.clientStartBTN.UseVisualStyleBackColor = true;
            this.clientStartBTN.Click += new System.EventHandler(this.clientStartBTN_Click);
            // 
            // serverDebugListBox
            // 
            this.serverDebugListBox.FormattingEnabled = true;
            this.serverDebugListBox.HorizontalScrollbar = true;
            this.serverDebugListBox.Location = new System.Drawing.Point(1076, 51);
            this.serverDebugListBox.Name = "serverDebugListBox";
            this.serverDebugListBox.Size = new System.Drawing.Size(249, 160);
            this.serverDebugListBox.TabIndex = 2;
            // 
            // clientDebugListBox
            // 
            this.clientDebugListBox.FormattingEnabled = true;
            this.clientDebugListBox.HorizontalScrollbar = true;
            this.clientDebugListBox.Location = new System.Drawing.Point(1076, 267);
            this.clientDebugListBox.Name = "clientDebugListBox";
            this.clientDebugListBox.Size = new System.Drawing.Size(249, 212);
            this.clientDebugListBox.TabIndex = 3;
            // 
            // boardPB
            // 
            this.boardPB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.boardPB.Location = new System.Drawing.Point(12, 31);
            this.boardPB.Name = "boardPB";
            this.boardPB.Size = new System.Drawing.Size(420, 420);
            this.boardPB.TabIndex = 4;
            this.boardPB.TabStop = false;
            this.boardPB.Click += new System.EventHandler(this.boardPB_Click);
            // 
            // bBishop
            // 
            this.bBishop.Image = ((System.Drawing.Image)(resources.GetObject("bBishop.Image")));
            this.bBishop.Location = new System.Drawing.Point(1265, 417);
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
            this.bKing.Location = new System.Drawing.Point(1265, 417);
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
            this.bPawn.Location = new System.Drawing.Point(1265, 417);
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
            this.bKnight.Location = new System.Drawing.Point(1265, 417);
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
            this.bRook.Location = new System.Drawing.Point(1265, 415);
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
            this.bQueen.Location = new System.Drawing.Point(1265, 415);
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
            this.wRook.Location = new System.Drawing.Point(1265, 415);
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
            this.wQueen.Location = new System.Drawing.Point(1265, 417);
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
            this.wPawn.Location = new System.Drawing.Point(1265, 417);
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
            this.wKnight.Location = new System.Drawing.Point(1265, 417);
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
            this.wKing.Location = new System.Drawing.Point(1265, 417);
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
            this.wBishop.Location = new System.Drawing.Point(1265, 415);
            this.wBishop.Name = "wBishop";
            this.wBishop.Size = new System.Drawing.Size(60, 60);
            this.wBishop.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.wBishop.TabIndex = 11;
            this.wBishop.TabStop = false;
            this.wBishop.Visible = false;
            // 
            // stopClientBTN
            // 
            this.stopClientBTN.Enabled = false;
            this.stopClientBTN.Location = new System.Drawing.Point(593, 366);
            this.stopClientBTN.Name = "stopClientBTN";
            this.stopClientBTN.Size = new System.Drawing.Size(111, 23);
            this.stopClientBTN.TabIndex = 17;
            this.stopClientBTN.Tag = "GAME";
            this.stopClientBTN.Text = "QUIT GAME";
            this.stopClientBTN.UseVisualStyleBackColor = true;
            this.stopClientBTN.Click += new System.EventHandler(this.stopClientBTN_Click);
            // 
            // directionsLB
            // 
            this.directionsLB.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.directionsLB.ForeColor = System.Drawing.Color.Red;
            this.directionsLB.Location = new System.Drawing.Point(12, 454);
            this.directionsLB.Name = "directionsLB";
            this.directionsLB.Size = new System.Drawing.Size(420, 25);
            this.directionsLB.TabIndex = 18;
            this.directionsLB.Text = "Left Click to Select - Right Click to Move";
            this.directionsLB.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.directionsLB.Visible = false;
            // 
            // playersLB
            // 
            this.playersLB.FormattingEnabled = true;
            this.playersLB.HorizontalScrollbar = true;
            this.playersLB.Location = new System.Drawing.Point(455, 25);
            this.playersLB.Name = "playersLB";
            this.playersLB.Size = new System.Drawing.Size(249, 186);
            this.playersLB.TabIndex = 19;
            this.playersLB.Click += new System.EventHandler(this.playersListBox_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(455, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 13);
            this.label1.TabIndex = 20;
            this.label1.Text = "Connected Players";
            // 
            // panel1
            // 
            this.colorPanel.Controls.Add(this.blackRB);
            this.colorPanel.Controls.Add(this.whiteRB);
            this.colorPanel.Location = new System.Drawing.Point(455, 422);
            this.colorPanel.Name = "panel1";
            this.colorPanel.Size = new System.Drawing.Size(82, 57);
            this.colorPanel.TabIndex = 22;
            // 
            // blackRB
            // 
            this.blackRB.AutoSize = true;
            this.blackRB.Location = new System.Drawing.Point(15, 35);
            this.blackRB.Name = "blackRB";
            this.blackRB.Size = new System.Drawing.Size(52, 17);
            this.blackRB.TabIndex = 1;
            this.blackRB.Text = "Black";
            this.blackRB.UseVisualStyleBackColor = true;
            // 
            // whiteRB
            // 
            this.whiteRB.AutoSize = true;
            this.whiteRB.Checked = true;
            this.whiteRB.Location = new System.Drawing.Point(15, 12);
            this.whiteRB.Name = "whiteRB";
            this.whiteRB.Size = new System.Drawing.Size(53, 17);
            this.whiteRB.TabIndex = 0;
            this.whiteRB.TabStop = true;
            this.whiteRB.Text = "White";
            this.whiteRB.UseVisualStyleBackColor = true;
            // 
            // gameLB
            // 
            this.gameLB.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gameLB.ForeColor = System.Drawing.Color.Blue;
            this.gameLB.Location = new System.Drawing.Point(12, 3);
            this.gameLB.Name = "gameLB";
            this.gameLB.Size = new System.Drawing.Size(420, 25);
            this.gameLB.TabIndex = 23;
            this.gameLB.Text = "Game On!";
            this.gameLB.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.gameLB.Visible = false;
            // 
            // serverIPTB
            // 
            this.serverIPTB.Location = new System.Drawing.Point(629, 431);
            this.serverIPTB.Name = "serverIPTB";
            this.serverIPTB.Size = new System.Drawing.Size(75, 20);
            this.serverIPTB.TabIndex = 24;
            this.serverIPTB.Text = "127.0.0.1";
            this.serverIPTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(569, 434);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 25;
            this.label2.Text = "Server IP:";
            // 
            // portTB
            // 
            this.portTB.Location = new System.Drawing.Point(629, 457);
            this.portTB.Name = "portTB";
            this.portTB.Size = new System.Drawing.Size(75, 20);
            this.portTB.TabIndex = 26;
            this.portTB.Text = "12345";
            this.portTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(554, 460);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 13);
            this.label3.TabIndex = 27;
            this.label3.Text = "Port Number:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(1073, 252);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(88, 13);
            this.label4.TabIndex = 28;
            this.label4.Text = "Client Information";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(1073, 35);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(93, 13);
            this.label5.TabIndex = 29;
            this.label5.Text = "Server Information";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1218, 217);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(107, 23);
            this.button1.TabIndex = 30;
            this.button1.Text = "Start Client Test";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.label6.Location = new System.Drawing.Point(1076, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(249, 23);
            this.label6.TabIndex = 31;
            this.label6.Text = "TEST AND DEBUG";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // requestsLB
            // 
            this.requestsLB.FormattingEnabled = true;
            this.requestsLB.HorizontalScrollbar = true;
            this.requestsLB.Location = new System.Drawing.Point(455, 252);
            this.requestsLB.Name = "requestsLB";
            this.requestsLB.Size = new System.Drawing.Size(249, 108);
            this.requestsLB.TabIndex = 33;
            this.requestsLB.Click += new System.EventHandler(this.requestsLB_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(455, 236);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(85, 13);
            this.label7.TabIndex = 34;
            this.label7.Text = "Match Requests";
            // 
            // playerNameTB
            // 
            this.playerNameTB.Location = new System.Drawing.Point(572, 405);
            this.playerNameTB.Name = "playerNameTB";
            this.playerNameTB.Size = new System.Drawing.Size(132, 20);
            this.playerNameTB.TabIndex = 35;
            this.playerNameTB.Text = "Sue";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(496, 408);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(70, 13);
            this.label8.TabIndex = 36;
            this.label8.Text = "Player Name:";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1337, 487);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.playerNameTB);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.requestsLB);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.portTB);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.serverIPTB);
            this.Controls.Add(this.gameLB);
            this.Controls.Add(this.colorPanel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.playersLB);
            this.Controls.Add(this.directionsLB);
            this.Controls.Add(this.stopClientBTN);
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
            this.colorPanel.ResumeLayout(false);
            this.colorPanel.PerformLayout();
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
        private System.Windows.Forms.Button stopClientBTN;
        private System.Windows.Forms.Label directionsLB;
        private System.Windows.Forms.ListBox playersLB;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel colorPanel;
        private System.Windows.Forms.RadioButton blackRB;
        private System.Windows.Forms.RadioButton whiteRB;
        private System.Windows.Forms.Label gameLB;
        private System.Windows.Forms.TextBox serverIPTB;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox portTB;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ListBox requestsLB;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox playerNameTB;
        private System.Windows.Forms.Label label8;
    }
}

