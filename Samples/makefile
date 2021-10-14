all: add.hex unit_tests.hex bin/hello.hex

clean:
	rm -f .*.swp *.o *.elf *.hex *.bin
	rm -f bin/.*.swp bin/*.o bin/*.elf bin/*.hex bin/*.bin

bin/hello.elf : hello.c init.S
	riscv32-unknown-elf-gcc -g -s -ffreestanding -Wl,--gc-sections,--section-start .init=0x0000 -Ttext=0x0100 \
		-o bin/hello.elf init.S hello.c

bin/hello.hex : bin/hello.elf
	riscv32-unknown-elf-objcopy -O verilog bin/hello.elf bin/hello.hex

add.o: add.S
	riscv32-unknown-elf-as -o add.o add.S

add.elf: add.o
	riscv32-unknown-elf-ld -O0 -nostdlib -Tsimple.ld -o add.elf add.o

add.hex: add.elf
	riscv32-unknown-elf-objcopy -O verilog add.elf add.hex

unit_tests.o: unit_tests.S
	riscv32-unknown-elf-gcc -c -o unit_tests.o unit_tests.S

unit_tests.elf: unit_tests.o
	riscv32-unknown-elf-ld -O0 -nostdlib -Tsimple.ld -o unit_tests.elf unit_tests.o

unit_tests.hex: unit_tests.elf
	riscv32-unknown-elf-objcopy -O verilog unit_tests.elf unit_tests.hex
