using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace wifi4
{
    public partial class MainMenuForm : Form
    {
        public MainMenuForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        // WiFi ağ taraması için butona tıklama olayı
        private void btnWifiScan_Click(object sender, EventArgs e)
        {
            // WiFi ağlarını gösterecek formu başlat
            FormWifiNetworks wifiNetworksForm = new FormWifiNetworks(); // FormWifiNetworks yerine uygun formu kullanın
            wifiNetworksForm.Show();
            this.Hide(); // Ana menüyü gizle
        }

        // Bağlı cihazları göstermek için butona tıklama olayı
        private void btnConnectedDevices_Click(object sender, EventArgs e)
        {
            ConnectedDevicesForm devicesForm = new ConnectedDevicesForm(); // Bağlı cihazları gösterecek form
            devicesForm.Show();
            this.Hide(); // Ana menüyü gizle
        }
    }
}
