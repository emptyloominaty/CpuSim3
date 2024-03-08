## CPU
* Architecture: **EMP 3 32-Bit**
* **Load–store** architecture
* Data Width: **32-bit** 
* Address Width: **32-bit**
* Bus: **32-bit** Data + **32-bit** Address
* Registers: 
    - 32 32-Bit Registers (**r0-r31**)  <br/>
    - RESERVED (**r32**)
    - 32-Bit Program Counter (**r33**)
    - 32-Bit Stack Pointer (**r34**) 
    - Carry (**r35**)
    - Overflow (**r36**)
    - Interrupt Disabled  (**r37**)
    <br/>
* IPC: **0.27**
* Stack: **8192 Bytes**

        
|               |  Start   |   End    |
|---------------|:--------:|:--------:|
| Interrupt Pointers | 0x000000 | 0x0002FF |
| OS RAM        | 0x000300 | 0x001FFF |
| Stack         | 0x002000 | 0x003FFF |
| App RAM       | 0x004000 | 0x3FEFFF |
| Functions RAM | 0x3FF000 | 0x3FFFFF |
| App ROM       | 0x400000 | 0x5FFFFF | 
| Functions ROM | 0x600000 | 0x6FFFFF | 
| OS ROM        | 0x700000 | 0x7FFFFF |
| Devices       | 0x800000 | 0xFFFFFF |

Reset Vector: 0x700000

