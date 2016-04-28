using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessHelpers
{
    public class TimeTracker
    {
        private class IndividualTime
        {
            public DateTime? timeStarted = null;
            public double totalMilliSecs = 0;
        }
        Dictionary<string, IndividualTime> dictTimes = new Dictionary<string, IndividualTime>();

        private string playerName1, playerName2;

        public TimeTracker(string playerName1, string playerName2)
        {
            this.playerName1 = playerName1.ToUpper();
            this.playerName2 = playerName2.ToUpper();
            dictTimes.Add(this.playerName1, new IndividualTime());
            dictTimes.Add(this.playerName2, new IndividualTime());
        }

        public double getMilliSec(string playerName)
        {
            playerName = playerName.ToUpper();
            if (dictTimes[playerName].timeStarted == null)
            {
                return dictTimes[playerName].totalMilliSecs;
            }
            TimeSpan ts = DateTime.Now.Subtract(dictTimes[playerName].timeStarted.Value);
            return dictTimes[playerName].totalMilliSecs + ts.TotalMilliseconds;
        }

        public void toggleTime()
        {
            // Stopping one and Starting the other
            if ((dictTimes[playerName1].timeStarted == null) &&
                (dictTimes[playerName2].timeStarted == null))
            {
                // No one has started yet ...
                dictTimes[playerName1].timeStarted = DateTime.Now;
                return;
            }
            if ((dictTimes[playerName1].timeStarted == null)
                )
            {
                // Need to stop player2 and start player1
                TimeSpan ts = DateTime.Now.Subtract(dictTimes[playerName2].timeStarted.Value);
                dictTimes[playerName2].totalMilliSecs += ts.Milliseconds;
                dictTimes[playerName2].timeStarted = null;
                dictTimes[playerName1].timeStarted = DateTime.Now;
            }
            else
            {
                // Need to stop player1 and start play
                TimeSpan ts = DateTime.Now.Subtract(dictTimes[playerName1].timeStarted.Value);
                dictTimes[playerName1].totalMilliSecs += ts.TotalMilliseconds;
                dictTimes[playerName1].timeStarted = null;
                dictTimes[playerName2].timeStarted = DateTime.Now;
            }
        }

    }


}
