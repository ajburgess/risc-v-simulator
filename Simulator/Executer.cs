using System;

namespace Simulator
{
    public static class Executer
    {
        public static void Execute(DecodeInfo info, RegisterSet registers, WordMemory memory, ref UInt32 pc)
        {
            UInt32 nextPC = pc + 4;
            UInt32 rs1_value = registers[info.RS1];
            UInt32 rs2_value = registers[info.RS2];
            Byte reg_shift = (Byte)(rs2_value & 0x1F);
            Byte imm_shift = (Byte)(info.I_Immediate & 0x1F);
            UInt32 address = rs1_value + info.I_Immediate;
            UInt32 mem_content = memory != null ? memory[address >> 2] : 0x00000000;
            Byte byte_shift = (Byte)((3 - (address & 0x03)) << 3);
            Byte half_shift = (Byte)((2 - (address & 0x02)) << 3);
            Byte mem_byte_content = (Byte)(mem_content >> byte_shift & 0xFF);
            UInt16 mem_half_content = (UInt16)(mem_content >> half_shift & 0xFFFF);

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
                {
                    registers[info.RD] = Logic.ReverseEndian(mem_content);
                    break;
                }
                case Instruction.LBU:
                {
                    registers[info.RD] = mem_byte_content;
                    break;
                }
                case Instruction.LB:
                {
                    registers[info.RD] = mem_byte_content.SignExtend(7);
                    break;
                }
                case Instruction.LHU:
                {
                    registers[info.RD] = Logic.ReverseEndian(mem_half_content);
                    break;
                }
                case Instruction.LH:
                {
                    registers[info.RD] = Logic.ReverseEndian(mem_half_content).SignExtend(15);
                    break;
                }
                default:
                    throw new Exception("Unknown instruction");
           }
           pc = nextPC;
        }
    }
}