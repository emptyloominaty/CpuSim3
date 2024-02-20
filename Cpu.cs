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
        public long clockSet = 1000000;
        public long clock = 0;

        public bool cpuRunning = true;
        public bool threadRunning = true;

        public long timeStart;
        public long timeEnd;
        public long timeA;
        public long timeB;
        public long timeC;
        public long timeD;
        public long timeClockA = DateTime.Now.Ticks / 100;
        public long timeClockB = DateTime.Now.Ticks / 100;

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
                        timeClockA = DateTime.Now.Ticks / 100;
                        Main();
                        timeClockD = 0;
                        while (timeClockD < timeClockWait) {
                            timeClockB = DateTime.Now.Ticks / 100;
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
            switch (op) {
                case 1: //ADD  r1+r2=r3
                    val = registers[instructionData[1]] + registers[instructionData[2]];
                    registers[instructionData[3]] = val;
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




