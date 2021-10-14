	.text
	.global	_start

_start:
	la	x7, results
	addi	x5, x0, 100
	addi	x6, x5, 200
	sw	x6, 0(x7)
	lw	x8, 0(x7)
	addi	x8, x8, 1
	sw	x8, 4(x7)

results:
	.word	0
	.word 	0
