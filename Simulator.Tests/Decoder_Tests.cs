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
            Decoder decoder = new Decoder();
            DecodeInfo info = decoder.Decode(instruction);
            Assert.Equal(opcode, info.Opcode);
        }

        [Theory]
        [InlineData(0b_111111111111_11111_111_00000_1111111, 0x00)]
        [InlineData(0b_000000000000_00000_000_11111_0000000, 0x1F)]
        public void Decode_Extracts_RD(UInt32 instruction, UInt16 rd)
        {
            Decoder decoder = new Decoder();
            DecodeInfo info = decoder.Decode(instruction);
            Assert.Equal(rd, info.RD);
        }

        [Theory]
        [InlineData(0b_111111111111_11111_000_11111_1111111, 0x00)]
        [InlineData(0b_000000000000_00000_111_00000_0000000, 0x07)]
        public void Decode_Extracts_Funct3(UInt32 instruction, UInt16 funct3)
        {
            Decoder decoder = new Decoder();
            DecodeInfo info = decoder.Decode(instruction);
            Assert.Equal(funct3, info.Funct3);
        }

        [Theory]
        [InlineData(0b_111111111111_00000_111_11111_1111111, 0x00)]
        [InlineData(0b_000000000000_11111_000_00000_0000000, 0x1F)]
        public void Decode_Extracts_RS1(UInt32 instruction, UInt16 rs1)
        {
            Decoder decoder = new Decoder();
            DecodeInfo info = decoder.Decode(instruction);
            Assert.Equal(rs1, info.RS1);
        }

        [Theory]
        [InlineData(0b_000000000000_11111_111_11111_1111111, 0x00000000)]
        [InlineData(0b_011111111111_00000_000_00000_0000000, 0x000007FF)]
        [InlineData(0b_000000110010_00000_000_00000_0000000, 50)]
        [InlineData(0b_111111111111_00000_000_00000_0000000, -1)]
        [InlineData(0b_111111001110_00000_000_00000_0000000, -50)]
        public void Decode_I_Format_Extracts_ImmediateValue_WithSignExtension(UInt32 instruction, Int32 immediate)
        {
            Decoder decoder = new Decoder();
            DecodeInfo info = decoder.Decode(instruction);
            Assert.Equal(immediate, info.I_SignedImmediate);
        }

        [Theory]
        [InlineData(0b_1111111_00000_11111_111_11111_1111111, 0x00)]
        [InlineData(0b_0000000_11111_00000_000_00000_0000000, 0x1F)]
        public void Decode_R_Format_Extracts_SR2(UInt32 instruction, UInt16 sr2)
        {
            Decoder decoder = new Decoder();
            DecodeInfo info = decoder.Decode(instruction);
            Assert.Equal(sr2, info.RS2);
        }

        [Theory]
        [InlineData(0b_0000000_11111_11111_111_11111_1111111, 0x00)]
        [InlineData(0b_1111111_00000_00000_000_00000_0000000, 0x7F)]
        public void Decode_R_Format_Extracts_Funct7(UInt32 instruction, UInt16 funct7)
        {
            Decoder decoder = new Decoder();
            DecodeInfo info = decoder.Decode(instruction);
            Assert.Equal(funct7, info.Funct7);
        }

        [Theory]
        [InlineData(0b_0000000_11111_11111_111_00000_1111111, 0x000)]
        [InlineData(0b_0000000_11111_11111_111_11111_1111111, 0x01F)]
        [InlineData(0b_0000001_11111_11111_111_10010_1111111, 50)]
        [InlineData(0b_1111111_11111_11111_111_11111_1111111, -1)]
        [InlineData(0b_1111110_11111_11111_111_01110_1111111, -50)]
        public void Decode_S_Format_Extracts_Immediate_WithSignExtension(UInt32 instruction, Int32 immediate)
        {
            Decoder decoder = new Decoder();
            DecodeInfo info = decoder.Decode(instruction);
            Assert.Equal(immediate, info.S_SignedImmediate);
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
        public void Decode_B_Format_Extracts_Immediate_WithSignExtension(UInt32 instruction, Int32 immediate)
        {
            Decoder decoder = new Decoder();
            DecodeInfo info = decoder.Decode(instruction);
            Assert.Equal(immediate, info.B_SignedImmediate);
        }

        [Theory]
        [InlineData(0b_00000000000000000000_11111_1111111, 0x0000)]
        [InlineData(0b_00000000000000000001_00000_0000000, 0x1000)]
        [InlineData(0b_00000000000000000010_11111_1111111, 0x2000)]
        [InlineData(0b_11111111111111111111_00000_0000000, 0xFFFFF000)]
        public void Decode_U_Format_Extracts_ImmediateValue(UInt32 instruction, UInt32 immediate)
        {
            Decoder decoder = new Decoder();
            DecodeInfo info = decoder.Decode(instruction);
            Assert.Equal(immediate, info.U_UnsignedImmediate);
        }

        [Theory]
        //             20 10:1       11 19:12    
        [InlineData(0b_0__0000000000_0__00000000_11111_1111111, 0)]
        [InlineData(0b_0__0000000001_0__00000000_11111_1111111, 2)]
        public void Decode_J_Format_Extracts_Immediate(UInt32 instruction, Int32 immediate)
        {
            Decoder decoder = new Decoder();
            DecodeInfo info = decoder.Decode(instruction);
            Assert.Equal(immediate, info.J_SignedImmediate);
        }

        [Theory]
        [InlineData(0b_0000000_00000_00000_000_00000_0110011, Instruction.ADD)]
        [InlineData(0b_0100000_00000_00000_000_00000_0110011, Instruction.SUB)]
        [InlineData(0b_0000000_00000_00000_001_00000_0110011, Instruction.SLL)]
        [InlineData(0b_0000000_00000_00000_010_00000_0110011, Instruction.SLT)]
        [InlineData(0b_0000000_00000_00000_011_00000_0110011, Instruction.SLTU)]
        [InlineData(0b_0000000_00000_00000_100_00000_0110011, Instruction.XOR)]
        [InlineData(0b_0000000_00000_00000_101_00000_0110011, Instruction.SRL)]
        [InlineData(0b_0100000_00000_00000_101_00000_0110011, Instruction.SRA)]
        [InlineData(0b_0000000_00000_00000_110_00000_0110011, Instruction.OR)]
        [InlineData(0b_0000000_00000_00000_111_00000_0110011, Instruction.AND)]
        [InlineData(0b_0000000_00000_00000_000_00000_0010011, Instruction.ADDI)]
        [InlineData(0b_0000000_00000_00000_010_00000_0010011, Instruction.SLTI)]
        [InlineData(0b_0000000_00000_00000_011_00000_0010011, Instruction.SLTIU)]
        [InlineData(0b_0000000_00000_00000_100_00000_0010011, Instruction.XORI)]
        [InlineData(0b_0000000_00000_00000_110_00000_0010011, Instruction.ORI)]
        [InlineData(0b_0000000_00000_00000_111_00000_0010011, Instruction.ANDI)]
        [InlineData(0b_0000000_00000_00000_001_00000_0010011, Instruction.SLLI)]
        [InlineData(0b_0000000_00000_00000_101_00000_0010011, Instruction.SRLI)]
        [InlineData(0b_0100000_00000_00000_101_00000_0010011, Instruction.SRAI)]
        [InlineData(0b_0000000_00000_00000_000_00000_0000011, Instruction.LB)]
        [InlineData(0b_0000000_00000_00000_010_00000_0000011, Instruction.LH)]
        [InlineData(0b_0000000_00000_00000_011_00000_0000011, Instruction.LW)]
        [InlineData(0b_0000000_00000_00000_100_00000_0000011, Instruction.LBU)]
        [InlineData(0b_0000000_00000_00000_110_00000_0000011, Instruction.LHU)]
        [InlineData(0b_0000000_00000_00000_000_00000_0100011, Instruction.SB)]
        [InlineData(0b_0000000_00000_00000_001_00000_0100011, Instruction.SH)]
        [InlineData(0b_0000000_00000_00000_010_00000_0100011, Instruction.SW)]
        [InlineData(0b_0000000_00000_00000_000_00000_1100011, Instruction.BEQ)]
        [InlineData(0b_0000000_00000_00000_001_00000_1100011, Instruction.BNE)]
        [InlineData(0b_0000000_00000_00000_100_00000_1100011, Instruction.BLT)]
        [InlineData(0b_0000000_00000_00000_101_00000_1100011, Instruction.BGE)]
        [InlineData(0b_0000000_00000_00000_110_00000_1100011, Instruction.BLTU)]
        [InlineData(0b_0000000_00000_00000_111_00000_1100011, Instruction.BGEU)]
        // [InlineData(0b_0000000_00000_00000_000_00000_0110111, Instruction.LUI)]
        // [InlineData(0b_0000000_00000_00000_000_00000_0010111, Instruction.AUIPC)]
        // [InlineData(0b_0000000_00000_00000_000_00000_1100111, Instruction.JALR)]
        // [InlineData(0b_0000000_00000_00000_000_00000_1101111, Instruction.JAL)]
        public void Decode_Identifies_Instruction(UInt32 word, Instruction instruction)
        {
            Decoder decoder = new Decoder();
            DecodeInfo info = decoder.Decode(word);
            Assert.Equal(instruction, info.Instruction);
        }
    }
}
