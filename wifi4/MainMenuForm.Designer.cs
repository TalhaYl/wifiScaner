namespace wifi4
{
    partial class MainMenuForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainMenuForm));
            btnWifiScan = new Button();
            btnConnectedDevices = new Button();
            labelWifi = new Label();
            labelDevices = new Label();
            SuspendLayout();
            // 
            // btnWifiScan
            // 
            btnWifiScan.BackColor = Color.FromArgb(192, 255, 255);
            btnWifiScan.BackgroundImage = (Image)resources.GetObject("btnWifiScan.BackgroundImage");
            btnWifiScan.BackgroundImageLayout = ImageLayout.Center;
            btnWifiScan.FlatStyle = FlatStyle.Popup;
            btnWifiScan.Location = new Point(44, 84);
            btnWifiScan.Name = "btnWifiScan";
            btnWifiScan.Size = new Size(142, 66);
            btnWifiScan.TabIndex = 0;
            btnWifiScan.UseVisualStyleBackColor = false;
            btnWifiScan.Click += btnWifiScan_Click;
            // 
            // btnConnectedDevices
            // 
            btnConnectedDevices.BackgroundImage = (Image)resources.GetObject("btnConnectedDevices.BackgroundImage");
            btnConnectedDevices.BackgroundImageLayout = ImageLayout.Center;
            btnConnectedDevices.FlatStyle = FlatStyle.Popup;
            btnConnectedDevices.Location = new Point(44, 186);
            btnConnectedDevices.Name = "btnConnectedDevices";
            btnConnectedDevices.Size = new Size(142, 74);
            btnConnectedDevices.TabIndex = 1;
            btnConnectedDevices.UseVisualStyleBackColor = true;
            btnConnectedDevices.Click += btnConnectedDevices_Click;
            // 
            // labelWifi
            // 
            labelWifi.AutoSize = true;
            labelWifi.Location = new Point(44, 61);
            labelWifi.Name = "labelWifi";
            labelWifi.Size = new Size(71, 20);
            labelWifi.TabIndex = 2;
            labelWifi.Text = "Wifi Scan";
            // 
            // labelDevices
            // 
            labelDevices.AutoSize = true;
            labelDevices.Location = new Point(47, 162);
            labelDevices.Name = "labelDevices";
            labelDevices.Size = new Size(95, 20);
            labelDevices.TabIndex = 3;
            labelDevices.Text = "Devices Scan";
            // 
            // MainMenuForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(192, 255, 255);
            ClientSize = new Size(439, 532);
            Controls.Add(labelDevices);
            Controls.Add(labelWifi);
            Controls.Add(btnConnectedDevices);
            Controls.Add(btnWifiScan);
            Name = "MainMenuForm";
            Text = "Form2";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnWifiScan;
        private Button btnConnectedDevices;
        private Label labelWifi;
        private Label labelDevices;
    }
}