using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simulator
{
    public class WordMemory : IWordMemory
    {
        private UInt32[] words;

        public WordMemory(UInt32 lengthInWords)
        {
            words = new UInt32[lengthInWords];
        }

        public ulong SizeInBytes => (UInt64)words.Length * 4;

        public void Clear()
        {
            for (UInt32 wordAddress = 0; wordAddress < words.Length; wordAddress++)
            {
                words[wordAddress] = 0x00000000;
            }
        }

        public uint GetWord(uint byteAddress)
        {
            UInt32 wordAddress = byteAddress >> 2;
            return wordAddress < words.Length ? words[wordAddress] : 0x00000000;
        }

        public void SetWord(uint byteAddress, uint value)
        {
            UInt32 wordAddress = byteAddress >> 2;
            if (wordAddress >= words.Length)
            {
                throw new Exception("Attempt to set word beyond size of memory");
            }
            words[wordAddress] = value;
        }
    }
}
