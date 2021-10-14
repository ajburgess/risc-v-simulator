using System;
using System.Diagnostics;
using System.IO;
using Xunit;
using System.Linq;

namespace Simulator
{
    public class Executer_Tests
    {
        [Theory]
        [InlineData(Instruction.ADD, 1000, 1234, 2234)]
        [InlineData(Instruction.ADD, 1000, -1234, -234)]
        [InlineData(Instruction.ADD, -1000, -1234, -2234)]
        [InlineData(Instruction.SUB, 1234, 1000, 234)]
        [InlineData(Instruction.SUB, 1000, 1234, -234)]
        [InlineData(Instruction.SRA, 8, 0, 8)]
        [InlineData(Instruction.SRA, 8, 1, 4)]
        [InlineData(Instruction.SRA, 9, 1, 4)]
        [InlineData(Instruction.SRA, -8, 1, -4)]
        [InlineData(Instruction.SRA, -1000, 3, -125)]
        [InlineData(Instruction.SRA, -1000, 4, -63)]
        [InlineData(Instruction.SLT, 1000, 999, 0)]
        [InlineData(Instruction.SLT, 1000, 1000, 0)]
        [InlineData(Instruction.SLT, 1000, 1001, 1)]
        [InlineData(Instruction.SLT, -1, 999, 1)]
        [InlineData(Instruction.SLT, 999, -1, 0)]
        public void Execute_R_Format_X5_X6_X7_arithmetic(Instruction instruction, Int32 x6, Int32 x7, Int32 x5_expected_value)
        {
            IWordMemory memory= new SparseWordMemory();
            using (AssemblerV2 assembler = AssemblerV2.Create(0x1000))
            {
                assembler.Assemble($"{instruction} x5,x6,x7");
                memory.Load(assembler.ToHex());
            }

            VirtualMachine vm = new VirtualMachine(memory, pc: 0x1000);
            vm.Registers.X6 = (UInt32)x6;
            vm.Registers.X7 = (UInt32)x7;

            vm.Step();

            Assert.Equal(x5_expected_value, (Int32)vm.Registers.X5);
            Assert.Equal((UInt32)0x1004, vm.PC);
        }

        [Theory]
        [InlineData(Instruction.SLL, 0x00000001, 0, 0x00000001)]
        [InlineData(Instruction.SLL, 0x00000001, 1, 0x00000002)]
        [InlineData(Instruction.SLL, 0x00000001, 31, 0x80000000)]
        [InlineData(Instruction.SLL, 0x80000001, 33, 0x00000002)] // Lower 5 bits of RS2 only
        [InlineData(Instruction.SLL, 0xFF000001, 4, 0xF0000010)]
        [InlineData(Instruction.SRL, 0x80000000, 0, 0x80000000)]
        [InlineData(Instruction.SRL, 0x80000000, 1, 0x40000000)]
        [InlineData(Instruction.SRL, 0x40000001, 1, 0x20000000)]
        [InlineData(Instruction.SRL, 0x80000000, 31, 0x00000001)]
        [InlineData(Instruction.OR, 0x55555555, 0x55AA0022, 0x55FF5577)]
        [InlineData(Instruction.AND, 0x55555555, 0x55AA0011, 0x55000011)]
        [InlineData(Instruction.XOR, 0x55555555, 0x55AA0011, 0x00FF5544)]
        [InlineData(Instruction.SLTU, 1000, 999, 0)]
        [InlineData(Instruction.SLTU, 1000, 1000, 0)]
        [InlineData(Instruction.SLTU, 1000, 1001, 1)]
        [InlineData(Instruction.SLTU, 0xFFFFFFFF, 999, 0)]
        [InlineData(Instruction.SLTU, 999, 0xFFFFFFFF, 1)]
        public void Execute_R_Format_X5_X6_X7_logical(Instruction instruction, UInt32 x6, UInt32 x7, UInt32 x5_expected_value)
        {
            IWordMemory memory= new SparseWordMemory();
            using (AssemblerV2 assembler = AssemblerV2.Create(0x1000))
            {
                assembler.Assemble($"{instruction} x5,x6,x7");
                memory.Load(assembler.ToHex());
            }

            VirtualMachine vm = new VirtualMachine(memory, pc: 0x1000);
            vm.Registers.X6 = (UInt32)x6;
            vm.Registers.X7 = (UInt32)x7;

            vm.Step();

            Assert.Equal(x5_expected_value, vm.Registers.X5);
            Assert.Equal((UInt32)0x1004, vm.PC);
        }

