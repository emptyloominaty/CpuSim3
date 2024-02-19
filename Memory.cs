using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CpuSim3 {
    public static class Memory {
        public static byte[] Data = new byte[16777216];
        public static bool[] DataROArray = new bool[16777216];

        static Memory() {
            for (int i = 0; i < Data.Length; i++) {
                Data[i] = 0;
                DataROArray[i] = true; //TODO
            }
        }

        public static byte Read(int address) {
            return Data[address];
        }
        public static void Write(int address, byte data) {
            if (DataROArray[address]) {
                Data[address] = data;
            }
            
        }

    }
}
