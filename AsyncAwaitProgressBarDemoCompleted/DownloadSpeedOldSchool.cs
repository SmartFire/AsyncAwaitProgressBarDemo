using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AsyncAwaitProgressBarDemoCompleted
{
    public partial class DownloadSpeedOldSchool : Form
    {
        private const int BUFFER_SIZE = 1024;

        private class DownloadFileRequestState
        {
            public DownloadFileRequestState()
            {
                this.AllBytes = new List<byte>();
            }

            public List<byte> AllBytes { get; private set; }
            public Stream Stream { get; set; }
            public byte[] Bytes { get; set; }
            public string Url { get; set; }
        }

        public DownloadSpeedOldSchool()
        {
            InitializeComponent();
        }

        private void ManipulateUI(Action action)
        {
            this.Invoke(action);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var url = this.textBox1.Text;
            if (String.IsNullOrEmpty(url))
            {
                return;
            }

            this.button1.Enabled = false;
            var webRequest = WebRequest.Create(url);
            webRequest.BeginGetResponse(iar =>
            {
                var response = webRequest.EndGetResponse(iar);
                var contentLength = (int)response.ContentLength;
                ManipulateUI(() => this.progressBar1.Maximum = contentLength);
                var stream = response.GetResponseStream();
                var bytes = new byte[BUFFER_SIZE];
                var state = new DownloadFileRequestState { Stream = stream, Bytes = bytes, Url = url };
                stream.BeginRead(bytes, 0, BUFFER_SIZE, ReadCallback, state);
            }, null);
        }

        private void ReadCallback(IAsyncResult iAsyncResult)
        {
            var state = (DownloadFileRequestState)iAsyncResult.AsyncState;
            var stream = state.Stream;
            var bytesRead = stream.EndRead(iAsyncResult);
            if (bytesRead > 0)
            {
                this.ManipulateUI(() => this.progressBar1.Increment(bytesRead));
                state.AllBytes.AddRange(state.Bytes.Take(bytesRead));
                stream.BeginRead(state.Bytes, 0, BUFFER_SIZE, ReadCallback, state);
            }
            else
            {
                var fileName = Path.GetFileName(state.Url);
                File.WriteAllBytes(fileName, state.AllBytes.ToArray());
                this.ManipulateUI(() => this.button1.Enabled = true);
            }
        }
    }
}
