using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Controls;

namespace CpuSim3 {
    public class Cpu {
        public Thread cpuThread;
        public OpCode[] opCodes;

        public bool maxClock = true;
        public long clockSet = 100000;
        public long clock = 0;

        public bool cpuRunning = false;
        public bool threadRunning = true;

        public long timeStart;
        public long timeEnd;
        public long timeA;
        public long timeB;
        public long timeC;
        public long timeD;
        public long timeClockA = DateTime.Now.Ticks * 100;
        public long timeClockB = DateTime.Now.Ticks * 100;

        public bool halted = false;
        public bool interruptHw = false;

        public long cyclesDone = 0;
        public long cyclesDoneA = 0;
        public long cyclesDoneB = 0;
        public long instructionsDone = 0;

        public int cyclesTotal = 0;
        public int cyclesExecuting = 0;

        public byte cpuPhase = 0;
        public byte op = 0;
        public byte bytes = 0;
        public byte cycles = 0;
        public byte cyclesI = 0;
        public byte bytesLeft = 0;
        public byte memCurrentCycleLeft = 4;
        public byte[] instructionData = new byte[6];
        public byte instructionDataIdx = 0;
        public bool fetchedOpCode = false;
        public int waitCycles = 0;
        public int interruptId = 0;

        public uint[] registers;
        //r0-r31
        //(r32)
        //Program Counter(r33)
        //Stack Pointer(r34)
        //Carry(r35)
        //Overflow(r36)
        //Interrupt Disabled(r37)

        public float[] registersF;
        public double[] registersD;


        public Cpu(OpCode[] opCodes) {
            this.opCodes = opCodes;
            Reset();
        }

        public void Reset() {
            registers = new uint[38];
            for (int i = 0; i < registers.Length; i++) {
                registers[i] = 0;
            }
            registers[33] = 7340032;
            registers[34] = 8192;
            registersF = new float[8];
            registersD = new double[8];

            cyclesDone = 0;
            cyclesDoneA = 0;
            instructionsDone = 0;
            cpuPhase = 0;
            op = 0;
            bytes = 0;
            cycles = 0;
            cyclesI = 0;
            bytesLeft = 0;
            memCurrentCycleLeft = 4;
            instructionData = new byte[6];
            instructionDataIdx = 0;
            timeStart = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            timeA = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            timeB = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            timeC = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            timeD = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            cyclesDoneA = 0;
            cyclesDoneB = 0;
            waitCycles = 0;
            interruptHw = false;
            halted = false;
            cyclesTotal = 0;
            cyclesExecuting = 0;
        }

        public void StartCpu() {
            cpuThread = new Thread(new ThreadStart(Run));
            cpuThread.IsBackground = true;
            cpuThread.Start();
        }

        public void Run() {
            long timeClockD = 0;
            while (threadRunning) {
                long timeClockWait = 1000000000 / clockSet;
                while (cpuRunning) {
                    if (maxClock) {
                        Main();
                    } else {
                        timeClockA = DateTime.Now.Ticks * 100;
                        Main();
                        timeClockD = 0;
                        while (timeClockD < timeClockWait) {
                            timeClockB = DateTime.Now.Ticks * 100;
                            timeClockD = timeClockB - timeClockA;
                        }
                    }
                }
                try {
                    Thread.Sleep(100);
                } catch (ThreadInterruptedException e) {
                    throw new Exception("InterruptedException occurred", e);
                }
            }
        }

