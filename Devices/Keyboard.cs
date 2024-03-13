using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CpuSim3.Devices {
    public class Keyboard : Device {

        public Keyboard(byte type, byte id, int bufferStartAddress, int bufferSize) 
            : base(type, id, bufferStartAddress, bufferSize) {
        }

        public override void Run() {
            while (true) {
                if (GlobalVars.key > 0 && Read(0x17)==0) {
                    Write(0x18, (byte)GlobalVars.key);
                    GlobalVars.key = 0;
                    Interrupt(0);
                }
              
                Thread.Sleep(10);
            }
        }

    }
}