        [Theory]
        [InlineData(Instruction.ADDI, 1000, 1234, 2234)]
        [InlineData(Instruction.ADDI, 1000, -1234, -234)]
        [InlineData(Instruction.ADDI, -1000, -1234, -2234)]
        [InlineData(Instruction.SRAI, -8, 1, -4)]
        [InlineData(Instruction.SRAI, -1000, 3, -125)]
        [InlineData(Instruction.SRAI, -1000, 4, -63)]
        [InlineData(Instruction.SLTI, 1000, 999, 0)]
        [InlineData(Instruction.SLTI, 1000, 1000, 0)]
        [InlineData(Instruction.SLTI, 1000, 1001, 1)]
        [InlineData(Instruction.SLTI, -1, 999, 1)]
        [InlineData(Instruction.SLTI, 999, -1, 0)]
        public void Execute_I_Format_X5_X6_arithmetic(Instruction instruction, Int32 x6, Int32 immediate, Int32 x5_expected_value)
        {
            IWordMemory memory= new SparseWordMemory();
            using (AssemblerV2 assembler = AssemblerV2.Create(0x1000))
            {
                assembler.Assemble($"{instruction} x5,x6,{immediate}");
                memory.Load(assembler.ToHex());
            }

            VirtualMachine vm = new VirtualMachine(memory, pc: 0x1000);
            vm.Registers.X6 = (UInt32)x6;

            vm.Step();

            Assert.Equal(x5_expected_value, (Int32)vm.Registers.X5);
            Assert.Equal((UInt32)0x1004, vm.PC);
        }

        [Theory]
        [InlineData(Instruction.ANDI, 0x12345678, 0x7FF, 0x00000678)]
        [InlineData(Instruction.ANDI, 0x12345678, -1, 0x12345678)] // sign extend immediate!
        [InlineData(Instruction.ORI, 0x12345678, 0x7FF, 0x123457FF)]
        [InlineData(Instruction.ORI, 0x12345678, -1, 0xFFFFFFFF)] // sign extend immediate!
        [InlineData(Instruction.XORI, 0x12345678, 0x7FF, 0x12345187)]
        [InlineData(Instruction.XORI, 0x12345678, -1, 0xEDCBA987)] // sign extend immediate!
        [InlineData(Instruction.SLLI, 0x00000001, 0, 0x00000001)]
        [InlineData(Instruction.SLLI, 0x00000001, 1, 0x00000002)]
        [InlineData(Instruction.SLLI, 0x00000001, 31, 0x80000000)]
        [InlineData(Instruction.SLLI, 0xFF000001, 4, 0xF0000010)]
        [InlineData(Instruction.SRLI, 0xFF000001, 4, 0x0FF00000)]
        [InlineData(Instruction.SLTIU, 1000, 999, 0)]
        [InlineData(Instruction.SLTIU, 1000, 1000, 0)]
        [InlineData(Instruction.SLTIU, 1000, 1001, 1)]
        [InlineData(Instruction.SLTIU, 0xFFFFFFFF, 999, 0)]
        [InlineData(Instruction.SLTIU, 999, -1, 1)]
        public void Execute_I_Format_X5_X6_logical(Instruction instruction, UInt32 x6, Int32 immediate, UInt32 x5_expected_value)
        {
            IWordMemory memory= new SparseWordMemory();
            using (AssemblerV2 assembler = AssemblerV2.Create(0x1000))
            {
                assembler.Assemble($"{instruction} x5,x6,{immediate}");
                memory.Load(assembler.ToHex());
            }

            VirtualMachine vm = new VirtualMachine(memory, pc: 0x1000);
            vm.Registers.X6 = (UInt32)x6;

            vm.Step();

            Assert.Equal(x5_expected_value, vm.Registers.X5);
            Assert.Equal((UInt32)0x1004, vm.PC);
        }

