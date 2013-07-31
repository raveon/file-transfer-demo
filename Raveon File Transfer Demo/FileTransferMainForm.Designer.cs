namespace Raveon.FileTransferDemo
{
    partial class FileTransferMainForm
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
            this.fileInformationPanel = new System.Windows.Forms.Panel();
            this.dragDropHidePanel = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.dragDropInfoPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.fileDragNameLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.fileInfo = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.fileInfoPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.fileInfoNameLabel = new System.Windows.Forms.Label();
            this.fileBrowseButton = new System.Windows.Forms.Button();
            this.fileNameTextBox = new System.Windows.Forms.TextBox();
            this.sendButton = new System.Windows.Forms.Button();
            this.serialPortTextBox = new System.Windows.Forms.TextBox();
            this.serialReconnectButton = new System.Windows.Forms.Button();
            this.incomingWMXGridView = new System.Windows.Forms.DataGridView();
            this.outgoingWMXGridView = new System.Windows.Forms.DataGridView();
            this.fileInformationPanel.SuspendLayout();
            this.dragDropHidePanel.SuspendLayout();
            this.dragDropInfoPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.fileInfoPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.incomingWMXGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.outgoingWMXGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // fileInformationPanel
            // 
            this.fileInformationPanel.AllowDrop = true;
            this.fileInformationPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.fileInformationPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.fileInformationPanel.Controls.Add(this.dragDropHidePanel);
            this.fileInformationPanel.Controls.Add(this.tableLayoutPanel1);
            this.fileInformationPanel.Controls.Add(this.label2);
            this.fileInformationPanel.Controls.Add(this.fileInfoPanel);
            this.fileInformationPanel.Controls.Add(this.fileBrowseButton);
            this.fileInformationPanel.Controls.Add(this.fileNameTextBox);
            this.fileInformationPanel.Location = new System.Drawing.Point(12, 12);
            this.fileInformationPanel.Name = "fileInformationPanel";
            this.fileInformationPanel.Size = new System.Drawing.Size(585, 214);
            this.fileInformationPanel.TabIndex = 0;
            this.fileInformationPanel.DragDrop += new System.Windows.Forms.DragEventHandler(this.fileInformationPanel_DragDrop);
            this.fileInformationPanel.DragEnter += new System.Windows.Forms.DragEventHandler(this.fileInformationPanel_DragEnter);
            this.fileInformationPanel.DragLeave += new System.EventHandler(this.fileInformationPanel_DragLeave);
            // 
            // dragDropHidePanel
            // 
            this.dragDropHidePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dragDropHidePanel.Controls.Add(this.label1);
            this.dragDropHidePanel.Controls.Add(this.dragDropInfoPanel);
            this.dragDropHidePanel.Location = new System.Drawing.Point(3, 3);
            this.dragDropHidePanel.Name = "dragDropHidePanel";
            this.dragDropHidePanel.Size = new System.Drawing.Size(575, 204);
            this.dragDropHidePanel.TabIndex = 2;
            this.dragDropHidePanel.Visible = false;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(3, 159);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(569, 45);
            this.label1.TabIndex = 2;
            this.label1.Text = "Drop files here to transfer them over-the-air!";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // dragDropInfoPanel
            // 
            this.dragDropInfoPanel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.dragDropInfoPanel.AutoSize = true;
            this.dragDropInfoPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.dragDropInfoPanel.Controls.Add(this.pictureBox1);
            this.dragDropInfoPanel.Controls.Add(this.fileDragNameLabel);
            this.dragDropInfoPanel.Location = new System.Drawing.Point(239, 29);
            this.dragDropInfoPanel.Name = "dragDropInfoPanel";
            this.dragDropInfoPanel.Size = new System.Drawing.Size(97, 91);
            this.dragDropInfoPanel.TabIndex = 1;
            this.dragDropInfoPanel.Resize += new System.EventHandler(this.centerControl_Resize);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Location = new System.Drawing.Point(3, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(85, 85);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // fileDragNameLabel
            // 
            this.fileDragNameLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.fileDragNameLabel.AutoSize = true;
            this.fileDragNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fileDragNameLabel.Location = new System.Drawing.Point(94, 33);
            this.fileDragNameLabel.Name = "fileDragNameLabel";
            this.fileDragNameLabel.Size = new System.Drawing.Size(0, 24);
            this.fileDragNameLabel.TabIndex = 3;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.fileInfo, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(6, 77);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(572, 130);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // fileInfo
            // 
            this.fileInfo.Enabled = false;
            this.fileInfo.Location = new System.Drawing.Point(3, 3);
            this.fileInfo.Name = "fileInfo";
            this.fileInfo.Size = new System.Drawing.Size(179, 124);
            this.fileInfo.TabIndex = 0;
            this.fileInfo.Text = "";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "File Path";
            // 
            // fileInfoPanel
            // 
            this.fileInfoPanel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.fileInfoPanel.AutoSize = true;
            this.fileInfoPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.fileInfoPanel.Controls.Add(this.pictureBox2);
            this.fileInfoPanel.Controls.Add(this.fileInfoNameLabel);
            this.fileInfoPanel.Location = new System.Drawing.Point(247, 32);
            this.fileInfoPanel.Name = "fileInfoPanel";
            this.fileInfoPanel.Size = new System.Drawing.Size(44, 39);
            this.fileInfoPanel.TabIndex = 2;
            this.fileInfoPanel.Resize += new System.EventHandler(this.horizontalCenterControl_Resize);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pictureBox2.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox2.Location = new System.Drawing.Point(3, 3);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(32, 33);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox2.TabIndex = 2;
            this.pictureBox2.TabStop = false;
            // 
            // fileInfoNameLabel
            // 
            this.fileInfoNameLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.fileInfoNameLabel.AutoSize = true;
            this.fileInfoNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fileInfoNameLabel.Location = new System.Drawing.Point(41, 7);
            this.fileInfoNameLabel.Name = "fileInfoNameLabel";
            this.fileInfoNameLabel.Size = new System.Drawing.Size(0, 24);
            this.fileInfoNameLabel.TabIndex = 3;
            // 
            // fileBrowseButton
            // 
            this.fileBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.fileBrowseButton.Location = new System.Drawing.Point(503, 4);
            this.fileBrowseButton.Name = "fileBrowseButton";
            this.fileBrowseButton.Size = new System.Drawing.Size(75, 23);
            this.fileBrowseButton.TabIndex = 1;
            this.fileBrowseButton.Text = "Browse...";
            this.fileBrowseButton.UseVisualStyleBackColor = true;
            this.fileBrowseButton.Click += new System.EventHandler(this.fileBrowseButton_Click);
            // 
            // fileNameTextBox
            // 
            this.fileNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.fileNameTextBox.Location = new System.Drawing.Point(52, 6);
            this.fileNameTextBox.Name = "fileNameTextBox";
            this.fileNameTextBox.Size = new System.Drawing.Size(445, 20);
            this.fileNameTextBox.TabIndex = 0;
            this.fileNameTextBox.TextChanged += new System.EventHandler(this.fileNameTextBox_TextChanged);
            // 
            // sendButton
            // 
            this.sendButton.Location = new System.Drawing.Point(522, 232);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(75, 23);
            this.sendButton.TabIndex = 1;
            this.sendButton.Text = "Send";
            this.sendButton.UseVisualStyleBackColor = true;
            this.sendButton.Click += new System.EventHandler(this.sendButton_Click);
            // 
            // serialPortTextBox
            // 
            this.serialPortTextBox.Location = new System.Drawing.Point(12, 233);
            this.serialPortTextBox.Name = "serialPortTextBox";
            this.serialPortTextBox.Size = new System.Drawing.Size(100, 20);
            this.serialPortTextBox.TabIndex = 4;
            this.serialPortTextBox.Text = "COM18";
            // 
            // serialReconnectButton
            // 
            this.serialReconnectButton.Location = new System.Drawing.Point(118, 232);
            this.serialReconnectButton.Name = "serialReconnectButton";
            this.serialReconnectButton.Size = new System.Drawing.Size(75, 23);
            this.serialReconnectButton.TabIndex = 5;
            this.serialReconnectButton.Text = "Connect";
            this.serialReconnectButton.UseVisualStyleBackColor = true;
            this.serialReconnectButton.Click += new System.EventHandler(this.serialReconnectButton_Click);
            // 
            // incomingWMXGridView
            // 
            this.incomingWMXGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.incomingWMXGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.incomingWMXGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.incomingWMXGridView.Location = new System.Drawing.Point(17, 260);
            this.incomingWMXGridView.Name = "incomingWMXGridView";
            this.incomingWMXGridView.Size = new System.Drawing.Size(285, 203);
            this.incomingWMXGridView.TabIndex = 6;
            // 
            // outgoingWMXGridView
            // 
            this.outgoingWMXGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.outgoingWMXGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.outgoingWMXGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.outgoingWMXGridView.Location = new System.Drawing.Point(304, 260);
            this.outgoingWMXGridView.Name = "outgoingWMXGridView";
            this.outgoingWMXGridView.Size = new System.Drawing.Size(288, 203);
            this.outgoingWMXGridView.TabIndex = 7;
            // 
            // FileTransferMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(609, 475);
            this.Controls.Add(this.outgoingWMXGridView);
            this.Controls.Add(this.incomingWMXGridView);
            this.Controls.Add(this.serialReconnectButton);
            this.Controls.Add(this.serialPortTextBox);
            this.Controls.Add(this.sendButton);
            this.Controls.Add(this.fileInformationPanel);
            this.Name = "FileTransferMainForm";
            this.Text = "Raveon File Transfer Demo";
            this.fileInformationPanel.ResumeLayout(false);
            this.fileInformationPanel.PerformLayout();
            this.dragDropHidePanel.ResumeLayout(false);
            this.dragDropHidePanel.PerformLayout();
            this.dragDropInfoPanel.ResumeLayout(false);
            this.dragDropInfoPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.fileInfoPanel.ResumeLayout(false);
            this.fileInfoPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.incomingWMXGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.outgoingWMXGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel fileInformationPanel;
        private System.Windows.Forms.Button fileBrowseButton;
        private System.Windows.Forms.TextBox fileNameTextBox;
        private System.Windows.Forms.FlowLayoutPanel dragDropInfoPanel;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label fileDragNameLabel;
        private System.Windows.Forms.Panel dragDropHidePanel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FlowLayoutPanel fileInfoPanel;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label fileInfoNameLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.RichTextBox fileInfo;
        private System.Windows.Forms.Button sendButton;
        private System.Windows.Forms.TextBox serialPortTextBox;
        private System.Windows.Forms.Button serialReconnectButton;
        private System.Windows.Forms.DataGridView incomingWMXGridView;
        private System.Windows.Forms.DataGridView outgoingWMXGridView;
    }
}

