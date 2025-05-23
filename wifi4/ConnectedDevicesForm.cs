using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net.Http;
using System.Net.Sockets;

namespace wifi4
{
    public partial class ConnectedDevicesForm : Form
    {
        private Dictionary<string, string> customDeviceNames = new Dictionary<string, string>();
        private string customNamesFilePath = "custom_device_names.txt";
        private PictureBox loadingSpinner;
        private PictureBox countSpinner;
        private System.Windows.Forms.Timer spinnerTimer;
        private System.Windows.Forms.Timer countSpinnerTimer;
        private int deviceCount = 0;

        public ConnectedDevicesForm()
        {
            InitializeComponent();
            LoadMacVendorCache();
            this.BackColor = Color.FromArgb(240, 240, 240);
            InitializeLoadingSpinner();
        }

        private void InitializeLoadingSpinner()
        {
            // Ana loading spinner (ekran ortasında)
            loadingSpinner = new PictureBox
            {
                Size = new Size(40, 40),
                Location = new Point((this.Width - 40) / 2, (this.Height - 40) / 2),
                Visible = false,
                BackColor = Color.White
            };

            // Sayı yanındaki küçük spinner
            countSpinner = new PictureBox
            {
                Size = new Size(24, 24),
                Location = new Point(labelCount.Right + 10, labelCount.Top + 2),
                Visible = false,
                BackColor = Color.White,
            };

            // Ana spinner timer'ı
            spinnerTimer = new System.Windows.Forms.Timer();
            spinnerTimer.Interval = 50;
            int angle = 0;

            spinnerTimer.Tick += (s, e) =>
            {
                angle = (angle + 10) % 360;
                loadingSpinner.Invalidate();
            };

            loadingSpinner.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.Clear(Color.White); // Arka planı şeffaf temizle

                e.Graphics.TranslateTransform(loadingSpinner.Width / 2, loadingSpinner.Height / 2);
                e.Graphics.RotateTransform(angle);

                using (Pen pen = new Pen(Color.FromArgb(0, 120, 215), 3))
                {
                    // Ana daire
                    e.Graphics.DrawEllipse(pen, -15, -15, 30, 30);

                    // Dönen kuyruk
                    for (int i = 0; i < 4; i++)
                    {
                        e.Graphics.RotateTransform(90);
                        float alpha = 1.0f - (i * 0.25f);
                        pen.Color = Color.FromArgb((int)(255 * alpha), 0, 120, 215);
                        e.Graphics.DrawArc(pen, -15, -15, 30, 30, 0, 90);
                    }
                }
            };

            // Count spinner timer'ı
            countSpinnerTimer = new System.Windows.Forms.Timer();
            countSpinnerTimer.Interval = 50;
            int countAngle = 0;

            countSpinnerTimer.Tick += (s, e) =>
            {
                countAngle = (countAngle + 10) % 360;
                countSpinner.Invalidate();
            };

            countSpinner.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.Clear(Color.White); // Arka planı şeffaf temizle

                e.Graphics.TranslateTransform(countSpinner.Width / 2, countSpinner.Height / 2);
                e.Graphics.RotateTransform(countAngle);

                using (Pen pen = new Pen(Color.FromArgb(0, 120, 215), 2))
                {
                    // Ana daire
                    e.Graphics.DrawEllipse(pen, -9, -9, 18, 18);

                    // Dönen kuyruk
                    for (int i = 0; i < 4; i++)
                    {
                        e.Graphics.RotateTransform(90);
                        float alpha = 1.0f - (i * 0.25f);
                        pen.Color = Color.FromArgb((int)(255 * alpha), 0, 120, 215);
                        e.Graphics.DrawArc(pen, -9, -9, 18, 18, 0, 90);
                    }
                }
            };

            this.Controls.Add(loadingSpinner);
            this.Controls.Add(countSpinner);

