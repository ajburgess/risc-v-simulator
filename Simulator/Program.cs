using System;
using System.IO;
using Utilities;

namespace Simulator
{
    class Program
    {
        static void Main(string[] args)
        {
            IWordMemory memory = new WordMemory(0x10000 >> 2);
            memory.LoadFromFile("../Samples/bin/hello.hex");
            VirtualMachine vm = new VirtualMachine(memory);
            while (!vm.Halted)
            {
                vm.DisplayRegisters(NumberFormat.Hexadecimal);
                Console.WriteLine();
                vm.DisplayMemory(0x7000, 0x700F);
                Console.WriteLine();
                vm.DisplayCurrentInstuction();
                System.Console.WriteLine();
                // System.Console.Write("Press enter to step...");
                // Console.ReadLine();
                vm.Step();
                System.Console.WriteLine();
            }            
        }
    }
}
