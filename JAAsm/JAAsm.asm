.code
count_asm proc
    ; Dodaj wartoœci w RDX (a) i R8 (b)
    add RDX, R8
    ; Zapisz wynik dodawania do miejsca wskazywanego przez RCX (wskaŸnik do result)
    mov [RCX], RDX
    ; Zwróæ wynik przez RAX (mo¿na pomin¹æ, jeœli wynik zwracany przez wskaŸnik wystarczy)
    mov RAX, RDX

    ret
count_asm endp
end
