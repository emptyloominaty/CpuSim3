using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CpuSim3.Devices {
    public class Storage : Device {
        public byte[] storageData;
        public Storage(byte type, byte id, int bufferStartAddress, int bufferSize,int size) 
            : base(type, id, bufferStartAddress, bufferSize) {

            storageData = new byte[size];
            GlobalVars.storages.Add(new StorageWindow(id));
        }



        public override void Run() {
            while (true) {
                //reg2_3 instruction read=1 write=2  (1B (DeviceAddress+26))
                //reg3 address (4B (DeviceAddress+27))
                bool interrupt = false;
                int address = Functions.ConvertTo32Bit(Memory.Data[startAddress + 27], Memory.Data[startAddress + 28], Memory.Data[startAddress + 29], Memory.Data[startAddress + 30]);      

                if (Memory.Read(startAddress + 26) == 1) { //read

                    for (int i = bufferStartAddress; i< bufferStartAddress+bufferSize; i++) {
                        Memory.Write(deviceAddress + bufferStartAddress + i, storageData[address + i]);
                    }

                    interrupt = true;
                    resetInstructionReg();
                } else if (Memory.Read(startAddress + 26) == 2) { //write

                    for (int i = bufferStartAddress; i < bufferStartAddress + bufferSize; i++) {
                        Write(address + i,Memory.Read(deviceAddress + bufferStartAddress + i));
                    }

                    interrupt = true;
                    resetInstructionReg();
                }

                if (interrupt) {
                    Interrupt((byte)(0));
                }

                Thread.Sleep(10);
            }
        }

        void resetInstructionReg() {
            Memory.Write(startAddress + 26, 0, true);
        }

        void Write(int address, byte data) {
            if (address >= 0 && address < storageData.Length) {
                storageData[address] = data;
            }
        }

    }
}
