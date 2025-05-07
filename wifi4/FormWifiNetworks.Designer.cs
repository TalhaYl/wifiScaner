namespace wifi4
{
    partial class FormWifiNetworks
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormWifiNetworks));
            btnWifiScan = new Button();
            btnback = new Button();
            labelNetworkCount = new Label();
            listViewWifiNetworks = new ListView();
            ssıd = new ColumnHeader();
            signal = new ColumnHeader();
            security = new ColumnHeader();
            lblScan = new Label();
            lblHomePage = new Label();
            SuspendLayout();
            // 
            // btnWifiScan
            // 
            btnWifiScan.BackgroundImage = (Image)resources.GetObject("btnWifiScan.BackgroundImage");
            btnWifiScan.BackgroundImageLayout = ImageLayout.Center;
            btnWifiScan.FlatAppearance.BorderSize = 0;
            btnWifiScan.FlatStyle = FlatStyle.Popup;
            btnWifiScan.ImageAlign = ContentAlignment.MiddleLeft;
            btnWifiScan.Location = new Point(22, 398);
            btnWifiScan.Name = "btnWifiScan";
            btnWifiScan.Size = new Size(73, 86);
            btnWifiScan.TabIndex = 0;
            btnWifiScan.TextAlign = ContentAlignment.BottomCenter;
            btnWifiScan.UseVisualStyleBackColor = true;
            btnWifiScan.Click += btnScanWifi_Click;
            // 
            // btnback
            // 
            btnback.BackgroundImage = (Image)resources.GetObject("btnback.BackgroundImage");
            btnback.BackgroundImageLayout = ImageLayout.Center;
            btnback.FlatAppearance.BorderSize = 0;
            btnback.FlatStyle = FlatStyle.Popup;
            btnback.Location = new Point(360, 398);
            btnback.Name = "btnback";
            btnback.Size = new Size(63, 86);
            btnback.TabIndex = 1;
            btnback.UseVisualStyleBackColor = true;
            btnback.Click += btnBack_Click;
            // 
            // labelNetworkCount
            // 
            labelNetworkCount.AutoSize = true;
            labelNetworkCount.Location = new Point(22, 87);
            labelNetworkCount.Name = "labelNetworkCount";
            labelNetworkCount.Size = new Size(88, 20);
            labelNetworkCount.TabIndex = 3;
            labelNetworkCount.Text = "Total Wifi :0";
            // 
            // listViewWifiNetworks
            // 
            listViewWifiNetworks.BackColor = Color.Gainsboro;
            listViewWifiNetworks.Columns.AddRange(new ColumnHeader[] { ssıd, signal, security });
            listViewWifiNetworks.Location = new Point(22, 128);
            listViewWifiNetworks.Name = "listViewWifiNetworks";
            listViewWifiNetworks.Size = new Size(401, 264);
            listViewWifiNetworks.TabIndex = 4;
            listViewWifiNetworks.UseCompatibleStateImageBehavior = false;
            listViewWifiNetworks.View = View.Details;
            // 
            // ssıd
            // 
            ssıd.Text = "ssıd";
            // 
            // signal
            // 
            signal.Text = "sıgnal";
            // 
            // security
            // 
            security.Text = "security";
            // 
            // lblScan
            // 
            lblScan.AutoSize = true;
            lblScan.Location = new Point(24, 487);
            lblScan.Name = "lblScan";
            lblScan.Size = new Size(71, 20);
            lblScan.TabIndex = 5;
            lblScan.Text = "Wifi Scan";
            // 
            // lblHomePage
            // 
            lblHomePage.AutoSize = true;
            lblHomePage.Location = new Point(350, 487);
            lblHomePage.Name = "lblHomePage";
            lblHomePage.Size = new Size(86, 20);
            lblHomePage.TabIndex = 6;
            lblHomePage.Text = "Home Page";
            // 
            // FormWifiNetworks
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(192, 255, 255);
            ClientSize = new Size(511, 604);
            Controls.Add(lblHomePage);
            Controls.Add(lblScan);
            Controls.Add(listViewWifiNetworks);
            Controls.Add(labelNetworkCount);
            Controls.Add(btnback);
            Controls.Add(btnWifiScan);
            Name = "FormWifiNetworks";
            Text = "FormWifiNetworks";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnWifiScan;
        private Button btnback;
        private Label labelNetworkCount;
        private ListView listViewWifiNetworks;
        private ColumnHeader ssıd;
        private ColumnHeader signal;
        private ColumnHeader security;
        private Label lblScan;
        private Label lblHomePage;
    }
}