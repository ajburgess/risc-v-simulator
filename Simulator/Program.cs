using System;
using System.IO;

namespace Simulator
{
    class Program
    {
        static void Main(string[] args)
        {
            string hexPath = "../Samples/add.hex";
            string hex = File.ReadAllText(hexPath);
            WordMemory memory = new WordMemory(hex);
            RegisterSet registers = new RegisterSet();
            UInt32 pc = 0x00000000;

            registers.Dump();
            System.Console.WriteLine();

            while (pc < 0x00002000)
            {
                UInt32 instruction = memory[pc >> 2].ReverseEndian();
                DecodeInfo decoded = Decoder.Decode(instruction);
                System.Console.WriteLine(decoded);
                System.Console.WriteLine();
                Console.ReadLine();
                Executer.Execute(decoded, registers, memory, ref pc);
                registers.Dump();
                System.Console.WriteLine();
            }
        }
    }
}
