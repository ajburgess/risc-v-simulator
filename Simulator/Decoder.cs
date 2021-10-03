using System;

namespace Simulator
{
    public enum Format : Byte
    {
        UNKNOWN, R, I, S, B, U, J
    }

    public enum Instruction : Byte
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
        public Byte Opcode { get; set; } // 7 bits
        public Byte Funct3 { get; set; } // 3 bits
        public Byte Funct7 { get; set; } // 7 bits
        public Byte RD { get; set; } // 5 bits
        public Byte RS1 { get; set; } // 5 bits
        public Byte RS2 { get; set; } // 5 bits
        public UInt32 I_Immediate { get; set; } // 12 bits
        public UInt32 S_Immediate { get; set; } // 12 bits
        public UInt32 B_Immediate { get; set; } // 13 bits
        public UInt32 U_Immediate { get; set; } // 20 bits
        public UInt32 J_Immediate { get; set; } // 21 bits
        public Instruction Instruction { get; set; }
        public Format Format { get; set; }

        public override string ToString()
        {
            string instruction = Instruction.ToString().ToLower();

            switch (Format)
            {
                case Format.R:
                    return $"{instruction} x{RD},x{RS1},x{RS2}";
                case Format.I:
                    return $"{instruction} x{RD},x{RS1},{(Int32)I_Immediate}";
                case Format.S:
                    return $"{instruction} x{RD},({(Int32)S_Immediate})x{RS1}";
                case Format.B:
                    return $"{instruction} x{RD},x{RS1},{(Int32)B_Immediate * 2}";
                case Format.U:
                    return $"{instruction} x{RD},0x{(U_Immediate >> 12):X5} # 0x{U_Immediate:X8}";
                case Format.J:
                    return $"{instruction} x{RD},{(Int32)J_Immediate * 2}";
                default:
                    return "";
            }
        }
    }

    public static class Decoder
    {
        public static DecodeInfo Decode(UInt32 instruction)
        {
            UInt32 bit_31 = instruction.Bit(31);
            UInt32 bits_31_downto_25 = instruction.Bits(31, 25);
            UInt32 bits_31_downto_20 = instruction.Bits(31, 20);
            UInt32 bits_31_downto_12 = instruction.Bits(31, 12);
            UInt32 bits_30_downto_25 = instruction.Bits(30, 25);
            UInt32 bits_30_downto_21 = instruction.Bits(30, 21);
            UInt32 bits_24_downto_20 = instruction.Bits(24, 20);
            UInt32 bit_20 = instruction.Bit(20);
            UInt32 bits_19_downto_15 = instruction.Bits(19, 15);
            UInt32 bits_19_downto_12 = instruction.Bits(19, 12);
            UInt32 bits_11_downto_8 = instruction.Bits(11, 8);
            UInt32 bits_11_downto_7 = instruction.Bits(11, 7);
            UInt32 bit_7 = instruction.Bit(7);

            Byte opcode = (Byte)instruction.Bits(6, 0);
            Byte funct3 = (Byte)instruction.Bits(14, 12);
            Byte funct7 = (Byte)(bits_31_downto_25);
            UInt16 funct7_funct3 = (UInt16)(funct7 << 3 | funct3);

            Instruction type = Instruction.UNKNOWN;
            Format format = Format.UNKNOWN;

            switch (opcode)
            {
                case 0b_0110011:
                {
                    format = Format.R;
                    switch (funct7_funct3)
                    {
                        case 0b_0000000_000: type = Instruction.ADD; break;
                        case 0b_0100000_000: type = Instruction.SUB; break;
                        case 0b_0000000_001: type = Instruction.SLL; break;
                        case 0b_0000000_010: type = Instruction.SLT; break;
                        case 0b_0000000_011: type = Instruction.SLTU; break;
                        case 0b_0000000_100: type = Instruction.XOR; break;
                        case 0b_0000000_101: type = Instruction.SRL; break;
                        case 0b_0100000_101: type = Instruction.SRA; break;
                        case 0b_0000000_110: type = Instruction.OR; break;
                        case 0b_0000000_111: type = Instruction.AND; break;
                    }
                    break;
                }
                case 0b_0010011:
                {
                    format = Format.I;
                    switch (funct3)
                    {
                        case 0b_000: type = Instruction.ADDI; break;
                        case 0b_010: type = Instruction.SLTI; break;
                        case 0b_011: type = Instruction.SLTIU; break;
                        case 0b_100: type = Instruction.XORI; break;
                        case 0b_110: type = Instruction.ORI; break;
                        case 0b_111: type = Instruction.ANDI; break;
                        default:
                            switch (funct7_funct3)
                            {
                                case 0b_0000000_001: type = Instruction.SLLI; break;
                                case 0b_0000000_101: type = Instruction.SRLI; break;
                                case 0b_0100000_101: type = Instruction.SRAI; break;
                            }
                            break;
                    }
                    break;
                }
                case 0b_0000011:
                    format = Format.I;
                    switch (funct3)
                    {
                        case 0b_000: type = Instruction.LB; break;
                        case 0b_010: type = Instruction.LH; break;
                        case 0b_011: type = Instruction.LW; break;
                        case 0b_100: type = Instruction.LBU; break;
                        case 0b_110: type = Instruction.LHU; break;
                    }
                    break;
                case 0b_0100011:
                    format = Format.S;
                    switch (funct3)
                    {
                        case 0b_000: type = Instruction.SB; break;
                        case 0b_001: type = Instruction.SH; break;
                        case 0b_010: type = Instruction.SW; break;
                    }
                    break;
                case 0b_1100011:
                    format = Format.B;
                    switch (funct3)
                    {
                        case 0b_000: type = Instruction.BEQ; break;
                        case 0b_001: type = Instruction.BNE; break;
                        case 0b_100: type = Instruction.BLT; break;
                        case 0b_101: type = Instruction.BGE; break;
                        case 0b_110: type = Instruction.BLTU; break;
                        case 0b_111: type = Instruction.BGEU; break;
                    }
                    break;
                case 0b_0110111:
                    format = Format.U;
                    switch (funct3)
                    {
                        case 0b_000: type = Instruction.LUI; break;
                    }
                    break;
                case 0b_0010111:
                    format = Format.U;
                    switch (funct3)
                    {
                        case 0b_000: type = Instruction.AUIPC; break;
                    }
                    break;
                case 0b_1100111:
                    format = Format.I;
                    switch (funct3)
                    {
                        case 0b_000: type = Instruction.JALR; break;
                    }
                    break;
                case 0b_1101111:
                    format = Format.J;
                    switch (funct3)
                    {
                        case 0b_000: type = Instruction.JAL; break;
                    }
                    break;
            }

            DecodeInfo info = new DecodeInfo
            {
                Opcode = opcode,
                RD = (Byte)(bits_11_downto_7),
                Funct3 = funct3,
                Funct7 = funct7,
                RS1 = (Byte)(bits_19_downto_15),
                RS2 = (Byte)(bits_24_downto_20),
                I_Immediate = Logic.SignExtend12(bits_31_downto_20),
                S_Immediate = Logic.SignExtend12(bits_31_downto_25 << 5 | bits_11_downto_7),
                B_Immediate = Logic.SignExtend13(bit_31 << 12 | bit_7 << 11 | bits_30_downto_25 << 5 | bits_11_downto_8 << 1),
                U_Immediate = bits_31_downto_12 << 12,
                J_Immediate = Logic.SignExtend20(bit_31 << 20 | bits_19_downto_12 << 12 | bit_20 << 11 | bits_30_downto_21 << 1),
                Instruction = type,
                Format = format
            };

            return info;
        }
    }
}
