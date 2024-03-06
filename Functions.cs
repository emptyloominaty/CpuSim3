using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CpuSim3 {
    public static class Functions {
        public static int ConvertTo32Bit(byte a, byte b, byte c, byte d) {
            return (int)((a << 24) | (b << 16) | (c << 8) | d);
        }

        public static byte[] ConvertFrom32Bit(int val) {
            int d = val & 0xff;
            int c = (val >> 8) & 0xff;
            int b = (val >> 16) & 0xff;
            int a = (val >> 24) & 0xff;
            return new byte[] { (byte)a, (byte)b, (byte)c, (byte)d };
        }

        public static int ConvertTo24Bit(byte a, byte b, byte c) {
            return (int)((a << 16) | (b << 8) | c);
        }

        public static byte[] ConvertFrom24Bit(int val) {
            int c = val & 0xff;
            int b = (val >> 8) & 0xff;
            int a = (val >> 16) & 0xff;
            return new byte[] { (byte)a, (byte)b, (byte)c };
        }

        public static ushort ConvertTo16Bit(byte a, byte b) {
            return (ushort)((a << 8) | b);
        }

        public static byte[] ConvertFrom16Bit(int val) {
            int b = (val & 0xff);
            int a = ((val >> 8) & 0xff);
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
