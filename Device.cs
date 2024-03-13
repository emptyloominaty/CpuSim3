﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CpuSim3 {
    public class Device {
        public Thread deviceThread;
        public int startAddress;
        public int deviceAddress;
        public byte id = 0;
        public byte type = 0;
        public int bufferStartAddress;
        public int bufferSize;
        public Device(byte type, byte id, int bufferStartAddress, int bufferSize) {
            //Array.Clear(memory, 0, memory.Length);

            this.bufferStartAddress = bufferStartAddress;
            this.bufferSize = bufferSize;

            startAddress = 0x800000 + (0x80000 * id);
            deviceAddress = startAddress;
            this.id = id;
            this.type = type;
            //Memory.Data

            for (int i = 0; i <= 0xFF; i++) {
                Memory.DataCanWriteArray[(8388608 + (524288 * id)) + i] = true;
            }
            for (int i = bufferStartAddress; i <= bufferStartAddress + bufferSize; i++) {
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

            GlobalVars.assemblerDebug += "DEVICE_INITIALISED_" + id + " type:" + type + Environment.NewLine;
            StartDevice();
        }

        public void Write(int address, byte data) {
            Memory.Write(address + startAddress, data, true);
        }

        public byte Read(int address) {
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