        public void Main() {
            cyclesDoneA++;
            memCurrentCycleLeft = 4;
            fetchedOpCode = false;
            timeA = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            if (timeA - timeB > 1000) {
                clock = cyclesDoneA - cyclesDoneB;
                timeB = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                cyclesDoneB = cyclesDoneA;
                if (timeA - timeD > 5000) {
                    cyclesTotal /= 2;
                    cyclesExecuting /= 2;
                    timeD = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                }
            }
            if (timeA - timeC > 33.3) {
                timeC = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                timeEnd = timeC;
            }
            cyclesTotal++;
            if (halted) {
                return;
            }
            cyclesExecuting++;
            cyclesDone++;
            switch (cpuPhase) {
                case 0:
                    if (waitCycles > 0) {
                        waitCycles--;
                        return;
                    }
                    if (interruptHw) {
                        interruptHw = false;
                        op = 57;
                        instructionData[0] = 57;
                        instructionData[1] = (byte)interruptId;
                        cpuPhase = 2;
                        cyclesI = 0;
                        break;
                    }
                    FetchOpCode();
                    break;
                case 1:
                    FetchOtherBytes();
                    break;
                case 2:
                    if (cyclesI > 0) {
                        cyclesI--;
                    }
                    if (cyclesI <= 0) {
                        Execute();
                    }
                    break;
            }
        }

        public void FetchOpCode() {
            for (byte i = 0; i < instructionData.Length; i++) {
                instructionData[i] = 0;
            }

            byte val = LoadByte(registers[33]);
            op = val;
            bytes = opCodes[val].bytes;
            bytesLeft = (byte)(bytes - 1);
            cycles = opCodes[val].cycles;
            cyclesI = (byte)(cycles - 1);
            instructionDataIdx = 1;
            memCurrentCycleLeft = 3;
            instructionData[0] = val;

            fetchedOpCode = true;
            registers[33]++;
            cpuPhase++;
            if (bytes == 1 && cyclesI == 0) {
                Execute();
            } else if (bytes != 1) {
                FetchOtherBytes();
                switch (op) {//xd
                    case 17:
                        cyclesI += (byte)Math.Log(registers[instructionData[1]] + registers[instructionData[2]],12);
                        break;
                    case 18:
                        cyclesI += (byte)Math.Log(registers[instructionData[1]] + registers[instructionData[2]],7);
                        break;
                    case 19:
                        cyclesI += (byte)Math.Log(registers[instructionData[1]] + registers[instructionData[2]],7);
                        break;
                    case 53:
                        cyclesI += (byte)Math.Log(registers[instructionData[1]] + instructionData[2],12);
                        break;
                    case 54:
                        cyclesI += (byte)Math.Log(registers[instructionData[1]] + instructionData[2],7);
                        break;
                }
            }
            if (op == 0) {
                StopCpu();
            }
        }

        public void FetchOtherBytes() {
            while (memCurrentCycleLeft > 0 && bytesLeft > 0) {
                byte val = LoadByte(registers[33]);
                memCurrentCycleLeft--;
                instructionData[instructionDataIdx] = val;
                instructionDataIdx++;
                registers[33]++;
                bytesLeft--;
            }
            if (!fetchedOpCode) {
                cyclesI--;
            }
            if (bytesLeft <= 0) {
                cpuPhase++;
            }
            if (cyclesI == 0) {
                Execute();
            }
        }

