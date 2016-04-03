using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TCPTest {
    public partial class MainForm : Form {

        private List<string> clientTestCommands = null;
        private Dictionary<string, List<string>> serverResponses = null;

        private Dictionary<string, PictureBox> pbPieces = new Dictionary<string, PictureBox>();

        private CancellationTokenSource tokenSource = new CancellationTokenSource();
        private object _serverLock = new object();
        private object _clientLock = new object();

        private string currentBoardLayout = "";

        public MainForm() {
            InitializeComponent();

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

            testInit();
        }

        private void testInit() {

            clientTestCommands = new List<string>() {
                "CONNECT,Kenny",
                "GET,players",
                "PLAY,Jacob",
                "CHOOSE,white",
                "GET,board",
                "MOVE,6:0,5:0",
                "GET,board",
            };

            var board = getInitialBoard();
            board.Add(board[0].Replace("6:0:W:PAWN", "5:0:W:PAWN"));

            serverResponses = new Dictionary<string, List<string>>() {
                { "CONNECT,Kenny", new List<string>() { "OK" } },
                { "GET,players", new List<string>() {"Players,Jacob,Chris,Kenny" } },
                { "PLAY,Jacob",new List<string>() {"OK" } },
                { "CHOOSE,white", new List<string>() {"OK" } },
                { "GET,board", board },
                { "MOVE,6:0,5:0", new List<string>() {"OK" } }
            };
        }

        private void drawBoard() {

            Graphics gObj = Graphics.FromImage(boardPB.Image);

            Brush black = new SolidBrush(Color.DarkGray);
            Brush white = new SolidBrush(Color.AntiqueWhite);

            int margin = 10;
            float width = 50;
            float height = 50;
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

            string[] pieces = currentBoardLayout.Split(',');
            foreach(var piece in pieces) {
                if (!piece.ToUpper().Equals("BOARD")) {
                    string[] info = piece.Split(':');
                    // ex layout 
                    // row:col:color:piece
                    // 1:0:B:PAWN
                    float row = height * Convert.ToSingle(info[0]);
                    float column = width * Convert.ToSingle(info[1]);

                    string key = info[2].ToUpper() + info[3].ToUpper();
                    if (pbPieces.ContainsKey(key)) {
                        gObj.DrawImage(pbPieces[key].Image, Convert.ToInt32(column) + margin, Convert.ToInt32(row) + margin, width, height);
                    }                              
                }
            }
            boardPB.Invalidate();
        }

        private void button1_Click(object sender, EventArgs e) {

            boardPB.Image = new Bitmap(420, 420);

            startServer();
        }

        private List<string> getInitialBoard() {
            List<string> rv = new List<string>();

            // Setup Black on top for now
            rv.Add("0:0:B:ROOK");
            rv.Add("0:1:B:KNIGHT");
            rv.Add("0:2:B:BISHOP");
            rv.Add("0:3:B:KING");
            rv.Add("0:4:B:QUEEN");
            rv.Add("0:5:B:BISHOP");
            rv.Add("0:6:B:KNIGHT");
            rv.Add("0:7:B:ROOK");

            rv.Add("1:0:B:PAWN");
            rv.Add("1:1:B:PAWN");
            rv.Add("1:2:B:PAWN");
            rv.Add("1:3:B:PAWN");
            rv.Add("1:4:B:PAWN");
            rv.Add("1:5:B:PAWN");
            rv.Add("1:6:B:PAWN");
            rv.Add("1:7:W:PAWN");

            // White on the bottom

            rv.Add("6:0:W:PAWN");
            rv.Add("6:1:W:PAWN");
            rv.Add("6:2:W:PAWN");
            rv.Add("6:3:W:PAWN");
            rv.Add("6:4:W:PAWN");
            rv.Add("6:5:W:PAWN");
            rv.Add("6:6:W:PAWN");
            rv.Add("6:7:W:PAWN");

            rv.Add("7:0:W:ROOK");
            rv.Add("7:1:W:KNIGHT");
            rv.Add("7:2:W:BISHOP");
            rv.Add("7:3:W:KING");
            rv.Add("7:4:W:QUEEN");
            rv.Add("7:5:W:BISHOP");
            rv.Add("7:6:W:KNIGHT");
            rv.Add("7:7:W:ROOK");

            StringBuilder sb = new StringBuilder();
            foreach(var square in rv) {
                sb.Append(square + ",");
            }
            sb.Remove(sb.Length - 1, 1);

            rv.Clear();
            rv.Add("Board,"+sb.ToString());

            return rv;
        }

        private async void startServer() {
            Progress<ReportingClass> progress = new Progress<ReportingClass>(ReportServerProgress);
            ChessServer server = new ChessServer(12345, tokenSource.Token, progress, serverResponses);
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
            startClient();
        }

        private async void startClient() {
            Progress<ReportingClass> progress = new Progress<ReportingClass>(ReportClientProgress);
            ChessClient client = new ChessClient(tokenSource.Token, progress, clientTestCommands);
            var t = Task.Run(async () =>  await client.Start(12345) );            
        }

        private void ReportClientProgress(ReportingClass src) {
            lock (_clientLock) {
                foreach (var info in src.getMessages()) {
                    clientDebugListBox.Items.Add(info);
                    if (info.ToUpper().StartsWith("BOARD,")) {
                        currentBoardLayout = info;
                        drawBoard();
                    }
                }
            }
        }
    }
}
