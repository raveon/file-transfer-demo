using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using Raveon.Messaging;

namespace Raveon.FileTransferDemo {
    class FileReceiveHandler {
        public string SaveLocation {
            get { return _saveLocation; }
            set { 
                if(Directory.Exists(value)){
                    _saveLocation = value;
                }
            }
        }
        private string _saveLocation;

        private Mutex fileMutex;

        public FileReceiveHandler(){
            _saveLocation = Environment.SpecialFolder.MyDocuments.ToString();
            if (!Directory.Exists(_saveLocation)) {
                Directory.CreateDirectory(_saveLocation);
            }

            fileMutex = new Mutex();
        }

        public void ProcessData(WMXMessage message){
            if (message.Type == WMXMessage.MessageType.RxData) {
                string fileName = "";
                uint chunkNumber;
                int i;

                for(i = 0; i < 50; i++){
                    if (message.Data[i] == 0) {
                        // Found the null
                        fileName = Encoding.ASCII.GetString(message.Data, 0, i);
                        break;
                    }
                }
                if (fileName == "") {
                    // Something wrong
                    return;
                }

                i++; // Jump past the null terminator

                chunkNumber = BitConverter.ToUInt32(message.Data, i);
                if (chunkNumber > 3000) {
                    // Just some arbitrary limit. It's just a demo, afterall
                    return;
                }

                i+=4; // Jump past the chunk number

                string fullPath = Path.Combine(_saveLocation, fileName);

                fileMutex.WaitOne();
                try {
                    FileStream output;
                    if (!File.Exists(fullPath)) {
                        output = File.Create(fullPath);
                    } else {
                        output = File.OpenWrite(fullPath);
                    }

                    uint byteStart = chunkNumber * FileTransmitHandler.FileChunkSize;

                    if (!output.CanSeek) {
                        throw new Exception("Cannot receive file!");
                    }
                    output.Seek(byteStart, SeekOrigin.Begin);

                    output.Write(message.Data, i, message.Data.Length - i);

                    output.Flush();

                    output.Close();
                } finally {
                    fileMutex.ReleaseMutex();
                }
            }
        }
    }
}
