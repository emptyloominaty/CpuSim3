﻿using System;
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

        public static byte[] ConvertFrom16Bit(ushort val) {
            ushort b = (ushort)(val & 0xff);
            ushort a = (ushort)((val >> 8) & 0xff);
            return new byte[] { (byte)a, (byte)b };
        }
    }
}
