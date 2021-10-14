using System;
using Xunit;

namespace Simulator.Tests
{
    public class IFormatDecoder_Tests
    {
        [Theory]
        [InlineData(0b_111111111111_11111_111_11111_0000000, 0x00)]
        [InlineData(0b_000000000000_00000_000_00000_1111111, 0x7F)]
        public void Decode_Extracts_Opcode(UInt32 instruction, UInt16 opcode)
        {
            DecodeInfo info = Decoder.Decode(instruction);
            Assert.Equal(opcode, info.Opcode);
        }

        [Theory]
        [InlineData(0b_111111111111_11111_111_00000_1111111, 0x00)]
        [InlineData(0b_000000000000_00000_000_11111_0000000, 0x1F)]
        public void Decode_Extracts_RD(UInt32 instruction, UInt16 rd)
        {
            DecodeInfo info = Decoder.Decode(instruction);
            Assert.Equal(rd, info.RD);
        }

        [Theory]
        [InlineData(0b_111111111111_11111_000_11111_1111111, 0x00)]
        [InlineData(0b_000000000000_00000_111_00000_0000000, 0x07)]
        public void Decode_Extracts_Funct3(UInt32 instruction, UInt16 funct3)
        {
            DecodeInfo info = Decoder.Decode(instruction);
            Assert.Equal(funct3, info.Funct3);
        }

        [Theory]
        [InlineData(0b_111111111111_00000_111_11111_1111111, 0x00)]
        [InlineData(0b_000000000000_11111_000_00000_0000000, 0x1F)]
        public void Decode_Extracts_RS1(UInt32 instruction, UInt16 rs1)
        {
            DecodeInfo info = Decoder.Decode(instruction);
            Assert.Equal(rs1, info.RS1);
        }

        [Theory]
        [InlineData(0b_000000000000_11111_111_11111_1111111, 0x00000000)]
        [InlineData(0b_011111111111_00000_000_00000_0000000, 0x000007FF)]
        [InlineData(0b_000000110010_00000_000_00000_0000000, 50)]
        [InlineData(0b_111111111111_00000_000_00000_0000000, -1)]
        [InlineData(0b_111111001110_00000_000_00000_0000000, -50)]
        public void Decode_Extracts_I_Format_Immediate_WithSignExtension(UInt32 instruction, Int32 immediate)
        {
            DecodeInfo info = Decoder.Decode(instruction);
            Assert.Equal(immediate, (Int32)info.I_Immediate);
        }

        [Theory]
        [InlineData(0b_1111111_00000_11111_111_11111_1111111, 0x00)]
        [InlineData(0b_0000000_11111_00000_000_00000_0000000, 0x1F)]
        public void Decode_Extracts_SR2(UInt32 instruction, UInt16 sr2)
        {
            DecodeInfo info = Decoder.Decode(instruction);
            Assert.Equal(sr2, info.RS2);
        }

        [Theory]
        [InlineData(0b_0000000_11111_11111_111_11111_1111111, 0x00)]
        [InlineData(0b_1111111_00000_00000_000_00000_0000000, 0x7F)]
        public void Decode_Extracts_Funct7(UInt32 instruction, UInt16 funct7)
        {
            DecodeInfo info = Decoder.Decode(instruction);
            Assert.Equal(funct7, info.Funct7);
        }

        [Theory]
        [InlineData(0b_0000000_11111_11111_111_00000_1111111, 0x000)]
        [InlineData(0b_0000000_11111_11111_111_11111_1111111, 0x01F)]
        [InlineData(0b_0000001_11111_11111_111_10010_1111111, 50)]
        [InlineData(0b_1111111_11111_11111_111_11111_1111111, -1)]
        [InlineData(0b_1111110_11111_11111_111_01110_1111111, -50)]
        public void Decode_Extracts_S_Format_Immediate_WithSignExtension(UInt32 instruction, Int32 immediate)
        {
            DecodeInfo info = Decoder.Decode(instruction);
            info.Format = Format.S;
            Assert.Equal(immediate, (Int32)info.S_Immediate);
        }

        [Theory]
        //             [12] [10:5]                 [4:1] [11]
        [InlineData(0b_0____000000_11111_11111_111_0000__0____1111111, 0x000)]
        [InlineData(0b_0____000000_11111_11111_111_0001__0____1111111, 2)]
        [InlineData(0b_1____111111_00000_00000_000_1111__1____0000000, -2)]
        [InlineData(0b_1____000000_11111_11111_111_0000__0____1111111, -4096)]
        [InlineData(0b_0____111111_00000_00000_000_1111__1____0000000, 4094)]
        [InlineData(0b_0____000000_11111_11111_111_0000__1____1111111, 0b_0_1000_0000_0000)]
        [InlineData(0b_0____111111_11111_11111_111_0000__0____1111111, 0b_0_0111_1110_0000)]
        [InlineData(0b_0____000000_11111_11111_111_1111__0____1111111, 0b_0_0000_0001_1110)]
        public void Decode_Extracts_B_Format_Immediate_WithSignExtension(UInt32 instruction, Int32 immediate)
        {
            DecodeInfo info = Decoder.Decode(instruction);
            Assert.Equal(immediate, (Int32)info.B_Immediate);
        }

