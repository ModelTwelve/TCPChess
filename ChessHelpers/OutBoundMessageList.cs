using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessHelpers {
    public class OutBoundMessageQueue {
        private List<string> list = null;
        private object _lock = new object();
        public OutBoundMessageQueue() {
            Initialize();
        }
        private void Initialize() {
            lock(_lock) {
                list = new List<string>();
            }
        }

        public void AddMessage(string data) {
            lock (_lock) {
                list.Add(data);
            }
        }
        public string RemoveMessage() {
            string rv = null;
            lock (_lock) {
                if (list.Count > 0) {
                    rv = list[0];
                    list.RemoveAt(0);
                }
            }
            return rv;
        }


    }
}
