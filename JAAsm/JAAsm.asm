.code
count_asm proc
    ; Dodaj warto�ci w RDX (a) i R8 (b)
    add RDX, R8
    ; Zapisz wynik dodawania do miejsca wskazywanego przez RCX (wska�nik do result)
    mov [RCX], RDX
    ; Zwr�� wynik przez RAX (mo�na pomin��, je�li wynik zwracany przez wska�nik wystarczy)
    mov RAX, RDX

    ret
count_asm endp
end
