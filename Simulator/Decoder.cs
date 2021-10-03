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

        public string ToString(UInt32 pc)
        {
            string text = this.ToString();
            switch (Format)
            {
                case Format.B:
                {
                    UInt32 offset = ((UInt32)B_Immediate << 1);
                    UInt32 destination_pc = pc + offset;
                    text += $" # 0x{destination_pc:X8}";
                    break;
                }
            }
            return text;
        }

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
        public static UInt32 SignExtend12(UInt32 value)
        {
            return ((value & (1 << 11)) != 0 ? value | 0xFFFFF000 : value);
        }

        public static UInt32 SignExtend13(UInt32 value)
        {
            return ((value & (1 << 12)) != 0 ? value | 0xFFFFE000 : value);
        }

        public static UInt32 SignExtend20(UInt32 value)
        {
            return ((value & (1 << 19)) != 0 ? value | 0xFFF00000 : value);
        }

        public static DecodeInfo Decode(UInt32 instruction)
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
                Opcode = (Byte)(bits_6_downto_0),
                RD = (Byte)(bits_11_downto_7),
                Funct3 = (Byte)(bits_14_downto_12),
                Funct7 = (Byte)(bits_31_downto_25),
                RS1 = (Byte)(bits_19_downto_15),
                RS2 = (Byte)(bits_24_downto_20),
                I_Immediate = SignExtend12(bits_31_downto_20),
                S_Immediate = SignExtend12(bits_31_downto_25 << 5 | bits_11_downto_7),
                B_Immediate = SignExtend13(bit_31 << 12 | bit_7 << 11 | bits_30_downto_25 << 5 | bits_11_downto_8 << 1),
                U_Immediate = bits_31_downto_12 << 12,
                J_Immediate = SignExtend20(bit_31 << 20 | bits_19_downto_12 << 12 | bit_20 << 11 | bits_30_downto_21 << 1),
                Instruction = Instruction.UNKNOWN,
                Format = Format.UNKNOWN
            };

            switch (info.Opcode)
            {
                case 0b_0110011:
                {
                    info.Format = Format.R;
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
                    info.Format = Format.I;
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
                    info.Format = Format.I;
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
                    info.Format = Format.S;
                    switch (info.Funct3)
                    {
                        case 0b_000: info.Instruction = Instruction.SB; break;
                        case 0b_001: info.Instruction = Instruction.SH; break;
                        case 0b_010: info.Instruction = Instruction.SW; break;
                    }
                    break;
                case 0b_1100011:
                    info.Format = Format.B;
                    switch (info.Funct3)
                    {
                        case 0b_000: info.Instruction = Instruction.BEQ; break;
                        case 0b_001: info.Instruction = Instruction.BNE; break;
                        case 0b_100: info.Instruction = Instruction.BLT; break;
                        case 0b_101: info.Instruction = Instruction.BGE; break;
                        case 0b_110: info.Instruction = Instruction.BLTU; break;
                        case 0b_111: info.Instruction = Instruction.BGEU; break;
                    }
                    break;
                case 0b_0110111:
                    info.Format = Format.U;
                    switch (info.Funct3)
                    {
                        case 0b_000: info.Instruction = Instruction.LUI; break;
                    }
                    break;
                case 0b_0010111:
                    info.Format = Format.U;
                    switch (info.Funct3)
                    {
                        case 0b_000: info.Instruction = Instruction.AUIPC; break;
                    }
                    break;
                case 0b_1100111:
                    info.Format = Format.I;
                    switch (info.Funct3)
                    {
                        case 0b_000: info.Instruction = Instruction.JALR; break;
                    }
                    break;
                case 0b_1101111:
                    info.Format = Format.J;
                    switch (info.Funct3)
                    {
                        case 0b_000: info.Instruction = Instruction.JAL; break;
                    }
                    break;
            }

            return info;
        }
    }
}
