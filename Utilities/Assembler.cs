using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Utilities
{
    public class Assembler : IDisposable
    {
        private bool disposedValue;
        private string tempPath;
        string sourcePath;
        string objectPath;
        string elfPath;
        string hexPath;

        private Assembler(string tempPath)
        {
            this.tempPath = tempPath;
            sourcePath = Path.ChangeExtension(tempPath, ".s");
            objectPath = Path.ChangeExtension(sourcePath, ".o");
            elfPath = Path.ChangeExtension(sourcePath, ".elf");
            hexPath = Path.ChangeExtension(sourcePath, ".hex");
        }

        public static Assembler Create()
        {
            string tempPath = Path.GetTempFileName();
            Assembler assembler = new Assembler(tempPath);
            return assembler;
        }

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
            Console.WriteLine($"{info.FileName} {info.Arguments}");
            Process p = Process.Start(info);
            p.WaitForExit();
            string stdOut = p.StandardOutput.ReadToEnd();
            string stdErr = p.StandardError.ReadToEnd();
            if (p.ExitCode != 0)
            {
                throw new Exception(stdErr);
            }
        }

        public void Build(string sourcePath, string linkerScriptPath,  UInt32 startAddress = 0x00000000)
        {
            ExecuteCommand("riscv32-unknown-elf-as", $"-o {objectPath} {sourcePath}");
            ExecuteCommand("riscv32-unknown-elf-ld", $"-nostdlib -Ttext=0x{startAddress:X8} -T{linkerScriptPath} -o {elfPath} {objectPath}");
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
                //File.Delete(sourcePath);
                File.Delete(objectPath);
                File.Delete(elfPath);
                File.Delete(hexPath);
                disposedValue = true;
            }
        }

        ~Assembler()
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