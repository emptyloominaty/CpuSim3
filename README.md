## CPU
* Architecture: **EMP 3 32-Bit**
* **Load–store** architecture
* Data Width: **32-bit** 
* Address Width: **32-bit**
* Bus: **32-bit** Data + **32-bit** Address
* Registers: 
    - 32 32-Bit Registers (**r0-r31**)  <br/>
    - 32-Bit Program Counter, 32-Bit Stack Pointer <br/>
* IPC: **0.27**
* Stack: **8192 Bytes**

        
|               |  Start   |   End    |
|---------------|:--------:|:--------:|
| Interrupt Pointers | 0x000000 | 0x0002FF |
| OS RAM        | 0x000300 | 0x001FFF |
| Stack         | 0x002000 | 0x003FFF |
| CPU Ram       | 0x000000 | 0x3FFFFF |
| ROM           | 0x400000 | 0x6FFFFF |
| OS ROM        | 0x700000 | 0x7FFFFF |
| Devices       | 0x800000 | 0xFFFFFF |

Reset Vector: 0x700000

TODO:
### Instructions
| OP - Name  | Cycles | Bytes |     | Byte1 | Byte2 |  Byte3  |  Byte4  |  Byte5  | Byte6 |
|------------|:------:|:-----:|:---:|:-----:|:-----:|:-------:|:-------:|:-------:|:-----:|   
| 0 - STOP   |   1    |   1   |  -  | 0x00  |   -   |    -    |    -    |    -    |   -   |
| 1 - ADD    |   4    |   4   |  -  | 0x01  |  reg  |   reg   |   reg   |    -    |   -   |
| 2 - SUB    |   4    |   4   |  -  | 0x02  |  reg  |   reg   |   reg   |    -    |   -   |
| 3 - LD1    |   4    |   5   |  -  | 0x03  |  reg  |  memHi  |  memMi  |  memLo  |   -   |
| 4 - ST1    |   4    |   5   |  -  | 0x04  |  reg  |  memHi  |  memMi  |  memLo  |   -   |
| 5 - LD2    |   4    |   5   |  -  | 0x05  |  reg  | valueHi |  memMi  |  memLo  |   -   |
| 6 - ST2    |   4    |   5   |  -  | 0x06  |  reg  |  memHi  |  memMi  |  memLo  |   -   |
| 7 - LD3    |   4    |   5   |  -  | 0x07  |  reg  |  memHi  |  memMi  |  memLo  |   -   |
| 8 - ST3    |   4    |   5   |  -  | 0x08  |  reg  |  memHi  |  memMi  |  memLo  |   -   |  
| 9 - LDI1   |   2    |   3   |  -  | 0x09  |  reg  |  value  |    -    |    -    |   -   |  
| 10 - LDI2  |   3    |   4   |  -  | 0x0a  |  reg  | valueHi | valueLo |    -    |   -   |  
| 11 - LDI3  |   3    |   5   |  -  | 0x0b  |  reg  | valueHi | valueMi | valueLo |   -   | 
| 12 - INC   |   3    |   2   |  -  | 0x0c  |  reg  |    -    |    -    |    -    |   -   |
| 13 - DEC   |   3    |   2   |  -  | 0x0d  |  reg  |    -    |    -    |    -    |   -   |
| 14 - MUL   |   24   |   4   |  -  | 0x0e  |  reg  |   reg   |   reg   |    -    |   -   |
| 15 - DIV   |   34   |   4   |  -  | 0x0f  |  reg  |   reg   |   reg   |    -    |   -   |
| 16 - DIVR  |   34   |   4   |  -  | 0x10  |  reg  |   reg   |   reg   |    -    |   -   |
| 17 - ADC   |   4    |   4   |  -  | 0x11  |  reg  |   reg   |   reg   |    -    |   -   |
| 18 - SUC   |   4    |   4   |  -  | 0x12  |  reg  |   reg   |   reg   |    -    |   -   |
| 19 - NOP   |   1    |   1   |  -  | 0x13  |   -   |    -    |    -    |    -    |   -   |
| 20 - JMP   |   3    |   4   |  -  | 0x14  | memHi |  memMi  |  memLo  |    -    |   -   |
| 21 - JSR   |   5    |   4   |  -  | 0x15  | memHi |  memMi  |  memLo  |    -    |   -   |
| 22 - RFS   |   5    |   1   |  -  | 0x16  |   -   |    -    |    -    |    -    |   -   |
| 23 - JG    |   5    |   6   |  -  | 0x17  |  reg  |   reg   |  memHi  |  memMi  | memLo |
| 24 - JL    |   5    |   6   |  -  | 0x18  |  reg  |   reg   |  memHi  |  memMi  | memLo |  
| 25 - JNG   |   5    |   6   |  -  | 0x19  |  reg  |   reg   |  memHi  |  memMi  | memLo |
| 26 - JNL   |   5    |   6   |  -  | 0x1a  |  reg  |   reg   |  memHi  |  memMi  | memLo |
| 27 - JE    |   5    |   6   |  -  | 0x1b  |  reg  |   reg   |  memHi  |  memMi  | memLo |
| 28 - JNE   |   5    |   6   |  -  | 0x1c  |  reg  |   reg   |  memHi  |  memMi  | memLo |
| 29 - JC    |   4    |   4   |  -  | 0x1d  | memHi |  memMi  |  memLo  |    -    |   -   |
| 30 - JNC   |   4    |   4   |  -  | 0x1e  | memHi |  memMi  |  memLo  |    -    |   -   |
| 31 - PSH1  |   2    |   2   |  -  | 0x1f  |  reg  |    -    |    -    |    -    |   -   |
| 32 - POP1  |   2    |   2   |  -  | 0x20  |  reg  |    -    |    -    |    -    |   -   |
| 33 - TRR   |   3    |   3   |  -  | 0x21  |  reg  |   reg   |    -    |    -    |   -   |
| 34 - TPR   |   3    |   2   |  -  | 0x22  |  reg  |    -    |    -    |    -    |   -   |
| 35 - TPR   |   3    |   2   |  -  | 0x23  |  reg  |    -    |    -    |    -    |   -   |
| 36 - TRS   |   3    |   2   |  -  | 0x24  |  reg  |    -    |    -    |    -    |   -   |
| 37 - TSR   |   3    |   2   |  -  | 0x25  |  reg  |    -    |    -    |    -    |   -   |
| 38 - TCR   |   3    |   2   |  -  | 0x26  |  reg  |    -    |    -    |    -    |   -   |
| 39 - TRC   |   3    |   2   |  -  | 0x27  |  reg  |    -    |    -    |    -    |   -   |
| 40 - AD2   |   3    |   3   |  -  | 0x28  |  reg  |   reg   |    -    |    -    |   -   |
| 41 - SU2   |   3    |   3   |  -  | 0x29  |  reg  |   reg   |    -    |    -    |   -   |
| 42 - ADDI1 |   3    |   3   |  -  | 0x2a  |  reg  |  value  |    -    |    -    |   -   |
| 43 - SUBI1 |   3    |   3   |  -  | 0x2b  |  reg  |  value  |    -    |    -    |   -   |
| 44 - MULI1 |   23   |   3   |  -  | 0x2c  |  reg  |  value  |    -    |    -    |   -   |
| 45 - DIVI1 |   33   |   3   |  -  | 0x2d  |  reg  |  value  |    -    |    -    |   -   |
| 46 - ADDI2 |   4    |   4   |  -  | 0x2e  |  reg  | valueHi | valueLo |    -    |   -   |
| 47 - SUBI2 |   4    |   4   |  -  | 0x2f  |  reg  | valueHi | valueLo |    -    |   -   |
| 48 - ADDI3 |   4    |   5   |  -  | 0x30  |  reg  | valueLo | valueMi | valueLo |   -   |
| 49 - SUBI3 |   4    |   5   |  -  | 0x31  |  reg  | valueLo | valueMi | valueLo |   -   |
| 50 - STAIP |   4    |   5   |  -  | 0x32  |  ip   |  memHi  |  memMi  |  memLo  |   -   |
| 51 - SEI   |   2    |   1   |  -  | 0x33  |   -   |    -    |    -    |    -    |   -   |
| 52 - SDI   |   2    |   1   |  -  | 0x34  |   -   |    -    |    -    |    -    |   -   |
| 53 - INT   |   5    |   2   |  -  | 0x35  |  ip   |    -    |    -    |    -    |   -   |
| 54 - RFI   |   5    |   1   |  -  | 0x36  |   -   |    -    |    -    |    -    |   -   |
| 55 - PSH2  |   2    |   2   |  -  | 0x37  |  reg  |    -    |    -    |    -    |   -   |
| 56 - POP2  |   2    |   2   |  -  | 0x38  |  reg  |    -    |    -    |    -    |   -   |
| 57 - PSH3  |   2    |   2   |  -  | 0x39  |  reg  |    -    |    -    |    -    |   -   |
| 58 - POP3  |   2    |   2   |  -  | 0x3a  |  reg  |    -    |    -    |    -    |   -   |
| 59 - HLT   |   1    |   1   |  -  | 0x3b  |   -   |    -    |    -    |    -    |   -   |
| 60 - LDR1  |   3    |   3   |  -  | 0x3c  |  reg  |   reg   |    -    |    -    |   -   |
| 61 - STR1  |   3    |   3   |  -  | 0x3d  |  reg  |   reg   |    -    |    -    |   -   |
| 62 - LDR2  |   3    |   3   |  -  | 0x3e  |  reg  |   reg   |    -    |    -    |   -   |
| 63 - STR2  |   3    |   3   |  -  | 0x3f  |  reg  |   reg   |    -    |    -    |   -   |
| 64 - LDR3  |   3    |   3   |  -  | 0x40  |  reg  |   reg   |    -    |    -    |   -   |
| 65 - STR3  |   3    |   3   |  -  | 0x41  |  reg  |   reg   |    -    |    -    |   -   |
| 70 - ROL   |   3    |   2   |  -  | 0x46  |  reg  |    -    |    -    |    -    |   -   |
| 71 - ROR   |   3    |   2   |  -  | 0x47  |  reg  |    -    |    -    |    -    |   -   |
| 72 - SLL   |   3    |   2   |  -  | 0x48  |  reg  |    -    |    -    |    -    |   -   |
| 73 - SLR   |   3    |   2   |  -  | 0x49  |  reg  |    -    |    -    |    -    |   -   |
| 74 - SRR   |   3    |   2   |  -  | 0x4a  |  reg  |    -    |    -    |    -    |   -   |
| 75 - AND   |   4    |   4   |  -  | 0x4b  |  reg  |   reg   |   reg   |    -    |   -   |
| 76 - OR    |   4    |   4   |  -  | 0x4c  |  reg  |   reg   |   reg   |    -    |   -   |
| 77 - XOR   |   4    |   4   |  -  | 0x4d  |  reg  |   reg   |   reg   |    -    |   -   |
| 78 - NOT   |   4    |   2   |  -  | 0x4e  |  reg  |    -    |    -    |    -    |   -   |
| 80 - CBT8  |   10   |   2   |  -  | 0x50  |  reg  |    -    |    -    |    -    |   -   |
| 81 - C8TB  |   6    |   2   |  -  | 0x51  |  reg  |    -    |    -    |    -    |   -   |

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


        