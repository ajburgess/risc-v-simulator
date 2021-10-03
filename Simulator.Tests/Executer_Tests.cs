using System;
using Xunit;

namespace Simulator
{
    public class Executer_Tests
    {
        [Theory]
        [InlineData(Instruction.ADD, 1000, 1234, 2234)]
        [InlineData(Instruction.ADD, 1000, -1234, -234)]
        [InlineData(Instruction.ADD, -1000, -1234, -2234)]
        [InlineData(Instruction.SUB, 1234, 1000, 234)]
        [InlineData(Instruction.SUB, 1000, 1234, -234)]
        [InlineData(Instruction.SRA, 8, 0, 8)]
        [InlineData(Instruction.SRA, 8, 1, 4)]
        [InlineData(Instruction.SRA, 9, 1, 4)]
        [InlineData(Instruction.SRA, -8, 1, -4)]
        [InlineData(Instruction.SRA, -1000, 3, -125)]
        [InlineData(Instruction.SRA, -1000, 4, -63)]
        [InlineData(Instruction.SLT, 1000, 999, 0)]
        [InlineData(Instruction.SLT, 1000, 1000, 0)]
        [InlineData(Instruction.SLT, 1000, 1001, 1)]
        [InlineData(Instruction.SLT, -1, 999, 1)]
        [InlineData(Instruction.SLT, 999, -1, 0)]
        public void Execute_R_Format_X5_X6_X7_arithmetic(Instruction instruction, Int32 x6, Int32 x7, Int32 x5_expected_value)
        {
            UInt32 pc = 0x1000;
            RegisterSet registers = new RegisterSet();
            registers.X6 = (UInt32)x6;
            registers.X7 = (UInt32)x7;

            DecodeInfo info = new DecodeInfo
            {
                Instruction = instruction,
                RD = 5,
                RS1 = 6,
                RS2 = 7
            };

            Executer.Execute(info, registers, null, ref pc);

            Assert.Equal(x5_expected_value, (Int32)registers.X5);
            Assert.Equal((UInt32)0x1004, pc);
        }

        [Theory]
        [InlineData(Instruction.SLL, 0x00000001, 0, 0x00000001)]
        [InlineData(Instruction.SLL, 0x00000001, 1, 0x00000002)]
        [InlineData(Instruction.SLL, 0x00000001, 31, 0x80000000)]
        [InlineData(Instruction.SLL, 0x80000001, 33, 0x00000002)] // Lower 5 bits of RS2 only
        [InlineData(Instruction.SLL, 0xFF000001, 4, 0xF0000010)]
        [InlineData(Instruction.SRL, 0x80000000, 0, 0x80000000)]
        [InlineData(Instruction.SRL, 0x80000000, 1, 0x40000000)]
        [InlineData(Instruction.SRL, 0x40000001, 1, 0x20000000)]
        [InlineData(Instruction.SRL, 0x80000000, 31, 0x00000001)]
        [InlineData(Instruction.OR, 0x55555555, 0x55AA0022, 0x55FF5577)]
        [InlineData(Instruction.AND, 0x55555555, 0x55AA0011, 0x55000011)]
        [InlineData(Instruction.XOR, 0x55555555, 0x55AA0011, 0x00FF5544)]
        [InlineData(Instruction.SLTU, 1000, 999, 0)]
        [InlineData(Instruction.SLTU, 1000, 1000, 0)]
        [InlineData(Instruction.SLTU, 1000, 1001, 1)]
        [InlineData(Instruction.SLTU, 0xFFFFFFFF, 999, 0)]
        [InlineData(Instruction.SLTU, 999, 0xFFFFFFFF, 1)]
        public void Execute_R_Format_X5_X6_X7_logical(Instruction instruction, UInt32 x6, UInt32 x7, UInt32 x5_expected_value)
        {
            UInt32 pc = 0x1000;
            RegisterSet registers = new RegisterSet();
            registers.X6 = x6;
            registers.X7 = x7;

            DecodeInfo info = new DecodeInfo
            {
                Instruction = instruction,
                RD = 5,
                RS1 = 6,
                RS2 = 7
            };

            Executer.Execute(info, registers, null, ref pc);

            Assert.Equal(x5_expected_value, registers.X5);
            Assert.Equal((UInt32)0x1004, pc);
        }

