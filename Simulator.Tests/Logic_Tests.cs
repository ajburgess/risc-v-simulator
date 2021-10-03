using System;
using Xunit;
using Simulator;

namespace Simulator.Tests
{
    public class Logic_Tests
    {
        [Theory]
        [InlineData(0x00000000, 0,  0,  0x00000000)]
        [InlineData(0x12345678, 15, 0,  0x00005678)]
        [InlineData(0x12345678, 31, 16, 0x00001234)]
        [InlineData(0x12345678, 31, 0,  0x12345678)]
        [InlineData(0x12345678, 0,  0,  0x00000000)]
        [InlineData(0x12345679, 0,  0,  0x00000001)]
        public void Bits_Extracts_Bits(UInt32 word, Byte from, Byte downTo, UInt32 expected)
        {
            UInt32 result = word.Bits(from, downTo);
            Assert.Equal(expected.ToString("X8"), result.ToString("X8"));
        }

        [Theory]
        [InlineData(0x00000002, 0, 0)]
        [InlineData(0x00000003, 0, 1)]
        [InlineData(0x00000003, 1, 1)]
        [InlineData(0x00000010, 0, 0)]
        [InlineData(0x00000010, 4, 1)]
        [InlineData(0x80000000, 31, 1)]
        [InlineData(0x7FFFFFFF, 31, 0)]
        public void Bit_Extracts_Single_Bit_As_Bool(UInt32 word, Byte bit, UInt32 expected)
        {
            UInt32 result = word.Bit(bit);
            Assert.Equal(expected, result);
        }
    }
}
