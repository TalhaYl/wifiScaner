using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace wifi4
{
    public partial class DeviceInfoForm : Form
    {
        private Dictionary<string, string> customDeviceNames = new Dictionary<string, string>();
        private string customNamesFilePath = "custom_device_names.txt";

        private string deviceMac;
        private string deviceName;
        private string deviceIp;
        private Panel cardPanel;
        private PictureBox loadingSpinner;

        public DeviceInfoForm(string deviceName, string ip, string mac, string vendor, string hostname, string connectionType, string deviceType)
        {
            InitializeComponent();

            LoadCustomDeviceNames();

            this.deviceMac = mac;
            this.deviceName = deviceName;
            this.deviceIp = ip;
            this.Text = deviceName + " - Detaylar";
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.ClientSize = new Size(400, 450);

            InitializeLoadingSpinner();
            InitializeAsync(deviceName, ip, mac, vendor, hostname, connectionType, deviceType);
        }

        private async void InitializeAsync(string deviceName, string ip, string mac, string vendor, string hostname, string connectionType, string deviceType)
        {
            await CreateVCardPanel(deviceName, ip, mac, vendor, hostname, connectionType, deviceType);
        }

        private void InitializeLoadingSpinner()
        {
            loadingSpinner = new PictureBox
            {
                Size = new Size(40, 40),
                Location = new Point(20, 380),
                Visible = false
            };

            System.Windows.Forms.Timer spinnerTimer = new System.Windows.Forms.Timer();
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

            this.Controls.Add(loadingSpinner);
            spinnerTimer.Start();
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

        private async Task CreateVCardPanel(string deviceName, string ip, string mac, string vendor, string hostname, string connectionType, string deviceType)
        {
            cardPanel = new Panel
            {
                Size = new Size(360, 380),
                Location = new Point(20, 20),
                BackColor = Color.White
            };
            this.Controls.Add(cardPanel);

            cardPanel.Paint += (sender, e) =>
            {
                Rectangle rect = new Rectangle(0, 0, cardPanel.Width, cardPanel.Height);
                using (GraphicsPath path = CreateRoundedRectangle(rect, 10))
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                    for (int i = 1; i <= 5; i++)
                    {
                        using (Pen shadowPen = new Pen(Color.FromArgb(20, 0, 0, 0), 1))
                        {
                            e.Graphics.DrawPath(shadowPen, CreateRoundedRectangle(new Rectangle(i, i, rect.Width - i * 2, rect.Height - i * 2), 10));
                        }
                    }

                    using (SolidBrush cardBrush = new SolidBrush(Color.White))
                    {
                        e.Graphics.FillPath(cardBrush, path);
                    }

                    using (Pen borderPen = new Pen(Color.FromArgb(230, 230, 230), 1))
                    {
                        e.Graphics.DrawPath(borderPen, path);
                    }
                }
            };

            Panel headerPanel = new Panel
            {
                Size = new Size(360, 100),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(0, 123, 255)
            };
            headerPanel.Paint += (sender, e) =>
            {
                Rectangle rect = new Rectangle(0, 0, headerPanel.Width, headerPanel.Height);
                using (GraphicsPath path = CreateRoundedRectanglePath(rect, 10, true, true, false, false))
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                    using (LinearGradientBrush brush = new LinearGradientBrush(
                        rect,
                        Color.FromArgb(0, 123, 255),
                        Color.FromArgb(0, 80, 200),
                        LinearGradientMode.ForwardDiagonal))
                    {
                        e.Graphics.FillPath(brush, path);
                    }
                }
            };
            cardPanel.Controls.Add(headerPanel);

            // Avatar Panel (Rounded White Background)
            Panel avatarPanel = new Panel
            {
                Size = new Size(70, 70),
                Location = new Point(15, 15),
                BackColor = Color.Transparent // Will be drawn in Paint event
            };
             avatarPanel.Paint += (sender, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle rect = new Rectangle(0, 0, avatarPanel.Width, avatarPanel.Height);
                using (GraphicsPath path = CreateRoundedRectangle(rect, 8))
                {
                     e.Graphics.FillPath(Brushes.White, path);
                     // Draw icon on top of the rounded background
                      using (Font iconFont = new Font("Segoe UI Symbol", 30)) // Increased font size
                        {
                            string iconChar = "📱";
                            if (deviceType.Contains("Router") || deviceType.Contains("Modem"))
                                iconChar = "📶";
                            else if (deviceType.Contains("PC") || deviceType.Contains("Bilgisayar"))
                                iconChar = "💻";
                            else if (deviceType.Contains("TV") || deviceType.Contains("Television"))
                                iconChar = "📺";
                            else if (deviceType.Contains("Printer") || deviceType.Contains("Yazıcı"))
                                iconChar = "🖨️";

                            SizeF size = e.Graphics.MeasureString(iconChar, iconFont);
                            e.Graphics.DrawString(iconChar, iconFont, Brushes.DodgerBlue,
                                (avatarPanel.Width - size.Width) / 2,
                                (avatarPanel.Height - size.Height) / 2);
                        }
                }
            };
            headerPanel.Controls.Add(avatarPanel);

            // Device Name Panel (Rounded White Background)
            Panel deviceNamePanel = new Panel
            {
                 Size = new Size(180, 30),
                 Location = new Point(95, 20),
                 BackColor = Color.Transparent // Will be drawn in Paint event
            };
            deviceNamePanel.Paint += (sender, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle rect = new Rectangle(0, 0, deviceNamePanel.Width, deviceNamePanel.Height);
                 using (GraphicsPath path = CreateRoundedRectangle(rect, 5))
                {
                     e.Graphics.FillPath(Brushes.White, path);
                }
            };
            headerPanel.Controls.Add(deviceNamePanel);

            Label lblDeviceName = new Label
            {
                Text = deviceName,
                Font = new Font("Segoe UI", 12, FontStyle.Bold), // Adjusted font size
                AutoSize = false, // Set AutoSize to false
                Size = deviceNamePanel.Size, // Match parent panel size
                Location = new Point(0, 0), // Position at top-left of parent panel
                TextAlign = ContentAlignment.MiddleCenter, // Center text
                BackColor = Color.Transparent,
                Name = "lblName"
            };
            deviceNamePanel.Controls.Add(lblDeviceName); // Add to the new panel

            // MAC Address Panel (Already has rounded corners, adjusting size and location)
            Panel macPanel = new Panel
            {
                Size = new Size(180, 25), // Adjusted size
                Location = new Point(95, 55), // Adjusted location
                BackColor = Color.Transparent // Will be drawn in Paint event
            };
            macPanel.Paint += (sender, e) => // Retained Paint event for rounded corners
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle rect = new Rectangle(0, 0, macPanel.Width, macPanel.Height);
                using (GraphicsPath path = CreateRoundedRectangle(rect, 5)) // Reduced radius slightly
                {
                    e.Graphics.FillPath(Brushes.White, path);
                }
            };
            headerPanel.Controls.Add(macPanel); // Ensure it's added to the header panel

            Label lblMacSubtitle = new Label
            {
                Text = mac,
                ForeColor = Color.FromArgb(80, 80, 80),
                Font = new Font("Segoe UI", 9),
                AutoSize = false, // Set AutoSize to false
                Size = macPanel.Size, // Match parent panel size
                Location = new Point(0, 0), // Position at top-left of parent panel
                 TextAlign = ContentAlignment.MiddleCenter, // Center text
                BackColor = Color.Transparent
            };
            macPanel.Controls.Add(lblMacSubtitle); // Add to macPanel

            Panel contentPanel = new Panel
            {
                Size = new Size(360, 280),
                Location = new Point(0, 100),
                BackColor = Color.White
            };
            cardPanel.Controls.Add(contentPanel);

            CreateInfoRow(contentPanel, "📡", "IP Adresi", ip, 20);
            CreateInfoRow(contentPanel, "🏭", "Üretici", vendor, 55);
            CreateInfoRow(contentPanel, "📛", "Hostname", hostname, 90);
            CreateInfoRow(contentPanel, "🌐", "Bağlantı", connectionType, 125);
            CreateInfoRow(contentPanel, "📱", "Cihaz Türü", deviceType, 160);

            // Port taraması başlat
            loadingSpinner.Visible = true;
            var openPorts = await ScanOpenPortsAsync(ip);
            loadingSpinner.Visible = false;

            var portsText = openPorts.Count > 0
                ? string.Join(", ", openPorts.Select(p => $"{p} ({GetPortService(p)})"))
                : "Açık port bulunamadı";

            CreateInfoRow(contentPanel, "🔌", "Açık Portlar", portsText, 195);

            Button btnRename = new Button
            {
                Text = "İsim Değiştir",
                Size = new Size(150, 40),
                Location = new Point(105, 240),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnRename.FlatAppearance.BorderSize = 0;
            btnRename.Paint += (sender, e) =>
            {
                Button btn = (Button)sender;
                Rectangle rect = new Rectangle(0, 0, btn.Width, btn.Height);

                using (GraphicsPath path = CreateRoundedRectangle(rect, 20))
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    using (SolidBrush brush = new SolidBrush(btn.BackColor))
                    {
                        e.Graphics.FillPath(brush, path);
                    }
                }

                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;

                using (SolidBrush textBrush = new SolidBrush(btn.ForeColor))
                {
                    e.Graphics.DrawString(btn.Text, btn.Font, textBrush, rect, sf);
                }
            };
            btnRename.Click += BtnRename_Click;
            contentPanel.Controls.Add(btnRename);
        }

        private void CreateInfoRow(Panel parent, string icon, string label, string value, int yPosition)
        {
            Label lblIcon = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI Symbol", 14),
                ForeColor = Color.FromArgb(0, 123, 255),
                AutoSize = true,
                Location = new Point(20, yPosition)
            };
            parent.Controls.Add(lblIcon);

            Label lblLabel = new Label
            {
                Text = label + ":",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 100, 100),
                AutoSize = true,
                Location = new Point(50, yPosition + 3)
            };
            parent.Controls.Add(lblLabel);

            Label lblValue = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(60, 60, 60),
                AutoSize = true,
                Location = new Point(155, yPosition + 3)
            };
            parent.Controls.Add(lblValue);
        }

        private GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
        {
            return CreateRoundedRectanglePath(rect, radius, true, true, true, true);
        }

        private GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int radius, bool topLeft, bool topRight, bool bottomRight, bool bottomLeft)
        {
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(rect.Location, size);
            GraphicsPath path = new GraphicsPath();

            if (topLeft)
            {
                path.AddArc(arc, 180, 90);
            }
            else
            {
                path.AddLine(arc.X, arc.Y, arc.X, arc.Y);
            }

            arc.X = rect.Right - diameter;
            if (topRight)
            {
                path.AddArc(arc, 270, 90);
            }
            else
            {
                path.AddLine(arc.Right, arc.Y, arc.Right, arc.Y);
            }

            arc.Y = rect.Bottom - diameter;
            if (bottomRight)
            {
                path.AddArc(arc, 0, 90);
            }
            else
            {
                path.AddLine(arc.Right, arc.Bottom, arc.Right, arc.Bottom);
            }

            arc.X = rect.Left;
            if (bottomLeft)
            {
                path.AddArc(arc, 90, 90);
            }
            else
            {
                path.AddLine(arc.X, arc.Bottom, arc.X, arc.Bottom);
            }

            path.CloseFigure();
            return path;
        }

        private void BtnRename_Click(object sender, EventArgs e)
        {
            RenameDevice(deviceMac, deviceName);
        }

        public void RenameDevice(string mac, string currentName)
        {
            var inputBox = new TextBox
            {
                Text = currentName,
                Font = new Font("Segoe UI", 10),
                Width = 300,
                Margin = new Padding(5),
                Location = new Point(20, 20)
            };

            var dialog = new Form
            {
                Width = 360,
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
                Location = new Point(130, 60),
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

                    deviceName = newName;
                    var lblName = this.Controls.Find("lblName", true).FirstOrDefault() as Label;
                    if (lblName != null)
                    {
                        lblName.Text = "Cihaz İsmi: " + newName;
                    }

                    MessageBox.Show("Cihaz adı başarıyla güncellendi!");
                }
            }
        }

        private void SaveCustomDeviceNames()
        {
            File.WriteAllLines(customNamesFilePath, customDeviceNames.Select(kvp => $"{kvp.Key}|{kvp.Value}"));
        }

        private void LoadCustomDeviceNames()
        {
            if (!File.Exists(customNamesFilePath)) return;

            foreach (var line in File.ReadAllLines(customNamesFilePath))
            {
                var parts = line.Split('|');
                if (parts.Length == 2)
                    customDeviceNames[parts[0]] = parts[1];
            }
        }
    }
}