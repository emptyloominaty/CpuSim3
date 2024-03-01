using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CpuSim3 {
    public static class Functions {
        public static uint ConvertTo32Bit(byte a, byte b, byte c, byte d) {
            return (uint)((a << 24) | (b << 16) | (c << 8) | d);
        }

        public static byte[] ConvertFrom32Bit(uint val) {
            uint d = val & 0xff;
            uint c = (val >> 8) & 0xff;
            uint b = (val >> 16) & 0xff;
            uint a = (val >> 24) & 0xff;
            return new byte[] { (byte)a, (byte)b, (byte)c, (byte)d };
        }

        public static uint ConvertTo24Bit(byte a, byte b, byte c) {
            return (uint)((a << 16) | (b << 8) | c);
        }

        public static byte[] ConvertFrom24Bit(uint val) {
            uint c = val & 0xff;
            uint b = (val >> 8) & 0xff;
            uint a = (val >> 16) & 0xff;
            return new byte[] { (byte)a, (byte)b, (byte)c };
        }

        public static ushort ConvertTo16Bit(byte a, byte b) {
            return (ushort)((a << 8) | b);
        }

        public static byte[] ConvertFrom16Bit(uint val) {
            uint b = (val & 0xff);
            uint a = ((val >> 8) & 0xff);
            return new byte[] { (byte)a, (byte)b };
        }

        public static string FormatClock(long clock) {
            if (clock > 1000000) {
                return ((double)Math.Round((clock / 1000000.0) * 10) / 10) + "MHz";
            } else if (clock > 1000) {
                return ((double)Math.Round((clock / 1000.0) * 10) / 10) + "kHz";
            } else {
                return clock + "Hz";
            }
        }

        public static bool IsNumeric(string str) {
            double result;
            return double.TryParse(str, out result);
        }

        public static int HexToDec(string str) {
            return Convert.ToInt32(str, 16);
        }

        public static string GetDeviceType(byte type) {
            switch(type) {
                case 0:
                    return "Keyboard";
                case 1:
                    return "GPU";
                case 2:
                    return "Storage";
                case 3:
                    return "Network";
                case 4:
                    return "VRAM+Display";
                case 5:
                    return "FPU";
                case 6:
                    return "User Storage Port";
                case 7:
                    return "Timer";
                default:
                    return "";
            }
        }

    }
}
