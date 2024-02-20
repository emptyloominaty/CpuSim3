using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CpuSim3 {
    public static class Memory {
        public static byte[] Data = new byte[16777216];
        public static bool[] DataCanWriteArray = new bool[16777216];
        static Memory() {
            for (int i = 0; i < Data.Length; i++) {
                Data[i] = 0;
                DataCanWriteArray[i] = true; //TODO
            }

            //TEST CODE
            Write(7340032, 3);
            Write(7340033, 0); 
            Write(7340034, 0x70);
            Write(7340035, 0);
            Write(7340036, 0);

            Write(7340037, 3);
            Write(7340038, 1);
            Write(7340039, 0x70);
            Write(7340040, 0);
            Write(7340041, 2);

            Write(7340042, 1);
            Write(7340043, 0);
            Write(7340044, 1);
            Write(7340045, 2);
        }

        public static byte Read(uint address) {
            return Data[address];
        }
        public static void Write(uint address, byte data) {
            if (DataCanWriteArray[address]) {
                Data[address] = data;
            }
            
        }

    }
}
