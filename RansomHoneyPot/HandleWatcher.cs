using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;

using ThreadState = System.Threading.ThreadState;

namespace RansomHoneyPot {
    public class HandleWatcher: IDisposable {
        public Predicate<Process> ProcessFilter;

        private Thread runThread;
        private HashSet<int> procIds, newProcIds;

        private uint handle;
        private uint pnProcInfoNeeded;
        private uint pnProcInfo;
        private uint lpdwRebootReasons = RestartManager.RmRebootReasonNone;
        private readonly string key;
        private RmProcesInfo[] processInfo;

        public HandleWatcher(string[] files) {
            key = Guid.NewGuid().ToString();
            if(RestartManager.RmStartSession(out handle, 0, key) != 0)
                throw new Exception("Could not start session.");
            if(RestartManager.RmRegisterResources(handle, (uint)files.Length, files, 0, null, 0, null) != 0)
                throw new Exception("Could not register resources");
            procIds = new HashSet<int>();
            newProcIds = new HashSet<int>();
            runThread = new Thread(Loop);
            runThread.Start();
        }

        private void Loop() {
            while(true) {
                try {
                    Step();
                } catch(Exception ex) {
                    Console.WriteLine("Error: {0}", ex.Message);
                }
                Thread.Sleep(0);
            }
        }

        private void Step() {
            int hResult = RestartManager.RmGetList(handle, out pnProcInfoNeeded, ref pnProcInfo, null, ref lpdwRebootReasons);
            if(hResult != RestartManager.ERROR_MORE_DATA) {
                if(hResult != 0)
                    throw new Exception(string.Format("Get List thrown {0}", hResult));
                return;
            }
            if(pnProcInfoNeeded == 0)
                return;
            Console.WriteLine("Detected {0} proces(s)", pnProcInfoNeeded);
            if(processInfo == null || processInfo.Length != pnProcInfoNeeded)
                processInfo = new RmProcesInfo[pnProcInfoNeeded];
            hResult = RestartManager.RmGetList(handle, out pnProcInfoNeeded, ref pnProcInfo, processInfo, ref lpdwRebootReasons);
            if(hResult != RestartManager.ERROR_MORE_DATA && hResult != 0)
                throw new Exception(string.Format("Get List thrown {0}", hResult));
            newProcIds.Clear();
            foreach(RmProcesInfo procInfo in processInfo) {
                try {
                    using(Process proc = Process.GetProcessById(procInfo.process.dwProcessId)) {
                        Console.WriteLine("Detected process {0} ({1}) is playing with one of your file!",
                            proc.ProcessName, procInfo.process.dwProcessId);
                        bool filter = true;
                        if(!procIds.Contains(procInfo.process.dwProcessId))
                            if(ProcessFilter != null)
                                filter = ProcessFilter(proc);
                        if(filter) {
                            newProcIds.Add(procInfo.process.dwProcessId);
                            proc.Kill();
                        }
                    }
                } catch(Exception ex) {
                    Console.WriteLine("Error: {0}", ex.Message);
                }
            }
            HashSet<int> temp = procIds;
            procIds = newProcIds;
            newProcIds = temp;
        }


        public void Dispose() {
            if(runThread.ThreadState == ThreadState.Running)
                runThread.Abort();
            if(handle != 0) {
                RestartManager.RmEndSession(handle);
                handle = 0;
            }
        }
    }
}
