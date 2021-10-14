using System;

namespace Simulator
{
    public interface IWordMemory
    {
        public UInt32 GetWord(UInt32 byteAddress);
        public void SetWord(UInt32 byteAddress, UInt32 value);
        public void Clear();
        public UInt64 SizeInBytes { get; }
    }
}