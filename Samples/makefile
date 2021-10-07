all: add.bin

clean:
	rm -f *.o *.elf *.hex *.bin

add.o: add.S
	riscv32-unknown-elf-as -o add.o add.S

add.elf: add.o
	riscv32-unknown-elf-ld -O0 -nostdlib -Tsimple.ld -o add.elf add.o

add.bin: add.elf
	riscv32-unknown-elf-objcopy -O verilog add.elf add.hex