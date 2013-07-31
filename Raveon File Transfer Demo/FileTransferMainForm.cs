using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using System.Threading;
using Raveon.Messaging;

namespace Raveon.FileTransferDemo {
    public partial class FileTransferMainForm : Form {
        private ObservableCollection<WMXMessage> incomingWMX { get; set; }
        private ObservableCollection<WMXMessage> outgoingWMX { get; set; }

        private BindingSource incomingBS;
        private BindingSource outgoingBS;

        public FileTransferMainForm() {
            InitializeComponent();

            incomingWMX = new ObservableCollection<WMXMessage>();
            outgoingWMX = new ObservableCollection<WMXMessage>();

            incomingBS = new BindingSource();
            incomingBS.DataSource = incomingWMX;
            incomingWMXGridView.DataSource = incomingBS;
            incomingWMXGridView.AllowUserToAddRows = false;
            incomingWMX.CollectionChanged += delegate { wmxList_CollectionChanged(); };

            outgoingBS = new BindingSource();
            outgoingBS.DataSource = outgoingWMX;
            outgoingWMXGridView.DataSource = outgoingBS;
            outgoingWMXGridView.AllowUserToAddRows = false;
            outgoingWMX.CollectionChanged += delegate { wmxList_CollectionChanged(); };
        }

        delegate void wmxList_CollectionChangedCallback();
        void wmxList_CollectionChanged() {
            if (!this.incomingWMXGridView.InvokeRequired) {
                incomingBS.ResetBindings(false);
                outgoingBS.ResetBindings(false);
            } else {
                wmxList_CollectionChangedCallback d = new wmxList_CollectionChangedCallback(wmxList_CollectionChanged);
                this.Invoke(d, new object[] { });
            }
        }

        private Image getIcon(string path) {
            return getIcon(path, SystemIcons.Question);
        }

        private Image getIcon(string path, Icon defaultIcon) {
            Icon fileIcon = Icon.ExtractAssociatedIcon(path);

            if (fileIcon != null) {
                return fileIcon.ToBitmap();
            } else {
                return defaultIcon.ToBitmap();
            }
        }

        private bool acceptDrag = false;
        private string filePath = "";
        private void fileInformationPanel_DragEnter(object sender, DragEventArgs e) {
            acceptDrag = false;
            filePath = "";
            string fileName = "";

            if (e.Data.GetDataPresent(DataFormats.Text)) {
                filePath = (string)e.Data.GetData(DataFormats.Text);
            } else if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                foreach (string file in (string[])e.Data.GetData(DataFormats.FileDrop)) {
                    filePath = file;
                }
            }

            if (filePath != "") {
                try {
                    if (File.Exists(filePath)) {
                        fileName = Path.GetFileName(filePath);

                        pictureBox1.Image = getIcon(filePath);

                        acceptDrag = true;
                    }
                } catch (Exception ex) {
                    if (ex is FileNotFoundException || ex is ArgumentException) {
                        acceptDrag = false;
                    } else {
                        throw;
                    }
                }
            }

            if (acceptDrag) {
                fileDragNameLabel.Text = fileName;
                e.Effect = DragDropEffects.Copy;
                dragDropHidePanel.Visible = true;
            } else {
                // React negatively to the drag
                pictureBox1.Image = null;
                e.Effect = DragDropEffects.None;
                dragDropHidePanel.Visible = false;
            }
        }

        private void fileInformationPanel_DragDrop(object sender, DragEventArgs e) {
            dragDropHidePanel.Visible = false;
            if(acceptDrag){
                fileNameTextBox.Text = filePath;
            }
        }

        private void fileInformationPanel_DragLeave(object sender, EventArgs e) {
            dragDropHidePanel.Visible = false;
        }

        private void centerControl_Resize(object sender, EventArgs e) {
            horizontalCenterControl_Resize(sender, e);
            verticalCenterControl_Resize(sender, e);
        }

        private void horizontalCenterControl_Resize(object sender, EventArgs e) {
            Control panel = (Control)sender;

            if (panel.Parent != null) {
                panel.Left = panel.Parent.Width / 2 - panel.Width / 2;
            }
        }

        private void verticalCenterControl_Resize(object sender, EventArgs e) {
            Control panel = (Control)sender;

            if (panel.Parent != null) {
                panel.Top = panel.Parent.Height / 2 - panel.Height / 2;
            }
        }

        private void fileNameTextBox_TextChanged(object sender, EventArgs e) {
            filePath = fileNameTextBox.Text;

            if (File.Exists(filePath)) {
                pictureBox2.Image = getIcon(filePath);
                fileInfoNameLabel.Text = Path.GetFileName(filePath);

                fileInfo.Text = "";
                fileInfo.Text += "Size: " + new FileInfo(filePath).Length + Environment.NewLine;

                foreach (string line in fileInfo.Lines) {
                    string name = line.Split(' ')[0];
                    int srt = fileInfo.Find(line);
                    if (srt > 0) {
                        fileInfo.Select(srt, name.Length);
                        fileInfo.SelectionFont = new Font(fileInfo.Font, FontStyle.Bold);
                    }
                }
            }
        }

        private void fileBrowseButton_Click(object sender, EventArgs e) {
            FileDialog FD = new System.Windows.Forms.OpenFileDialog();
            if (FD.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                fileNameTextBox.Text = FD.FileName;
            }
        }

        FileTransmitHandler fileTransfer;
        FileReceiveHandler fileReceive = new FileReceiveHandler();
        private void sendButton_Click(object sender, EventArgs e) {
            incomingWMX.Clear();
            outgoingWMX.Clear();

            fileTransfer = new FileTransmitHandler(fileNameTextBox.Text);
            fileTransfer.WMXMessagesAvailable += ProcessOutboundWMX;

            Thread transferThread = new Thread(new ThreadStart(delegate { fileTransfer.send(false); }));
            transferThread.IsBackground = true;
            transferThread.Start();
        }

        SerialHandler serialHandler;
        Thread serialThread;
        private void serialReconnectButton_Click(object sender, EventArgs e) {
            if (serialHandler != null) {
                serialHandler.stop();
            }
            if (serialThread != null) {
                serialThread.Join();
            }

            serialHandler = new SerialHandler(serialPortTextBox.Text);
            serialHandler.WMXReceived += ProcessIncomingWMX;

            serialThread = new Thread(new ThreadStart(serialHandler.run));
            serialThread.IsBackground = true;
            serialThread.Start();
        }

        public void ProcessIncomingWMX(WMXMessage message) {
            switch (message.Type) {
                case WMXMessage.MessageType.RxData:
                    // RX data is directed to the receive handler
                    fileReceive.ProcessData(message);
                    break;
                default:
                    // All other data goes to the transmit handler
                    if (fileTransfer != null) {
                        fileTransfer.addInboundMessage(message);
                    }
                    break;
            }
            incomingWMX.Add(message);
        }

        public void ProcessOutboundWMX() {
            if(fileTransfer != null){
                foreach (WMXMessage message in fileTransfer.getOutboundMessages()) {
                    serialHandler.sendWMX(message);
                    outgoingWMX.Add(message);
                }
            }
        }
    }
}
