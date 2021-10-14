using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Simulator
{
    public class AssemblerV2 : IDisposable
    {
        private bool disposedValue;
        private string tempPath;
        string sourcePath;
        string objectPath;
        string elfPath;
        string hexPath;

        private AssemblerV2(string tempPath)
        {
            this.tempPath = tempPath;
            sourcePath = Path.ChangeExtension(tempPath, ".s");
            objectPath = Path.ChangeExtension(sourcePath, ".o");
            elfPath = Path.ChangeExtension(sourcePath, ".elf");
            hexPath = Path.ChangeExtension(sourcePath, ".hex");
        }

        public static AssemblerV2 Create(UInt32 startAddress = 0x00000000)
        {
            string tempPath = Path.GetTempFileName();
            AssemblerV2 assembler = new AssemblerV2(tempPath);
            assembler.StartAddress = startAddress;
            return assembler;
        }

        public UInt32 StartAddress { get; set; }

        private void ExecuteCommand(string filename, string arguments)
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
            Process p = Process.Start(info);
            p.WaitForExit();
            string stdOut = p.StandardOutput.ReadToEnd();
            string stdErr = p.StandardError.ReadToEnd();
            if (p.ExitCode != 0)
            {
                throw new Exception(stdErr);
            }
        }

        public void Assemble(params string[] source)
        {
            List<string> lines = new List<string>();
            lines.Add(".global _start");
            lines.Add("_start:");
            lines.AddRange(source);
            File.WriteAllLines(sourcePath, lines);

            ExecuteCommand("riscv32-unknown-elf-as", $"-o {objectPath} {sourcePath}");
            ExecuteCommand("riscv32-unknown-elf-ld", $"-nostdlib -Ttext=0x{StartAddress:X8} -Tsimple.ld -o {elfPath} {objectPath}");
        }

        public string ToHex()
        {
            ExecuteCommand("riscv32-unknown-elf-objcopy", $"-O verilog {elfPath} {hexPath}");
            return File.ReadAllText(hexPath);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                File.Delete(tempPath);
                File.Delete(sourcePath);
                File.Delete(objectPath);
                File.Delete(elfPath);
                File.Delete(hexPath);
                disposedValue = true;
            }
        }

        ~AssemblerV2()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}