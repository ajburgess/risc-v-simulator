using System;

namespace Simulator
{
    public static class Logic
    {
        private static UInt32[] masks = {
            0x00000001,
            0x00000003,
            0x00000007,
            0x0000000F,
            0x0000001F,
            0x0000003F,
            0x0000007F,
            0x000000FF,
            0x000001FF,
            0x000003FF,
            0x000007FF,
            0x00000FFF,
            0x00001FFF,
            0x00003FFF,
            0x00007FFF,
            0x0000FFFF,
            0x0001FFFF,
            0x0003FFFF,
            0x0007FFFF,
            0x000FFFFF,
            0x001FFFFF,
            0x003FFFFF,
            0x007FFFFF,
            0x00FFFFFF,
            0x01FFFFFF,
            0x03FFFFFF,
            0x07FFFFFF,
            0x0FFFFFFF,
            0x1FFFFFFF,
            0x3FFFFFFF,
            0x7FFFFFFF,
            0xFFFFFFFF
        };

        public static UInt32 Bits(this UInt32 word, Byte bit, Byte downTo)
        {
            return word >> downTo & masks[bit - downTo];
        }

        public static UInt16 Bits(this UInt16 word, Byte bit, Byte downTo)
        {
            return (UInt16)Bits((UInt32)word, bit, downTo);
        }

        public static UInt32 Bit(this UInt32 word, Byte bit)
        {
            return word.Bits(bit, bit);
        }

        public static UInt32 SignExtend(this UInt32 word, Byte msb)
        {
            UInt32 all_ones = 0xFFFFFFFF;
            if (word.Bit(msb) != 0)
                return word | all_ones.Bits(31, msb) << msb;
            else
                return word & all_ones.Bits(msb, 0);
        }

        public static UInt32 SignExtend(this UInt16 word, Byte msb)
        {
            return SignExtend((UInt32)word, msb);
        }

        public static UInt32 SignExtend(this Byte word, Byte msb)
        {
            return SignExtend((UInt32)word, msb);
        }

        public static bool UnsignedLessThan(UInt32 a, UInt32 b)
        {
            return a < b;
        }

        public static bool SignedLessThan(UInt32 a, UInt32 b)
        {
            return (Int32)a < (Int32)b;
        }

        public static UInt32 RightShift(this UInt32 value, Byte n)
        {
            return value >> n;
        }

        public static UInt16 RightShift(this UInt16 value, Byte n)
        {
            return (UInt16)(value >> n);
        }

        public static Byte RightShift(this Byte value, Byte n)
        {
            return (Byte)(value >> n);
        }

        public static UInt32 LeftShift(this UInt32 value, Byte n)
        {
            return value << n;
        }

        public static UInt32 ReverseEndian(this UInt32 w)
        {
            Byte b1 = (Byte)w.Bits(7, 0);
            Byte b2 = (Byte)w.Bits(15, 8);
            Byte b3 = (Byte)w.Bits(23, 16);
            Byte b4 = (Byte)w.Bits(31, 24);
            return (UInt32)(b1 << 24 | b2 << 16 | b3 << 8 | b4);
        }

        public static UInt16 ReverseEndian(this UInt16 h)
        {
            Byte b1 = (Byte)h.Bits(7, 0);
            Byte b2 = (Byte)h.Bits(15, 8);
            return (UInt16)(b1 << 8 | b2);
        }

        public static UInt32 ShiftRightArithmetic(UInt32 value, Byte amount)
        {
            UInt32 msb = value & 0x80000000;
            for (Byte i = 0; i < amount; i++)
            {
                value = msb | (value >> 1);
            }
            return value;
        }
    }
}