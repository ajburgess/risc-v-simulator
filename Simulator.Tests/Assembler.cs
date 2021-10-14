using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Simulator
{
    public class Assembler
    {
        private static void ExecuteCommand(string filename, string arguments)
        {
            ProcessStartInfo info = new ProcessStartInfo
            {
                FileName = filename,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                UseShellExecute = false
            };
            // Console.WriteLine(filename + " " + arguments);
            Process p = Process.Start(info);
            p.WaitForExit();
            string stdOut = p.StandardOutput.ReadToEnd();
            string stdErr = p.StandardError.ReadToEnd();
            // Console.WriteLine(stdOut);
            // Console.WriteLine(stdErr);
            if (p.ExitCode != 0)
            {
                throw new Exception(stdErr);
            }
        }

        public static UInt32 Assemble(params string[] source)
        {
            return Assemble(0x00000000, source);
        }

        public static UInt32 Assemble(UInt32 startAddress, params string[] source)
        {
            string tmpPath = Path.GetTempFileName();
            string sourcePath = Path.ChangeExtension(tmpPath, ".s");
            string objectPath = Path.ChangeExtension(sourcePath, ".o");
            string elfPath = Path.ChangeExtension(sourcePath, ".elf");
            string binPath = Path.ChangeExtension(sourcePath, ".bin");

            List<string> lines = new List<string>();
            lines.Add(".global _start");
            lines.Add("_start:");
            lines.AddRange(source);
            File.WriteAllLines(sourcePath, lines);

            ExecuteCommand("riscv32-unknown-elf-as", $"-o {objectPath} {sourcePath}");
            ExecuteCommand("riscv32-unknown-elf-ld", $"-nostdlib -Ttext=0x{startAddress:X8} -Tsimple.ld -o {elfPath} {objectPath}");
            ExecuteCommand("riscv32-unknown-elf-objcopy", $"-O binary {elfPath} {binPath}");

            Byte[] bytes = File.ReadAllBytes(binPath);
            UInt32 instruction = (UInt32)(bytes[0] | bytes[1] << 8 | bytes[2] << 16 | bytes[3] << 24);

            File.Delete(tmpPath);
            File.Delete(sourcePath);
            File.Delete(objectPath);
            File.Delete(elfPath);
            File.Delete(binPath);

            return instruction;
        }
    }
}