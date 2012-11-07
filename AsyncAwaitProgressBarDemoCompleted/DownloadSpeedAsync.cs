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
    public partial class DownloadSpeedAsync : Form
    {
        private const int BUFFER_SIZE = 1024;

        public DownloadSpeedAsync()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var url = this.textBox1.Text;
            if (String.IsNullOrEmpty(url))
            {
                return;
            }

            this.button1.Enabled = false;
            var webRequest = WebRequest.Create(url);
            var response = await webRequest.GetResponseAsync();
            this.progressBar1.Maximum = (int)response.ContentLength;
            var list = new List<byte>();
            var bytes = new byte[BUFFER_SIZE];
            var stream = response.GetResponseStream();

            int bytesRead = 0;
            do
            {
                bytesRead = await stream.ReadAsync(bytes, 0, BUFFER_SIZE);
                list.AddRange(bytes.Take(bytesRead));
                this.progressBar1.Increment(bytesRead);
            } while (bytesRead > 0);

            var fileName = Path.GetFileName(url);
            File.WriteAllBytes(fileName, list.ToArray());
            this.button1.Enabled = true;
        }
    }
}
