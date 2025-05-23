using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace wifi4
{
    public partial class MainMenuForm : Form
    {
        public MainMenuForm()
        {
            InitializeComponent();
            SetupForm();
        }

        private void SetupForm()
        {
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "WiFi Yönetimi";

           
            btnWifiScan.FlatStyle = FlatStyle.Flat;
            btnWifiScan.FlatAppearance.BorderSize = 0;
            btnWifiScan.Cursor = Cursors.Hand;

            btnConnectedDevices.FlatStyle = FlatStyle.Flat;
            btnConnectedDevices.FlatAppearance.BorderSize = 0;
            btnConnectedDevices.Cursor = Cursors.Hand;

    
            labelWifi.Font = new Font("Segoe UI", 9F);
            labelWifi.Text = "WiFi Ağları";

            labelDevices.Font = new Font("Segoe UI", 9F);
            labelDevices.Text = "Bağlı Cihazlar";

            btnWifiScan.MouseEnter += (s, e) => btnWifiScan.BackColor = Color.FromArgb(230, 230, 230);
            btnWifiScan.MouseLeave += (s, e) => btnWifiScan.BackColor = Color.FromArgb(240, 240, 240);

            btnConnectedDevices.MouseEnter += (s, e) => btnConnectedDevices.BackColor = Color.FromArgb(230, 230, 230);
            btnConnectedDevices.MouseLeave += (s, e) => btnConnectedDevices.BackColor = Color.FromArgb(240, 240, 240);
        }

   
        private void btnWifiScan_Click(object sender, EventArgs e)
        {
         
            FormWifiNetworks wifiNetworksForm = new FormWifiNetworks(); 
            wifiNetworksForm.Show();
            this.Hide(); 
        }

        
        private void btnConnectedDevices_Click(object sender, EventArgs e)
        {
            ConnectedDevicesForm devicesForm = new ConnectedDevicesForm(); 
            devicesForm.Show();
            this.Hide(); 
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            Application.Exit();
        }
    }
}
// cihazların özellikleri  isimleri yanında ikon  görüntüyü güzelleştir 