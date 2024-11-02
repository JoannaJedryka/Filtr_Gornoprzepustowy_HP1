.code
count_asm proc
    mov RAX, [RCX]  
    add RAX, RDX    
    mov [RCX], RAX 

    ret
count_asm endp
end