        [Theory]
        [InlineData(Instruction.LW, 84, -20, "@0040 78 56 34 12", 0x12345678)]
        [InlineData(Instruction.LBU, 44, 20, "@0040 78 56 34 12", 0x00000078)]
        [InlineData(Instruction.LBU, 44, 20, "@0040 F4 56 34 12", 0x000000F4)]
        [InlineData(Instruction.LB, 44, 20, "@0040 78 56 34 12", 0x00000078)]
        [InlineData(Instruction.LB, 44, 20, "@0040 F4 56 34 12", 0xFFFFFFF4)] // sign extend!
        [InlineData(Instruction.LHU, 44, 20, "@0040 78 56 34 12", 0x00005678)]
        [InlineData(Instruction.LHU, 44, 22, "@0040 78 56 34 12", 0x00001234)]
        [InlineData(Instruction.LH, 44, 20, "@0040 78 56 34 12", 0x00005678)]
        [InlineData(Instruction.LH, 44, 20, "@0040 78 96 34 12", 0xFFFF9678)] // sign extend!
        public void Execute_Load(Instruction instruction, UInt32 x6, Int32 immediate, string memoryInit, UInt32 x5_expected_value)
        {
            IWordMemory memory= new SparseWordMemory();
            using (AssemblerV2 assembler = AssemblerV2.Create(0x1000))
            {
                assembler.Assemble($"{instruction} x5,{immediate}(x6)");
                memory.Load(assembler.ToHex());
                memory.Load(memoryInit);
            }

            VirtualMachine vm = new VirtualMachine(memory, pc: 0x1000);
            vm.Registers.X6 = (UInt32)x6;

            vm.Step();

            Assert.Equal(x5_expected_value.ToString("X8"), vm.Registers.X5.ToString("X8"));
            Assert.Equal((UInt32)0x1004, vm.PC);
        }

        [Theory]
        [InlineData(Instruction.SW, 0x12345678, 44, 20, "@0040 AB CD EF 99", "@0040 78 56 34 12")]
        [InlineData(Instruction.SB, 0x12345678, 44, 20, "@0040 AB CD EF 99", "@0040 78 CD EF 99")]
        [InlineData(Instruction.SB, 0x12345678, 44, 21, "@0040 AB CD EF 99", "@0040 AB 78 EF 99")]
        [InlineData(Instruction.SB, 0x12345678, 44, 22, "@0040 AB CD EF 99", "@0040 AB CD 78 99")]
        [InlineData(Instruction.SB, 0x12345678, 44, 23, "@0040 AB CD EF 99", "@0040 AB CD EF 78")]
        [InlineData(Instruction.SH, 0x12345678, 44, 20, "@0040 AB CD EF 99", "@0040 78 56 EF 99")]
        [InlineData(Instruction.SH, 0x12345678, 44, 22, "@0040 AB CD EF 99", "@0040 AB CD 78 56")]
        public void Execute_Store(Instruction instruction, UInt32 x5, UInt32 x6, Int32 immediate, string memoryInit, string memoryExpected)
        {
            IWordMemory memory= new SparseWordMemory();
            using (AssemblerV2 assembler = AssemblerV2.Create(0x1000))
            {
                assembler.Assemble($"{instruction} x5,{immediate}(x6)");
                memory.Load(assembler.ToHex());
                memory.Load(memoryInit);
            }

            VirtualMachine vm = new VirtualMachine(memory, pc: 0x1000);
            vm.Registers.X5 = (UInt32)x5;
            vm.Registers.X6 = (UInt32)x6;

            vm.Step();

            Assert.Equal(memoryExpected, memory.ToString(0x0040, 4));
            Assert.Equal((UInt32)0x1004, vm.PC);
        }

        [Theory]
        [InlineData(Instruction.BEQ, 123, 123, 40, 1000, 1044)]
        [InlineData(Instruction.BEQ, 123, 456, 40, 1000, 1004)]
        [InlineData(Instruction.BNE, 123, 456, 40, 1000, 1044)]
        [InlineData(Instruction.BNE, 123, 123, 40, 1000, 1004)]
        [InlineData(Instruction.BLT, 123, 122, 40, 1000, 1004)]
        [InlineData(Instruction.BLT, 123, 123, 40, 1000, 1004)]
        [InlineData(Instruction.BLT, 123, 124, 40, 1000, 1044)]
        [InlineData(Instruction.BLT, 0x88888888, 124, 40, 1000, 1044)] // signed compare
        [InlineData(Instruction.BGE, 123, 122, 40, 1000, 1044)]
        [InlineData(Instruction.BGE, 123, 123, 40, 1000, 1044)]
        [InlineData(Instruction.BGE, 123, 124, 40, 1000, 1004)]
        [InlineData(Instruction.BGE, 0x88888888, 124, 40, 1000, 1004)] // signed compare
        [InlineData(Instruction.BLTU, 123, 122, 40, 1000, 1004)]
        [InlineData(Instruction.BLTU, 123, 123, 40, 1000, 1004)]
        [InlineData(Instruction.BLTU, 123, 124, 40, 1000, 1044)]
        [InlineData(Instruction.BLTU, 0x88888888, 124, 40, 1000, 1004)] // unsigned compare
        [InlineData(Instruction.BGEU, 123, 122, 40, 1000, 1044)]
        [InlineData(Instruction.BGEU, 123, 123, 40, 1000, 1044)]
        [InlineData(Instruction.BGEU, 123, 124, 40, 1000, 1004)]
        [InlineData(Instruction.BGEU, 0x88888888, 124, 40, 1000, 1044)] // unsigned compare
        public void Execute_Branch(Instruction instruction, UInt32 x5, UInt32 x6, UInt32 spaces, UInt32 initialPC, UInt32 expectedPC)
        {
            IWordMemory memory= new SparseWordMemory();
            using (AssemblerV2 assembler = AssemblerV2.Create(initialPC))
            {
                assembler.Assemble(
                    $"{instruction} x5,x6,dest",
                    $".skip {spaces}, 0",
                    $"dest:"
                );
                memory.Load(assembler.ToHex());
            }

            VirtualMachine vm = new VirtualMachine(memory, pc: initialPC);
            vm.Registers.X5 = (UInt32)x5;
            vm.Registers.X6 = (UInt32)x6;

            vm.Step();

            Assert.Equal(expectedPC, vm.PC);
        }

