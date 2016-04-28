using ChessHelpers;
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

namespace ServerForm
{
    public partial class MainForm : Form
    {

        private CancellationTokenSource serverTokenSource = new CancellationTokenSource();
        private object _serverLock = new object();

        public MainForm()
        {
            InitializeComponent();
            startServer();
        }        
        private async void startServer()
        {
            Progress<ReportingClass> progress = new Progress<ReportingClass>(ReportServerProgress);
            ChessServer server = new ChessServer(12345, serverTokenSource.Token, progress);
            var t = Task.Run(async () => await server.Start());
        }
        private void ReportServerProgress(ReportingClass src)
        {
            lock (_serverLock)
            {
                foreach (var info in src.getMessages())
                {
                    addToListBox(serverDebugListBox, info);
                }
            }
        }
        private void addToListBox(ListBox lb, string info)
        {
            lb.Items.Insert(0, DateTime.Now.ToString("H:mm:ss.ffff") + ": " + info);
        }
    }
}