        [Theory]
        [InlineData(0b_00000000000000000000_11111_1111111, 0x00000000)]
        [InlineData(0b_00000000000000000001_00000_0000000, 0x00000001)]
        [InlineData(0b_00000000000000000010_11111_1111111, 0x00000002)]
        [InlineData(0b_11111111111111111111_00000_0000000, 0x000FFFFF)]
        [InlineData(0b_01111111111111111111_00000_0000000, 0x0007FFFF)]
        public void Decode_Extracts_U_Format_Immediate_NoSignExtension(UInt32 instruction, UInt32 immediate)
        {
            DecodeInfo info = Decoder.Decode(instruction);
            Assert.Equal(immediate, info.U_Immediate);
        }

        [Theory]
        //             20 10:1       11 19:12    
        [InlineData(0b_0__0000000000_0__00000000_11111_1111111, 0)]
        [InlineData(0b_0__0000000001_0__00000000_11111_1111111, 2)]
        public void Decode_Extracts_J_Format_Immediate_WithSignExtension(UInt32 instruction, Int32 immediate)
        {
            DecodeInfo info = Decoder.Decode(instruction);
            Assert.Equal(immediate, (Int32)info.J_Immediate);
        }

        [Theory]
        [InlineData(0b_0000000_00000_00000_000_00000_0110011, Instruction.ADD, Format.R)]
        [InlineData(0b_0100000_00000_00000_000_00000_0110011, Instruction.SUB, Format.R)]
        [InlineData(0b_0000000_00000_00000_001_00000_0110011, Instruction.SLL, Format.R)]
        [InlineData(0b_0000000_00000_00000_010_00000_0110011, Instruction.SLT, Format.R)]
        [InlineData(0b_0000000_00000_00000_011_00000_0110011, Instruction.SLTU, Format.R)]
        [InlineData(0b_0000000_00000_00000_100_00000_0110011, Instruction.XOR, Format.R)]
        [InlineData(0b_0000000_00000_00000_101_00000_0110011, Instruction.SRL, Format.R)]
        [InlineData(0b_0100000_00000_00000_101_00000_0110011, Instruction.SRA, Format.R)]
        [InlineData(0b_0000000_00000_00000_110_00000_0110011, Instruction.OR, Format.R)]
        [InlineData(0b_0000000_00000_00000_111_00000_0110011, Instruction.AND, Format.R)]
        [InlineData(0b_0000000_00000_00000_000_00000_0010011, Instruction.ADDI, Format.I)]
        [InlineData(0b_0000000_00000_00000_010_00000_0010011, Instruction.SLTI, Format.I)]
        [InlineData(0b_0000000_00000_00000_011_00000_0010011, Instruction.SLTIU, Format.I)]
        [InlineData(0b_0000000_00000_00000_100_00000_0010011, Instruction.XORI, Format.I)]
        [InlineData(0b_0000000_00000_00000_110_00000_0010011, Instruction.ORI, Format.I)]
        [InlineData(0b_0000000_00000_00000_111_00000_0010011, Instruction.ANDI, Format.I)]
        [InlineData(0b_0000000_00000_00000_001_00000_0010011, Instruction.SLLI, Format.I)]
        [InlineData(0b_0000000_00000_00000_101_00000_0010011, Instruction.SRLI, Format.I)]
        [InlineData(0b_0100000_00000_00000_101_00000_0010011, Instruction.SRAI, Format.I)]
        [InlineData(0b_0000000_00000_00000_000_00000_0000011, Instruction.LB, Format.I)]
        [InlineData(0b_0000000_00000_00000_001_00000_0000011, Instruction.LH, Format.I)]
        [InlineData(0b_0000000_00000_00000_010_00000_0000011, Instruction.LW, Format.I)]
        [InlineData(0b_0000000_00000_00000_100_00000_0000011, Instruction.LBU, Format.I)]
        [InlineData(0b_0000000_00000_00000_101_00000_0000011, Instruction.LHU, Format.I)]
        [InlineData(0b_0000000_00000_00000_000_00000_0100011, Instruction.SB, Format.S)]
        [InlineData(0b_0000000_00000_00000_001_00000_0100011, Instruction.SH, Format.S)]
        [InlineData(0b_0000000_00000_00000_010_00000_0100011, Instruction.SW, Format.S)]
        [InlineData(0b_0000000_00000_00000_000_00000_1100011, Instruction.BEQ, Format.B)]
        [InlineData(0b_0000000_00000_00000_001_00000_1100011, Instruction.BNE, Format.B)]
        [InlineData(0b_0000000_00000_00000_100_00000_1100011, Instruction.BLT, Format.B)]
        [InlineData(0b_0000000_00000_00000_101_00000_1100011, Instruction.BGE, Format.B)]
        [InlineData(0b_0000000_00000_00000_110_00000_1100011, Instruction.BLTU, Format.B)]
        [InlineData(0b_0000000_00000_00000_111_00000_1100011, Instruction.BGEU, Format.B)]
        [InlineData(0b_0000000_00000_00000_000_00000_0110111, Instruction.LUI, Format.U)]
        [InlineData(0b_0000000_00000_00000_000_00000_0010111, Instruction.AUIPC, Format.U)]
        [InlineData(0b_0000000_00000_00000_000_00000_1100111, Instruction.JALR, Format.I)]
        [InlineData(0b_0000000_00000_00000_000_00000_1101111, Instruction.JAL, Format.J)]
        public void Decode_Identifies_Instruction_and_Format(UInt32 instruction, Instruction opcode, Format format)
        {
            DecodeInfo info = Decoder.Decode(instruction);
            Assert.Equal(opcode, info.Instruction);
            Assert.Equal(format, info.Format);
        }

