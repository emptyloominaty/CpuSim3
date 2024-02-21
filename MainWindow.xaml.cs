using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CpuSim3 {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public long time1 = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        public OpCodes opCodes;
        public Cpu cpu;
        public MainWindow() {
            InitializeComponent();

            CompositionTarget.Rendering += Loop;

            opCodes = new OpCodes();
            cpu = new Cpu(opCodes.codes);
            cpu.StartCpu();



            //TEST
            /*Random random = new Random();
            Color[] colors = new Color[] { Colors.Red, Colors.White, Colors.Black };

            int pxSize = 2;

            for (int x = 0; x < 200; x++) {
                for (int y = 0; y < 200; y++) {

                    int randomNumber = random.Next(0, 3);

                    DrawFilledRectangle(0 + (x * pxSize), 0 + (y * pxSize), pxSize, pxSize, colors[randomNumber]);
                }
            }*/
        }

        public void Loop(object sender, EventArgs e) {
            long ticks = TimeSpan.TicksPerMillisecond;
            if (ticks < 1) {
                ticks = 1;
            }
            long ms = ((DateTime.Now.Ticks / ticks) - time1);
            long fps = 0;
            if (ms > 0) {
                fps = 1000 / ms;
            }

            fpsText.Text = "FPS: " + fps + "";
            time1 = DateTime.Now.Ticks / ticks;

            Register0.Text = "r0: " + cpu.registers[0];
            Register1.Text = "r1: " + cpu.registers[1];
            Register2.Text = "r2: " + cpu.registers[2];
            Register3.Text = "r3: " + cpu.registers[3];
            Register4.Text = "r4: " + cpu.registers[4];
            Register5.Text = "r5: " + cpu.registers[5];
            Register6.Text = "r6: " + cpu.registers[6];
            Register7.Text = "r7: " + cpu.registers[7];
            Register8.Text = "r8: " + cpu.registers[8];
            Register9.Text = "r9: " + cpu.registers[9];
            Register10.Text = "r10: " + cpu.registers[10];
            Register11.Text = "r11: " + cpu.registers[11];
            Register12.Text = "r12: " + cpu.registers[12];
            Register13.Text = "r13: " + cpu.registers[13];
            Register14.Text = "r14: " + cpu.registers[14];
            Register15.Text = "r15: " + cpu.registers[15];
            Register16.Text = "r16: " + cpu.registers[16];
            Register17.Text = "r17: " + cpu.registers[17];
            Register18.Text = "r18: " + cpu.registers[18];
            Register19.Text = "r19: " + cpu.registers[19];
            Register20.Text = "r20: " + cpu.registers[20];
            Register21.Text = "r21: " + cpu.registers[21];
            Register22.Text = "r22: " + cpu.registers[22];
            Register23.Text = "r23: " + cpu.registers[23];
            Register24.Text = "r24: " + cpu.registers[24];
            Register25.Text = "r25: " + cpu.registers[25];
            Register26.Text = "r26: " + cpu.registers[26];
            Register27.Text = "r27: " + cpu.registers[27];
            Register28.Text = "r28: " + cpu.registers[28];
            Register29.Text = "r29: " + cpu.registers[29];
            Register30.Text = "r30: " + cpu.registers[30];
            Register31.Text = "r31: " + cpu.registers[31];


            OP_Text.Text = "OP: " + cpu.op;
            OPString_Text.Text = opCodes.codes[cpu.op].name; 
            A_Text.Text = "" + cpu.instructionData[0];
            B_Text.Text = "" + cpu.instructionData[1];
            C_Text.Text = "" + cpu.instructionData[2];
            D_Text.Text = "" + cpu.instructionData[3];
            E_Text.Text = "" + cpu.instructionData[4];

            SP_Text.Text = "SP: " + cpu.registers[34] + " ("+Math.Round(((cpu.registers[34]-8192.0)/8192)*100,1) +"%)";
            PC_Text.Text = "PC: " + cpu.registers[33];

            Running.Text = "Running: " + cpu.cpuRunning;
            Clock_Text.Text = Functions.FormatClock(cpu.clock);
            IPC_Text.Text = "IPC: "+ Math.Round(cpu.instructionsDone / (cpu.cyclesDone + 1.0), 2);
            Instructions_Done_Text.Text = "Instructions Done: " + cpu.instructionsDone;
            Cycles_Done_Text.Text = "Cycles Done: " + cpu.cyclesDone;

            double cpu1p = cpu.cyclesTotal / 100.0;
            if (cpu1p == 0) {
                cpu1p = 1;
            }
            double cpuUsage = Math.Round(cpu.cyclesExecuting / cpu1p,1);
            if (cpuUsage > 100) {
                cpuUsage = 100;
            }
            Usage_Text.Text = "CPU Usage: " + cpuUsage + "%";

            DEBUG_Text.Text = " ";

            //Random random = new Random();
            //Color[] colors = new Color[] { Colors.Red, Colors.White, Colors.Black };
            //int randomNumber = random.Next(0, 3);
            //DrawFilledRectangle(50,50,25, 25, colors[randomNumber]);
        }

        public void DrawFilledRectangle(double x, double y, double width, double height, Color color) {
            Rectangle rect = new Rectangle {
                Width = width,
                Height = height,
                Fill = new SolidColorBrush(color)
            };

            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);

            //myCanvas.Children.Add(rect);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e) {
            if (sender is TextBox textBox && cpu != null) {
                // Attempt to parse the text to a number
                if (long.TryParse(textBox.Text, out long number)) {
                    cpu.clockSet = number;
                    if (number > 1000000000) {
                        cpu.maxClock = true;
                    } else {
                        cpu.maxClock = false;
                    }
                }
            }
        }

        private void ToggleCpu(object sender, RoutedEventArgs e) {
            if (cpu.cpuRunning) {
                cpu.cpuRunning = false;
                BtnCpuToggle.Content = "Start";
            } else {
                cpu.Reset();
                cpu.cpuRunning = true;
                BtnCpuToggle.Content = "Stop";


            }
        }

        private void TextBox_MemoryViewer(object sender, TextChangedEventArgs e) {
            if (sender is TextBox textBox) {
                if (!textBox.Text.StartsWith("0x") || textBox.Text.Length <= 2) {
                    return;
                }
                uint address = 0;
                uint.TryParse(textBox.Text.Substring(2), System.Globalization.NumberStyles.HexNumber, null, out address);

                string add0 = address.ToString("X6");
                string add1 = (address + 16).ToString("X6");
                string add2 = (address + 32).ToString("X6");
                string add3 = (address + 48).ToString("X6");
                string add4 = (address + 64).ToString("X6");
                string add5 = (address + 80).ToString("X6");
                string add6 = (address + 96).ToString("X6");
                string add7 = (address + 112).ToString("X6");

                string line0 = "";
                for (int i = 0; i < 16; i++) {
                    line0 += " " + Memory.Read((uint)(address + i)).ToString("X2");
                }
                string line1 = "";
                for (int i = 16; i < 32; i++) {
                    line1 += " " + Memory.Read((uint)(address + i)).ToString("X2");
                }
                string line2 = "";
                for (int i = 32; i < 48; i++) {
                    line2 += " " + Memory.Read((uint)(address + i)).ToString("X2");
                }
                string line3 = "";
                for (int i = 48; i < 64; i++) {
                    line3 += " " + Memory.Read((uint)(address + i)).ToString("X2");
                }
                string line4 = "";
                for (int i = 64; i < 80; i++) {
                    line4 += " " + Memory.Read((uint)(address + i)).ToString("X2");
                }
                string line5 = "";
                for (int i = 80; i < 96; i++) {
                    line5 += " " + Memory.Read((uint)(address + i)).ToString("X2");
                }
                string line6 = "";
                for (int i = 96; i < 112; i++) {
                    line6 += " " + Memory.Read((uint)(address + i)).ToString("X2");
                }
                string line7 = "";
                for (int i = 112; i < 128; i++) {
                    line7 += " " + Memory.Read((uint)(address + i)).ToString("X2");
                }

                MemoryLine0.Text = "0x" + add0 + ": " + line0;
                MemoryLine1.Text = "0x" + add1 + ": " + line1;
                MemoryLine2.Text = "0x" + add2 + ": " + line2;
                MemoryLine3.Text = "0x" + add3 + ": " + line3;
                MemoryLine4.Text = "0x" + add4 + ": " + line4;
                MemoryLine5.Text = "0x" + add5 + ": " + line5;
                MemoryLine6.Text = "0x" + add6 + ": " + line6;
                MemoryLine7.Text = "0x" + add7 + ": " + line7;
            }
        }

        //TODO:assembler
        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e) {

        }
    }
}
