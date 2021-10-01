using System;

namespace Simulator
{
    public enum Instruction
    {
        UNKNOWN,
        NOP,
        BEQ,
        BNE,
        BLT,
        BGE,
        BLTU,
        BGEU,
        JALR,
        JAL,
        LUI,
        AUIPC,
        ADDI,
        SLTI,
        SLTIU,
        XORI,
        ORI,
        ANDI,
        SLLI,
        SRLI,
        SRAI,
        ADD,
        SUB,
        SLL,
        SLT,
        SLTU,
        XOR,
        SRL,
        SRA,
        OR,
        AND,
        LB,
        LH,
        LW,
        LBU,
        LHU,
        SB,
        SH,
        SW
    }

    public class DecodeInfo
    {
        public UInt16 Opcode { get; set; }
        public UInt16 Funct3 { get; set; }
        public UInt16 Funct7 { get; set; }
        public UInt16 RD { get; set; }
        public UInt16 RS1 { get; set; }
        public UInt16 RS2 { get; set; }
        public Int32 I_SignedImmediate { get; set; }
        public Int32 S_SignedImmediate { get; set; }
        public Int32 B_SignedImmediate { get; set; }
        public Int32 J_SignedImmediate { get; set; }
        public UInt32 U_UnsignedImmediate { get; set; }
        public Instruction Instruction { get; set; }
    }

    public class Decoder
    {
        private Int32 SignExtend12(UInt32 value)
        {
            return (Int32)((value & (1 << 11)) != 0 ? value | 0xFFFFF000 : value);
        }

        private Int32 SignExtend13(UInt32 value)
        {
            return (Int32)((value & (1 << 12)) != 0 ? value | 0xFFFFE000 : value);
        }

        private Int32 SignExtend20(UInt32 value)
        {
            return (Int32)((value & (1 << 19)) != 0 ? value | 0xFFF00000 : value);
        }

        public DecodeInfo Decode(UInt32 instruction)
        {
            UInt32 bit_31 = (instruction >> 31) & 0b_1;
            UInt32 bits_31_downto_25 = (instruction >> 25) & 0b_111_1111;
            UInt32 bits_31_downto_20 = (instruction >> 20) & 0b_1111_1111_1111;
            UInt32 bits_31_downto_12 = (instruction >> 12) & 0b_1111_1111_1111_1111_1111;
            UInt32 bits_30_downto_25 = (instruction >> 25) & 0b_11_1111;
            UInt32 bits_30_downto_21 = (instruction >> 21) & 0b_11_1111_1111;
            UInt32 bits_24_downto_20 = (instruction >> 20) & 0b_1_1111;
            UInt32 bit_20 = (instruction >> 20) & 0b_1;
            UInt32 bits_19_downto_15 = (instruction >> 15) & 0b_1_1111;
            UInt32 bits_19_downto_12 = (instruction >> 12) & 0b_1111_1111;
            UInt32 bits_14_downto_12 = (instruction >> 12) & 0b_111;
            UInt32 bits_11_downto_8 = (instruction >> 8) & 0b_1111;
            UInt32 bits_11_downto_7 = (instruction >> 7) & 0b_1_1111;
            UInt32 bit_7 = (instruction >> 7) & 0b_1;
            UInt32 bits_6_downto_0 = instruction & 0b_111_1111;
            
            DecodeInfo info = new DecodeInfo
            {
                Opcode = (UInt16)(bits_6_downto_0),
                RD = (UInt16)(bits_11_downto_7),
                Funct3 = (UInt16)(bits_14_downto_12),
                Funct7 = (UInt16)(bits_31_downto_25),
                RS1 = (UInt16)(bits_19_downto_15),
                RS2 = (UInt16)(bits_24_downto_20),
                I_SignedImmediate = SignExtend12(bits_31_downto_20),
                S_SignedImmediate = SignExtend12(bits_31_downto_25 << 5 | bits_11_downto_7),
                B_SignedImmediate = SignExtend13(bit_31 << 12 | bit_7 << 11 | bits_30_downto_25 << 5 | bits_11_downto_8 << 1),
                U_UnsignedImmediate = bits_31_downto_12 << 12,
                J_SignedImmediate = SignExtend20(bit_31 << 20 | bits_19_downto_12 << 12 | bit_20 << 11 | bits_30_downto_21 << 1),
                Instruction = Instruction.UNKNOWN
            };

            switch (info.Opcode)
            {
                case 0b_0110011:
                {
                    switch (info.Funct7 << 3 | info.Funct3)
                    {
                        case 0b_0000000_000: info.Instruction = Instruction.ADD; break;
                        case 0b_0100000_000: info.Instruction = Instruction.SUB; break;
                        case 0b_0000000_001: info.Instruction = Instruction.SLL; break;
                        case 0b_0000000_010: info.Instruction = Instruction.SLT; break;
                        case 0b_0000000_011: info.Instruction = Instruction.SLTU; break;
                        case 0b_0000000_100: info.Instruction = Instruction.XOR; break;
                        case 0b_0000000_101: info.Instruction = Instruction.SRL; break;
                        case 0b_0100000_101: info.Instruction = Instruction.SRA; break;
                        case 0b_0000000_110: info.Instruction = Instruction.OR; break;
                        case 0b_0000000_111: info.Instruction = Instruction.AND; break;
                    }
                    break;
                }
                case 0b_0010011:
                {
                    switch (info.Funct3)
                    {
                        case 0b_000: info.Instruction = Instruction.ADDI; break;
                        case 0b_010: info.Instruction = Instruction.SLTI; break;
                        case 0b_011: info.Instruction = Instruction.SLTIU; break;
                        case 0b_100: info.Instruction = Instruction.XORI; break;
                        case 0b_110: info.Instruction = Instruction.ORI; break;
                        case 0b_111: info.Instruction = Instruction.ANDI; break;
                        default:
                            switch (info.Funct7 << 3 | info.Funct3)
                            {
                                case 0b_0000000_001: info.Instruction = Instruction.SLLI; break;
                                case 0b_0000000_101: info.Instruction = Instruction.SRLI; break;
                                case 0b_0100000_101: info.Instruction = Instruction.SRAI; break;
                            }
                            break;
                    }
                    break;
                }
                case 0b_0000011:
                    switch (info.Funct3)
                    {
                        case 0b_000: info.Instruction = Instruction.LB; break;
                        case 0b_010: info.Instruction = Instruction.LH; break;
                        case 0b_011: info.Instruction = Instruction.LW; break;
                        case 0b_100: info.Instruction = Instruction.LBU; break;
                        case 0b_110: info.Instruction = Instruction.LHU; break;
                    }
                    break;
                case 0b_0100011:
                    switch (info.Funct3)
                    {
                        case 0b_000: info.Instruction = Instruction.SB; break;
                        case 0b_001: info.Instruction = Instruction.SH; break;
                        case 0b_010: info.Instruction = Instruction.SW; break;
                    }
                    break;
            }

            return info;
        }
    }
}
