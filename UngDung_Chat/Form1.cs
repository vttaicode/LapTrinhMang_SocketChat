using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Threading;
namespace UngDung_Chat
{
    public partial class Form1: Form
    {
        TcpClient client;
        TcpListener server;
        NetworkStream stream;
        Thread receiveThread;
        public Form1()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            int port = int.Parse(txtPort.Text);
            server = new TcpListener(IPAddress.Any, port);
            server.Start();
            rtbChat.AppendText("Server started...\n");
            Thread acceptThread = new Thread(() =>
            {
                client = server.AcceptTcpClient();
                stream = client.GetStream();
                rtbChat.Invoke((MethodInvoker)(() =>
                    rtbChat.AppendText("Client connected!\n")));

                receiveThread = new Thread(ReceiveData);
                receiveThread.Start();
            });
            acceptThread.Start();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            string ip = txtIP.Text;
            int port = int.Parse(txtPort.Text);

            client = new TcpClient(ip, port);
            stream = client.GetStream();

            rtbChat.AppendText("Connected to server...\n");

            receiveThread = new Thread(ReceiveData);
            receiveThread.Start();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (stream != null)
            {
                string message = txtNhapTinNhan.Text;
                byte[] data = Encoding.UTF8.GetBytes(message);
                stream.Write(data, 0, data.Length);

                rtbChat.AppendText("Me: " + message + "\n");
                txtNhapTinNhan.Clear();
            }
        }
        private void ReceiveData()
        {
            try
            {
                while (true)
                {
                    byte[] buffer = new byte[1024];
                    int bytes = stream.Read(buffer, 0, buffer.Length);
                    string message = Encoding.UTF8.GetString(buffer, 0, bytes);

                    rtbChat.Invoke((MethodInvoker)(() =>
                        rtbChat.AppendText("Friend: " + message + "\n")));
                }
            }
            catch
            {
                rtbChat.Invoke((MethodInvoker)(() =>
                    rtbChat.AppendText("Disconnected.\n")));
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            rdoServer.Checked = true;
            Mode();
            this.AcceptButton = btnSend;
        }

        private void rdoServer_CheckedChanged(object sender, EventArgs e)
        {
            Mode();
        }

        private void rdoClient_CheckedChanged(object sender, EventArgs e)
        {
            Mode();
        }
        private void Mode()
        {
            if (rdoServer.Checked)
            {
                txtIP.Enabled = false;         
                btnStart.Enabled = true;   
                btnConnect.Enabled = false;         
            }
            else if (rdoClient.Checked)
            {
                txtIP.Enabled = true;             
                btnStart.Enabled = false;    
                btnConnect.Enabled = true;         
            }
        }
    }
}