        [Theory]
        [InlineData(Instruction.LUI, 0x12345, 0xABCDEF99, 0x12345000)]
        public void Execute_LUI(Instruction instruction, UInt32 immediate, UInt32 x5, UInt32 expected_x5)
        {
            IWordMemory memory= new SparseWordMemory();
            using (AssemblerV2 assembler = AssemblerV2.Create(0x1000))
            {
                assembler.Assemble($"{instruction} x5,{immediate}");
                memory.Load(assembler.ToHex());
            }

            VirtualMachine vm = new VirtualMachine(memory, pc: 0x1000);
            vm.Registers.X5 = (UInt32)x5;

            vm.Step();

            Assert.Equal(expected_x5.ToString("X8"), vm.Registers.X5.ToString("X8"));
            Assert.Equal((UInt32)0x1004, vm.PC);
        }

        [Theory]
        [InlineData(Instruction.AUIPC, 0x12345, 0x81000AA8, 0xDEADBEEF, 0x93345AA8)]
        public void Execute_AUIPC(Instruction instruction, UInt32 immediate, UInt32 pc_before, UInt32 x5_before, UInt32 expected_x5)
        {
            IWordMemory memory= new SparseWordMemory();
            using (AssemblerV2 assembler = AssemblerV2.Create(pc_before))
            {
                assembler.Assemble($"{instruction} x5,{immediate}");
                memory.Load(assembler.ToHex());
            }

            VirtualMachine vm = new VirtualMachine(memory, pc: pc_before);
            vm.Registers.X5 = (UInt32)x5_before;

            vm.Step();

            Assert.Equal(expected_x5.ToString("X8"), vm.Registers.X5.ToString("X8"));
            Assert.Equal(pc_before + 4, vm.PC);
        }

        [Theory]
        [InlineData(0x04000, 0x10001234, 0x10005238, 0x10001238)]
        public void Execute_JAL(UInt32 spaces, UInt32 previousPC, UInt32 expectedPC, UInt32 expected_x5)
        {
            IWordMemory memory= new SparseWordMemory();
            using (AssemblerV2 assembler = AssemblerV2.Create(previousPC))
            {
                assembler.Assemble(
                    $"jal x5,dest",
                    $".skip {spaces},0",
                    $"dest:"
                );
                memory.Load(assembler.ToHex());
            }

            VirtualMachine vm = new VirtualMachine(memory, pc: previousPC);

            vm.Step();

            Assert.Equal(expected_x5.ToString("X8"), vm.Registers.X5.ToString("X8"));
            Assert.Equal(expectedPC.ToString("X8"), vm.PC.ToString("X8"));
        }

        [Theory]
        [InlineData(0x400, 0x11223344, 0x10001234, 0x11223744, 0x10001238)]
        public void Execute_JALR(UInt32 immediate, UInt32 x6, UInt32 previousPC, UInt32 expectedPC, UInt32 expected_x5)
        {
            IWordMemory memory= new SparseWordMemory();
            using (AssemblerV2 assembler = AssemblerV2.Create(previousPC))
            {
                assembler.Assemble($"JALR x5,{immediate}(x6)");
                memory.Load(assembler.ToHex());
            }

            VirtualMachine vm = new VirtualMachine(memory, pc: previousPC);
            vm.Registers.X6 = x6;

            vm.Step();

            Assert.Equal(expected_x5.ToString("X8"), vm.Registers.X5.ToString("X8"));
            Assert.Equal(expectedPC.ToString("X8"), vm.PC.ToString("X8"));
        }

        // --------------------------------------------------------------------
        // TO DO: exceptions when try to read / write misaligned w/h/b address!
        // --------------------------------------------------------------------

    }
}
