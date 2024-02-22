using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CpuSim3 {
    public static class Assembler {
        public static int CheckHex(string valIn) {
            string val = valIn;
            int val2 = 0;
            string[] chars;
            bool hex = false;
            chars = valIn.Select(c => c.ToString()).ToArray();
            if (chars.Length > 2) {
                if (chars[0] == "0" && chars[1] == "x") {
                    hex = true;
                    val = valIn.Substring(2);
                }
            }
            if (hex) {
                val2 = Convert.ToInt32(val, 16);
            } else if (Functions.IsNumeric(valIn)) {
                val2 = Convert.ToInt32(val);
            }
            return val2;
        }

        public static void Assemble(string code, OpCodes opcodes, string codeType = "OS") {
            Memory.Init();
            string[] lines;

            //remove comments ;;
            code = code.Replace(";[^:\\r\\n]+(?=;)", "");
            code = code.Replace(";", "");

            lines = code.Split("\r\n");

            AsVar[] vars = new AsVar[lines.Length];
            AsInstruction[] instructions = new AsInstruction[lines.Length];
            AsFunction[] functions = new AsFunction[lines.Length];

            Dictionary<string, AsFunction> functionsMap = new Dictionary<string, AsFunction>();
            Dictionary<string, AsVar> varsMap = new Dictionary<string, AsVar>();

            uint codeStartAddress = 7340032;
            if (codeType == "OS") {
                codeStartAddress = 7340032;
            } else if (codeType == "App") {
                codeStartAddress = 4194304;
            }

            int varIdx = 0;
            int instructionIdx = 0;
            int functionIdx = 0;

            uint bytes = 0;

            for (int i = 0; i < lines.Length; i++) {
                string[] line;
                string name;
                uint address = 0;
                byte[] values = new byte[6];

                line = lines[i].Split(" ");

                bool func = line[0].Contains("<") && line[0].Contains(">");

                if (line[0] == "VAR" || line[0] == "var") {
                    // VAR 1 name1 25
                    // VAR 2 name2 500
                    // VAR 3 name3 100000

                    string[] chars;
                    string val = line[3];
                    int val2;
                    bool hex = false;
                    name = line[2];
                    chars = line[3].Select(c => c.ToString()).ToArray();
                    if (chars.Length > 2) {
                        if (chars[0] == "0" && chars[1] == "x") {
                            hex = true;
                            val = line[3].Substring(2);
                        }
                    }
                    if (hex) {
                        val2 = Convert.ToInt32(val, 16);
                    } else {
                        val2 = Convert.ToInt32(val);
                    }
                    
                    vars[varIdx] = new AsVar(name, val2, 0, Convert.ToByte(line[1]));
                    varsMap.Add(name, vars[varIdx]);
                    varIdx++;
                } else if (func) { //function
                    string functionName = line[0].Replace("<", "").Replace(">", "");
                    functions[functionIdx] = new AsFunction(functionName, i, codeStartAddress + bytes);
                    functionsMap.Add(functionName, functions[functionIdx]);
                } else if (!string.IsNullOrEmpty(line[0].Trim())) { //instruction
                    int idInst;
                    try {
                        idInst = opcodes.names[line[0].ToUpper()];
                    } catch (Exception e) {
                        Console.WriteLine("Instruction (" + line[0].ToUpper() + ") does not exist");
                        return;
                    }
                    byte bytesInst = opcodes.codes[idInst].bytes;
                    string[] vals = new string[6];
                    if (line.Length > 6) {
                        vals[5] = line[6];
                        if (!string.IsNullOrEmpty(vals[5])) {
                            vals[5] = vals[5].Replace("\n", "").Replace(" ", "");
                        }
                    }
                    if (line.Length > 5) {
                        vals[4] = line[5];
                        if (!string.IsNullOrEmpty(vals[4])) {
                            vals[4] = vals[4].Replace("\n", "").Replace(" ", "");
                        }
                    }
                    if (line.Length > 4) {
                        vals[3] = line[4];
                        if (!string.IsNullOrEmpty(vals[3])) {
                            vals[3] = vals[3].Replace("\n", "").Replace(" ", "");
                        }
                    }
                    if (line.Length > 3) {
                        vals[2] = line[3];
                        if (!string.IsNullOrEmpty(vals[2])) {
                            vals[2] = vals[2].Replace("\n", "").Replace(" ", "");
                        }
                    }
                    if (line.Length > 2) {
                        vals[1] = line[2];
                        if (!string.IsNullOrEmpty(vals[1])) {
                            vals[1] = vals[1].Replace("\n", "").Replace(" ", "");
                        }
                    }
                    if (line.Length > 1) {
                        vals[0] = line[1];
                        if (!string.IsNullOrEmpty(vals[0])) {
                            vals[0] = vals[0].Replace("\n", "").Replace(" ", "");
                        }
                    }
                    instructions[instructionIdx] = new AsInstruction(line[0], bytesInst, vals, codeStartAddress + bytes, i);

                    bytes += bytesInst;
                    instructionIdx++;
                }
            }

            uint varsBytes = 0;
            uint varsAddress = codeStartAddress + bytes;
            for (int i = 0; i < varIdx; i++) {
                //calc vars addresses
                if (vars[i].address == 0) {
                    vars[i].address = varsAddress + varsBytes;
                    varsBytes += vars[i].bytes;
                }

                //write vars to memory
                byte[] store;
                if (vars[i].bytes == 1) {
                    Memory.Write(vars[i].address, (byte)vars[i].value);
                } else if (vars[i].bytes == 2) {
                    store = Functions.ConvertFrom16Bit((uint)vars[i].value);
                    Memory.Write(vars[i].address, store[0],true);
                    Memory.Write(vars[i].address + 1, store[1], true);
                } else if (vars[i].bytes == 3) {
                    store = Functions.ConvertFrom24Bit((uint)vars[i].value);
                    Memory.Write(vars[i].address, store[0], true);
                    Memory.Write(vars[i].address + 1, store[1], true);
                    Memory.Write(vars[i].address + 2, store[2], true);
                } else if (vars[i].bytes == 4) {
                    store = Functions.ConvertFrom32Bit((uint)vars[i].value);
                    Memory.Write(vars[i].address, store[0], true);
                    Memory.Write(vars[i].address + 1, store[1], true);
                    Memory.Write(vars[i].address + 2, store[2], true);
                    Memory.Write(vars[i].address + 3, store[3], true);
                }
            }
            //write instruction to memory
            for (int i = 0; i < instructionIdx; i++) {
                byte instructionBytes = instructions[i].bytes;
                string iname = instructions[i].name.ToUpper();
                int opCode = opcodes.names[iname];

                Memory.Write(instructions[i].address, (byte)opCode,true);

                //-------------------------------------------------------------------
                if (iname.Equals("INT")) {
                    Memory.Write(instructions[i].address + 1, Convert.ToByte(instructions[i].values[0]),true);
                    //-------------------------------------------------------------------
                } else if (iname.Equals("LDI1") || iname.Equals("LDI2") || iname.Equals("LDI3") || iname.Equals("LDI4") || iname.Equals("ADDI1") ||
                          iname.Equals("ADDI2") || iname.Equals("ADDI3") || iname.Equals("ADDI4") || iname.Equals("MULI1") || iname.Equals("DIVI1") ||
                          iname.Equals("SUBI1") || iname.Equals("SUBI2") || iname.Equals("SUBI3") || iname.Equals("SUBI4")) {
                    Memory.Write(instructions[i].address + 1, Convert.ToByte(RemoveRfromCode(instructions[i].values[0])),true);
                    if (iname.Equals("LDI2") || iname.Equals("ADDI2") || iname.Equals("SUBI2")) {
                        byte[] values = Functions.ConvertFrom16Bit((uint)CheckHex(instructions[i].values[1]));
                        Memory.Write(instructions[i].address + 2, values[0], true);
                        Memory.Write(instructions[i].address + 3, values[1], true);
                    } else if (iname.Equals("LDI3") || iname.Equals("ADDI3") || iname.Equals("SUBI3")) {
                        byte[] values = Functions.ConvertFrom24Bit((uint)CheckHex(instructions[i].values[1]));
                        Memory.Write(instructions[i].address + 2, values[0], true);
                        Memory.Write(instructions[i].address + 3, values[1], true);
                        Memory.Write(instructions[i].address + 4, values[2], true);
                    } else if (iname.Equals("LDI4") || iname.Equals("ADDI4") || iname.Equals("SUBI4")) {
                        byte[] values = Functions.ConvertFrom32Bit((uint)CheckHex(instructions[i].values[1]));
                        Memory.Write(instructions[i].address + 2, values[0], true);
                        Memory.Write(instructions[i].address + 3, values[1], true);
                        Memory.Write(instructions[i].address + 4, values[2], true);
                        Memory.Write(instructions[i].address + 5, values[3], true);
                    } else {
                        Memory.Write(instructions[i].address + 2, Convert.ToByte(instructions[i].values[1]), true);
                    }
                    //-------------------------------------------------------------------
                } else if (iname.Equals("LDR1") || iname.Equals("LDR2") || iname.Equals("LDR3") || iname.Equals("LDR4") ||
                          iname.Equals("STR1") || iname.Equals("STR2") || iname.Equals("STR3") || iname.Equals("STR4")) {
                    Memory.Write(instructions[i].address + 1, Convert.ToByte(RemoveRfromCode(instructions[i].values[0])), true);
                    Memory.Write(instructions[i].address + 2, Convert.ToByte(RemoveRfromCode(instructions[i].values[1])), true);
                    //-------------------------------------------------------------------
                } else {
                    for (int j = 0; j < instructions[i].values.Length; j++) {
                        int k = j + 1; //1
                        if (instructions[i].values[j] != null) {
                            if (FindRinCode(instructions[i].values[j])) {
                                //REGISTER
                                Memory.Write((uint)(instructions[i].address + k), Convert.ToByte(RemoveRfromCode(instructions[i].values[j])), true);
                            } else if (Functions.IsNumeric(instructions[i].values[j])) {
                                //????
                                Memory.Write(instructions[i].address + 1, Convert.ToByte(instructions[i].values[0]), true);
                            } else if (!(iname.Equals("JSR") || iname.Equals("JG") || iname.Equals("JNG") || iname.Equals("JL") || iname.Equals("JNL")
                                      || iname.Equals("JC") || iname.Equals("JNC") || iname.Equals("JE") || iname.Equals("JNE") || iname.Equals("JMP"))) {
                                //VAR MEM ADDRESS
                                byte[] varAddress = Functions.ConvertFrom24Bit(varsMap[instructions[i].values[j]].address);
                                Memory.Write((uint)(instructions[i].address + k), varAddress[0], true);
                                Memory.Write((uint)(instructions[i].address + k + 1), varAddress[1], true);
                                Memory.Write((uint)(instructions[i].address + k + 2), varAddress[2], true);
                            } else {
                                //FUNCTIONS (JUMPS)
                                byte[] jumpAddress = Functions.ConvertFrom24Bit(functionsMap[instructions[i].values[j]].address);
                                Memory.Write((uint)(instructions[i].address + k), jumpAddress[0], true);
                                Memory.Write((uint)(instructions[i].address + k + 1), jumpAddress[1], true);
                                Memory.Write((uint)(instructions[i].address + k + 2), jumpAddress[2], true);
                            }
                        } else {
                            break;
                        }
                    }
                }
            }
        }

        public static string RemoveRfromCode(string str) {
            bool found = System.Text.RegularExpressions.Regex.IsMatch(str, @"\b([R-R-r-r][0-9]{1,2})");
            if (found) {
                return str.Substring(1);
            }
            return str;
        }

        public static bool FindRinCode(string str) {
            return System.Text.RegularExpressions.Regex.IsMatch(str, @"\b([R-R-r-r][0-9]{1,2})");
        }

        public static void LoadMachineCode(string code, uint codeStartAddress) {
            Memory.Init();
            string[] bytes;
            byte[] bytes2 = new byte[4194304];
            code = code.Replace("\n", "");
            bytes = code.Split(' ');
            for (int i = 0; i < bytes.Length; i++) {
                bytes2[i] = Convert.ToByte(bytes[i], 16);
                Memory.Write((uint)i + codeStartAddress, bytes2[i],true);
            }
        }

    }



    public class AsVar {
        public string name;
        public int value;
        public uint address;
        public byte bytes;

        public AsVar(string name, int value, uint address, byte bytes) {
            this.name = name;
            this.value = value;
            this.address = address;
            this.bytes = bytes;
        }
    }

    public class AsInstruction {
        public string name;
        public byte bytes;
        public string[] values;
        public uint address;
        public int line;

        public AsInstruction(string name, byte bytes, string[] values, uint address, int line) {
            this.name = name;
            this.bytes = bytes;
            this.values = values;
            this.address = address;
            this.line = line;

        }
    }

    public class AsFunction {
        public string name;
        public int line;
        public uint address;

        public AsFunction(string name, int line, uint address) {
            this.name = name;
            this.line = line;
            this.address = address;

        }

    }

}
