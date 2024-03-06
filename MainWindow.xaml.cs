using CpuSim3.Devices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public List<Display> displays = new List<Display>();
        public List<Device> devices = new List<Device>();
        public bool autoRefreshMemory = false;
        public string memoryText = "0x000000";
        public byte memoryWait = 0;

        public int displayWait = 0;

        public List<float> cpuUsage = new List<float>();

        public TextBox textBoxKey;

        public MemoryViewer memoryViewerWindow = new MemoryViewer();

        public List<StackPanel> deviceList = new List<StackPanel>();

        public MainWindow() {
            InitializeComponent();
            Application.Current.MainWindow = this;
            GlobalVars.devices = devices;
            GlobalVars.displays = displays;

            memoryViewerWindow.Show();


            CompositionTarget.Rendering += Loop;

            opCodes = new OpCodes();
            cpu = new Cpu(opCodes.codes);
            cpu.StartCpu();

            GlobalVars.cpu = cpu;

            devices.Add(new Devices.Keyboard (0, 0, 0xF, 64));
            devices.Add(new Devices.Timer(7, 1, 0xF, 64));
            devices.Add(new Devices.VramDisplay(4, 2, 0xF, 64, 524288, 512, 400));

            bool keyBoardDev = false;
            bool displayDev = false;
            for (int i = 0; i<devices.Count; i++) {
                StackPanel stackPanel = new StackPanel();
                stackPanel.Orientation = Orientation.Horizontal;
                deviceList.Add(stackPanel);

                TextBlock textBox = new TextBlock();
                textBox.Text = "Device_"+i;
                textBox.Margin = new Thickness(0, 0, 10, 0);

                TextBlock textBox_Type = new TextBlock();
                textBox_Type.Text = Functions.GetDeviceType(devices[i].type);
                textBox_Type.Margin  = new Thickness(0, 0, 10, 0);

                TextBlock textBox_Address = new TextBlock();
                textBox_Address.Text = "0x"+(8388608 + (524288 * i)).ToString("X6");
                textBox_Address.Margin = new Thickness(0, 0, 10, 0);

                stackPanel.Children.Add(textBox);
                stackPanel.Children.Add(textBox_Address);
                stackPanel.Children.Add(textBox_Type);
               
                if (devices[i].type == 0 && !keyBoardDev) {
                    keyBoardDev = true;
                    textBoxKey = new TextBox();
                    textBoxKey.Width = 80;
                    textBoxKey.Height = 30;
                    textBoxKey.Name = "textBoxKey";
                    textBoxKey.KeyDown += OnKeyDownHandler;
                    stackPanel.Children.Add(textBoxKey);
                } else if ((devices[i].type == 1 || devices[i].type == 4) && !displayDev) {
                    displayDev = true;
                    Button Btn_Display = new Button();
                    Btn_Display.Name = "Btn_Display";
                    Btn_Display.Content = "Display";
                    Btn_Display.Click += Btn_Display_Click;
                    stackPanel.Children.Add(Btn_Display);
                }


                DeviceList.Children.Add(stackPanel);

            }
         


            // Add the StackPanel to a parent container (e.g., a Grid)
            // For example:
            // parentGrid.Children.Add(stackPanel);
            // Set the content of the window to the StackPanel
            //this.Content = stackPanel;

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

            if (displayWait < 30) {
                displayWait++;
            } else {
                displayWait = 0;
                for (int i = 0; i < displays.Count; i++) {
                    if (displays[i].IsLoaded && displays[i].IsVisible) {
                        displays[i].UpdateWindow();
                    }
                }
            }
        

            if (memoryViewerWindow.IsLoaded) {
                memoryViewerWindow.UpdateWindow();
            }

            fpsText.Text = "FPS: " + fps + "";
            time1 = DateTime.Now.Ticks / ticks;

            Register0.Text = "r0: " + cpu.registers[0].ToString("X8");
            Register1.Text = "r1: " + cpu.registers[1].ToString("X8");
            Register2.Text = "r2: " + cpu.registers[2].ToString("X8");
            Register3.Text = "r3: " + cpu.registers[3].ToString("X8");
            Register4.Text = "r4: " + cpu.registers[4].ToString("X8");
            Register5.Text = "r5: " + cpu.registers[5].ToString("X8");
            Register6.Text = "r6: " + cpu.registers[6].ToString("X8");
            Register7.Text = "r7: " + cpu.registers[7].ToString("X8");
            Register8.Text = "r8: " + cpu.registers[8].ToString("X8");
            Register9.Text = "r9: " + cpu.registers[9].ToString("X8");
            Register10.Text = "r10: " + cpu.registers[10].ToString("X8");
            Register11.Text = "r11: " + cpu.registers[11].ToString("X8");
            Register12.Text = "r12: " + cpu.registers[12].ToString("X8");
            Register13.Text = "r13: " + cpu.registers[13].ToString("X8");
            Register14.Text = "r14: " + cpu.registers[14].ToString("X8");
            Register15.Text = "r15: " + cpu.registers[15].ToString("X8");
            Register16.Text = "r16: " + cpu.registers[16].ToString("X8");
            Register17.Text = "r17: " + cpu.registers[17].ToString("X8");
            Register18.Text = "r18: " + cpu.registers[18].ToString("X8");
            Register19.Text = "r19: " + cpu.registers[19].ToString("X8");
            Register20.Text = "r20: " + cpu.registers[20].ToString("X8");
            Register21.Text = "r21: " + cpu.registers[21].ToString("X8");
            Register22.Text = "r22: " + cpu.registers[22].ToString("X8");
            Register23.Text = "r23: " + cpu.registers[23].ToString("X8");
            Register24.Text = "r24: " + cpu.registers[24].ToString("X8");
            Register25.Text = "r25: " + cpu.registers[25].ToString("X8");
            Register26.Text = "r26: " + cpu.registers[26].ToString("X8");
            Register27.Text = "r27: " + cpu.registers[27].ToString("X8");
            Register28.Text = "r28: " + cpu.registers[28].ToString("X8");
            Register29.Text = "r29: " + cpu.registers[29].ToString("X8");
            Register30.Text = "r30: " + cpu.registers[30].ToString("X8");
            Register31.Text = "r31: " + cpu.registers[31].ToString("X8");


            OP_Text.Text = "OP: " + cpu.op;
            OPString_Text.Text = opCodes.codes[cpu.op].name; 
            A_Text.Text = "" + cpu.instructionData[0];
            B_Text.Text = "" + cpu.instructionData[1];
            C_Text.Text = "" + cpu.instructionData[2];
            D_Text.Text = "" + cpu.instructionData[3];
            E_Text.Text = "" + cpu.instructionData[4];

            SP_Text.Text = "SP: " + cpu.registers[34].ToString("X6") + " ("+Math.Round(((cpu.registers[34]-8192.0)/8192)*100,1) +"%)";
            PC_Text.Text = "PC: " + cpu.registers[33].ToString("X6");

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
            Usage_Text_Avg.Text = "Avg CPU Usage: " + cpuUsage + "%";

            if (cpu.cpuRunning) {
                UpdateCpuUsageGraph();
            }
            


            DEBUG_Text.Text = "";

            
            if (autoRefreshMemory) {
                //TODO: memory viewer window

                if (memoryWait<4) {
                    memoryWait++;
                } else {
                    memoryWait = 0;
                    MemoryLoad();
                }
            }



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

        private void MemoryLoad() {
            int address = 0;
            int.TryParse(memoryText.Substring(2), System.Globalization.NumberStyles.HexNumber, null, out address);

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
                line0 += " " + Memory.Read((int)(address + i)).ToString("X2");
            }
            string line1 = "";
            for (int i = 16; i < 32; i++) {
                line1 += " " + Memory.Read((int)(address + i)).ToString("X2");
            }
            string line2 = "";
            for (int i = 32; i < 48; i++) {
                line2 += " " + Memory.Read((int)(address + i)).ToString("X2");
            }
            string line3 = "";
            for (int i = 48; i < 64; i++) {
                line3 += " " + Memory.Read((int)(address + i)).ToString("X2");
            }
            string line4 = "";
            for (int i = 64; i < 80; i++) {
                line4 += " " + Memory.Read((int)(address + i)).ToString("X2");
            }
            string line5 = "";
            for (int i = 80; i < 96; i++) {
                line5 += " " + Memory.Read((int)(address + i)).ToString("X2");
            }
            string line6 = "";
            for (int i = 96; i < 112; i++) {
                line6 += " " + Memory.Read((int)(address + i)).ToString("X2");
            }
            string line7 = "";
            for (int i = 112; i < 128; i++) {
                line7 += " " + Memory.Read((int)(address + i)).ToString("X2");
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

        private void TextBox_MemoryViewer(object sender, TextChangedEventArgs e) {
            if (sender is TextBox textBox) {
                if (!textBox.Text.StartsWith("0x") || textBox.Text.Length <= 2) {
                    return;
                }
                memoryText = textBox.Text;
                MemoryLoad();
            }
        }

        private void Btn_LoadASM(object sender, RoutedEventArgs e) {
            Assembler.Assemble(CodeEditor.Text, opCodes);
        }

        private void Btn_LoadMC(object sender, RoutedEventArgs e) {
            Assembler.LoadMachineCode(CodeEditor.Text); 
        }

        private void Btn_AppOS_Click(object sender, RoutedEventArgs e) {
            if (Assembler.os) {
                Assembler.os = false;
                Assembler.functionMode = false;
                Btn_AppOS.Content = "App";
            } else if (!Assembler.functionMode) {
                Assembler.os = true;
                Assembler.functionMode = false;
                Btn_AppOS.Content = "OS";
            } else {
                Assembler.os = false;
                Assembler.functionMode = true;
                Btn_AppOS.Content = "Func";
            }
        }

        private void Btn_Interrupt(object sender, RoutedEventArgs e) {
            int result = 0;
            if (int.TryParse(TextBox_Interrupt.Text, out result)) {
                   cpu.Interrupt(byte.Parse(TextBox_Interrupt.Text));
            }
        }

        private void Btn_AutoRefresh(object sender, RoutedEventArgs e) {
            if (autoRefreshMemory) {
                BtnAutoRefresh.Content = "Auto Refresh: Off";
                autoRefreshMemory = false;
            } else {
                BtnAutoRefresh.Content = "Auto Refresh: On";
                autoRefreshMemory = true;
            }
        }

        public long cpuCyclesExB = 0;
        public long cpuCyclesToB = 0;
        public long cpuUpdateTimerA = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        public long cpuUpdateTimerB = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

        public int cpuUsageTime = 100;

        public void UpdateCpuUsageGraph() {
            cpuUpdateTimerA = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            if (cpuUpdateTimerA-cpuUpdateTimerB > cpuUsageTime) {

                if (cpuCyclesToB > cpu.cyclesTotal) {
                    cpuCyclesToB = cpuCyclesToB / 2;
                    cpuCyclesExB = cpuCyclesExB / 2;
                }

                double cyclesTotalSec = cpu.cyclesTotal - cpuCyclesToB;  
                double cyclesExecSec = cpu.cyclesExecuting - cpuCyclesExB;

                cpuUsage.Add((float)Math.Round(cyclesExecSec / cyclesTotalSec * 100, 1));
                if (cpuUsage.Count>100) {
                    cpuUsage.RemoveAt(0);
                }


                cpuUsageChartLine.Points.Clear();
                for (int i = 0;  i < cpuUsage.Count; i++) {
                    int x = i * 6;
                    int y = (int)(200 - (cpuUsage[i] * 2));

                    if (y<0) {
                        y = 0;
                    } else if (y>200) {
                        y = 200;
                    }
                    Point point = new Point(x, y);
                    cpuUsageChartLine.Points.Add(point);
                }

                //cpuUsageCanvas.InvalidateVisual();

                cpuCyclesExB = cpu.cyclesExecuting;
                cpuCyclesToB = cpu.cyclesTotal;
                cpuUpdateTimerB = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                Usage_Text.Text = "CPU Usage: " + cpuUsage[cpuUsage.Count-1] + "%";
            }
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e) { 
            GlobalVars.key = (byte)KeyInterop.VirtualKeyFromKey(e.Key);
            textBoxKey.Text = "";
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            if (this.IsLoaded) {
                cpuUsageSlider.Text = $"{(int)((Slider)sender).Value}ms";
                cpuUsageTime = (int)((Slider)sender).Value;
            }
        }

        private void Btn_MemoryViewer_Click(object sender, RoutedEventArgs e) {
            if (memoryViewerWindow.IsVisible) {
                memoryViewerWindow.Hide();
            } else if (memoryViewerWindow.IsLoaded) {
                memoryViewerWindow.Show();
            } else {
                memoryViewerWindow = new MemoryViewer();
                memoryViewerWindow.Show();
            }
        }
        private void Btn_Display_Click(object sender, RoutedEventArgs e) {
            if (displays[0].IsVisible) {
                displays[0].Hide();
            } else if (displays[0].IsLoaded) {
                displays[0].Show();
            } else {
                displays[0] = new Display(2); //TODO device id
                displays[0].Show();
            }
        }
    }

}
