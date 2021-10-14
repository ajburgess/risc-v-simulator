using System;
using System.Collections.Generic;

namespace Simulator
{
    public class SparseWordMemory : IWordMemory
    {
        private Dictionary<UInt32, UInt32> words = new Dictionary<uint, uint>();
        private UInt64 sizeInBytes;

        public SparseWordMemory(UInt64 sizeInBytes = 0x100000000)
        {
            this.sizeInBytes = sizeInBytes;
        }

        public UInt64 SizeInBytes => this.sizeInBytes;

        public void Clear()
        {
            words.Clear();
        }

        public uint GetWord(uint byteAddress)
        {
            UInt32 wordAddress = byteAddress >> 2;
            if (words.TryGetValue(wordAddress, out UInt32 word))
                return word;
            else
                return 0x00000000;
        }

        public void SetWord(uint byteAddress, uint value)
        {
            UInt32 wordAddress = byteAddress >> 2;
            if (wordAddress >= sizeInBytes >> 2)
            {
                throw new Exception("Attempt to set word beyond size of memory");
            }
            words[wordAddress] = value;
        }
    }
}