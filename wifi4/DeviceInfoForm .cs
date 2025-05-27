using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using System.Linq;

#nullable enable

namespace wifi4
{
    public partial class DeviceInfoForm : Form
    {
        private Dictionary<string, string> customDeviceNames = new Dictionary<string, string>();
        private string customNamesFilePath = "custom_device_names.txt";
        private string deviceMac;
        private string deviceName;
        private string deviceIp;
        private Panel? cardPanel;

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

            CreateVCardPanel(deviceName, ip, mac, vendor, hostname, connectionType, deviceType);
        }

        private void CreateVCardPanel(string deviceName, string ip, string mac, string vendor, string hostname, string connectionType, string deviceType)
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
                            e.Graphics.DrawPath(shadowPen, path);
                        }
                    }

                    using (SolidBrush brush = new SolidBrush(Color.White))
                    {
                        e.Graphics.FillPath(brush, path);
                    }

                    using (Pen pen = new Pen(Color.FromArgb(200, 200, 200), 1))
                    {
                        e.Graphics.DrawPath(pen, path);
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

            // Avatar Panel
            Panel avatarPanel = new Panel
            {
                Size = new Size(70, 70),
                Location = new Point(15, 15),
                BackColor = Color.Transparent
            };
            avatarPanel.Paint += (sender, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle rect = new Rectangle(0, 0, avatarPanel.Width, avatarPanel.Height);
                using (GraphicsPath path = CreateRoundedRectangle(rect, 8))
                {
                    e.Graphics.FillPath(Brushes.White, path);
                    using (Font iconFont = new Font("Segoe UI Symbol", 30))
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

            // Device Name Panel
            Panel deviceNamePanel = new Panel
            {
                Size = new Size(180, 30),
                Location = new Point(95, 20),
                BackColor = Color.Transparent
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
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = false,
                Size = deviceNamePanel.Size,
                Location = new Point(0, 0),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent,
                Name = "lblName"
            };
            deviceNamePanel.Controls.Add(lblDeviceName);

            // MAC Address Panel
            Panel macPanel = new Panel
            {
                Size = new Size(180, 25),
                Location = new Point(95, 55),
                BackColor = Color.Transparent
            };
            macPanel.Paint += (sender, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle rect = new Rectangle(0, 0, macPanel.Width, macPanel.Height);
                using (GraphicsPath path = CreateRoundedRectangle(rect, 5))
                {
                    e.Graphics.FillPath(Brushes.White, path);
                }
            };
            headerPanel.Controls.Add(macPanel);

            Label lblMacSubtitle = new Label
            {
                Text = mac,
                ForeColor = Color.FromArgb(80, 80, 80),
                Font = new Font("Segoe UI", 9),
                AutoSize = false,
                Size = macPanel.Size,
                Location = new Point(0, 0),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            macPanel.Controls.Add(lblMacSubtitle);

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
                if (sender is Button btn)
                {
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
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;

            if (topLeft)
            {
                path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            }
            else
            {
                path.AddLine(rect.X, rect.Y, rect.X, rect.Y);
            }

            if (topRight)
            {
                path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            }
            else
            {
                path.AddLine(rect.Right, rect.Y, rect.Right, rect.Y);
            }

            if (bottomRight)
            {
                path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            }
            else
            {
                path.AddLine(rect.Right, rect.Bottom, rect.Right, rect.Bottom);
            }

            if (bottomLeft)
            {
                path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            }
            else
            {
                path.AddLine(rect.X, rect.Bottom, rect.X, rect.Bottom);
            }

            path.CloseFigure();
            return path;
        }

        private void BtnRename_Click(object? sender, EventArgs e)
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
            try
            {
                var lines = customDeviceNames.Select(kvp => $"{kvp.Key}|{kvp.Value}");
                File.WriteAllLines(customNamesFilePath, lines);
            }
            catch { }
        }

        private void LoadCustomDeviceNames()
        {
            try
            {
                if (File.Exists(customNamesFilePath))
                {
                    foreach (var line in File.ReadAllLines(customNamesFilePath))
                    {
                        var parts = line.Split('|');
                        if (parts.Length == 2)
                            customDeviceNames[parts[0]] = parts[1];
                    }
                }
            }
            catch { }
        }
    }
}