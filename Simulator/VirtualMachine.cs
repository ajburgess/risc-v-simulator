using System;
using System.IO;
using System.Linq;

namespace Simulator
{
    public enum NumberFormat
    {
        Decimal = 1,
        Hexadecimal = 2
    }

    public class VirtualMachine
    {
        private IWordMemory memory;
        private UInt32 pc;
        private RegisterSet registers;
        private bool halted;

        public VirtualMachine(IWordMemory memory, UInt32 pc = 0x00000000)
        {
            this.memory = memory;
            this.pc = pc;
            this.registers = new RegisterSet();
            this.halted = false;
            this.Registers.SP = (UInt32)memory.SizeInBytes;
        }

        public RegisterSet Registers => this.registers;

        public UInt32 PC
        {
            get { return this.pc; }
            set { this.pc = value; }
        }

        public bool Halted => this.halted;

        public void ClearMemory()
        {
            memory.Clear();
            Console.WriteLine("Memory cleared.");
        }

        public void DisplayMemory(UInt32 startAddess, UInt32 endAddress)
        {
            UInt32 address = startAddess & 0xFFFFFFF0;
            while (address <= (endAddress | 0x0000000F))
            {
                Console.Write($"{address:X8}  ");
                for (int i = 0; i < 16; i++)
                {
                    Byte b = memory.GetByte(address);
                    Console.Write($"{b:X2} ");
                    if (i == 7)
                        Console.Write(" ");
                    address++;
                }
                Console.WriteLine();
            }
        }

        public void Step()
        {
            if (halted)
                throw new Exception("CPU is halted");

            if ((pc & 0x03) != 0x00)
                throw new Exception($"Misaligned word memory access at 0x{pc:X8}");

            UInt32 instruction = memory.GetWord(pc).ReverseEndian();
            if (instruction == 0x00000000)
            {
                halted = true;
                return;
            }

            DecodeInfo decoded = Decoder.Decode(instruction);
            Executer.Execute(decoded, Registers, memory, ref pc);
        }

        public void DisplayInstructionsAroundPC(int before, int after)
        {
            for (int i = before; i <= after; i++)
            {
                UInt32 instruction = memory.GetWord(pc + (UInt32)(i * 4)).ReverseEndian();
                DecodeInfo decoded = Decoder.Decode(instruction);
                string highlight = (i == 0 ? "*" : " ");
                System.Console.WriteLine($"{pc:X8} {highlight} {decoded}");
            }
        }

        public void DisplayCurrentInstuction()
        {
            DisplayInstructionsAroundPC(0, 0);
        }

        public void DisplayRegisters(NumberFormat format)
        {
            string f = format == NumberFormat.Decimal ? "D8" : "X8";
            Console.Write($"X0(Z):   {registers[0].ToString(f)} ");
            Console.Write($"X1(RA):  {registers[1].ToString(f)} ");
            Console.Write($"X2(SP):  {registers[2].ToString(f)} ");
            Console.Write($"X3(GP):  {registers[3].ToString(f)}\n");
            Console.Write($"X4(TP):  {registers[4].ToString(f)} ");
            Console.Write($"X5(T0):  {registers[5].ToString(f)} ");
            Console.Write($"X6(T1):  {registers[6].ToString(f)} ");
            Console.Write($"X7(T2):  {registers[7].ToString(f)}\n");
            Console.Write($"X8(S0):  {registers[8].ToString(f)} ");
            Console.Write($"X9(S1):  {registers[9].ToString(f)} ");
            Console.Write($"X10(A0): {registers[10].ToString(f)} ");
            Console.Write($"X11(A1): {registers[11].ToString(f)}\n");
            Console.Write($"X12(A2): {registers[12].ToString(f)} ");
            Console.Write($"X13(A3): {registers[13].ToString(f)} ");
            Console.Write($"X14(A4): {registers[14].ToString(f)} ");
            Console.Write($"X15(A5): {registers[15].ToString(f)}\n");
        }

        public void DisplayRegister(Byte n, NumberFormat format)
        {
            string f = format == NumberFormat.Decimal ? "D8" : "X8";
            Console.Write($"X{n:D2}: {registers[n].ToString(f)}");
        }
    }
}
