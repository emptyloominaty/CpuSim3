﻿using System;
using System.Collections.Generic;
using System.Linq;
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
            Random random = new Random();
            Color[] colors = new Color[] { Colors.Red, Colors.White, Colors.Black };

            int pxSize = 2;

            for (int x = 0; x < 200; x++) {
                for (int y = 0; y < 200; y++) {

                    int randomNumber = random.Next(0, 3);

                    DrawFilledRectangle(0 + (x * pxSize), 0 + (y * pxSize), pxSize, pxSize, colors[randomNumber]);
                }
            }
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

            Register0.Text = "Reg0: " + cpu.registers[0];
            Register1.Text = "Reg1: " + cpu.registers[1];
            Register2.Text = "Reg2: " + cpu.registers[2];
            Register3.Text = "Reg3: " + cpu.registers[3];
            Register4.Text = "Reg4: " + cpu.registers[4];

            OP_Text.Text = "OP: " + cpu.op;
            A_Text.Text = "A: " + cpu.instructionData[0];
            B_Text.Text = "B: " + cpu.instructionData[1];
            C_Text.Text = "C: " + cpu.instructionData[2];
            D_Text.Text = "D: " + cpu.instructionData[3];
            E_Text.Text = "E: " + cpu.instructionData[4];

            SP_Text.Text = "SP: " + cpu.registers[34];
            PC_Text.Text = "PC: " + cpu.registers[33];

            Running.Text = "Running: " + cpu.cpuRunning;
            DEBUG_Text.Text = "Instructions: " + cpu.instructionsDone+" - cycles: "+cpu.cyclesDone + " - clock: "+ cpu.clock;

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

            myCanvas.Children.Add(rect);
        }
    }
}
