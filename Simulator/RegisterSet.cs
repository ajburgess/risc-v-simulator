using System;
using System.Collections.Generic;
using System.Linq;

namespace Simulator
{
    public enum R
    {
        X0 = 0, ZERO = X0,
        X1 = 1, RA = X1,
        X2 = 2, SP = X2,
        X3 = 3, GP = X3,
        X4 = 4, TP = X4,
        X5 = 5, T0 = X5,
        X6 = 6, T1 = X6,
        X7 = 7, T2 = X7,
        X8 = 8, S0 = X8, FP = X8,
        X9 = 9, S1 = X9,
        X10 = 10, A0 = X10,
        X11 = 11, A1 = X11,
        X12 = 12, A2 = X12,
        X13 = 13, A3 = X13,
        X14 = 14, A4 = X14,
        X15 = 15, A5 = X15
    }

    public class RegisterSet
    {
        private UInt32[] registers = new UInt32[16];

        public UInt32[] AsReadOnly()
        {
            return this.registers.ToArray();
        }

        public void Dump()
        {
            for (int i = 0; i < 16; i++)
            {
                System.Console.WriteLine($"X{i:D2} 0x{registers[i]:X8}");
            }
        }

        public UInt32 X0
        {
            get { return this[R.X0]; }
            set { this[R.X0] = value; }
        }

        public UInt32 Zero
        {
            get { return this[R.ZERO]; }
            set { this[R.ZERO] = value; }
        }

        public UInt32 X1
        {
            get { return this[R.X1]; }
            set { this[R.X1] = value; }
        }

        public UInt32 RA
        {
            get { return this[R.RA]; }
            set { this[R.RA] = value; }
        }

        public UInt32 X2
        {
            get { return this[R.X2]; }
            set { this[R.X2] = value; }
        }

        public UInt32 SP
        {
            get { return this[R.SP]; }
            set { this[R.SP] = value; }
        }

        public UInt32 X3
        {
            get { return this[R.X3]; }
            set { this[R.X3] = value; }
        }

        public UInt32 GP
        {
            get { return this[R.GP]; }
            set { this[R.GP] = value; }
        }

        public UInt32 X4
        {
            get { return this[R.X4]; }
            set { this[R.X4] = value; }
        }

        public UInt32 X5
        {
            get { return this[R.X5]; }
            set { this[R.X5] = value; }
        }

        public UInt32 X6
        {
            get { return this[R.X6]; }
            set { this[R.X6] = value; }
        }

        public UInt32 X7
        {
            get { return this[R.X7]; }
            set { this[R.X7] = value; }
        }

        public UInt32 X8
        {
            get { return this[R.X8]; }
            set { this[R.X8] = value; }
        }

        public UInt32 X9
        {
            get { return this[R.X9]; }
            set { this[R.X9] = value; }
        }

        public UInt32 X10
        {
            get { return this[R.X10]; }
            set { this[R.X10] = value; }
        }

        public UInt32 X11
        {
            get { return this[R.X11]; }
            set { this[R.X11] = value; }
        }

        public UInt32 X12
        {
            get { return this[R.X12]; }
            set { this[R.X12] = value; }
        }

        public UInt32 X13
        {
            get { return this[R.X13]; }
            set { this[R.X13] = value; }
        }

        public UInt32 X14
        {
            get { return this[R.X14]; }
            set { this[R.X14] = value; }
        }

        public UInt32 X15
        {
            get { return this[R.X15]; }
            set { this[R.X15] = value; }
        }

        public UInt32 this[int r]
        {
            get
            {
                if (r < 0 || r >= registers.Length)
                    return 0x00000000;
                else
                    return (r == 0) ? 0x00000000 : registers[r];
            }
            set
            {
                if (r != 0)
                {
                    registers[r] = value;
                }
            }
        }

        public UInt32 this[R r]
        {
            get
            {
                return this[(Int32)r];
            }
            set
            {
                this[(Int32)r] = value;
            }
        }        
    }
}