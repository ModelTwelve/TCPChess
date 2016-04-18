using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ChessHelpers;

namespace ChessClient {
    public partial class MainForm : Form {
        private List<string> clientTestCommands = null;
        private Dictionary<string, PictureBox> pbPieces = new Dictionary<string, PictureBox>();        
        private CancellationTokenSource clientTokenSource = new CancellationTokenSource();        
        private object _clientLock = new object();
        private string currentBoardLayout = "";
        private Brush black = new SolidBrush(Color.DarkGray);
        private Brush white = new SolidBrush(Color.AntiqueWhite);
        private Brush yellow = new SolidBrush(Color.Yellow);

        private int margin = 10;
        private float width = 50;
        private float height = 50;

        private ChessClient client = null;

        private int selectedX = -1, selectedY = -1;

        private string opponentPlayerName = "";
        private Dictionary<string, string> dictRequests = new Dictionary<string, string>();

        public MainForm() {
            InitializeComponent();
            
            foreach (var control in Controls.OfType<Button>()) {
                control.TabStop = false;
            }
            foreach (var control in Controls.OfType<ListBox>()) {
                control.TabStop = false;
            }
            foreach (var control in Controls.OfType<Label>()) {
                control.TabStop = false;
            }
            whiteRB.TabStop = false;

            playerNameTB.Text = "Player" + DateTime.Now.ToString("fffff");
            playerNameTB.TabIndex = 0;
            serverIPTB.TabIndex = 1;
            portTB.TabIndex = 2;
            clientStartBTN.TabStop = true;
            clientStartBTN.TabIndex = 3;

            typeof(PictureBox).InvokeMember("DoubleBuffered", BindingFlags.SetProperty
            | BindingFlags.Instance | BindingFlags.NonPublic, null,
            boardPB, new object[] { true });

            ((Bitmap)bBishop.Image).MakeTransparent(((Bitmap)bBishop.Image).GetPixel(1, 1));
            bBishop.BackColor = System.Drawing.Color.Transparent;

            ((Bitmap)bKing.Image).MakeTransparent(((Bitmap)bKing.Image).GetPixel(1, 1));
            bKing.BackColor = System.Drawing.Color.Transparent;

            ((Bitmap)bKnight.Image).MakeTransparent(((Bitmap)bKnight.Image).GetPixel(1, 1));
            bKnight.BackColor = System.Drawing.Color.Transparent;

            ((Bitmap)bPawn.Image).MakeTransparent(((Bitmap)bPawn.Image).GetPixel(1, 1));
            bPawn.BackColor = System.Drawing.Color.Transparent;

            ((Bitmap)bQueen.Image).MakeTransparent(((Bitmap)bQueen.Image).GetPixel(1, 1));
            bBishop.BackColor = System.Drawing.Color.Transparent;

            ((Bitmap)bRook.Image).MakeTransparent(((Bitmap)bRook.Image).GetPixel(1, 1));
            bRook.BackColor = System.Drawing.Color.Transparent;

            ((Bitmap)wBishop.Image).MakeTransparent(((Bitmap)wBishop.Image).GetPixel(1, 1));
            wBishop.BackColor = System.Drawing.Color.Transparent;

            ((Bitmap)wKing.Image).MakeTransparent(((Bitmap)wKing.Image).GetPixel(1, 1));
            wKing.BackColor = System.Drawing.Color.Transparent;

            ((Bitmap)wKnight.Image).MakeTransparent(((Bitmap)wKnight.Image).GetPixel(1, 1));
            wKnight.BackColor = System.Drawing.Color.Transparent;

            ((Bitmap)wPawn.Image).MakeTransparent(((Bitmap)wPawn.Image).GetPixel(1, 1));
            wPawn.BackColor = System.Drawing.Color.Transparent;

            ((Bitmap)wQueen.Image).MakeTransparent(((Bitmap)wQueen.Image).GetPixel(1, 1));
            wQueen.BackColor = System.Drawing.Color.Transparent;

            ((Bitmap)wRook.Image).MakeTransparent(((Bitmap)wRook.Image).GetPixel(1, 1));
            wRook.BackColor = System.Drawing.Color.Transparent;

            pbPieces.Add("BBISHOP", bBishop);
            pbPieces.Add("BKING", bKing);
            pbPieces.Add("BKNIGHT", bKnight);
            pbPieces.Add("BPAWN", bPawn);
            pbPieces.Add("BQUEEN", bQueen);
            pbPieces.Add("BROOK", bRook);

            pbPieces.Add("WBISHOP", wBishop);
            pbPieces.Add("WKING", wKing);
            pbPieces.Add("WKNIGHT", wKnight);
            pbPieces.Add("WPAWN", wPawn);
            pbPieces.Add("WQUEEN", wQueen);
            pbPieces.Add("WROOK", wRook);

            boardPB.Image = new Bitmap(420, 420);

            showBoard();            
        }

