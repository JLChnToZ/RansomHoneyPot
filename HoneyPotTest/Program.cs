using System;
using System.Collections.Generic;
using System.Text;
using RansomHoneyPot;
using System.Threading;

namespace HoneyPotTest {
    public class Program {
        public static void Main(string[] args) {
            HandleWatcher watcher = new HandleWatcher(args);
            while(true) Thread.Sleep(1000);
        }
    }
}
