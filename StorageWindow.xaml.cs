using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;

namespace CpuSim3 {
    /// <summary>
    /// Interaction logic for StorageWindow.xaml
    /// </summary>
    public partial class StorageWindow : Window {
        public byte deviceId;
        public StorageWindow(byte id) {
            InitializeComponent();
            deviceId = id;
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            //0x0100-01 05 08 09.0x0200-FF 01 FF 02
            string data = dataText.Text;
            string[] dataArray = data.Split(".");
            for (int i = 0; i < dataArray.Length; i++) {
                string[] d = dataArray[i].Split("-");
                int address = Convert.ToInt32(d[0], 16); 
                string[] bytes = d[1].Split(" ");

                for (int j = 0; j < bytes.Length; j++) {
                    byte value = Convert.ToByte(bytes[j], 16);
                    GlobalVars.devices[deviceId].Write(address+j, value);
                }
                

            }

        }
    }
}
