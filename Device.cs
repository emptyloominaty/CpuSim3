using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CpuSim3 {
    public class Device {
        public Thread deviceThread;
        public byte[] memory = new byte[524288];
        public byte id = 0;

        public Device(byte type, byte id, uint bufferStartAddress, uint bufferSize) {
            //Array.Clear(memory, 0, memory.Length);

            Array.Copy(Memory.Data, 8388608+(524288 * id), memory, 0, 524288);
            this.id = id;

            memory[0] = type; //type   0=keyboard 1=gpu 2=storage 3=network 4=vram+display, 5=fpu, 6-user storage port
            memory[1] = id; //id
            memory[2] = 0; //
            memory[3] = 0; //
            memory[4] = 0; //
            memory[5] = 0; //
            memory[6] = 0; //

            byte[] bytes = Functions.ConvertFrom16Bit(bufferStartAddress);
            memory[7] = bytes[0]; //bufferStartAddressHi
            memory[8] = bytes[1]; //bufferStartAddressLo
            
            memory[9] = 0;  //bufferWriteAddressHi
            memory[10] = 0; //bufferWriteAddressLo

            memory[11] = 0; //bufferReadAddressHi
            memory[12] = 0; //bufferReadAddressLo

            bytes = Functions.ConvertFrom16Bit(bufferSize);
            memory[13] = bytes[0]; //bufferSizeHi
            memory[14] = bytes[1]; //bufferSizeLo

            //OUT/IN
            memory[15] = 0; //data3
            memory[16] = 0; //data2
            memory[17] = 0; //data1
            memory[18] = 0; //data0

            StartDevice();
        }

        public void Interrupt(byte interrupt) {
            GlobalVars.cpu.Interrupt((byte)(128 + (id*16)+ interrupt));
        }

        public virtual void Run() {
            while(true) {

                Thread.Sleep(10);
            }
        }


        public void StartDevice() {
            deviceThread = new Thread(new ThreadStart(Run));
            deviceThread.IsBackground = true;
            deviceThread.Start();
        }

    }

}
