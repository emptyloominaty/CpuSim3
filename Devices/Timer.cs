using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;

namespace CpuSim3.Devices {
    public class Timer : Device {
        byte timers;
        long timeA = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        long[] timeB = new long[16];


        public Timer(byte type, byte id, int bufferStartAddress, int bufferSize, byte timers = 4) 
            : base(type, id, bufferStartAddress, bufferSize) {
            this.timers = timers;

            for (int i = 0x100; i <= 0x1000; i++) {
                Memory.DataCanWriteArray[(8388608 + (524288 * id)) + i] = true;
            }

            for (int i = 0; i < timers; i++) {
                timeB[i] = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                Write((int)(0x100 + (i * 5)), 00); //timer enabled

                Write((int)(0x100 + (i * 5) + 1), 00); //timerSet
                Write((int)(0x100 + (i * 5) + 2), 00); //timerSet

                Write((int)(0x100 + (i * 5) + 3), 00); //timer
                Write((int)(0x100 + (i * 5) + 4), 00); //timer
            }
            

        }

        public override void Run() {
            while (true) {
                long time = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                long timeA = time;
                for (int i = 0; i < timers; i++) {
                    if (Read((int)(0x100 + (i * 5))) == 1) {
                        long timerSet = Functions.ConvertTo16Bit(Read((int)(0x100 + (i * 5) + 1)), Read((int)(0x100 + (i * 5) + 2)));
                        int d = (int)(timeA - timeB[i]);
                        byte[] timer = Functions.ConvertFrom16Bit(d);
                        Write((int)(0x100 + (i * 5) + 3), timer[0]);
                        Write((int)(0x100 + (i * 5) + 4), timer[1]); 

                        if (timeA - timeB[i] > timerSet) {
                            Interrupt((byte)(i));
                            timeB[i] = time;
                        }

                        
                    }
                }
              
                Thread.Sleep(1);
            }
        }

    }
}
