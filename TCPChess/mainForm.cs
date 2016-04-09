using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TCPChess {
    public partial class MainForm : Form {

        private List<string> clientTestCommands = null;
        private List<string[]> serverResponses = null;

        private Dictionary<string, PictureBox> pbPieces = new Dictionary<string, PictureBox>();

        private CancellationTokenSource serverTokenSource = new CancellationTokenSource();
        private CancellationTokenSource clientTokenSource = new CancellationTokenSource();
        private object _serverLock = new object();
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

        public MainForm() {
            InitializeComponent();

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

            //

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

            testInit();
        }

        private void testInit() {

            clientTestCommands = new List<string>() {
                "Server_Command,Connect,Jacob",
                "CONNECT,Kenny",
                "GET,players",
                "PLAY,Jacob,W",
                "GET,board",                
            };            
        }        

        private void showBoard() {

            directionsLB.Visible = true;

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

        private void button1_Click(object sender, EventArgs e) {
            serverStartBTN.Enabled = false;
            startServer();
        }



        private async void startServer() {
            Progress<ReportingClass> progress = new Progress<ReportingClass>(ReportServerProgress);
            ChessServer server = new ChessServer(12345, serverTokenSource.Token, progress);
            var t = Task.Run(async() => await server.Start());
        }

        private void ReportServerProgress(ReportingClass src) {
            lock(_serverLock) {
                foreach(var info in src.getMessages()) {
                    serverDebugListBox.Items.Add(info);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e) {
            clientStartBTN.Enabled = false;
            stopClientBTN.Enabled = true;
            startClient();
        }

        private async void startClient() {
            Progress<ReportingClass> progress = new Progress<ReportingClass>(ReportClientProgress);
            client = new ChessClient(clientTokenSource.Token, progress, clientTestCommands);
            var t = Task.Run(async () => {
                await client.Start(12345);
                clientTokenSource = new CancellationTokenSource();
                testInit();
                client = null;
            });
            
        }

        private void ReportClientProgress(ReportingClass src) {
            lock (_clientLock) {
                foreach (var info in src.getMessages()) {
                    clientDebugListBox.Items.Add(info);
                    if (info.ToUpper().StartsWith("BOARD,")) {
                        currentBoardLayout = info;

                        showBoard();
                        showPieces();
                        boardPB.Invalidate();
                    }
                }
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
                        client.getBoard();
                    }
                    selectedX = -1;
                    selectedY = -1;
                }
            }
            showPieces();
            boardPB.Invalidate();
        }

        private void stopClientBTN_Click(object sender, EventArgs e) {
            clientStartBTN.Enabled = true;
            clientTokenSource.Cancel();
            stopClientBTN.Enabled = false;
            directionsLB.Visible = false;
        }
    }
}
