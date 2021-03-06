using System;

namespace Simulator
{
    public static class Executer
    {
        public static void Execute(DecodeInfo info, RegisterSet registers, IWordMemory memory, ref UInt32 pc)
        {
            UInt32 rd_value = registers[info.RD];
            UInt32 rs1_value = registers[info.RS1];
            UInt32 rs2_value = registers[info.RS2];
            Byte rs2_byte_value = (Byte)rs2_value.Bits(7, 0);
            UInt16 rs2_half_value = (UInt16)rs2_value.Bits(15, 0);
            Byte reg_shift = (Byte)(rs2_value & 0x1F);
            Byte imm_shift = (Byte)(info.I_Immediate & 0x1F);
            UInt32 address = rs1_value + (info.Format == Format.S ? info.S_Immediate : info.I_Immediate);
            UInt32 mem_content = memory != null ? memory.GetWord(address) : 0x00000000;
            Byte byte_shift = (Byte)((3 - (address & 0x03)) << 3);
            Byte half_shift = (Byte)((2 - (address & 0x02)) << 3);
            Byte mem_byte_content = (Byte)(mem_content >> byte_shift & 0xFF);
            UInt16 mem_half_content = (UInt16)(mem_content >> half_shift & 0xFFFF);
            bool willBranch = false;

            switch (info.Instruction)
            {
                case Instruction.ADD:
                    registers[info.RD] = rs1_value + rs2_value;
                    break;
                case Instruction.SUB:
                    registers[info.RD] = rs1_value - rs2_value;
                    break;
                case Instruction.SLL:
                    registers[info.RD] = rs1_value << reg_shift;
                    break;
                case Instruction.SRL:
                    registers[info.RD] = rs1_value >> reg_shift;
                    break;
                case Instruction.SRA:
                    registers[info.RD] = Logic.ShiftRightArithmetic(rs1_value, reg_shift);
                    break;
                case Instruction.OR:
                    registers[info.RD] = rs1_value | rs2_value;
                    break;
                case Instruction.AND:
                    registers[info.RD] = rs1_value & rs2_value;
                    break;
                case Instruction.XOR:
                    registers[info.RD] = rs1_value ^ rs2_value;
                    break;
                case Instruction.SLTU:
                    registers[info.RD] = Logic.UnsignedLessThan(rs1_value, rs2_value) ? 1u : 0;
                    break;
                case Instruction.SLT:
                    registers[info.RD] = Logic.SignedLessThan(rs1_value, rs2_value) ? 1u : 0;
                    break;
                case Instruction.ADDI:
                    registers[info.RD] = rs1_value + info.I_Immediate;
                    break;
                case Instruction.ANDI:
                    registers[info.RD] = rs1_value & info.I_Immediate;
                    break;
                case Instruction.ORI:
                    registers[info.RD] = rs1_value | info.I_Immediate;
                    break;
                case Instruction.XORI:
                    registers[info.RD] = rs1_value ^ info.I_Immediate;
                    break;
                 case Instruction.SLLI:
                    registers[info.RD] = rs1_value << imm_shift;
                    break;
                 case Instruction.SRLI:
                    registers[info.RD] = rs1_value >> imm_shift;
                    break;
                 case Instruction.SRAI:
                    registers[info.RD] = Logic.ShiftRightArithmetic(rs1_value, imm_shift);
                    break;
                case Instruction.SLTIU:
                    registers[info.RD] = Logic.UnsignedLessThan(rs1_value, info.I_Immediate) ? 1u : 0;
                    break;
                case Instruction.SLTI:
                    registers[info.RD] = Logic.SignedLessThan(rs1_value, info.I_Immediate) ? 1u : 0;
                    break;
                case Instruction.LW:
                    registers[info.RD] = mem_content.ReverseEndian();
                    break;
                case Instruction.LBU:
                    registers[info.RD] = mem_byte_content;
                    break;
                case Instruction.LB:
                    registers[info.RD] = mem_byte_content.SignExtend(7);
                    break;
                case Instruction.LHU:
                    registers[info.RD] = mem_half_content.ReverseEndian();
                    break;
                case Instruction.LH:
                    registers[info.RD] = mem_half_content.ReverseEndian().SignExtend(15);
                    break;
                case Instruction.SW:
                    memory.SetWord(address, rs2_value.ReverseEndian());
                    break;
                case Instruction.SB:
                    {
                        UInt32 mask = ~(0xFF000000 >> (24 - byte_shift));
                        memory.SetWord(address, (mem_content & mask) | (UInt32)(rs2_byte_value << byte_shift));
                        break;
                    }
                case Instruction.SH:
                    {
                        UInt32 mask = ~(0xFFFF0000 >> (16 - half_shift));
                        memory.SetWord(address, (mem_content & mask) | (UInt32)(rs2_half_value.ReverseEndian() << (half_shift)));
                        break;
                    }
                case Instruction.BEQ:
                    willBranch = rs1_value == rs2_value;
                    break;
                case Instruction.BNE:
                    willBranch = rs1_value != rs2_value;
                    break;
                case Instruction.BLT:
                    willBranch = Logic.SignedLessThan(rs1_value, rs2_value);
                    break;
                case Instruction.BGE:
                    willBranch = !Logic.SignedLessThan(rs1_value, rs2_value);
                    break;
                case Instruction.BLTU:
                    willBranch = Logic.UnsignedLessThan(rs1_value, rs2_value);
                    break;
                case Instruction.BGEU:
                    willBranch = !Logic.UnsignedLessThan(rs1_value, rs2_value);
                    break;
                case Instruction.LUI:
                    registers[info.RD] = info.U_Immediate << 12;
                    break;
                case Instruction.AUIPC:
                    registers[info.RD] = (UInt32)(info.U_Immediate << 12) + pc;
                    break;
                case Instruction.JAL:
                    registers[info.RD] = pc + 4;
                    willBranch = true;
                    break;
                case Instruction.JALR:
                    registers[info.RD] = pc + 4;
                    willBranch = true;
                    break;
                default:
                    throw new Exception($"Unknown instruction: {info.Instruction}");
            }
            if (willBranch && info.Format == Format.B)
                pc = pc + (info.B_Immediate);
            else if (willBranch && info.Format == Format.J)
                pc = pc + (info.J_Immediate);
            else if (willBranch && info.Format == Format.I)
                pc = rs1_value + info.I_Immediate;
            else
                pc = pc + 4;
        }
    }
}