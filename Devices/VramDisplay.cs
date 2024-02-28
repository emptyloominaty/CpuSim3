using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CpuSim3.Devices {
    public class VramDisplay : Device {

        public VramDisplay(byte type, byte id, uint bufferStartAddress, uint bufferSize,uint vramSize, uint screenWidth, uint screenHeight, byte[] colorModes) 
            : base(4, id, bufferStartAddress, bufferSize) {

            /*byte[] screenWidthB = Functions.ConvertFrom16Bit(screenWidth);
            memory[0x0100] = screenWidthB[0];
            memory[0x0101] = screenWidthB[1];
            byte[] screenHeightB = Functions.ConvertFrom16Bit(screenHeight);
            memory[0x0102] = screenHeightB[0];
            memory[0x0103] = screenHeightB[1];*/

            for (uint i = 0; i <= vramSize; i++) {
                Memory.DataCanWriteArray[(8388608 + (524288 * id))+i] = true;
            }


        }

    }
}
