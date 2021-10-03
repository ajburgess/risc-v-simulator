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

        public static UInt32 Bit(this UInt32 word, Byte bit)
        {
            return word.Bits(bit, bit);
        }

        public static UInt32 SignExtend8(UInt32 value)
        {
            return (value & (1 << 7)) != 0 ? value | 0xFFFFFF00 : value;
        }

        public static UInt32 SignExtend12(UInt32 value)
        {
            return ((value & (1 << 11)) != 0 ? value | 0xFFFFF000 : value);
        }

        public static UInt32 SignExtend13(UInt32 value)
        {
            return ((value & (1 << 12)) != 0 ? value | 0xFFFFE000 : value);
        }

        public static UInt32 SignExtend16(UInt32 value)
        {
            return (value & (1 << 15)) != 0 ? value | 0xFFFF0000 : value;
        }

        public static UInt32 SignExtend20(UInt32 value)
        {
            return ((value & (1 << 19)) != 0 ? value | 0xFFF00000 : value);
        }

        public static bool UnsignedLessThan(UInt32 a, UInt32 b)
        {
            return a < b;
        }

        public static bool SignedLessThan(UInt32 a, UInt32 b)
        {
            return (Int32)a < (Int32)b;
        }

        public static UInt32 ReverseEndian(UInt32 word)
        {
            UInt32 newWord = 0x00000000;
            for (int n = 0; n < 4; n++)
            {
                Byte b = (Byte)word;
                newWord = newWord << 8 | b;
                word = word >> 8;
            }
            return newWord;
        }

        public static UInt16 ReverseHalfWord(UInt16 halfWord)
        {
            UInt16 newHalfWord = 0x0000;
            for (int n = 0; n < 2; n++)
            {
                Byte b = (Byte)halfWord;
                newHalfWord = (UInt16)(newHalfWord << 8 | b);
                halfWord = (UInt16)(halfWord >> 8);
            }
            return newHalfWord;
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