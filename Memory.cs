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

            Write(7340046, 15);
            Write(7340047, 3);

            Write(7340048, 14);
            Write(7340049, 4);
            Write(7340050, 0);
            Write(7340051, 1); 
            Write(7340052, 255); 
            Write(7340053, 255); 

            Write(7340054, 26);
            Write(7340055, 4);
            Write(7340056, 3);
            Write(7340057, 0x70);
            Write(7340058, 0);
            Write(7340059, 0);

            Write(7340060, 24);
            Write(7340061, 0x70);   
            Write(7340062, 0x00);
            Write(7340063, 0x2F);

            Write(7340064, 0);

            Write(7340065, 13);
            Write(7340066, 0);
            Write(7340067, 25); 
            Write(7340068, 255); 
            Write(7340069, 255); 

            Write(7340070, 15);
            Write(7340071, 1);

            Write(7340072, 26);
            Write(7340073, 0);
            Write(7340074, 1);
            Write(7340075, 0x70);
            Write(7340076, 0x00);
            Write(7340077, 0x26);

            Write(7340078, 25);

            Write(7340079, 23);
            Write(7340080, 0x70);
            Write(7340081, 0x00);
            Write(7340082, 0x21);
            /*
            Write(7340083, );
            Write(7340084, );
            Write(7340085, );
            Write(7340086, );
            Write(7340087, );
            Write(7340088, );
            Write(7340089, );
            Write(7340090, );
            Write(7340091, );
            Write(7340092, );
            Write(7340093, );
            Write(7340094, );
            Write(7340095, );
            Write(7340096, );*/



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
