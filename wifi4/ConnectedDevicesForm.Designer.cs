namespace wifi4
{
    partial class ConnectedDevicesForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConnectedDevicesForm));
            btnScan = new Button();
            flowDevices = new FlowLayoutPanel();
            labelCount = new Label();
            btnBack = new Button();
            SuspendLayout();
            // 
            // btnScan
            // 
            btnScan.BackgroundImage = (Image)resources.GetObject("btnScan.BackgroundImage");
            btnScan.BackgroundImageLayout = ImageLayout.Center;
            btnScan.FlatStyle = FlatStyle.Popup;
            btnScan.Location = new Point(47, 576);
            btnScan.Name = "btnScan";
            btnScan.Size = new Size(63, 58);
            btnScan.TabIndex = 0;
            btnScan.UseVisualStyleBackColor = true;
            btnScan.Click += btnScan_Click;
            // 
            // flowDevices
            // 
            flowDevices.AutoScroll = true;
            flowDevices.Location = new Point(47, 116);
            flowDevices.Name = "flowDevices";
            flowDevices.Size = new Size(250, 441);
            flowDevices.TabIndex = 1;
            // 
            // labelCount
            // 
            labelCount.AutoSize = true;
            labelCount.Location = new Point(47, 93);
            labelCount.Name = "labelCount";
            labelCount.Size = new Size(112, 20);
            labelCount.TabIndex = 2;
            labelCount.Text = "Total Devices :0";
            // 
            // btnBack
            // 
            btnBack.BackgroundImage = (Image)resources.GetObject("btnBack.BackgroundImage");
            btnBack.BackgroundImageLayout = ImageLayout.Center;
            btnBack.FlatStyle = FlatStyle.Popup;
            btnBack.Location = new Point(225, 576);
            btnBack.Name = "btnBack";
            btnBack.Size = new Size(72, 58);
            btnBack.TabIndex = 3;
            btnBack.UseVisualStyleBackColor = true;
            btnBack.Click += btnBack_Click;
            // 
            // ConnectedDevicesForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(192, 255, 255);
            ClientSize = new Size(502, 657);
            Controls.Add(btnScan);
            Controls.Add(labelCount);
            Controls.Add(btnBack);
            Controls.Add(flowDevices);
            Name = "ConnectedDevicesForm";
            Text = "ConnectedDevicesForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnScan;
        private FlowLayoutPanel flowDevices;
        private Label labelCount;
        private Button btnBack;
    }
}