        [Theory]
        [InlineData(Instruction.ADDI, 1000, 1234, 2234)]
        [InlineData(Instruction.ADDI, 1000, -1234, -234)]
        [InlineData(Instruction.ADDI, -1000, -1234, -2234)]
        [InlineData(Instruction.SRAI, -8, 1, -4)]
        [InlineData(Instruction.SRAI, -1000, 3, -125)]
        [InlineData(Instruction.SRAI, -1000, 4, -63)]
        [InlineData(Instruction.SLTI, 1000, 999, 0)]
        [InlineData(Instruction.SLTI, 1000, 1000, 0)]
        [InlineData(Instruction.SLTI, 1000, 1001, 1)]
        [InlineData(Instruction.SLTI, -1, 999, 1)]
        [InlineData(Instruction.SLTI, 999, -1, 0)]
        public void Execute_I_Format_X5_X6_arithmetic(Instruction instruction, Int32 x6, Int32 immediate12, Int32 x5_expected_value)
        {
            UInt32 pc = 0x1000;
            RegisterSet registers = new RegisterSet();
            registers.X6 = (UInt32)x6;

            DecodeInfo info = new DecodeInfo
            {
                Instruction = instruction,
                RD = 5,
                RS1 = 6,
                I_Immediate = (UInt32)immediate12
            };

            Executer.Execute(info, registers, null, ref pc);

            Assert.Equal(x5_expected_value, (Int32)registers.X5);
            Assert.Equal((UInt32)0x1004, pc);
        }

        [Theory]
        [InlineData(Instruction.ANDI, 0x12345678, 0x7FF, 0x00000678)]
        [InlineData(Instruction.ANDI, 0x12345678, 0xFFF, 0x12345678)] // sign extend immediate!
        [InlineData(Instruction.ORI, 0x12345678, 0x7FF, 0x123457FF)]
        [InlineData(Instruction.ORI, 0x12345678, 0xFFF, 0xFFFFFFFF)] // sign extend immediate!
        [InlineData(Instruction.XORI, 0x12345678, 0x7FF, 0x12345187)]
        [InlineData(Instruction.XORI, 0x12345678, 0xFFF, 0xEDCBA987)] // sign extend immediate!
        [InlineData(Instruction.SLLI, 0x00000001, 0, 0x00000001)]
        [InlineData(Instruction.SLLI, 0x00000001, 1, 0x00000002)]
        [InlineData(Instruction.SLLI, 0x00000001, 31, 0x80000000)]
        [InlineData(Instruction.SLLI, 0xFF000001, 4, 0xF0000010)]
        [InlineData(Instruction.SRLI, 0xFF000001, 4, 0x0FF00000)]
        [InlineData(Instruction.SLTIU, 1000, 999, 0)]
        [InlineData(Instruction.SLTIU, 1000, 1000, 0)]
        [InlineData(Instruction.SLTIU, 1000, 1001, 1)]
        [InlineData(Instruction.SLTIU, 0xFFFFFFFF, 999, 0)]
        [InlineData(Instruction.SLTIU, 999, 0xFFF, 1)]
        public void Execute_I_Format_X5_X6_logical(Instruction instruction, UInt32 x6, UInt16 immediate, UInt32 x5_expected_value)
        {
            UInt32 pc = 0x1000;
            RegisterSet registers = new RegisterSet();
            registers.X6 = x6;

            DecodeInfo info = new DecodeInfo
            {
                Instruction = instruction,
                RD = 5,
                RS1 = 6,
                I_Immediate = immediate.SignExtend(11)
            };

            Executer.Execute(info, registers, null, ref pc);

            Assert.Equal(x5_expected_value, registers.X5);
            Assert.Equal((UInt32)0x1004, pc);
        }

        [Theory]
        [InlineData(Instruction.LW, 84, -20, "@0040 78 56 34 12", 0x12345678)]
        [InlineData(Instruction.LBU, 44, 20, "@0040 78 56 34 12", 0x00000078)]
        [InlineData(Instruction.LBU, 44, 20, "@0040 F4 56 34 12", 0x000000F4)]
        [InlineData(Instruction.LB, 44, 20, "@0040 78 56 34 12", 0x00000078)]
        [InlineData(Instruction.LB, 44, 20, "@0040 F4 56 34 12", 0xFFFFFFF4)] // sign extend!
        [InlineData(Instruction.LHU, 44, 20, "@0040 78 56 34 12", 0x00005678)]
        [InlineData(Instruction.LHU, 44, 22, "@0040 78 56 34 12", 0x00001234)]
        [InlineData(Instruction.LH, 44, 20, "@0040 78 56 34 12", 0x00005678)]
        [InlineData(Instruction.LH, 44, 20, "@0040 78 96 34 12", 0xFFFF9678)] // sign extend!
        public void Execute_Load(Instruction instruction, UInt32 x6, Int32 immediate, string memoryInit, UInt32 x5_expected_value)
        {
            UInt32 pc = 0x1000;
            RegisterSet registers = new RegisterSet();
            registers.X6 = x6;

            WordMemory memory = new WordMemory(memoryInit);

            DecodeInfo info = new DecodeInfo
            {
                Instruction = instruction,
                RD = 5,
                RS1 = 6,
                I_Immediate = (UInt32)immediate
            };

            Executer.Execute(info, registers, memory, ref pc);

            Assert.Equal(x5_expected_value.ToString("X8"), registers.X5.ToString("X8"));
            Assert.Equal((UInt32)0x1004, pc);
        }

