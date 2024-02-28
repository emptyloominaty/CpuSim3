using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CpuSim3 {
    public class Device {
        public Thread deviceThread;
        public uint startAddress;
        public byte id = 0;


        public Device(byte type, byte id, uint bufferStartAddress, uint bufferSize) {
            //Array.Clear(memory, 0, memory.Length);
            startAddress = (uint)(8388608 + (524288 * id));
            this.id = id;

            //Memory.Data

            for (uint i = 0; i <= 0xFF; i++) {
                Memory.DataCanWriteArray[(8388608 + (524288 * id)) + i] = true;
            }

            Memory.Data[startAddress+0] = type; //type   0=keyboard 1=gpu 2=storage 3=network 4=vram+display, 5=fpu, 6-user storage port, 7=timer
            Memory.Data[startAddress+1] = id; //id
            Memory.Data[startAddress+2] = 0; //
            Memory.Data[startAddress+3] = 0; //
            Memory.Data[startAddress+4] = 0; //
            Memory.Data[startAddress+5] = 0; //
            Memory.Data[startAddress+6] = 0; //

            byte[] bytes = Functions.ConvertFrom16Bit(bufferStartAddress);
            Memory.Data[startAddress+7] = bytes[0]; //bufferStartAddressHi
            Memory.Data[startAddress+8] = bytes[1]; //bufferStartAddressLo
            
            Memory.Data[startAddress+9] = 0;  //bufferWriteAddressHi
            Memory.Data[startAddress+10] = 0; //bufferWriteAddressLo

            Memory.Data[startAddress+11] = 0; //bufferReadAddressHi
            Memory.Data[startAddress+12] = 0; //bufferReadAddressLo

            bytes = Functions.ConvertFrom16Bit(bufferSize);
            Memory.Data[startAddress+13] = bytes[0]; //bufferSizeHi
            Memory.Data[startAddress+14] = bytes[1]; //bufferSizeLo

            //Reg0 (4x8B)
            Memory.Data[startAddress+15] = 0;
            Memory.Data[startAddress+16] = 0;
            Memory.Data[startAddress+17] = 0;
            Memory.Data[startAddress+18] = 0;

            //Reg1 (4x8B)
            Memory.Data[startAddress+19] = 0;
            Memory.Data[startAddress+20] = 0;
            Memory.Data[startAddress+21] = 0;
            Memory.Data[startAddress+22] = 0;

            //Reg2 (4x8B)
            Memory.Data[startAddress+23] = 0;
            Memory.Data[startAddress+24] = 0;
            Memory.Data[startAddress+25] = 0;
            Memory.Data[startAddress+26] = 0;

            //Reg3 (4x8B)
            Memory.Data[startAddress+27] = 0;
            Memory.Data[startAddress+28] = 0;
            Memory.Data[startAddress+29] = 0;
            Memory.Data[startAddress+30] = 0;

            Debug.WriteLine("DEVICE_INITIALISED_"+id+" type:"+type);
            StartDevice();
        }

        public void Write(uint address, byte data) {
            Memory.Write(address + startAddress, data, true);
        }

        public byte Read(uint address) {
            return Memory.Read(address + startAddress);
        }

        public void Interrupt(byte interrupt) {
            GlobalVars.cpu.Interrupt((byte)(128 + (id * 8)+ interrupt));
            //Debug.WriteLine("Interrupt" + (128 + (id * 8) + interrupt));
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
