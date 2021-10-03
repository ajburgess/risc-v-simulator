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
                    UInt32 address = rs1_value + info.I_Immediate;
                    UInt32 wordLitteEndian = memory[address >> 2];
                    UInt32 wordBigEndian = Logic.ReverseEndian(wordLitteEndian);
                    registers[info.RD] = wordBigEndian;
                    break;
                }
                case Instruction.LBU:
                {
                    UInt32 address = rs1_value + info.I_Immediate;
                    UInt32 word = memory[address >> 2];
                    Byte byteOffset = (Byte)(3 - (address & 0x03));
                    Byte bitsOffset = (Byte)(byteOffset << 3);
                    UInt32 b = (word >> bitsOffset) & 0xFF;
                    registers[info.RD] = b;
                    break;
                }
                case Instruction.LB:
                {
                    UInt32 address = rs1_value + info.I_Immediate;
                    UInt32 word = memory[address >> 2];
                    Byte byteOffset = (Byte)(3 - (address & 0x03));
                    Byte bitsOffset = (Byte)(byteOffset << 3);
                    UInt32 b = (word >> bitsOffset) & 0xFF;
                    b = Logic.SignExtend8(b);
                    registers[info.RD] = b;
                    break;
                }
                case Instruction.LHU:
                {
                    UInt32 address = (UInt32)(rs1_value + info.I_Immediate);
                    UInt32 word = memory[address >> 2];
                    Byte halfOffset = (Byte)(2 - (address & 0x02));
                    Byte bitsOffset = (Byte)(halfOffset << 3);
                    UInt16 h = (UInt16)(word >> bitsOffset & 0xffff);
                    h = Logic.ReverseHalfWord(h);
                    registers[info.RD] = h;
                    break;
                }
                case Instruction.LH:
                {
                    UInt32 address = (UInt32)(rs1_value + info.I_Immediate);
                    UInt32 word = memory[address >> 2];
                    UInt16 halfOffset = (UInt16)(2 - (address & 0x02));
                    UInt16 bitsOffset = (UInt16)(halfOffset * 8);
                    UInt16 h = (UInt16)(word >> bitsOffset & 0xffff);
                    h = Logic.ReverseHalfWord(h);
                    registers[info.RD] = Logic.SignExtend16(h);
                    break;
                }
                default:
                    throw new Exception("Unknown instruction");
           }
           pc = nextPC;
        }
    }
}