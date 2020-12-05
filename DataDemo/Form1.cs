using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TCPSocket;
using Game;
using GameSpec;
using Buff;
using System.Net.Sockets;
using System.IO;

namespace DataDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void login_Click(object sender, EventArgs e)
        {
            string name = id_textbox.Text.Trim();
            string pwd_str = password_texbox.Text.Trim();
            if (String.IsNullOrEmpty(name) || String.IsNullOrEmpty(pwd_str))
            {
                return;
            }
        }

        private TCPSocketClient tcp_client;
        private Buffers buffers;

        private SocketError socket_error;
        private Buffers buff = new Buffers();
        private TCPSocketClient tcp_socket;
        private bool is_logined = false;
        private FileInfo[] file_infos;
        private Dictionary<int, string> dicid_2_file;
        private Players player;

        // 界面load
        private void Form1_Load(object sender, EventArgs e)
        {
            string ip = "10.0.150.52";
            int port = 3000;
            tcp_socket = new TCPSocketClient(ip, port);
            buff.Init(1024, 1024 * 1024);
        }

    }
}
