.data
redMask   dq 00ff000000ff0000h     ; Maska dla kana³u czerwonego (64 bity)
greenMask dq 0000ff000000ff00h     ; Maska dla kana³u zielonego (64 bity)
blueMask  dq 000000ff000000ffh     ; Maska dla kana³u niebieskiego (64 bity)
filter    dd 0, -1, 0, -1, 5, -1, 0, -1, 0 ; Filtr górnoprzepustowy 3x3

.code
count_asm proc
    ; Argumenty funkcji:
    ; rdi - wskaŸnik na obraz (tablica pikseli)
    ; rsi - szerokoœæ obrazu (width)
    ; rdx - wysokoœæ obrazu (height)
    ; rcx - yStart (pocz¹tek wiersza)
    ; r8  - stripHeight (wysokoœæ przetwarzanego paska)

    ; Zapisanie rejestrów
    push rbx
    push r9
    push r10

    ; Obliczenie adresu pocz¹tku przetwarzanego paska
    mov r9, rdi                ; r9 = wskaŸnik na obraz (image)
    imul r9, rsi               ; r9 = rdi * rsi (szerokoœæ * yStart)
    add r9, rcx                ; r9 = r9 + yStart (pocz¹tek wiersza do przetworzenia)

    ; Pêtla po wierszach obrazu
    mov r10, rcx                ; r10 = yStart
    mov r11, r8                 ; r11 = stripHeight (iloœæ wierszy do przetworzenia)

process_rows:
    ; Sprawdzanie granic
    cmp r10, rdx
    jge end_process

    ; Pêtla po kolumnach (pikselach w jednym wierszu)
    mov r12, 1                  ; r12 = 1 (rozpoczynamy od drugiego piksela)
    mov r13, rsi                ; r13 = szerokoœæ obrazu

process_columns:
    ; U¿ywamy instrukcji SIMD do obliczeñ na 4 pikselach jednoczeœnie
    ; Wczytanie czterech pikseli
    movaps xmm0, [r9 + r12*4]   ; Wczytanie czterech pikseli do xmm0
    pshufb xmm0, xmm0           ; Shuffle bytes for RGB channels

    ; Zastosowanie filtra górnoprzepustowego (obliczenia na RGB)
    ; ... Implementacja obliczenia na 4 pikselach w jednym cyklu
    ; Przy zastosowaniu SSE, mo¿emy operowaæ na 4 pikselach jednoczeœnie

    ; Zapisanie wyniku z powrotem do obrazu
    movaps [r9 + r12*4], xmm0   ; Zapisanie zmienionych pikseli

    ; Zwiêkszanie indeksu kolumny
    add r12, 4
    cmp r12, r13
    jl process_columns

    ; Przechodzimy do kolejnego wiersza
    add r10, 1
    add r9, rsi                 ; Zwiêkszamy adres o szerokoœæ obrazu
    dec r11
    jnz process_rows

end_process:
    ; Przywracanie rejestrów
    pop r10
    pop r9
    pop rbx
    ret
count_asm endp
end
