using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CpuSim3 {
    public class Cpu {
        public Thread cpuThread;
        public OpCode[] opCodes;

        public uint[] registers = new uint[64];

        public float[] registersF = new float[8];
        public double[] registersD = new double[8];


        public Cpu(OpCode[] opCodes) {
            this.opCodes = opCodes;
        }

        public void StartCpu() {
            cpuThread = new Thread(new ThreadStart(Run));
            cpuThread.IsBackground = true;
            cpuThread.Start();
        }

        public void Run() {
            int i = 0;
            while (true) {
                /*Memory.Data[i] = 200;
                i++;*/
                Thread.Sleep(1000); 
            }
        }

    }
}




