using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace wifi4
{
    public partial class ConnectedDevicesForm : Form
    {
        private Dictionary<string, string> customDeviceNames = new Dictionary<string, string>();
        private string customNamesFilePath = "custom_device_names.txt";

        public ConnectedDevicesForm()
        {
            InitializeComponent();
            LoadCustomDeviceNames();
        }

        private async void btnScan_Click(object sender, EventArgs e)
        {
            flowDevices.Controls.Clear();
            labelCount.Text = "Tarama yapılıyor...";

            try
            {
                string arpOutput = RunCommand("arp", "-a");
                string[] arpLines = arpOutput.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                HashSet<string> seenMacs = new HashSet<string>();

                foreach (string line in arpLines)
                {
                    Match match = Regex.Match(line, @"(?<ip>\d+\.\d+\.\d+\.\d+)\s+([-\w]+)?\s+(?<mac>([0-9A-Fa-f]{2}[:-]){5}[0-9A-Fa-f]{2})");

                    if (match.Success)
                    {
                        string ip = match.Groups["ip"].Value;
                        string mac = match.Groups["mac"].Value.ToUpper();

                        if (seenMacs.Contains(mac))
                            continue;
                        seenMacs.Add(mac);

                        string hostname = "Çözümlenemedi";
                        try
                        {
                            var hostEntry = await Dns.GetHostEntryAsync(ip);
                            hostname = hostEntry.HostName;
                        }
                        catch { }

                        string vendor = await GetVendorFromMacAsync(mac);
                        string customName = customDeviceNames.ContainsKey(mac) ? customDeviceNames[mac] : "Bilinmeyen";

                        AddDeviceCard(ip, mac, hostname, vendor, customName);
                    }
                }

                labelCount.Text = "Toplam cihaz sayısı: " + seenMacs.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        private string RunCommand(string command, string args)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo(command, args)
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                
                using (Process process = Process.Start(psi))
                {
                    process.WaitForExit();
                    return process.StandardOutput.ReadToEnd();
                }
            }
            catch
            {
                return "";
            }
        }

        private async Task<string> GetVendorFromMacAsync(string mac)
        {
            try
            {
                using (var client = new WebClient())
                {
                    client.Headers.Add("User-Agent", "CSharpApp");
                    string result = await client.DownloadStringTaskAsync("https://api.macvendors.com/" + mac);
                    return result;
                }
            }
            catch
            {
                return "Bilinmeyen Üretici";
            }
        }

        private void AddDeviceCard(string ip, string mac, string hostname, string vendor, string name)
        {
            var panel = new Panel
            {
                Width = 350,
                Height = 150,
                BackColor = Color.WhiteSmoke,
                Margin = new Padding(10),
                Padding = new Padding(10),
                BorderStyle = BorderStyle.FixedSingle
            };

           
            panel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                e.Graphics.FillRectangle(new SolidBrush(Color.LightGray), 0, 0, panel.Width, panel.Height);
            };

            var lblInfo = new Label
            {
                Text = $"🖧 IP: {ip}\n🔗 MAC: {mac}\n🌐 Host: {hostname}\n🏷️ Üretici: {vendor}\n📛 Ad: {name}",
                Font = new Font("Segoe UI", 10),
                AutoSize = true
            };

            panel.Controls.Add(lblInfo);

            panel.DoubleClick += (sender, e) => RenameDevice(panel, mac, name);

            flowDevices.Controls.Add(panel);
        }

        private void RenameDevice(Panel panel, string mac, string currentName)
        {
           
            var inputBox = new TextBox
            {
                Text = currentName,
                Font = new Font("Segoe UI", 10),
                Width = 300,
                Margin = new Padding(5)
            };

            var dialog = new Form
            {
                Width = 350,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                Text = "Cihaz İsmi Değiştir"
            };

            var btnSave = new Button
            {
                Text = "Kaydet",
                Width = 100,
                Height = 40,
                Margin = new Padding(5),
                DialogResult = DialogResult.OK
            };

            dialog.Controls.Add(inputBox);
            dialog.Controls.Add(btnSave);
            dialog.AcceptButton = btnSave;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string newName = inputBox.Text.Trim();

                if (!string.IsNullOrWhiteSpace(newName) && newName != currentName)
                {
                    customDeviceNames[mac] = newName;
                    SaveCustomDeviceNames();

                    var lbl = (Label)panel.Controls[0];
                    lbl.Text = lbl.Text.Replace($"📛 Ad: {currentName}", $"📛 Ad: {newName}");

                    MessageBox.Show("Cihaz adı başarıyla güncellendi!");
                }
            }
        }

        private void SaveCustomDeviceNames()
        {
            System.IO.File.WriteAllLines(customNamesFilePath, customDeviceNames.Select(kvp => $"{kvp.Key}|{kvp.Value}"));
        }

        private void LoadCustomDeviceNames()
        {
            if (!System.IO.File.Exists(customNamesFilePath)) return;

            foreach (var line in System.IO.File.ReadAllLines(customNamesFilePath))
            {
                var parts = line.Split('|');
                if (parts.Length == 2)
                    customDeviceNames[parts[0]] = parts[1];
            }
        }
    }
}
