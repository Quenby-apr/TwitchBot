using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TwitchBot
{
    public class TwitchInit
    {
        public const string Host = "irc.twitch.tv";
        public const int Port = 6667;
    }
    class TwitchIRCClient
    {
        private static Emotion emotions;
        private static DinoWorld dinoWorld;
        private TcpClient client;
        private StreamReader reader;
        private StreamWriter writer { get; }
        private TextBox textBox;
        private string passToken;
        private string botName;
        private string channelName;
        private Dictionary<string, Action<string, TwitchIRCClient>> answers = new Dictionary<string, Action<string, TwitchIRCClient>>
        {
            { "!roll",
                delegate(string msg, TwitchIRCClient client)
                {
                    int value;
                    int indexOfSubstring = msg.IndexOf("!") ;
                    string userName=msg.Substring(1, indexOfSubstring-1);
                    indexOfSubstring = msg.IndexOf("#"+client.channelName) +client.channelName.Length+2 ;
                    msg=msg.Substring(indexOfSubstring, msg.Length-indexOfSubstring);
                    List<string> numbers = Regex.Split(msg, @"\D+").ToList();
                    numbers.RemoveAll(x => x == string.Empty);
                    Console.WriteLine(msg);
                    Console.WriteLine(numbers.Count);
                    for (int i =0; i<numbers.Count;i++)
                    {
                        Console.WriteLine(numbers[i]);
                    }
                   
                    Random rnd = new Random();
                    if (numbers.Count == 1)
                    {
                        
                        value = rnd.Next(0, int.Parse(numbers[0]));
                    }
                    else if (numbers.Count == 2)
                    {
                        if (int.Parse(numbers[0])>int.Parse(numbers[1]))
                        {
                            string buf = numbers[1];
                            numbers[1]=numbers[0];
                            numbers[0]=buf;
                        }
                        value = rnd.Next(int.Parse(numbers[0]), int.Parse(numbers[1]));
                    }
                    else
                    {
                        value = rnd.Next(0, 100);
                    }

                    client.SendMessage(userName+ ", твой результат: "+value.ToString());
                }
            },
            { "!flip",
                delegate(string msg, TwitchIRCClient client)
                {
                    int indexOfSubstring = msg.IndexOf("!") ;
                    string userName=msg.Substring(1, indexOfSubstring-1);
                    Random rnd = new Random();
                    int value = rnd.Next(0, 2);
                    if (value == 0) {
                        client.SendMessage(userName+", тебе выпадает Решка");
                    }
                    else
                    {
                         client.SendMessage(userName+", тебе выпадает Орёл");
                    }
                }
            },
            { "!dino new",
                delegate(string msg, TwitchIRCClient client)
                {
                    int indexOfSubstring = msg.IndexOf("!") ;
                    string userName=msg.Substring(1, indexOfSubstring-1);
                    Console.WriteLine(userName);
                    client.SendMessage(dinoWorld.createDino(userName, userName));
                }
            },
            { "!dino dinner",
                delegate(string msg, TwitchIRCClient client)
                {
                    int indexOfSubstring = msg.IndexOf("!") ;
                    string dinoName=msg.Substring(1, indexOfSubstring-1);
                    Console.WriteLine(dinoName);
                    client.SendMessage("Динозавр ушёл за фруктами");
                    var dino = dinoWorld.dinozavrs.FirstOrDefault(x => x.Name == dinoName);
                    if (dino.Equals(null))
                    {
                        client.SendMessage("Вам нужен свой личный динозавр! "+emotions.emotions["dinoStandart"]);
                    }
                    client.SendMessage(dino.dinner());
                }
            },
        };
        public TwitchIRCClient(TextBox outputTextBox, string channelName, string botName, string token)
        {
            emotions = new Emotion();
            dinoWorld = new DinoWorld();
            textBox = outputTextBox;
            client = new TcpClient(TwitchInit.Host, TwitchInit.Port);
            reader = new StreamReader(client.GetStream());
            writer = new StreamWriter(client.GetStream());
            writer.AutoFlush = true;
            this.botName = botName;
            this.channelName = channelName;
            passToken = token;
            if (passToken == String.Empty)
            {
                Console.WriteLine("Токен пуст");
            }
        }
        public void Connect()
        {
            SendCommand("PASS", passToken);
            SendCommand("USER", string.Format("{0} 0 * {0}", botName));
            SendCommand("NICK", botName);
            SendCommand("JOIN", "#" + channelName);
        }

        public void CheckCommand(string msg)
        {
            foreach (var pair in answers)
            {
                if (msg.Contains(pair.Key))
                {
                    pair.Value.Invoke(msg, this);
                    return;
                }
            }
        }

        public async void Chat(CancellationToken cancellationToken)
        {
            try
            {
                string message;

                while ((message = await reader.ReadLineAsync()) != null && !cancellationToken.IsCancellationRequested)
                {
                    if (message != null)
                    {
                        textBox.Invoke((MethodInvoker)delegate { textBox.Text += message + '\n'; });
                        CheckCommand(message);
                        if (message == "PING :tmi.twitch.tv")
                        {
                            SendCommand("PONG", ":tmi.twitch.tv");
                            return;
                        }
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                return;
            }
        }

        public void SendMessage(string message)
        {
            Command command = new Command(client, "PRIVMSG", string.Format("#{0} :{1}", channelName, message.ToString()));
            Thread mythread = new Thread(new ThreadStart(command.SendCommand));
            mythread.Start();
            //SendCommand("PRIVMSG", string.Format("#{0} :{1}", channelName, message.ToString()));
        }

        public void SendCommand(string cmd, string param)
        {
            writer.WriteLine(cmd + " " + param);
        }

        public class Command
        {
            public string cmd;
            public string param;
            private TcpClient client;
            private StreamWriter writer;


            public Command(TcpClient client, string cmd, string param)
            {
                this.cmd = cmd;
                this.param = param;
                this.client = client;

            }

            public void SendCommand()
            {
                writer.WriteLine(cmd + " " + param);
            }
        }
    }
}
