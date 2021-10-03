using System;

namespace Simulator
{
    public static class Executer
    {
        public static void Execute(DecodeInfo info, RegisterSet registerSet, WordMemory memory, ref UInt32 pc)
        {
            UInt32 nextPC = pc + 4;
            switch (info.Instruction)
            {
                case Instruction.ADD:
                    registerSet[info.RD] = registerSet[info.RS1] + registerSet[info.RS2];
                    break;
                case Instruction.SUB:
                    registerSet[info.RD] = registerSet[info.RS1] - registerSet[info.RS2];
                    break;
                case Instruction.SLL:
                    registerSet[info.RD] = registerSet[info.RS1] << (UInt16)(registerSet[info.RS2] & 0x1F);
                    break;
                case Instruction.SRL:
                    registerSet[info.RD] = registerSet[info.RS1] >> (UInt16)(registerSet[info.RS2] & 0x1F);
                    break;
                case Instruction.SRA:
                    registerSet[info.RD] = ShiftRightArithmetic(registerSet[info.RS1], (UInt16)(registerSet[info.RS2] & 0x1F));
                    break;
                case Instruction.OR:
                    registerSet[info.RD] = registerSet[info.RS1] | registerSet[info.RS2];
                    break;
                case Instruction.AND:
                    registerSet[info.RD] = registerSet[info.RS1] & registerSet[info.RS2];
                    break;
                case Instruction.XOR:
                    registerSet[info.RD] = registerSet[info.RS1] ^ registerSet[info.RS2];
                    break;
                case Instruction.SLTU:
                    registerSet[info.RD] = registerSet[info.RS1] < registerSet[info.RS2] ? 1u : 0;
                    break;
                case Instruction.SLT:
                    registerSet[info.RD] = (Int32)registerSet[info.RS1] < (Int32)registerSet[info.RS2] ? 1u : 0;
                    break;
                case Instruction.ADDI:
                    registerSet[info.RD] = (UInt32)((Int32)registerSet[info.RS1] + info.I_Immediate);
                    break;
                case Instruction.ANDI:
                    registerSet[info.RD] = registerSet[info.RS1] & (UInt32)info.I_Immediate;
                    break;
                case Instruction.ORI:
                    registerSet[info.RD] = registerSet[info.RS1] | (UInt32)info.I_Immediate;
                    break;
                case Instruction.XORI:
                    registerSet[info.RD] = registerSet[info.RS1] ^ (UInt32)info.I_Immediate;
                    break;
                 case Instruction.SLLI:
                    registerSet[info.RD] = registerSet[info.RS1] << (UInt16)(info.I_Immediate & 0x1F);
                    break;
                 case Instruction.SRLI:
                    registerSet[info.RD] = registerSet[info.RS1] >> (UInt16)(info.I_Immediate & 0x1F);
                    break;
                 case Instruction.SRAI:
                    registerSet[info.RD] = ShiftRightArithmetic(registerSet[info.RS1], (UInt16)(info.I_Immediate & 0x1F));
                    break;
                case Instruction.SLTIU:
                    registerSet[info.RD] = registerSet[info.RS1] < (UInt32)info.I_Immediate ? 1u : 0;
                    break;
                case Instruction.SLTI:
                    registerSet[info.RD] = (Int32)registerSet[info.RS1] < info.I_Immediate ? 1u : 0;
                    break;
                case Instruction.LW:
                {
                    UInt32 address = (UInt32)(registerSet[info.RS1] + info.I_Immediate);
                    UInt32 wordLitteEndian = memory[address >> 2];
                    UInt32 wordBigEndian = ReverseEndian(wordLitteEndian);
                    registerSet[info.RD] = wordBigEndian;
                    break;
                }
                case Instruction.LBU:
                {
                    UInt32 address = (UInt32)(registerSet[info.RS1] + info.I_Immediate);
                    UInt32 word = memory[address >> 2];
                    UInt16 byteOffset = (UInt16)(3 - (address & 0x03));
                    UInt16 bitsOffset = (UInt16)(byteOffset * 8);
                    UInt32 b = (word >> bitsOffset) & 0xff;
                    registerSet[info.RD] = b;
                    break;
                }
                case Instruction.LB:
                {
                    UInt32 address = (UInt32)(registerSet[info.RS1] + info.I_Immediate);
                    UInt32 word = memory[address >> 2];
                    UInt16 byteOffset = (UInt16)(3 - (address & 0x03));
                    UInt16 bitsOffset = (UInt16)(byteOffset * 8);
                    UInt32 b = (word >> bitsOffset) & 0xff;
                    b = (UInt32)SignExtend8(b);
                    registerSet[info.RD] = b;
                    break;
                }
                case Instruction.LHU:
                {
                    UInt32 address = (UInt32)(registerSet[info.RS1] + info.I_Immediate);
                    UInt32 word = memory[address >> 2];
                    UInt16 halfOffset = (UInt16)(2 - (address & 0x02));
                    UInt16 bitsOffset = (UInt16)(halfOffset * 8);
                    UInt32 h = (word >> bitsOffset) & 0xffff;
                    h = ReverseHalfWord(h);
                    registerSet[info.RD] = h;
                    break;
                }
                case Instruction.LH:
                {
                    UInt32 address = (UInt32)(registerSet[info.RS1] + info.I_Immediate);
                    UInt32 word = memory[address >> 2];
                    UInt16 halfOffset = (UInt16)(2 - (address & 0x02));
                    UInt16 bitsOffset = (UInt16)(halfOffset * 8);
                    UInt32 h = (word >> bitsOffset) & 0xffff;
                    h = ReverseHalfWord(h);
                    h = (UInt32)SignExtend16(h);
                    registerSet[info.RD] = h;
                    break;
                }
                default:
                    throw new Exception("Unknown instruction");
           }
           pc = nextPC;
        }

        public static Int32 SignExtend8(UInt32 value)
        {
            return (Int32)((value & (1 << 7)) != 0 ? value | 0xFFFFFF00 : value);
        }

        public static Int32 SignExtend16(UInt32 value)
        {
            return (Int32)((value & (1 << 15)) != 0 ? value | 0xFFFF0000 : value);
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

        private static UInt32 ReverseHalfWord(UInt32 word)
        {
            UInt32 newWord = 0x00000000;
            for (int n = 0; n < 2; n++)
            {
                Byte b = (Byte)word;
                newWord = newWord << 8 | b;
                word = word >> 8;
            }
            return newWord;
        }

        private static UInt32 ShiftRightArithmetic(UInt32 value, UInt16 amount)
        {
            UInt32 msb = value & 0x80000000;
            for (int i = 0; i < amount; i++)
            {
                value = msb | (value >> 1);
            }
            return value;
        }
    }
}