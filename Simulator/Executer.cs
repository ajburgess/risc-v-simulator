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
                    registers[info.RD] = ShiftRightArithmetic(registers[info.RS1], (Byte)(registers[info.RS2] & 0x1F));
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
                    registers[info.RD] = UnsignedLessThan(registers[info.RS1], registers[info.RS2]) ? 1u : 0;
                    break;
                case Instruction.SLT:
                    registers[info.RD] = SignedLessThan(registers[info.RS1], registers[info.RS2]) ? 1u : 0;
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
                    registers[info.RD] = ShiftRightArithmetic(registers[info.RS1], (Byte)(info.I_Immediate & 0x1F));
                    break;
                case Instruction.SLTIU:
                    registers[info.RD] = UnsignedLessThan(registers[info.RS1], info.I_Immediate) ? 1u : 0;
                    break;
                case Instruction.SLTI:
                    registers[info.RD] = SignedLessThan(registers[info.RS1], info.I_Immediate) ? 1u : 0;
                    break;
                case Instruction.LW:
                {
                    UInt32 address = registers[info.RS1] + info.I_Immediate;
                    UInt32 wordLitteEndian = memory[address >> 2];
                    UInt32 wordBigEndian = ReverseEndian(wordLitteEndian);
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
                    b = SignExtend8(b);
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
                    h = ReverseHalfWord(h);
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
                    h = ReverseHalfWord(h);
                    registers[info.RD] = SignExtend16(h);
                    break;
                }
                default:
                    throw new Exception("Unknown instruction");
           }
           pc = nextPC;
        }

        public static bool UnsignedLessThan(UInt32 a, UInt32 b)
        {
            return a < b;
        }

        public static bool SignedLessThan(UInt32 a, UInt32 b)
        {
            return (Int32)a < (Int32)b;
        }

        public static UInt32 SignExtend8(UInt32 value)
        {
            return (value & (1 << 7)) != 0 ? value | 0xFFFFFF00 : value;
        }

        public static UInt32 SignExtend16(UInt32 value)
        {
            return (value & (1 << 15)) != 0 ? value | 0xFFFF0000 : value;
        }

        private static UInt32 ReverseEndian(UInt32 word)
        {
            UInt32 newWord = 0x00000000;
            for (int n = 0; n < 4; n++)
            {
                Byte b = (Byte)word;
                newWord = newWord << 8 | b;
                word = word >> 8;
            }
            return newWord;
        }

        private static UInt16 ReverseHalfWord(UInt16 halfWord)
        {
            UInt16 newHalfWord = 0x0000;
            for (int n = 0; n < 2; n++)
            {
                Byte b = (Byte)halfWord;
                newHalfWord = (UInt16)(newHalfWord << 8 | b);
                halfWord = (UInt16)(halfWord >> 8);
            }
            return newHalfWord;
        }

        private static UInt32 ShiftRightArithmetic(UInt32 value, Byte amount)
        {
            UInt32 msb = value & 0x80000000;
            for (Byte i = 0; i < amount; i++)
            {
                value = msb | (value >> 1);
            }
            return value;
        }
    }
}