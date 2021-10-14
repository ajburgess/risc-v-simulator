using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Simulator
{
    public static class IWordMemoryExtensions
    {
        public static void SetByte(this IWordMemory memory, UInt32 byteAddress, Byte value)
        {
            UInt32 word = memory.GetWord(byteAddress);
            switch (byteAddress & 0x00000003)
            {
                case 0x00: word = (word & 0x00FFFFFF) | (UInt32)value << 24; break;
                case 0x01: word = (word & 0xFF00FFFF) | (UInt32)value << 16; break;
                case 0x02: word = (word & 0xFFFF00FF) | (UInt32)value << 8; break;
                case 0x03: word = (word & 0xFFFFFF00) | (UInt32)value; break;
            }
            memory.SetWord(byteAddress, word);
        }

        public static Byte GetByte(this IWordMemory memory, UInt32 byteAddress)
        {
            UInt32 word = memory.GetWord(byteAddress);
            switch (byteAddress & 0x00000003)
            {
                case 0x00: return (Byte)(word >> 24);
                case 0x01: return (Byte)(word >> 16);
                case 0x02: return (Byte)(word >> 8);
                case 0x03: return (Byte)(word);
                default: return 0x00;
            }
        }

        public static string ToString(this IWordMemory memory, UInt32 byteAddress, UInt32 count)
        {
            string addressFormat = byteAddress + count <= 0xFFFF ? "X4" : "X8";

            StringBuilder sb = new StringBuilder();
            sb.Append($"@{byteAddress.ToString(addressFormat)}");
            for (UInt32 i = 0; i < count; i++)
            {
                Byte b = memory.GetByte(byteAddress + i);
                sb.Append($" {b:X2}");
            }
            return sb.ToString();
        }

        public static UInt32 LoadFromFile(this IWordMemory memory, string path)
        {
            string content = File.ReadAllText(path);
            return memory.Load(content);
        }

        public static UInt32 Load(this IWordMemory memory, string content)
        {
            UInt32 address = 0x0000;
            UInt32? start = null;

            string[] parts = content.Replace("\r\n", " ").Split(" ");
            foreach (string part in parts.Where(p => !string.IsNullOrEmpty(p)))
            {
                if (part.StartsWith("@"))
                {
                    address = ParseHex(part.TrimStart('@'));
                }
                else
                {
                    if (address >= memory.SizeInBytes)
                        throw new Exception("Content is too large for memory");

                    if (start == null)
                        start = address;

                    UInt32 data = ParseHex(part);
                    memory.SetByte(address, (Byte)data);
                    address++;
                }
            }

            return start ?? 0x00000000;
        }

        private static UInt32 ParseHex(string hex)
        {
            return Convert.ToUInt32(hex, 16);
        }
    }
}