### Instructions
| OP - Name  | Cycles | Bytes |     | Byte1 | Byte2 |  Byte3  |  Byte4  |  Byte5  | Byte6 |
|------------|:------:|:-----:|:---:|:-----:|:-----:|:-------:|:-------:|:-------:|:-----:|   
| 0 - STOP   |   1    |   1   |  -  | 0x00  |   -   |    -    |    -    |    -    |   -   |
| 1 - ADD    |   3    |   4   |  -  | 0x01  |  reg  |   reg   |   reg   |    -    |   -   |
| 2 - SUB    |   3    |   4   |  -  | 0x02  |  reg  |   reg   |   reg   |    -    |   -   |
| 3 - LD1    |   4    |   5   |  -  | 0x03  |  reg  |  mem2  |  mem1  |  mem0  |   -   |
| 4 - ST1    |   4    |   5   |  -  | 0x04  |  reg  |  mem2  |  mem1  |  mem0  |   -   |
| 5 - LD2    |   4    |   5   |  -  | 0x05  |  reg  | mem2 |  mem1  |  mem0  |   -   |
| 6 - ST2    |   4    |   5   |  -  | 0x06  |  reg  |  mem2  |  mem1  |  mem0  |   -   |
| 7 - LD3    |   4    |   5   |  -  | 0x07  |  reg  |  mem2  |  mem1  |  mem0  |   -   |
| 8 - ST3    |   4    |   5   |  -  | 0x08  |  reg  |  mem2  |  mem1  |  mem0  |   -   |  
| 9 - LD4    |   4    |   5   |  -  | 0x09  |  reg  |  mem2  |  mem1  |  mem0  |   -   |
| 10 - ST4    |   4    |   5   |  -  | 0x0a  |  reg  |  mem2  |  mem1  |  mem0  |   -   | 
| 11 - LDI1   |   2    |   3   |  -  | 0x0b  |  reg  |  value  |    -    |    -    |   -   |  
| 12 - LDI2  |   2    |   4   |  -  | 0x0c  |  reg  | value1 | value0 |    -    |   -   |  
| 13 - LDI3  |   3    |   5   |  -  | 0x0d  |  reg  | value2 | value1 | value0 |   -   | 
| 14 - LDI4  |   3    |   6   |  -  | 0x0e  |  reg  | value3 | value2 | value1 |   value0   | 
| 15 - INC   |   3    |   2   |  -  | 0x0f  |  reg  |    -    |    -    |    -    |   -   |
| 16 - DEC   |   3    |   2   |  -  | 0x10  |  reg  |    -    |    -    |    -    |   -   |
| 17 - MUL   |   7-16   |   4   |  -  | 0x11  |  reg  |   reg   |   reg   |    -    |   -   |
| 18 - DIV   |   18-30   |   4   |  -  | 0x12  |  reg  |   reg   |   reg   |    -    |   -   |
| 19 - DIVR  |   18-30   |   4   |  -  | 0x13  |  reg  |   reg   |   reg   |    -    |   -   |
| 20 - ADC   |   4    |   4   |  -  | 0x14  |  reg  |   reg   |   reg   |    -    |   -   |
| 21 - SUC   |   4    |   4   |  -  | 0x15  |  reg  |   reg   |   reg   |    -    |   -   |
| 22 - NOP   |   1    |   1   |  -  | 0x16  |   -   |    -    |    -    |    -    |   -   |
| 23 - JMP   |   2    |   4   |  -  | 0x17  | mem2 |  mem1  |  mem0  |    -    |   -   |
| 24 - JSR   |   4    |   4   |  -  | 0x18  | mem2 |  mem1  |  mem0  |    -    |   -   |
| 25 - RFS   |   3    |   1   |  -  | 0x19  |   -   |    -    |    -    |    -    |   -   |
| 26 - JG    |   5    |   6   |  -  | 0x1a  |  reg  |   reg   |  mem2  |  mem1  | mem0 |
| 27 - JL    |   5    |   6   |  -  | 0x1b  |  reg  |   reg   |  mem2  |  mem1  | mem0 | 
| 28 - JE    |   5    |   6   |  -  | 0x1c  |  reg  |   reg   |  mem2  |  mem1  | mem0 |
| 29 - JC    |   2    |   4   |  -  | 0x1d  | mem2 |  mem1  |  mem0  |    -    |   -   |
| 30 - JNG   |   5    |   6   |  -  | 0x1e  |  reg  |   reg   |  mem2  |  mem1  | mem0 |
| 31 - JNL   |   5    |   6   |  -  | 0x1f  |  reg  |   reg   |  mem2  |  mem1  | mem0 |
| 32 - JNE   |   5    |   6   |  -  | 0x20  |  reg  |   reg   |  mem2  |  mem1  | mem0 |
| 33 - JNC   |   2    |   4   |  -  | 0x21  | mem2 |  mem1  |  mem0  |    -    |   -   |
| 34 - PUSH1  |   3    |   2   |  -  | 0x22  |  reg  |    -    |    -    |    -    |   -   |
| 35 - PUSH2  |   3    |   2   |  -  | 0x23  |  reg  |    -    |    -    |    -    |   -   |
| 36 - PUSH3  |   3    |   2   |  -  | 0x24  |  reg  |    -    |    -    |    -    |   -   |
| 37 - PUSH4  |   3    |   2   |  -  | 0x25  |  reg  |    -    |    -    |    -    |   -   |
| 38 - POP1  |   3    |   2   |  -  | 0x26  |  reg  |    -    |    -    |    -    |   -   |
| 39 - POP2  |   3    |   2   |  -  | 0x27  |  reg  |    -    |    -    |    -    |   -   |
| 40 - POP3  |   3    |   2   |  -  | 0x28  |  reg  |    -    |    -    |    -    |   -   |
| 41 - POP4  |   3    |   2   |  -  | 0x29  |  reg  |    -    |    -    |    -    |   -   |
| 42 - MOV   |   3    |   3   |  -  | 0x2a  |  reg  |   reg   |    -    |    -    |   -   |
| 43 - ADDE  |   3    |   3   |  -  | 0x2b  |  reg  |   reg   |    -    |    -    |   -   |
| 44 - SUBE  |   3    |   3   |  -  | 0x2c  |  reg  |   reg   |    -    |    -    |   -   |
| 45 - ADDI1 |   3    |   3   |  -  | 0x2d  |  reg  |  value  |    -    |    -    |   -   |
| 46 - ADDI2 |   3    |   4   |  -  | 0x2e  |  reg  | value1 | value0 |    -    |   -   |
| 47 - ADDI3 |   4    |   5   |  -  | 0x2f  |  reg  | value2 | value1 | value0 |   -   |
| 48 - ADDI4 |   4    |   6   |  -  | 0x30  |  reg  | value3 | value2 | value1 |   value0   |
| 49 - SUBI1 |   3    |   3   |  -  | 0x31  |  reg  |  value  |    -    |    -    |   -   |
| 50 - SUBI2 |   3    |   4   |  -  | 0x32  |  reg  | value1 | value0 |    -    |   -   |
| 51 - SUBI3 |   4    |   5   |  -  | 0x33  |  reg  | value2 | value1 | value0 |   -   |
| 52 - SUBI4 |   4    |   6   |  -  | 0x34  |  reg  | value3 | value2 | value1 |   value0   |
| 53 - MULI1 |   7-16   |   3   |  -  | 0x35  |  reg  |  value  |    -    |    -    |   -   |
| 54 - DIVI1 |   18-30   |   3   |  -  | 0x36  |  reg  |  value  |    -    |    -    |   -   |
| 55 - SEI   |   1    |   1   |  -  | 0x37  |   -   |    -    |    -    |    -    |   -   |
| 56 - SDI   |   1    |   1   |  -  | 0x38  |   -   |    -    |    -    |    -    |   -   |
| 57 - INT   |   5    |   2   |  -  | 0x39  |  ip   |    -    |    -    |    -    |   -   |
| 58 - RFI   |   5    |   1   |  -  | 0x3a  |   -   |    -    |    -    |    -    |   -   |
| 59 - HLT   |   1    |   1   |  -  | 0x3b  |   -   |    -    |    -    |    -    |   -   |
| 60 - LDR1  |   3    |   3   |  -  | 0x3c  |  reg  |   reg   |    -    |    -    |   -   |
| 61 - STR1  |   3    |   3   |  -  | 0x3d  |  reg  |   reg   |    -    |    -    |   -   |
| 62 - LDR2  |   3    |   3   |  -  | 0x3e  |  reg  |   reg   |    -    |    -    |   -   |
| 63 - STR2  |   3    |   3   |  -  | 0x3f  |  reg  |   reg   |    -    |    -    |   -   |
| 64 - LDR3  |   3    |   3   |  -  | 0x40  |  reg  |   reg   |    -    |    -    |   -   |
| 65 - STR3  |   3    |   3   |  -  | 0x41  |  reg  |   reg   |    -    |    -    |   -   |
| 66 - LDR4  |   3    |   3   |  -  | 0x42  |  reg  |   reg   |    -    |    -    |   -   |
| 67 - STR4  |   3    |   3   |  -  | 0x43  |  reg  |   reg   |    -    |    -    |   -   |
| 68 - ROL   |   3    |   2   |  -  | 0x44  |  reg  |    -    |    -    |    -    |   -   |
| 69 - ROR   |   3    |   2   |  -  | 0x45  |  reg  |    -    |    -    |    -    |   -   |
| 70 - SLL   |   3    |   2   |  -  | 0x46  |  reg  |    -    |    -    |    -    |   -   |
| 71 - SLR   |   3    |   2   |  -  | 0x47  |  reg  |    -    |    -    |    -    |   -   |
| 72 - SRR   |   3    |   2   |  -  | 0x48  |  reg  |    -    |    -    |    -    |   -   |
| 73 - AND   |   3    |   4   |  -  | 0x49  |  reg  |   reg   |   reg   |    -    |   -   |
| 74 - OR    |   3    |   4   |  -  | 0x4a  |  reg  |   reg   |   reg   |    -    |   -   |
| 75 - XOR   |   3    |   4   |  -  | 0x4b  |  reg  |   reg   |   reg   |    -    |   -   |
| 76 - NOT   |   3    |   2   |  -  | 0x4c  |  reg  |    -    |    -    |    -    |   -   |
| 77 - CBT8  |   6   |   2   |  -  | 0x4d  |  reg  |    -    |    -    |    -    |   -   |
| 78 - C8TB  |   6    |   2   |  -  | 0x4e  |  reg  |    -    |    -    |    -    |   -   |
| 79 - SJG    |   4    |   4   |  -  | 0x4f  |  reg  |   reg   |  mem  |  -  | - |
| 80 - SJL    |   4    |   4   |  -  | 0x50  |  reg  |   reg   |  mem  |  -  | - | 
| 81 - SJE    |   4    |   4   |  -  | 0x51  |  reg  |   reg   |  mem  |  -  | - |
| 82 - SJC    |   2    |   2   |  -  | 0x52  | mem |  -  |  - |    -    |   -   |
| 83 - SJNG   |   4    |   4   |  -  | 0x53  |  reg  |   reg   |  mem  |  -  | - |
| 84 - SJNL   |   4    |   4   |  -  | 0x54  |  reg  |   reg   |  mem  |  -  | - |
| 85 - SJNE   |   4    |   4   |  -  | 0x55  |  reg  |   reg   |  mem  |  -  | - |
| 86 - SJNC   |   2    |   2   |  -  | 0x56  | mem |  -  |  -  |    -    |   -   |
| 87 - SJMP   |   2    |   2   |  -  | 0x57  | mem |  -  |  -  |    -    |   -   |
| 88 - JGR    |   4    |   4   |  -  | 0x58  |  reg  |   reg   |  reg  |  -  | - |
| 89 - JLR    |   4    |   4   |  -  | 0x59  |  reg  |   reg   |  reg  |  -  | - | 
| 90 - JER    |   4    |   4   |  -  | 0x5a  |  reg  |   reg   |  reg  |  -  | - |
| 91 - JNGR   |   4    |   4   |  -  | 0x5b  |  reg  |   reg   |  reg  |  -  | - |
| 92 - JNLR   |   4    |   4   |  -  | 0x5c  |  reg  |   reg   |  reg  |  -  | - |
| 93 - JNER   |   4    |   4   |  -  | 0x5d  |  reg  |   reg   |  reg  |  -  | - |
| 94 - JMPR   |   2    |   2   |  -  | 0x6e  | reg |  -  |  -  |    -    |   -   |

### Interrupts

|  IP   | Interrupt Name |
|:-----:|:--------------:|
| 0-63 |    App    |
| 64-127 |      OS      | 
|  128-255   |   Devices    | 

    
# Memory
* 4MB RAM
* 4MB ROM


## Max Clock(sim)
* on Amd Ryzen 3 5350G
    -  **10** MHz
* on Amd Ryzen 9 5900x
    -  **11.5** MHz


## Assembler
 - Array->Const->Var
 - Function->App->OS