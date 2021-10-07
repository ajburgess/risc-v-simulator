	.text
	.global	_start

_start:
	la	x7, results
	addi	x5, x0, 100
	addi	x6, x5, 200
	sw	x6, 0(x7)

results:
	.space	4
