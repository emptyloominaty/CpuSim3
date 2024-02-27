using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CpuSim3.Devices {
    public class KeyboardD : Device {

        public KeyboardD(byte type, byte id, uint bufferStartAddress, uint bufferSize) 
            : base(4, id, bufferStartAddress, bufferSize) {
        }

        public override void Run() {
            while (true) {
                if (GlobalVars.key > 0 && memory[0x17]==0) {
                    memory[0x18] = (byte)GlobalVars.key;
                    GlobalVars.key = 0;
                    Interrupt(0);
                }
              
                Thread.Sleep(10);
            }
        }

    }
}
