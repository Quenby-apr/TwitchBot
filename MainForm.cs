using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TwitchBot.Properties;

namespace TwitchBot
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
        private static CancellationTokenSource cancellation;
        private static Task chatTask;

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopChatTask();
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            SaveUserSettings();
            StopChatTask();

            TwitchIRCClient client = new TwitchIRCClient(textBoxOutput,
                textBoxChannel.Text,
                "Quenby_Bot", // ник на твиче данной уч. записи
                textBoxToken.Text);
            client.Connect();
            cancellation = new CancellationTokenSource();
            chatTask = Task.Run(() => client.Chat(cancellation.Token));
        }

        private void SaveUserSettings()
        {
            Settings.Default["AccountName"] = textBoxChannel.Text;
            Settings.Default["AuthToken"] = textBoxToken.Text;
            Settings.Default.Save();
        }

        private void LoadUserSettings()
        {
            textBoxChannel.Text = Properties.Settings.Default["AccountName"].ToString();
            textBoxToken.Text = Properties.Settings.Default["AuthToken"].ToString();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadUserSettings();
        }

        private void StopChatTask()
        {
            if (chatTask != null)
            {
                cancellation.Cancel();
            }
        }
    }
}
