using System;

namespace Simulator
{
    public static class Executer
    {
        public static void Execute(DecodeInfo info, RegisterSet registers, WordMemory memory, ref UInt32 pc)
        {
            UInt32 nextPC = pc + 4;
            switch (info.Instruction)
            {
                case Instruction.ADD:
                    registers[info.RD] = registers[info.RS1] + registers[info.RS2];
                    break;
                case Instruction.SUB:
                    registers[info.RD] = registers[info.RS1] - registers[info.RS2];
                    break;
                case Instruction.SLL:
                    registers[info.RD] = registers[info.RS1] << ((Byte)registers[info.RS2] & 0x1F);
                    break;
                case Instruction.SRL:
                    registers[info.RD] = registers[info.RS1] >> (Byte)(registers[info.RS2] & 0x1F);
                    break;
                case Instruction.SRA:
                    registers[info.RD] = Logic.ShiftRightArithmetic(registers[info.RS1], (Byte)(registers[info.RS2] & 0x1F));
                    break;
                case Instruction.OR:
                    registers[info.RD] = registers[info.RS1] | registers[info.RS2];
                    break;
                case Instruction.AND:
                    registers[info.RD] = registers[info.RS1] & registers[info.RS2];
                    break;
                case Instruction.XOR:
                    registers[info.RD] = registers[info.RS1] ^ registers[info.RS2];
                    break;
                case Instruction.SLTU:
                    registers[info.RD] = Logic.UnsignedLessThan(registers[info.RS1], registers[info.RS2]) ? 1u : 0;
                    break;
                case Instruction.SLT:
                    registers[info.RD] = Logic.SignedLessThan(registers[info.RS1], registers[info.RS2]) ? 1u : 0;
                    break;
                case Instruction.ADDI:
                    registers[info.RD] = registers[info.RS1] + info.I_Immediate;
                    break;
                case Instruction.ANDI:
                    registers[info.RD] = registers[info.RS1] & info.I_Immediate;
                    break;
                case Instruction.ORI:
                    registers[info.RD] = registers[info.RS1] | info.I_Immediate;
                    break;
                case Instruction.XORI:
                    registers[info.RD] = registers[info.RS1] ^ info.I_Immediate;
                    break;
                 case Instruction.SLLI:
                    registers[info.RD] = registers[info.RS1] << (Byte)(info.I_Immediate & 0x1F);
                    break;
                 case Instruction.SRLI:
                    registers[info.RD] = registers[info.RS1] >> (Byte)(info.I_Immediate & 0x1F);
                    break;
                 case Instruction.SRAI:
                    registers[info.RD] = Logic.ShiftRightArithmetic(registers[info.RS1], (Byte)(info.I_Immediate & 0x1F));
                    break;
                case Instruction.SLTIU:
                    registers[info.RD] = Logic.UnsignedLessThan(registers[info.RS1], info.I_Immediate) ? 1u : 0;
                    break;
                case Instruction.SLTI:
                    registers[info.RD] = Logic.SignedLessThan(registers[info.RS1], info.I_Immediate) ? 1u : 0;
                    break;
                case Instruction.LW:
                {
                    UInt32 address = registers[info.RS1] + info.I_Immediate;
                    UInt32 wordLitteEndian = memory[address >> 2];
                    UInt32 wordBigEndian = Logic.ReverseEndian(wordLitteEndian);
                    registers[info.RD] = wordBigEndian;
                    break;
                }
                case Instruction.LBU:
                {
                    UInt32 address = registers[info.RS1] + info.I_Immediate;
                    UInt32 word = memory[address >> 2];
                    Byte byteOffset = (Byte)(3 - (address & 0x03));
                    Byte bitsOffset = (Byte)(byteOffset << 3);
                    UInt32 b = (word >> bitsOffset) & 0xFF;
                    registers[info.RD] = b;
                    break;
                }
                case Instruction.LB:
                {
                    UInt32 address = registers[info.RS1] + info.I_Immediate;
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
                    UInt32 address = (UInt32)(registers[info.RS1] + info.I_Immediate);
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
                    UInt32 address = (UInt32)(registers[info.RS1] + info.I_Immediate);
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