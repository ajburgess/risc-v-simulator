	.text
	.global	_start

#define outcome 0x7000
#define failing_test 0x7004

_start:

/* Register instructions */

  .macro REGISTER_TEST test_num, instruction, x5, x6, x7_expected
  li x15, \test_num
  li x5, \x5
  li x6, \x6
  \instruction x7, x5, x6
  li x8, \x7_expected
  bne x7, x8, fail
  .endm

  REGISTER_TEST 0x0101, "add", 1000, 1234, 2234
  REGISTER_TEST 0x0102, "add", 1000, -1234, -234
  REGISTER_TEST 0x0103, "add", -1000, -1234, -2235

/*
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
*/

end:
  la x14, outcome
  li x13, 0x01
  sw x13, 0(x14)
  .word 0x00000000

fail:
  la x14, outcome
  li x13, 0x02
  sw x13, 0(x14)
  la x14, failing_test
  sw x15, 0(x14) 
  .word 0x00000000
