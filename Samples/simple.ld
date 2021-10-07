OUTPUT_FORMAT("elf32-littleriscv", "elf32-littleriscv", "elf32-littleriscv")
OUTPUT_ARCH(riscv)
ENTRY(_start)
MEMORY
{
    RAM (rwx) : ORIGIN = 0x00000000, LENGTH = 8K/* RAM size 8KB */
}
SECTIONS
{
    PROVIDE( _stack_ptr = ORIGIN(RAM) + LENGTH(RAM));
    .text :
    {
        _text = .;
        *(.text*) 
        *(.rodata*)
        _etext = .;
    } > RAM

    .data : 
    {
        _data = .;
        *(.data*) 
        _edata = .;
    } > RAM

    .bss :
    {
        _bss = .;
        *(.bss*) 
        *(COMMON)       
        _ebss = .; 
    } > RAM
}