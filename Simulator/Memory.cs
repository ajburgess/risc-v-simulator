using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simulator
{
    public class WordMemory
    {
        private UInt32[] words;

        public WordMemory(UInt32 lengthInWords)
        {
            words = new UInt32[lengthInWords];
        }

        public WordMemory(string memoryInit)
        {
            Dictionary<UInt32, Byte> cells = new System.Collections.Generic.Dictionary<uint, byte>();
            UInt32 address = 0x0000;
            string[] parts = memoryInit.Split(" ");
            foreach (string part in parts)
            {
                if (part.StartsWith("@"))
                {
                    address = ParseHex(part.TrimStart('@'));
                }
                else
                {
                    UInt32 data = ParseHex(part);
                    cells[address] = (Byte)data;
                    address++;
                }
            }
            UInt32 size = (cells.Keys.Max() / 4) + 1;
            words = new uint[size];
            for (UInt32 wordAddress = 0; wordAddress < size; wordAddress++)
            {
                UInt32 word = 0x00000000;
                for (int n = 0; n < 4; n++)
                {
                    Byte b = cells.GetValueOrDefault((UInt32)(wordAddress * 4 + n), (Byte)0x00);
                    word = word << 8 | b;
                }
                words[wordAddress] = word;
            }
        }

        private Byte GetByte(UInt32 address)
        {
            UInt32 word = words[address >> 2];
            Byte byte_shift = (Byte)((3 - (address & 0x03)) << 3);
            return (Byte)(word >> byte_shift);
        }

        public string Dump(UInt32 startAddress, UInt32 size)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("@");
            sb.Append(startAddress.ToString("X4"));
            for (UInt32 address = startAddress; address < startAddress + size; address++)
            {
                sb.Append(" ");
                sb.Append(GetByte(address).ToString("X2"));
            }
            return sb.ToString();
        }

        private UInt32 ParseHex(string hex)
        {
            return Convert.ToUInt32(hex, 16);
        }

        public UInt32 this[UInt32 wordAddress]
        {
            get
            {
                return wordAddress < words.Length ? words[wordAddress] : 0x00000000;
            }
            set
            {
                if (wordAddress < words.Length)
                {
                    words[wordAddress] = value;
                }
            }
        }
    }
}
