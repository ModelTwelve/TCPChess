using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessHelpers {
    public class ReportingClass {
        public object _lock = new object();
        private List<string> messages = new List<string>();

        public void addMessage(string message) {
            lock (_lock) {
                messages.Add(message);
            }
        }

        public List<string> getMessages() {
            lock (_lock) {
                List<string> rv = messages;
                messages = new List<string>();
                return rv;
            }
        }
    }
}