        [Theory]
        [InlineData(Instruction.ADD, 5, 6, 7, @"^add\s+x5,\s?x6,\s?x7$")]
        public void DecodeInfo_R_Format_ToString(Instruction instruction, Byte rd, Byte rs1, Byte rs2, string expected)
        {
            DecodeInfo info = new DecodeInfo
            {
                Format = Format.R,
                Instruction = instruction,
                RD = rd,
                RS1 = rs1,
                RS2 = rs2
            };
            string text = info.ToString();
            Assert.Matches(expected, text);
        }

        [Theory]
        [InlineData(Instruction.ADDI, 5, 6, 50, @"^addi\s+x5,\s?x6,\s?50$")]
        [InlineData(Instruction.ADDI, 5, 6, -50, @"^addi\s+x5,\s?x6,\s?-50$")]
        public void DecodeInfo_I_Format_ToString(Instruction instruction, Byte rd, Byte rs1, Int32 immediate, string expected)
        {
            DecodeInfo info = new DecodeInfo
            {
                Format = Format.I,
                Instruction = instruction,
                RD = rd,
                RS1 = rs1,
                I_Immediate = (UInt32)immediate
            };
            string text = info.ToString();
            Assert.Matches(expected, text);
        }

        [Theory]
        [InlineData(Instruction.SW, 5, 6, -50, @"^sw\s+x5,\s?\(-50\)x6$")]
        public void DecodeInfo_S_Format_ToString(Instruction instruction, Byte rs2, Byte rs1, Int32 immediate, string expected)
        {
            DecodeInfo info = new DecodeInfo
            {
                Format = Format.S,
                Instruction = instruction,
                RS1 = rs1,
                RS2 = rs2,
                S_Immediate = (UInt32)immediate
            };
            string text = info.ToString();
            Assert.Matches(expected, text);
        }

        [Theory]
        [InlineData(Instruction.BEQ, 5, 6, 16, @"^beq\s+x5,\s?x6,\s?16$")]
        [InlineData(Instruction.BEQ, 5, 6, -16, @"^beq\s+x5,\s?x6,\s?-16$")]
        public void DecodeInfo_B_Format_ToString(Instruction instruction, Byte rd, Byte rs1, Int32 immediate, string expected)
        {
            DecodeInfo info = new DecodeInfo
            {
                Format = Format.B,
                Instruction = instruction,
                RD = rd,
                RS1 = rs1,
                B_Immediate = (UInt32)immediate
            };
            string text = info.ToString();
            Assert.Matches(expected, text);
        }

        [Theory]
        [InlineData(Instruction.LUI, 5, 0x12345, @"^lui\s+x5,\s?0x12345\s+#\s+0x12345000$")]
        public void DecodeInfo_U_Format_ToString(Instruction instruction, Byte rd, UInt32 immediate, string expected)
        {
            DecodeInfo info = new DecodeInfo
            {
                Format = Format.U,
                Instruction = instruction,
                RD = rd,
                U_Immediate = immediate
            };
            string text = info.ToString();
            Assert.Matches(expected, text);
        }

        [Theory]
        [InlineData(Instruction.JAL, 5, 16, @"^jal\s+x5,\s?16$")]
        public void DecodeInfo_J_Format_ToString_No_PC(Instruction instruction, Byte rd, Int32 immediate, string expected)
        {
            DecodeInfo info = new DecodeInfo
            {
                Format = Format.J,
                Instruction = instruction,
                RD = rd,
                J_Immediate = (UInt32)immediate
            };
            string text = info.ToString();
            Assert.Matches(expected, text);
        }
    }
}