            // Ensure spinners are on top and handle transparency better
            loadingSpinner.Parent = this;
            loadingSpinner.BringToFront();
            countSpinner.Parent = this;
            countSpinner.BringToFront();
        }

        private void ShowLoadingSpinner()
        {
            loadingSpinner.Visible = true;
            loadingSpinner.Location = new Point((this.Width - loadingSpinner.Width) / 2, (this.Height - loadingSpinner.Height) / 2);
            loadingSpinner.BringToFront();
            spinnerTimer.Start();
        }

        private void HideLoadingSpinner()
        {
            loadingSpinner.Visible = false;
            spinnerTimer.Stop();
        }

        private void ShowCountSpinner()
        {
            countSpinner.Visible = true;
            countSpinner.Location = new Point(labelCount.Right + 10, labelCount.Top + 2);
            countSpinner.BringToFront();
            countSpinnerTimer.Start();
        }

        private void HideCountSpinner()
        {
            countSpinner.Visible = false;
            countSpinnerTimer.Stop();
        }

        private async void btnScan_Click(object sender, EventArgs e)
        {
            flowDevices.Controls.Clear();
            ShowLoadingSpinner();
            ShowCountSpinner();
            labelCount.Text = "Tarama yapılıyor...";
            deviceCount = 0;

            // Tarama sırasında butonları devre dışı bırak
            btnScan.Enabled = false;
            btnBack.Enabled = false;

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
                        string connectionType = GetConnectionType(ip);
                        string deviceType = GetDeviceType(vendor);

                        AddDeviceCard(ip, mac, hostname, vendor, customName, connectionType, deviceType);
                        deviceCount++;

                        // İlk cihaz bulunduğunda ana spinner'ı gizle
                        if (deviceCount == 1)
                        {
                            HideLoadingSpinner();
                        }

                        labelCount.Text = $"Toplam cihaz sayısı: {deviceCount}";

                        // Count spinner'ın pozisyonunu güncelle
                        countSpinner.Location = new Point(labelCount.Right + 10, labelCount.Top + 2);

                        // UI'yi güncelle
                        Application.DoEvents();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                // Eğer hiç cihaz bulunamadıysa ana spinner'ı gizle
                if (deviceCount == 0)
                {
                    HideLoadingSpinner();
                }

                // Count spinner'ı gizle ve son sayıyı göster
                HideCountSpinner();
                labelCount.Text = $"Toplam cihaz sayısı: {deviceCount}";

                // Butonları tekrar etkinleştir
                btnScan.Enabled = true;
                btnBack.Enabled = true;
            }
        }

        private string GetConnectionType(string ip)
        {
            string[] parts = ip.Split('.');
            if (parts.Length == 4)
            {
                int firstOctet = int.Parse(parts[0]);
                if (firstOctet == 192 && parts[1] == "168")
                    return "Yerel Ağ";
                else if (firstOctet == 10)
                    return "Yerel Ağ";
                else if (firstOctet == 172 && int.Parse(parts[1]) >= 16 && int.Parse(parts[1]) <= 31)
                    return "Yerel Ağ";
            }
            return "Harici Ağ";
        }

        private string GetDeviceType(string vendor)
        {
            vendor = vendor.ToLower();
            if (vendor.Contains("apple") || vendor.Contains("iphone") || vendor.Contains("ipad"))
                return "Apple Cihazı";
            else if (vendor.Contains("samsung") || vendor.Contains("android"))
                return "Android Cihazı";
            else if (vendor.Contains("microsoft") || vendor.Contains("windows"))
                return "Windows Cihazı";
            else if (vendor.Contains("router") || vendor.Contains("gateway"))
                return "Ağ Cihazı";
            else
                return "Diğer Cihaz";
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

        private Dictionary<string, string> macVendorCache = new Dictionary<string, string>();
        private string macVendorCacheFile = "mac_vendors.txt";

        private async Task<string> GetVendorFromMacAsync(string mac)
        {
            // Clean up the MAC address
            mac = mac.Replace(":", "").Replace("-", "").ToUpper();

            // Check the cache first
            if (macVendorCache.ContainsKey(mac))
                return macVendorCache[mac];

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "CSharpApp");
                    string url = $"https://api.macvendors.com/{mac}";
                    var response = await client.GetStringAsync(url);

                    if (!string.IsNullOrWhiteSpace(response))
                    {
                        // Cache the result to reduce API calls
                        macVendorCache[mac] = response;
                        File.AppendAllLines(macVendorCacheFile, new[] { $"{mac}|{response}" });
                        return response;
                    }
                }
            }
            catch
            {
                return "Bilinmeyen Üretici";
            }

            return "Bilinmeyen Üretici";
        }

        private void LoadMacVendorCache()
        {
            if (File.Exists(macVendorCacheFile))
            {
                foreach (var line in File.ReadAllLines(macVendorCacheFile))
                {
                    var parts = line.Split('|');
                    if (parts.Length == 2)
                        macVendorCache[parts[0]] = parts[1];
                }
            }
        }

        private async Task<List<int>> ScanOpenPortsAsync(string ip)
        {
            List<int> openPorts = new List<int>();
            int[] commonPorts = { 21, 22, 23, 25, 53, 80, 110, 143, 443, 445, 3306, 3389, 8080 };

            foreach (int port in commonPorts)
            {
                try
                {
                    using (TcpClient client = new TcpClient())
                    {
                        var connectTask = client.ConnectAsync(ip, port);
                        if (await Task.WhenAny(connectTask, Task.Delay(100)) == connectTask)
                        {
                            openPorts.Add(port);
                        }
                    }
                }
                catch { }
            }
            return openPorts;
        }

        private string GetPortService(int port)
        {
            switch (port)
            {
                case 21: return "FTP";
                case 22: return "SSH";
                case 23: return "Telnet";
                case 25: return "SMTP";
                case 53: return "DNS";
                case 80: return "HTTP";
                case 110: return "POP3";
                case 143: return "IMAP";
                case 443: return "HTTPS";
                case 445: return "SMB";
                case 3306: return "MySQL";
                case 3389: return "RDP";
                case 8080: return "HTTP-Alt";
                default: return "Unknown";
            }
        }

        private string GetWiFiInterface(string ip)
        {
            try
            {
                string output = RunCommand("netsh", "wlan show interfaces");
                string[] lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                
                foreach (string line in lines)
                {
                    if (line.Contains("Name") && !line.Contains("Description"))
                    {
                        return line.Split(':')[1].Trim();
                    }
                }
            }
            catch { }
            return "Bilinmiyor";
        }

        private async void AddDeviceCard(string ip, string mac, string hostname, string vendor, string name, string connectionType, string deviceType)
        {
            var panel = new Panel
            {
                Width = 240,
                Height = 240,
                Margin = new Padding(10),
                Padding = new Padding(15)
            };

            panel.Paint += (s, e) =>
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    panel.ClientRectangle,
                    Color.FromArgb(255, 255, 255),
                    Color.FromArgb(240, 240, 240),
                    45F))
                {
                    e.Graphics.FillRectangle(brush, panel.ClientRectangle);
                }

                using (Pen pen = new Pen(Color.FromArgb(200, 200, 200), 1))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, panel.Width - 1, panel.Height - 1);
                }
            };

            var iconLabel = new Label
            {
                Text = GetDeviceIcon(deviceType),
                Font = new Font("Segoe UI Symbol", 32),
                Location = new Point(10, 10),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            var nameLabel = new Label
            {
                Text = name,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(iconLabel.Right + 5, 15),
                AutoSize = true
            };

            var typeLabel = new Label
            {
                Text = deviceType,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Gray,
                Location = new Point(iconLabel.Right + 5, 40),
                Size = new Size(panel.Width - (iconLabel.Right + 10), 30),
                AutoSize = false
            };

            var connectionLabel = new Label
            {
                Text = $"🌐 {connectionType}",
                Font = new Font("Segoe UI", 9),
                Location = new Point(15, 80),
                AutoSize = true
            };

            var ipLabel = new Label
            {
                Text = $"📡 IP: {ip}",
                Font = new Font("Segoe UI", 9),
                Location = new Point(15, 105),
                AutoSize = true
            };

            var macLabel = new Label
            {
                Text = $"🔑 MAC: {mac}",
                Font = new Font("Segoe UI", 9),
                Location = new Point(15, 130),
                AutoSize = true
            };

            var vendorLabel = new Label
            {
                Text = $"🏭 Üretici: {vendor}",
                Font = new Font("Segoe UI", 9),
                Location = new Point(15, 155),
                AutoSize = true
            };

            // Port taraması yap
            var openPorts = await ScanOpenPortsAsync(ip);
            var portsText = openPorts.Count > 0 
                ? string.Join(", ", openPorts.Select(p => $"{p} ({GetPortService(p)})"))
                : "Açık port bulunamadı";

            var wifiInterface = GetWiFiInterface(ip);
            var portsLabel = new Label
            {
                Text = $"📶 WiFi: {wifiInterface}",
                Font = new Font("Segoe UI", 9),
                Location = new Point(15, 180),
                Size = new Size(210, 40),
                AutoSize = false
            };

            panel.MouseEnter += (s, e) => panel.BackColor = Color.FromArgb(245, 245, 245);
            panel.MouseLeave += (s, e) => panel.BackColor = Color.White;

            panel.Controls.AddRange(new Control[] { 
                iconLabel, nameLabel, typeLabel, connectionLabel, 
                ipLabel, macLabel, vendorLabel, portsLabel 
            });

            panel.DoubleClick += (sender, e) =>
            {
                var deviceInfoForm = new DeviceInfoForm(
                    name, ip, mac, vendor, hostname, connectionType, deviceType
                );
                deviceInfoForm.ShowDialog();
            };

            flowDevices.Controls.Add(panel);
        }

        private string GetDeviceIcon(string deviceType)
        {
            switch (deviceType)
            {
                case "Apple Cihazı":
                    return "🍎";
                case "Android Cihazı":
                    return "📱";
                case "Windows Cihazı":
                    return "💻";
                case "Ağ Cihazı":
                    return "🌐";
                default:
                    return "📟";
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            MainMenuForm mainMenu = new MainMenuForm();
            mainMenu.Show();
            this.Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                MainMenuForm mainMenu = new MainMenuForm();
                mainMenu.Show();
            }
            base.OnFormClosing(e);
        }

        // Form yeniden boyutlandırıldığında spinner'ları ortala
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (loadingSpinner != null && loadingSpinner.Visible)
            {
                loadingSpinner.Location = new Point((this.Width - loadingSpinner.Width) / 2, (this.Height - loadingSpinner.Height) / 2);
            }
            if (countSpinner != null && countSpinner.Visible)
            {
                countSpinner.Location = new Point(labelCount.Right + 10, labelCount.Top + 2);
            }
        }
    }
}