        public void Execute() {
            instructionsDone++;
            uint val;
            uint load;
            byte[] store;
            byte byte1;
            byte byte2;
            byte byte3; 
            byte byte4;
            byte[] bytes;
            uint address1;
            uint address2;
            uint address3;
            uint address4;
            switch (op) {
                case 1: //ADD  r1+r2=r3
                    val = registers[instructionData[1]] + registers[instructionData[2]];
                    registers[instructionData[3]] = val;
                    if (val < registers[instructionData[1]] || val < registers[instructionData[2]]) {
                        registers[35] = 1;
                    } else {
                        registers[35] = 0;
                    }

                    // Detecting overflow
                    if (val < registers[instructionData[1]] && val < registers[instructionData[2]]) {
                        registers[36] = 1;
                    } else {
                        registers[36] = 0;
                    }

                    break;
                case 2: //SUB r1-r2=r3
                    val = registers[instructionData[1]] - registers[instructionData[2]];
                    registers[instructionData[3]] = val;
                    break;
                case 3: //LD1
                    registers[instructionData[1]] = Memory.Read(Functions.ConvertTo24Bit(instructionData[2], instructionData[3], instructionData[4]));
                    break;
                case 4: //ST1
                    Memory.Write(Functions.ConvertTo24Bit(instructionData[2], instructionData[3], instructionData[4]), (byte)registers[instructionData[1]]);
                    break;
                case 5: //LD2
                    load = Functions.ConvertTo16Bit(Memory.Read(Functions.ConvertTo24Bit(instructionData[2], instructionData[3], instructionData[4])),
                        Memory.Read(Functions.ConvertTo24Bit(instructionData[2], instructionData[3], instructionData[4]) + 1));
                    registers[instructionData[1]] = load;
                    break;
                case 6: //ST2
                    store = Functions.ConvertFrom16Bit(registers[instructionData[1]]);
                    Memory.Write(Functions.ConvertTo24Bit(instructionData[2], instructionData[3], instructionData[4]), store[0]);
                    Memory.Write(Functions.ConvertTo24Bit(instructionData[2], instructionData[3], instructionData[4]) + 1, store[1]);
                    break;
                case 7: //LD3
                    byte1 = Memory.Read(Functions.ConvertTo24Bit(instructionData[2], instructionData[3], instructionData[4]));
                    byte2 = Memory.Read(Functions.ConvertTo24Bit(instructionData[2], instructionData[3], instructionData[4]) + 1);
                    byte3 = Memory.Read(Functions.ConvertTo24Bit(instructionData[2], instructionData[3], instructionData[4]) + 2);
                    load = Functions.ConvertTo24Bit(byte1, byte2, byte3);
                    registers[instructionData[1]] = load;
                    break;
                case 8: //ST3
                    address1 = Functions.ConvertTo24Bit(instructionData[2], instructionData[3], instructionData[4]);
                    address2 = Functions.ConvertTo24Bit(instructionData[2], instructionData[3], instructionData[4]) + 1;
                    address3 = Functions.ConvertTo24Bit(instructionData[2], instructionData[3], instructionData[4]) + 2;
                    store = Functions.ConvertFrom24Bit(registers[instructionData[1]]);
                    Memory.Write(address1, store[0]);
                    Memory.Write(address2, store[1]);
                    Memory.Write(address3, store[2]);
                    break;
                case 9: //LD4
                    byte1 = Memory.Read(Functions.ConvertTo24Bit(instructionData[2], instructionData[3], instructionData[4]));
                    byte2 = Memory.Read(Functions.ConvertTo24Bit(instructionData[2], instructionData[3], instructionData[4]) + 1);
                    byte3 = Memory.Read(Functions.ConvertTo24Bit(instructionData[2], instructionData[3], instructionData[4]) + 2);
                    byte4 = Memory.Read(Functions.ConvertTo24Bit(instructionData[2], instructionData[3], instructionData[4]) + 3);
                    load = Functions.ConvertTo32Bit(byte1, byte2, byte3, byte4);
                    registers[instructionData[1]] = load;
                    break;
                case 10: //ST4
                    address1 = Functions.ConvertTo24Bit(instructionData[2], instructionData[3], instructionData[4]);
                    address2 = Functions.ConvertTo24Bit(instructionData[2], instructionData[3], instructionData[4]) + 1;
                    address3 = Functions.ConvertTo24Bit(instructionData[2], instructionData[3], instructionData[4]) + 2;
                    address4 = Functions.ConvertTo24Bit(instructionData[2], instructionData[3], instructionData[4]) + 3;
                    store = Functions.ConvertFrom32Bit(registers[instructionData[1]]);
                    Memory.Write(address1, store[0]);
                    Memory.Write(address2, store[1]);
                    Memory.Write(address3, store[2]);
                    Memory.Write(address4, store[3]);
                    break;
                case 11: //LDI1
                    registers[instructionData[1]] = instructionData[2];
                    break;
                case 12: //LDI2
                    registers[instructionData[1]] = Functions.ConvertTo16Bit(instructionData[2], instructionData[3]);
                    break;
                case 13: //LDI3
                    registers[instructionData[1]] = Functions.ConvertTo24Bit(instructionData[2], instructionData[3], instructionData[4]);
                    break;
                case 14: //LDI4
                    registers[instructionData[1]] = Functions.ConvertTo32Bit(instructionData[2], instructionData[3], instructionData[4], instructionData[5]);
                    break;
                case 15: //INC
                    registers[instructionData[1]]++;
                    //TODO: carry
                    break;
                case 16: //DEC
                    registers[instructionData[1]]--;
                    break;
                case 17: //MUL
                    val = registers[instructionData[1]] * registers[instructionData[2]];
                    registers[instructionData[3]] = val;
                    if (registers[instructionData[1]] != 0 && val / registers[instructionData[1]] != registers[instructionData[2]]) {
                        registers[36] = 1;
                    } else {
                        registers[36] = 0;
                    }
                    break;
                case 18: //DIV
                    if (registers[instructionData[2]] != 0) {
                        val = registers[instructionData[1]] / registers[instructionData[2]];
                    } else {
                        val = 0;
                    }
                    registers[instructionData[3]] = val;
                    break;
                case 19: //DIVR
                    if (registers[instructionData[2]] != 0) {
                        val = registers[instructionData[1]] % registers[instructionData[2]];
                    } else {
                        val = 0;
                    }
                    registers[instructionData[3]] = val;
                    break;
                case 20: //ADC
                    val = registers[instructionData[1]] + registers[instructionData[2]];
                    if (registers[35]==1) {
                        val++;
                    }
                    registers[instructionData[3]] = val;
                    //TODO: carry
                    break;
                case 21: //SUC
                    val = registers[instructionData[1]] - registers[instructionData[2]];
                    if (registers[35] == 1) {
                        val--;
                    }
                    registers[instructionData[3]] = val;
                    break;
                case 22: //NOP
                    break;
                case 23: //JMP
                    registers[33] = Functions.ConvertTo24Bit(instructionData[1], instructionData[2], instructionData[3]);
                    break;
                case 24: //JSR
                    bytes = Functions.ConvertFrom24Bit(registers[33]);
                    Memory.Write(registers[34], bytes[0]);
                    registers[34]++; 
                    Memory.Write(registers[34], bytes[1]);
                    registers[34]++;
                    Memory.Write(registers[34], bytes[2]);
                    registers[34]++;
                    registers[33] = Functions.ConvertTo24Bit(instructionData[1], instructionData[2], instructionData[3]);
                    break;
                case 25: //RFS
                    registers[34]--;
                    byte3 = Memory.Read(registers[34]);
                    registers[34]--;
                    byte2 = Memory.Read(registers[34]);
                    registers[34]--;
                    byte1 = Memory.Read(registers[34]);
                    registers[33] = Functions.ConvertTo24Bit(byte1, byte2, byte3);
                    break;
                case 26: //JG
                    if (registers[instructionData[1]] > registers[instructionData[2]]) {
                        registers[33] = Functions.ConvertTo24Bit(instructionData[3], instructionData[4], instructionData[5]);
                    }
                    break;
                case 27: //JL
                    if (registers[instructionData[1]] < registers[instructionData[2]]) {
                        registers[33] = Functions.ConvertTo24Bit(instructionData[3], instructionData[4], instructionData[5]);
                    }
                    break;
                case 28: //JE
                    if (registers[instructionData[1]] == registers[instructionData[2]]) {
                        registers[33] = Functions.ConvertTo24Bit(instructionData[3], instructionData[4], instructionData[5]);
                    }
                    break;
                case 29: //JC
                    if (registers[35] == 1) {
                        registers[33] = Functions.ConvertTo24Bit(instructionData[3], instructionData[4], instructionData[5]);
                        registers[35] = 0;
                    }
                    break;
                case 30: //JNG
                    if (!(registers[instructionData[1]] > registers[instructionData[2]])) {
                        registers[33] = Functions.ConvertTo24Bit(instructionData[3], instructionData[4], instructionData[5]);
                    }
                    break;
                case 31: //JNL
                    if (!(registers[instructionData[1]] < registers[instructionData[2]])) {
                        registers[33] = Functions.ConvertTo24Bit(instructionData[3], instructionData[4], instructionData[5]);
                    }
                    break;
     
                case 32: //JNE
                    if (!(registers[instructionData[1]] == registers[instructionData[2]])) {
                        registers[33] = Functions.ConvertTo24Bit(instructionData[3], instructionData[4], instructionData[5]);
                    }
                    break;
                case 33: //JNC
                    if (registers[35]==0) {
                        registers[33] = Functions.ConvertTo24Bit(instructionData[3], instructionData[4], instructionData[5]);
                    }
                    break;
                case 34: //PSH1
                    bytes = Functions.ConvertFrom24Bit(registers[instructionData[1]]);
                    Memory.Write(registers[34], bytes[2]);
                    registers[34]++;
                    break;
                case 35: //PSH2
                    bytes = Functions.ConvertFrom24Bit(registers[instructionData[1]]);
                    Memory.Write(registers[34], bytes[1]);
                    registers[34]++;
                    Memory.Write(registers[34], bytes[2]);
                    registers[34]++;
                    break;
                case 36: //PSH3
                    bytes = Functions.ConvertFrom24Bit(registers[instructionData[1]]);
                    Memory.Write(registers[34], bytes[0]);
                    registers[34]++;
                    Memory.Write(registers[34], bytes[1]);
                    registers[34]++;
                    Memory.Write(registers[34], bytes[2]);
                    registers[34]++;
                    break;
                case 37: //PSH4
                    bytes = Functions.ConvertFrom32Bit(registers[instructionData[1]]);
                    Memory.Write(registers[34], bytes[0]);
                    registers[34]++;
                    Memory.Write(registers[34], bytes[1]);
                    registers[34]++;
                    Memory.Write(registers[34], bytes[2]);
                    registers[34]++;
                    Memory.Write(registers[34], bytes[3]);
                    registers[34]++;
                    break;
                case 38: //POP1
                    registers[34]--;
                    byte3 = Memory.Read(registers[34]);
                    registers[instructionData[1]] = Functions.ConvertTo24Bit(0, 0, byte3);
                    break;
                case 39: //POP2
                    byte1 = 0;
                    registers[34]--;
                    byte3 = Memory.Read(registers[34]);
                    registers[34]--;
                    byte2 = Memory.Read(registers[34]);
                    registers[instructionData[1]] = Functions.ConvertTo24Bit(byte1, byte2, byte3);
                    break;
                case 40: //POP3
                    registers[34]--;
                    byte3 = Memory.Read(registers[34]);
                    registers[34]--;
                    byte2 = Memory.Read(registers[34]);
                    registers[34]--;
                    byte1 = Memory.Read(registers[34]);
                    registers[instructionData[1]] = Functions.ConvertTo24Bit(byte1, byte2, byte3);
                    break;
                case 41: //POP4
                    registers[34]--;
                    byte4 = Memory.Read(registers[34]);
                    registers[34]--;
                    byte3 = Memory.Read(registers[34]);
                    registers[34]--;
                    byte2 = Memory.Read(registers[34]);
                    registers[34]--;
                    byte1 = Memory.Read(registers[34]);
                    registers[instructionData[1]] = Functions.ConvertTo32Bit(byte1, byte2, byte3, byte4);
                    break;
                case 42: //MOV
                    registers[instructionData[2]] = registers[instructionData[1]];
                    break;
                case 43: //ADE
                    val = registers[instructionData[1]] + registers[instructionData[2]];
                    registers[instructionData[1]] = val;
                    break;
                case 44: //SUE
                    val = registers[instructionData[1]] - registers[instructionData[2]];
                    registers[instructionData[1]] = val;
                    break;
                case 45: //ADDI1
                    val = registers[instructionData[1]] + instructionData[2];
                    registers[instructionData[1]] = val;
                    break;
                case 46: //ADDI2
                    val = registers[instructionData[1]] + Functions.ConvertTo16Bit(instructionData[2], instructionData[3]);
                    registers[instructionData[1]] = val;
                    break;
                case 47: //ADDI3
                    val = registers[instructionData[1]] + Functions.ConvertTo24Bit(instructionData[2], instructionData[3], instructionData[4]);
                    registers[instructionData[1]] = val;
                    break;
                case 48: //ADDI4
                    val = registers[instructionData[1]] + Functions.ConvertTo32Bit(instructionData[2], instructionData[3], instructionData[4], instructionData[5]);
                    registers[instructionData[1]] = val;
                    //TODO: carry
                    break;
                case 49: //SUBI1
                    val = registers[instructionData[1]] - instructionData[2];
                    registers[instructionData[1]] = val;
                    break;
                case 50: //SUBI2
                    val = registers[instructionData[1]] - Functions.ConvertTo16Bit(instructionData[2], instructionData[3]);
                    registers[instructionData[1]] = val;
                    break;
                case 51: //SUBI3
                    val = registers[instructionData[1]] - Functions.ConvertTo24Bit(instructionData[2], instructionData[3], instructionData[4]);
                    registers[instructionData[1]] = val;
                    break;
                case 52: //SUBI4
                    val = registers[instructionData[1]] - Functions.ConvertTo32Bit(instructionData[2], instructionData[3], instructionData[4], instructionData[5]);
                    registers[instructionData[1]] = val;
                    break;
                case 53: //MULI1
                    val = registers[instructionData[1]] * instructionData[2];
                    registers[instructionData[1]] = val;
                    if (registers[instructionData[1]] != 0 && val / registers[instructionData[1]] != instructionData[2]) {
                        registers[36] = 1;
                    } else {
                        registers[36] = 0;
                    }
                    break;
                case 54: //DIVI1
                    if (registers[instructionData[2]] != 0) {
                        val = registers[instructionData[1]] / instructionData[2];
                    } else {
                        val = 0;
                    }
                    registers[instructionData[1]] = val;
                    break;
                case 55: //SEI
                    registers[37] = 0;
                    break;
                case 56: //DEI
                    registers[37] = 1;
                    break;
                case 57: //INT
                    if (registers[37]==0) {
                        bytes = Functions.ConvertFrom24Bit(registers[33]);
                        Memory.Write(registers[34], bytes[0]);
                        registers[34]++;
                        Memory.Write(registers[34], bytes[1]);
                        registers[34]++;
                        Memory.Write(registers[34], bytes[2]);
                        registers[34]++;

            
                        Memory.Write(registers[34], (byte)registers[35]);

                        registers[34]++;
                        Memory.Write(registers[34], (byte)registers[36]);
 
                        registers[34]++;
                        registers[33] = Functions.ConvertTo24Bit(Memory.Read((uint)instructionData[1] * 3), Memory.Read(((uint)instructionData[1] * 3) + 1), Memory.Read(((uint)instructionData[1] * 3) + 2));
                    }
                    break;
                case 58: //RFI
                    registers[34]--;
                    registers[36] = Memory.Read(registers[34]);
                    registers[34]--;
                    registers[35] = Memory.Read(registers[34]);
                    registers[34]--;
                    byte3 = Memory.Read(registers[34]);
                    registers[34]--;
                    byte2 = Memory.Read(registers[34]);
                    registers[34]--;
                    byte1 = Memory.Read(registers[34]);
                    registers[33] = Functions.ConvertTo24Bit(byte1, byte2, byte3);
                    break;
                case 59: //HLT
                    halted = true;
                    registers[37] = 0;
                    break;

                case 60: //LDR1
                    registers[instructionData[1]] = Memory.Read(registers[instructionData[2]]);
                    break;
                case 61: //STR1
                    Memory.Write(registers[instructionData[2]], (byte)registers[instructionData[1]]);
                    break;
                case 62: //LDR2
                    load = Functions.ConvertTo16Bit(Memory.Read(registers[instructionData[2]]),
                            (Memory.Read(registers[instructionData[2]] + 1)));
                    registers[instructionData[1]] = load;
                    break;
                case 63: //STR2
                    store = Functions.ConvertFrom16Bit(registers[instructionData[1]]);
                    Memory.Write(registers[instructionData[2]], store[0]);
                    Memory.Write(registers[instructionData[2]] + 1, store[1]);
                    break;
                case 64: //LDR3
                    load = Functions.ConvertTo24Bit(
                             Memory.Read(registers[instructionData[2]]),
                            (Memory.Read(registers[instructionData[2]] + 1)),
                            (Memory.Read(registers[instructionData[2]] + 2)));
                    registers[instructionData[1]] = load;
                    break;
                case 65: //STR3
                    store = Functions.ConvertFrom24Bit(registers[instructionData[1]]);
                    Memory.Write(registers[instructionData[2]], store[0]);
                    Memory.Write(registers[instructionData[2]] + 1, store[1]);
                    Memory.Write(registers[instructionData[2]] + 2, store[2]);
                    break;
                case 66: //LDR4
                    load = Functions.ConvertTo32Bit(
                             Memory.Read(registers[instructionData[2]]),
                            (Memory.Read(registers[instructionData[2]] + 1)),
                            (Memory.Read(registers[instructionData[2]] + 2)),
                            (Memory.Read(registers[instructionData[2]] + 3)));
                    registers[instructionData[1]] = load;
                    break;
                case 67: //STR4
                    store = Functions.ConvertFrom32Bit(registers[instructionData[1]]);
                    Memory.Write(registers[instructionData[2]], store[0]);
                    Memory.Write(registers[instructionData[2]] + 1, store[1]);
                    Memory.Write(registers[instructionData[2]] + 2, store[2]);
                    Memory.Write(registers[instructionData[2]] + 3, store[3]);
                    break;
                case 68: //ROL
                    val = (registers[instructionData[1]] << 1) | (registers[instructionData[1]] >> (32 - 1));
                    registers[instructionData[1]] = val;
                    break;
                case 69: //ROR
                    val = (registers[instructionData[1]] >> 1) | (registers[instructionData[1]] << (32 - 1));
                    registers[instructionData[1]] = val;
                    break;
                case 70: //SLL
                    val = registers[instructionData[1]] << 1;
                    registers[instructionData[1]] = val;
                    break;
                case 71: //SLR
                    val = registers[instructionData[1]] >>> 1;
                    registers[instructionData[1]] = val;
                    break;
                case 72: //SRR
                    val = registers[instructionData[1]] >> 1;
                    registers[instructionData[1]] = val;
                    break;
                case 73: //AND
                    val = registers[instructionData[1]] & registers[instructionData[2]];
                    registers[instructionData[3]] = val;
                    break;
                case 74: //OR
                    val = registers[instructionData[1]] | registers[instructionData[2]];
                    registers[instructionData[3]] = val;
                    break;
                case 75: //XOR
                    val = registers[instructionData[1]] ^ registers[instructionData[2]];
                    registers[instructionData[3]] = val;
                    break;
                case 76: //NOT
                    val = ~registers[instructionData[1]];
                    registers[instructionData[1]] = val;
                    break;
                case 77: //CBT8
                    val = registers[instructionData[1]];
                    registers[instructionData[1]] = val & 0x01;
                    registers[instructionData[1] + 1] = (val >> 1) & 0x01;
                    registers[instructionData[1] + 2] = (val >> 2) & 0x01;
                    registers[instructionData[1] + 3] = (val >> 3) & 0x01;
                    registers[instructionData[1] + 4] = (val >> 4) & 0x01;
                    registers[instructionData[1] + 5] = (val >> 5) & 0x01;
                    registers[instructionData[1] + 6] = (val >> 6) & 0x01;
                    registers[instructionData[1] + 7] = (val >> 7) & 0x01;
                    break;
                case 78: //C8TB
                    val = registers[instructionData[1]];
                    val |= registers[instructionData[1] + 1] << 1;
                    val |= registers[instructionData[1] + 2] << 2;
                    val |= registers[instructionData[1] + 3] << 3;
                    val |= registers[instructionData[1] + 4] << 4;
                    val |= registers[instructionData[1] + 5] << 5;
                    val |= registers[instructionData[1] + 6] << 6;
                    val |= registers[instructionData[1] + 7] << 7;
                    registers[instructionData[1]] = val;
                    break;
                case 79: //SJG  
                    if (registers[instructionData[1]] > registers[instructionData[2]]) {
                        registers[33] = registers[33] + instructionData[3] - 128;
                    }
                    break;

                case 80: //SJL
                    if (registers[instructionData[1]] < registers[instructionData[2]]) {
                        registers[33] = registers[33] + instructionData[3] - 128;
                    }
                    break;
                case 81: //SJE
                    if (registers[instructionData[1]] == registers[instructionData[2]]) {
                        registers[33] = registers[33] + instructionData[3] - 128;
                    }
                    break;

                case 82: //SJC
                    if (registers[35] == 1) {
                        registers[33] = registers[33] + instructionData[1] - 128;
                        registers[35] = 0;
                    }
                    break;
                case 83: //SJNG
                    if (!(registers[instructionData[1]] > registers[instructionData[2]])) {
                        registers[33] = registers[33] + instructionData[3] - 128;
                    }
                    break;
                case 84: //SJNL
                    if (!(registers[instructionData[1]] < registers[instructionData[2]])) {
                        registers[33] = registers[33] + instructionData[3] - 128;
                    }
                    break;

                case 85: //SJNE
                    if (!(registers[instructionData[1]] == registers[instructionData[2]])) {
                        registers[33] = registers[33] + instructionData[3] - 128;
                    }
                    break;
                case 86: //SJNC
                    if (registers[35] == 0) {
                        registers[33] = registers[33] + instructionData[1] - 128;
                    }
                    break;
                case 87: //SJMP
                    registers[33] = registers[33] + instructionData[1] - 128;
                    break;
                case 88: //JGR
                    if (registers[instructionData[1]] > registers[instructionData[2]]) {
                        registers[33] = registers[instructionData[3]];
                    }
                    break;

                case 89: //JLR
                    if (registers[instructionData[1]] < registers[instructionData[2]]) {
                        registers[33] = registers[instructionData[3]];
                    }
                    break;
                case 90: //JER
                    if (registers[instructionData[1]] == registers[instructionData[2]]) {
                        registers[33] = registers[instructionData[3]];
                    }
                    break;

                case 91: //JNGR
                    if (!(registers[instructionData[1]] > registers[instructionData[2]])) {
                        registers[33] = registers[instructionData[3]];
                    }
                    break;
                case 92: //JNLR
                    if (!(registers[instructionData[1]] < registers[instructionData[2]])) {
                        registers[33] = registers[instructionData[3]];
                    }
                    break;

                case 93: //JNER
                    if (!(registers[instructionData[1]] == registers[instructionData[2]])) {
                        registers[33] = registers[instructionData[3]];
                    }
                    break;
                case 94: //JMPR
                    registers[33] = registers[instructionData[1]];
                    break;

            }
            cpuPhase = 0;
        }

        public void Interrupt(byte id) {
            if (registers[37]==0) {
                interruptHw = true;
                interruptId = id;
                waitCycles = 4;
                halted = false;
            }
        }

        public byte LoadByte(uint pc) {
            return Memory.Read(pc);
        }

        public void StopCpu() {
            cpuRunning = false;
        }


    }
}




