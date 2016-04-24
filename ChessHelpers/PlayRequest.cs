using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessHelpers {
    public class PlayRequest {
        public string RemoteEndPoint { get; set; }
        public string Color { get; set; }
        public PlayRequest(string remoteEndPoint, string color) {
            this.RemoteEndPoint = remoteEndPoint;
            this.Color = color;
        }
    }
}