        [Theory]
        [InlineData(Instruction.SW, 0x12345678, 44, 20, "@0040 AB CD EF 99", "@0040 78 56 34 12")]
        [InlineData(Instruction.SB, 0x12345678, 44, 20, "@0040 AB CD EF 99", "@0040 78 CD EF 99")]
        [InlineData(Instruction.SB, 0x12345678, 44, 21, "@0040 AB CD EF 99", "@0040 AB 78 EF 99")]
        [InlineData(Instruction.SB, 0x12345678, 44, 22, "@0040 AB CD EF 99", "@0040 AB CD 78 99")]
        [InlineData(Instruction.SB, 0x12345678, 44, 23, "@0040 AB CD EF 99", "@0040 AB CD EF 78")]
        [InlineData(Instruction.SH, 0x12345678, 44, 20, "@0040 AB CD EF 99", "@0040 78 56 EF 99")]
        [InlineData(Instruction.SH, 0x12345678, 44, 22, "@0040 AB CD EF 99", "@0040 AB CD 78 56")]
        public void Execute_Store(Instruction instruction, UInt32 x5, UInt32 x6, Int32 immediate, string memoryInit, string memoryExpected)
        {
            UInt32 pc = 0x1000;
            RegisterSet registers = new RegisterSet();
            registers.X5 = x5;
            registers.X6 = x6;

            WordMemory memory = new WordMemory(memoryInit);

            DecodeInfo info = new DecodeInfo
            {
                Instruction = instruction,
                RD = 5,
                RS1 = 6,
                I_Immediate = (UInt32)immediate
            };

            Executer.Execute(info, registers, memory, ref pc);

            Assert.Equal(memoryExpected, memory.Dump(0x0040, 4));
            Assert.Equal((UInt32)0x1004, pc);
        }

        [Theory]
        [InlineData(Instruction.BEQ, 123, 456, 20, 1000, 1004)]
        [InlineData(Instruction.BEQ, 123, 123, 20, 1000, 1040)]
        [InlineData(Instruction.BNE, 123, 456, 20, 1000, 1040)]
        [InlineData(Instruction.BNE, 123, 123, 20, 1000, 1004)]
        [InlineData(Instruction.BLT, 123, 122, 20, 1000, 1004)]
        [InlineData(Instruction.BLT, 123, 123, 20, 1000, 1004)]
        [InlineData(Instruction.BLT, 123, 124, 20, 1000, 1040)]
        [InlineData(Instruction.BLT, 0x88888888, 124, 20, 1000, 1040)] // signed compare
        [InlineData(Instruction.BGE, 123, 122, 20, 1000, 1040)]
        [InlineData(Instruction.BGE, 123, 123, 20, 1000, 1040)]
        [InlineData(Instruction.BGE, 123, 124, 20, 1000, 1004)]
        [InlineData(Instruction.BGE, 0x88888888, 124, 20, 1000, 1004)] // signed compare
        [InlineData(Instruction.BLTU, 123, 122, 20, 1000, 1004)]
        [InlineData(Instruction.BLTU, 123, 123, 20, 1000, 1004)]
        [InlineData(Instruction.BLTU, 123, 124, 20, 1000, 1040)]
        [InlineData(Instruction.BLTU, 0x88888888, 124, 20, 1000, 1004)] // unsigned compare
        [InlineData(Instruction.BGEU, 123, 122, 20, 1000, 1040)]
        [InlineData(Instruction.BGEU, 123, 123, 20, 1000, 1040)]
        [InlineData(Instruction.BGEU, 123, 124, 20, 1000, 1004)]
        [InlineData(Instruction.BGEU, 0x88888888, 124, 20, 1000, 1040)] // unsigned compare
        public void Execute_Branch(Instruction instruction, UInt32 x5, UInt32 x6, Int32 immediate, UInt32 previousPC, UInt32 expectedPC)
        {
            UInt32 pc = previousPC;
            RegisterSet registers = new RegisterSet();
            registers.X5 = x5;
            registers.X6 = x6;

            DecodeInfo info = new DecodeInfo
            {
                Instruction = instruction,
                RS1 = 5,
                RS2 = 6,
                B_Immediate = (UInt32)immediate
            };

            Executer.Execute(info, registers, null, ref pc);

            Assert.Equal(expectedPC, pc);
        }

        [Theory]
        [InlineData(Instruction.LUI, 0x12345, 0xABCDEF99, 0x12345000)]
        public void Execute_Upper_Immediate(Instruction instruction, UInt32 immediate, UInt32 x5, UInt32 expected_x5)
        {
            UInt32 pc = 0x1000;
            RegisterSet registers = new RegisterSet();
            registers.X5 = x5;

            DecodeInfo info = new DecodeInfo
            {
                Instruction = instruction,
                RD = 5,
                U_Immediate = (UInt32)immediate
            };

            Executer.Execute(info, registers, null, ref pc);

            Assert.Equal(expected_x5.ToString("X8"), registers.X5.ToString("X8"));
            Assert.Equal((UInt32)0x1004, pc);
        }

        // --------------------------------------------------------------------
        // TO DO: exceptions when try to read / write misaligned w/h/b address!
        // --------------------------------------------------------------------

    }
}