        private void testInit() {

            // PLAYERS,Donald:Lobby,David:Playing Jacob,Chris:Lobby,Jacob:Playing David,Joe:Lobby
            clientTestCommands = new List<string>() {
                "Server_Command,Add,Donald,ACCEPT", // When asked to PLAY Donald will ACCEPT
                "Server_Command,Add,David",
                "Server_Command,Add,Chris", // When asked to PLAY Chris will not respond
                "Server_Command,Add,Jacob",
                "Server_Command,Add,Joe,REFUSE", // When asked to PLAY Joe will REFUSE
                "Server_Command,Match,David:W,Jacob:B",
                "Server_Command,Add,George,REQUESTW", // When asked to PLAY George will send you a REQUEST instead
            };            
        }        

        private void showBoard() {          

            Graphics gObj = Graphics.FromImage(boardPB.Image);

            for (int row = 0; row < 8; ++row) {
                Brush even;
                Brush odd;
                if (row % 2 == 0) {
                    even = white;
                    odd = black;
                }
                else {
                    odd = white;
                    even = black;
                }
                for (int column = 0; column < 8; ++column) {
                    if (column % 2 == 0) {
                        gObj.FillRectangle(even, column * width + margin, row * height + margin, width, height);
                    }
                    else {
                        gObj.FillRectangle(odd, column * width + margin, row * height + margin, width, height);
                    }
                }
            }                      
        }
        private void showPieces() {

            Graphics gObj = Graphics.FromImage(boardPB.Image);
            
            string[] pieces = currentBoardLayout.Split(',');
            foreach (var piece in pieces) {
                if (!piece.ToUpper().Equals("BOARD")) {
                    string[] info = piece.Split(':');
                    // ex layout 
                    // row:col:color:piece
                    // 1:0:B:PAWN
                    float column = height * Convert.ToSingle(info[0]);
                    float row = width * Convert.ToSingle(info[1]);

                    string key = info[2].ToUpper() + info[3].ToUpper();
                    if (pbPieces.ContainsKey(key)) {
                        gObj.DrawImage(pbPieces[key].Image, Convert.ToInt32(column) + margin, Convert.ToInt32(row) + margin, width, height);
                    }
                }
            }
        }        
        private void addToListBox(ListBox lb, string info) {
            lb.Items.Insert(0, DateTime.Now.ToString("H:mm:ss.ffff")+": "+info);
        }        
        private void whenConnectPushed() {
            clientStartBTN.Text = "PLAY MATCH";
            clientStartBTN.Tag = "PLAY";
            clientStartBTN.Enabled = false;
            stopClientBTN.Enabled = true;
            clientTestCommands = new List<string>();
        }
        private void clientStartBTN_Click(object sender, EventArgs e) {
            switch(clientStartBTN.Tag.ToString()) {
                case "CONNECT":
                    whenConnectPushed();
                    startClient();
                    break;
                case "PLAY":
                    if (playersLB.SelectedIndex >= 0) {
                        string toPlay = playersLB.Items[playersLB.SelectedIndex].ToString();
                        client.requestPlay(toPlay, whiteRB.Checked ? "W" : "B");                        
                    }
                    else if (requestsLB.SelectedIndex >= 0) {
                        string toPlay = requestsLB.Items[requestsLB.SelectedIndex].ToString();
                        client.acceptPlay(toPlay);
                        stopClientBTN.Tag = "MATCH";
                        stopClientBTN.Text = "QUIT MATCH";
                    }
                    else {
                        stopClientBTN.Tag = "GAME";
                        stopClientBTN.Text = "QUIT GAME";
                        addToListBox(clientDebugListBox,"No Player Selected. Please select a player!");
                    }
                    break;
            }             
        }
        private async void startClient() {
            try {
                int port = Convert.ToInt32(portTB.Text);
                IPAddress ipAddress=IPAddress.Parse(serverIPTB.Text);
                Progress<ReportingClass> progress = new Progress<ReportingClass>(ReportClientProgress);
                
                client = new ChessClient(clientTokenSource.Token, progress, clientTestCommands);
                var t = Task.Run(async () => {
                    await client.Start(ipAddress, port, playerNameTB.Text);
                    clientTokenSource = new CancellationTokenSource();                    
                    client = null;
                });
            }
            catch(Exception e) {
                addToListBox(clientDebugListBox,e.Message);
            }            
        }
        private void ReportClientProgress(ReportingClass src) {
            lock (_clientLock) {
                foreach (var info in src.getMessages()) {
                    addToListBox(clientDebugListBox,info);
                    if (info.ToUpper().StartsWith("BOARD,")) {
                        currentBoardLayout = info;
                        enableGameOn();
                        showBoard();                        
                        showPieces();
                        boardPB.Invalidate();
                        client.requestGetPlayers();
                    }
                    else if (info.ToUpper().StartsWith("PLAYERS,")) {
                        showPlayers(info);
                    }
                    else if (info.ToUpper().StartsWith("REQUEST,")) {
                        showRequested(info);
                    }
                    else if (info.ToUpper().StartsWith("ACCEPTED,")) {                        
                        showAccepted(info);
                    }
                    else if (info.ToUpper().StartsWith("WINNER,")) {                        
                        showWinner(info);
                    }
                    else if (info.ToUpper().StartsWith("REFUSED,")) {
                        showRefused(info);
                    }
                }
            }
        }
        private void showRequested(string info) {
            if (opponentPlayerName.Length == 0) {
                string[] split = info.Split(',');
                dictRequests.Add(split[1], split[2]);
                requestsLB.Items.Add(split[1]+":"+ split[2]);
            }
        }
        private void showRefused(string info) {
            if (opponentPlayerName.Length == 0) {
                string[] split = info.Split(',');
                string[] playerData = split[1].Split(':');
                if (dictRequests.ContainsKey(playerData[0])) {
                    dictRequests.Remove(playerData[0]);
                    requestsLB.Items.Remove(split[1]);
                }
            }
        }
        private void showAccepted(string info) {
            requestsLB.Items.Clear();
            string[] split = info.Split(',');
            opponentPlayerName = split[1];
            string color = split[2].ToUpper();
            if (color.Equals("W")) {
                // If your opponent is white you must be black
                blackRB.Checked = true;                
            }
            else {
                // If your opponent is black you must be white
                whiteRB.Checked = true;
            }
            colorPanel.Enabled = false;
            dictRequests = new Dictionary<string, string>();
            gameLB.Text = "Playing " + split[1];
        }
        private void showWinner(string info) {
            string[] split = info.Split(',');
            opponentPlayerName = "";
            dictRequests = new Dictionary<string, string>();
            gameLB.Text = "Game Over. Winner is " + split[1];
        }
        private void enableGameOn() {
            dictRequests.Clear();
            directionsLB.Visible = true;
            gameLB.Visible = true;
            stopClientBTN.Tag = "MATCH";
            stopClientBTN.Text = "QUIT MATCH";
            clientStartBTN.Enabled= false;
        }
        private void showPlayers(string info) {
            playersLB.Items.Clear();
            string[] split = info.Split(',');
            if (split.Length > 1) {
                clientStartBTN.Enabled = true;
                for (int x = 1; x < split.Length; x++) {
                    playersLB.Items.Add(split[x]);
                }
            }
            else {
                clientStartBTN.Enabled = false;
            }
        }
        private void boardPB_Click(object sender, EventArgs e) {
            // Reset the board
            showBoard();            
            float x, y;
            
            Point point = boardPB.PointToClient(Cursor.Position);
            MouseEventArgs me = (MouseEventArgs)e;
            if (me.Button == MouseButtons.Left) {
                x = point.X - margin;
                y = point.Y - margin;
                selectedX = Convert.ToInt32(x) / Convert.ToInt32(width);
                selectedY = Convert.ToInt32(y) / Convert.ToInt32(height);
                Graphics gObj = Graphics.FromImage(boardPB.Image);
                gObj.FillRectangle(yellow, selectedX * width + margin, selectedY * height + margin, width, height);
            }
            else {
                if (selectedX != -1) {

                    x = point.X - margin;
                    y = point.Y - margin;

                    int newX = 0, newY = 0;
                    newX = Convert.ToInt32(x) / Convert.ToInt32(width);
                    newY = Convert.ToInt32(y) / Convert.ToInt32(height);

                    if (client != null) {
                        client.requestMove(selectedX.ToString()+":"+selectedY.ToString()+","+newX.ToString()+":"+newY.ToString());
                        //client.getBoard();
                    }
                    selectedX = -1;
                    selectedY = -1;
                }
            }
            showPieces();
            boardPB.Invalidate();
        }
        private void button1_Click(object sender, EventArgs e) {
            whenConnectPushed();
            testInit();
            startClient();            
        }
        private void playersListBox_Click(object sender, EventArgs e) {
            requestsLB.SelectedIndex = -1;
        }
        private void requestsLB_Click(object sender, EventArgs e) {
            playersLB.SelectedIndex = -1;
        }
        private void stopClientBTN_Click(object sender, EventArgs e) {

            switch (stopClientBTN.Tag.ToString()) {
                case "MATCH":
                    stopClientBTN.Tag = "GAME";
                    stopClientBTN.Text = "QUIT GAME";
                    clientStartBTN.Enabled = true;
                    colorPanel.Enabled = true;
                    clientStartBTN.Text = "PLAY MATCH";
                    client.quitMatch();
                    opponentPlayerName = "";
                    dictRequests = new Dictionary<string, string>();
                    requestsLB.Items.Clear();
                    break;
                case "GAME":
                    stopClientBTN.Tag = "";
                    client.quitGame();
                    opponentPlayerName = "";
                    colorPanel.Enabled = true;
                    dictRequests = new Dictionary<string, string>();
                    requestsLB.Items.Clear();
                    clientTokenSource.Cancel();

                    clientStartBTN.Text = "CONNECT";
                    clientStartBTN.Tag = "CONNECT";

                    directionsLB.Visible = false;
                    gameLB.Visible = false;

                    clientStartBTN.Enabled = true;
                    stopClientBTN.Enabled = false;

                    currentBoardLayout = "";
                    showBoard();
                    boardPB.Invalidate();
                    break;
                default:
                    break;
            }
            Task.Delay(1000).Wait();
        }
    }
}
