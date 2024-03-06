using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CpuSim3 {
    public static class Assembler {
        public static bool os = true;
        public static bool app = false;
        public static bool functionMode = false;
        public static List<OsFunction> osFunctions = new List<OsFunction>();

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

        public static void Assemble(string code, OpCodes opcodes) {
            /*for (int i = 0; i < 0x800000; i++) {
                Memory.Data[i] = 0;
            }*/
            GlobalVars.cpu.cpuRunning = false;

            string[] lines;

            //remove comments ;;
            code = Regex.Replace(code, ";(.*?);", "");
            code = code.Replace(";", "");

            lines = code.Split("\r\n");

            AsVar[] consts = new AsVar[lines.Length];
            AsVar[] vars = new AsVar[lines.Length];
            AsInstruction[] instructions = new AsInstruction[lines.Length];
            AsFunction[] functions = new AsFunction[lines.Length];

            Dictionary<string, AsFunction> functionsMap = new Dictionary<string, AsFunction>();
            Dictionary<string, AsVar> varsMap = new Dictionary<string, AsVar>();
            Dictionary<string, AsVar> constsMap = new Dictionary<string, AsVar>();


            int codeStartAddress = 7340032;
            int varStartAddress = 0x000300;
            if (os) {
                codeStartAddress = 7340032;
                varStartAddress = 0x000300;
            } else if (!functionMode) {
                codeStartAddress = 4194304;
                varStartAddress = 0x004000;
            } else {
                codeStartAddress = 0x600000;
                varStartAddress = 0x3FF000;
            }

            int constIdx = 0;
            int varIdx = 0;
            int instructionIdx = 0;
            int functionIdx = 0;


            for (int i = 0; i<osFunctions.Count; i++) {
                functions[i] = new AsFunction(osFunctions[i].name, 9999, osFunctions[i].address);
                functionsMap.Add(osFunctions[i].name, functions[functionIdx]);
                functionIdx++;
            }  
            

            int bytes = 4;
            int constsBytes = 0;
            int varsBytes = 0;

            bool org = false;
            int orgAddress = 0;
            int orgBytes = 0;
            byte inOrg = 0;
            for (int i = 0; i < lines.Length; i++) {
                string[] line;
                string name;
                int address = 0;
                byte[] values = new byte[6];

                

                line = lines[i].Split(" ");

                if (functionMode && i == 0) {
                    if (line[0] != "FUNC") {
                        Debug.WriteLine("ERROR:" + line[0]);
                        break;
                    }
                    int startAddress = codeStartAddress + GetFuncAddress();
                    osFunctions.Add(new OsFunction(line[1], 0, startAddress));
                    
                    continue;
                }

                bool func = line[0].Contains("<") && line[0].Contains(">");

                org = line[0].Equals(".ORG", StringComparison.OrdinalIgnoreCase);
                bool orgEnd = line[0].Equals("-ORG", StringComparison.OrdinalIgnoreCase);
                if (org) {
                    orgBytes = 0;
                    inOrg = 1;
                    orgAddress = Convert.ToInt32(line[1].Replace("$", "0x"), 16);
                    continue;
                }
                if (orgEnd) {
                    inOrg = 0;
                    continue;
                }

                if (line[0] == "CONST" || line[0] == "const") {
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
                    
                    consts[constIdx] = new AsVar(name, val2, 0, Convert.ToByte(line[1]));
                    constsMap.Add(name, consts[constIdx]);
                    constIdx++;
                } else if (line[0] == "VAR" || line[0] == "var") {
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
                    if (inOrg == 0) {
                        functions[functionIdx] = new AsFunction(functionName, i, codeStartAddress + bytes);
                    } else {
                        functions[functionIdx] = new AsFunction(functionName, i, orgAddress + orgBytes);
                    }
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

                    if (inOrg == 0) {
                        instructions[instructionIdx] = new AsInstruction(line[0], bytesInst, vals, codeStartAddress + bytes, i);
                        bytes += bytesInst;
                    } else {
                        instructions[instructionIdx] = new AsInstruction(line[0], bytesInst, vals, orgAddress + orgBytes, i);
                        orgBytes += bytesInst;
                    }
                    instructionIdx++;
                }
            }



            int constsAddress = codeStartAddress + bytes;
            if (functionMode) {
                Memory.Write(codeStartAddress + bytes, 25, true);
                constsAddress = codeStartAddress + bytes + 1;
            }

            for (int i = 0; i < constIdx; i++) {
                //calc consts addresses
                if (consts[i].address == 0) {
                    consts[i].address = constsAddress + constsBytes;
                    constsBytes += consts[i].bytes;
                }
 
                //write consts to memory(rom)
                byte[] store;
                if (consts[i].bytes == 1) {
                    Memory.Write(consts[i].address, (byte)consts[i].value);
                } else if (consts[i].bytes == 2) {
                    store = Functions.ConvertFrom16Bit((int)consts[i].value);
                    Memory.Write(consts[i].address, store[0], true);
                    Memory.Write(consts[i].address + 1, store[1], true);
                } else if (consts[i].bytes == 3) {
                    store = Functions.ConvertFrom24Bit((int)consts[i].value);
                    Memory.Write(consts[i].address, store[0], true);
                    Memory.Write(consts[i].address + 1, store[1], true);
                    Memory.Write(consts[i].address + 2, store[2], true);
                } else if (consts[i].bytes == 4) {
                    store = Functions.ConvertFrom32Bit((int)consts[i].value);
                    Memory.Write(consts[i].address, store[0], true);
                    Memory.Write(consts[i].address + 1, store[1], true);
                    Memory.Write(consts[i].address + 2, store[2], true);
                    Memory.Write(consts[i].address + 3, store[3], true);
                }
            }

            int ldibytes = bytes + constsBytes;
            if (functionMode) {
                ldibytes += 1;
            }
            Memory.Write(codeStartAddress, 24, true);
            byte[] varInitFunctionAddress = Functions.ConvertFrom24Bit(codeStartAddress + ldibytes);
            Memory.Write(codeStartAddress + 1, varInitFunctionAddress[0], true);
            Memory.Write(codeStartAddress + 2, varInitFunctionAddress[1], true);
            Memory.Write(codeStartAddress + 3, varInitFunctionAddress[2], true);

            int varsAddress = varStartAddress;
            if (functionMode) {
                varsAddress = varStartAddress + 1;
            }
            for (int i = 0; i < varIdx; i++) {
                //calc vars addresses
                if (vars[i].address == 0) {
                    vars[i].address = varsAddress + varsBytes;
                    varsBytes += vars[i].bytes;
                }

                //write vars to memory (ram)
                byte[] store;
                if (vars[i].bytes == 1) { //LDI1
                    Memory.Write(codeStartAddress + ldibytes, 11); //LDI1
                    Memory.Write(codeStartAddress + ldibytes + 1, 0, true);
                    Memory.Write(codeStartAddress + ldibytes + 2, (byte)vars[i].value, true);
                    Memory.Write(codeStartAddress + ldibytes + 3, 4, true); //ST1
                    Memory.Write(codeStartAddress + ldibytes + 4, 0, true);
                    byte[] varaddress = Functions.ConvertFrom24Bit(vars[i].address);
                    Memory.Write(codeStartAddress + ldibytes + 5, varaddress[0], true);
                    Memory.Write(codeStartAddress + ldibytes + 6, varaddress[1], true);
                    Memory.Write(codeStartAddress + ldibytes + 7, varaddress[2], true);
                    ldibytes += 8;
                } else if (vars[i].bytes == 2) {  //LDI2
                    store = Functions.ConvertFrom16Bit((int)vars[i].value);
                    Memory.Write(vars[i].address, store[0], true);
                    Memory.Write(vars[i].address + 1, store[1], true);
                } else if (vars[i].bytes == 3) {  //LDI3
                    store = Functions.ConvertFrom24Bit((int)vars[i].value);
                    Memory.Write(vars[i].address, store[0], true);
                    Memory.Write(vars[i].address + 1, store[1], true);
                    Memory.Write(vars[i].address + 2, store[2], true);
                } else if (vars[i].bytes == 4) {  //LDI4
                    store = Functions.ConvertFrom32Bit((int)vars[i].value);
                    Memory.Write(vars[i].address, store[0], true);
                    Memory.Write(vars[i].address + 1, store[1], true);
                    Memory.Write(vars[i].address + 2, store[2], true);
                    Memory.Write(vars[i].address + 3, store[3], true);
                }
            }

            Memory.Write(codeStartAddress + ldibytes, 25, true);


            //write instruction to memory
            for (int i = 0; i < instructionIdx; i++) {
                byte instructionBytes = instructions[i].bytes;
                string iname = instructions[i].name.ToUpper();
                int opCode = opcodes.names[iname];

                Memory.Write(instructions[i].address, (byte)opCode,true);
                Debug.WriteLine(instructions[i].address.ToString("X6"));

                //-------------------------------------------------------------------
                if (iname.Equals("INT")) {
                    Memory.Write(instructions[i].address + 1, Convert.ToByte(instructions[i].values[0]),true);
                    //-------------------------------------------------------------------
                } else if (iname.Equals("LDI1") || iname.Equals("LDI2") || iname.Equals("LDI3") || iname.Equals("LDI4") || iname.Equals("ADDI1") ||
                          iname.Equals("ADDI2") || iname.Equals("ADDI3") || iname.Equals("ADDI4") || iname.Equals("MULI1") || iname.Equals("DIVI1") ||
                          iname.Equals("SUBI1") || iname.Equals("SUBI2") || iname.Equals("SUBI3") || iname.Equals("SUBI4")) {
                    Memory.Write(instructions[i].address + 1, Convert.ToByte(RemoveRfromCode(instructions[i].values[0])),true);
                    if (iname.Equals("LDI2") || iname.Equals("ADDI2") || iname.Equals("SUBI2")) {
                        byte[] values = Functions.ConvertFrom16Bit((int)CheckHex(instructions[i].values[1]));
                        Memory.Write(instructions[i].address + 2, values[0], true);
                        Memory.Write(instructions[i].address + 3, values[1], true);
                    } else if (iname.Equals("LDI3") || iname.Equals("ADDI3") || iname.Equals("SUBI3")) {
                        byte[] values = Functions.ConvertFrom24Bit((int)CheckHex(instructions[i].values[1]));
                        Memory.Write(instructions[i].address + 2, values[0], true);
                        Memory.Write(instructions[i].address + 3, values[1], true);
                        Memory.Write(instructions[i].address + 4, values[2], true);
                    } else if (iname.Equals("LDI4") || iname.Equals("ADDI4") || iname.Equals("SUBI4")) {
                        byte[] values = Functions.ConvertFrom32Bit((int)CheckHex(instructions[i].values[1]));
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
                                Memory.Write((int)(instructions[i].address + k), Convert.ToByte(RemoveRfromCode(instructions[i].values[j])), true);
                            } else if (Functions.IsNumeric(instructions[i].values[j])) {
                                //????
                                Memory.Write(instructions[i].address + 1, Convert.ToByte(instructions[i].values[0]), true);
                            } else if (!(iname.Equals("JSR") || iname.Equals("JG") || iname.Equals("JNG") || iname.Equals("JL") || iname.Equals("JNL")
                                      || iname.Equals("JC") || iname.Equals("JNC") || iname.Equals("JE") || iname.Equals("JNE") || iname.Equals("JMP"))) {
                                //VAR MEM ADDRESS
                                if (varsMap.ContainsKey(instructions[i].values[j])) {
                                    byte[] varAddress = Functions.ConvertFrom24Bit(varsMap[instructions[i].values[j]].address);
                                    Memory.Write((int)(instructions[i].address + k), varAddress[0], true);
                                    Memory.Write((int)(instructions[i].address + k + 1), varAddress[1], true);
                                    Memory.Write((int)(instructions[i].address + k + 2), varAddress[2], true);
                                } else if (constsMap.ContainsKey(instructions[i].values[j])) {
                                    byte[] constAddress = Functions.ConvertFrom24Bit(constsMap[instructions[i].values[j]].address);
                                    Memory.Write((int)(instructions[i].address + k), constAddress[0], true);
                                    Memory.Write((int)(instructions[i].address + k + 1), constAddress[1], true);
                                    Memory.Write((int)(instructions[i].address + k + 2), constAddress[2], true);
                                }

                            } else {
                                //FUNCTIONS (JUMPS)
                                byte[] jumpAddress = Functions.ConvertFrom24Bit(functionsMap[instructions[i].values[j]].address);
                                Memory.Write((int)(instructions[i].address + k), jumpAddress[0], true);
                                Memory.Write((int)(instructions[i].address + k + 1), jumpAddress[1], true);
                                Memory.Write((int)(instructions[i].address + k + 2), jumpAddress[2], true);
                            }
                        } else {
                            break;
                        }
                    }
                }
            }
            if (functionMode) {
                osFunctions[osFunctions.Count - 1].size = bytes + constsBytes + varsBytes + 6;
            }


            Debug.WriteLine("----------");
            Debug.WriteLine("Size: "+bytes); 
            Debug.WriteLine("Const: " + constsBytes);
            Debug.WriteLine("Var: " + varsBytes);

            Debug.WriteLine("----------");
            for (int i = 0; i < osFunctions.Count; i++) {
                Debug.WriteLine("Function"+i+": " + osFunctions[i].name+" (" + osFunctions[i].address.ToString("X6") +")");
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

        public static void LoadMachineCode(string code) {
            int codeStartAddress = 7340032;
            if (os) {
                codeStartAddress = 7340032;
            } else {
                codeStartAddress = 4194304;
            }

            Memory.Init();
            string[] bytes;
            byte[] bytes2 = new byte[4194304];
            code = code.Replace("\n", "");
            bytes = code.Split(' ');
            for (int i = 0; i < bytes.Length; i++) {
                bytes2[i] = Convert.ToByte(bytes[i], 16);
                Memory.Write((int)i + codeStartAddress, bytes2[i],true);
            }
        }

        public static int GetFuncAddress() {
            int address = 0;
            for (int i = 0; i<osFunctions.Count; i++) {
                address += osFunctions[i].size;
            }
            Debug.WriteLine(address);
            return address;
        }

    }


    public class AsVar {
        public string name;
        public int value;
        public int address;
        public byte bytes;

        public AsVar(string name, int value, int address, byte bytes) {
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
        public int address;
        public int line;

        public AsInstruction(string name, byte bytes, string[] values, int address, int line) {
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
        public int address;

        public AsFunction(string name, int line, int address) {
            this.name = name;
            this.line = line;
            this.address = address;
        }
    }

    public class OsFunction {
        public string name;
        public int size;
        public int address;

        public OsFunction(string name, int size, int address) {
            this.name = name;
            this.size = size;
            this.address = address;
        }
    }

}
