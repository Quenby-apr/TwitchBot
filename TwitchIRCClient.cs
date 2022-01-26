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
        public List<string> commands;
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
                    string userName=client.getUserName(msg);
                    int indexOfSubstring = msg.IndexOf("#"+client.channelName) +client.channelName.Length+2 ;
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
                        
                        value = rnd.Next(0, int.Parse(numbers[0])+1);
                    }
                    else if (numbers.Count == 2)
                    {
                        if (int.Parse(numbers[0])>int.Parse(numbers[1]))
                        {
                            string buf = numbers[1];
                            numbers[1]=numbers[0];
                            numbers[0]=buf;
                        }
                        value = rnd.Next(int.Parse(numbers[0]), int.Parse(numbers[1])+1);
                    }
                    else
                    {
                        value = rnd.Next(0, 101);
                    }

                    client.SendMessage(userName+ ", ваш результат: "+value.ToString());
                }
            },
            { "!flip",
                delegate(string msg, TwitchIRCClient client)
                {
                    client.SendCommand("PONG", ":tmi.twitch.tv");
                    string userName=client.getUserName(msg);
                    Random rnd = new Random();
                    int value = rnd.Next(0, 2);
                    if (value == 0) {
                        client.SendMessage(userName+", вам выпадает Решка");
                    }
                    else
                    {
                         client.SendMessage(userName+", вам выпадает Орёл");
                    }
                }
            },
            { "!dino new",
                delegate(string msg, TwitchIRCClient client)
                {
                    string userName=client.getUserName(msg);
                    Console.WriteLine(userName);
                    client.SendMessage(dinoWorld.createDino(userName, userName));
                }
            },
            { "!dino dinner",
                delegate(string msg, TwitchIRCClient client)
                {
                    string userName=client.getUserName(msg);
                    Console.WriteLine(userName);
                    string answer = String.Empty;
                    var dino = dinoWorld.dinozavrs.FirstOrDefault(x => x.Name == userName); //userName, а не dinoName, потому что 1 человек = 1 динозавр
                    if (dino == null)
                    {
                        client.SendMessage(userName+", вам нужен свой личный динозавр! "+emotions.emotions["dinoStandart"]);
                        return;
                    }
                    if (dino is Herbivore)
                    {
                        if (!dino.Busy) 
                        {
                            client.SendMessage(userName+", ваш динозавр ушёл за фруктами");
                        }
                        answer = dino.dinner();
                    }
                    if (dino is Predator)
                    {
                        if (!dino.Busy)
                        {
                            client.SendMessage(userName+", ваш динозавр ушёл на охоту");
                        }
                        answer = dino.dinner(dinoWorld.dinozavrs);
                    }
                    client.SendMessage(answer);
                }
            },
            { "!dino fruits",
                delegate(string msg, TwitchIRCClient client)
                {
                    string userName=client.getUserName(msg);
                    Console.WriteLine(userName);
                    var dino = dinoWorld.dinozavrs.FirstOrDefault(x => x.Name == userName);
                    if (dino is Herbivore)
                    {
                        var param = (Herbivore)dino;
                        client.SendMessage(userName+", за всё время ваш динозавр нашёл "+param.Fruits+" фруктов");
                    }
                    else
                    {
                        client.SendMessage(userName+", у вас не травоядный динозавр");
                    }
                }
            },
            { "!dino preys",
                delegate(string msg, TwitchIRCClient client)
                {
                    string userName=client.getUserName(msg);
                    Console.WriteLine(userName);
                    var dino = dinoWorld.dinozavrs.FirstOrDefault(x => x.Name == userName);
                    if (dino is Predator)
                    {
                        var param = (Predator)dino;
                        client.SendMessage(userName+", за всё время ваш динозавр поймал "+param.Preys+" других динозавров");
                    }
                    else
                    {
                        client.SendMessage(userName+", у вас не хищный динозавр");
                    }
                }
            },
            { "!dino uplvl",
                delegate(string msg, TwitchIRCClient client)
                {
                   string userName=client.getUserName(msg);
                    Console.WriteLine(userName);
                    var dino = dinoWorld.dinozavrs.FirstOrDefault(x => x.Name == userName);
                    client.SendMessage(dino.lvlUp());
                }
            },
            { "!dino lvl",
                delegate(string msg, TwitchIRCClient client)
                {
                    string userName=client.getUserName(msg);
                    Console.WriteLine(userName);
                    var dino = dinoWorld.dinozavrs.FirstOrDefault(x => x.Name == userName);
                    client.SendMessage(dino.getLevel());
                }
            },
            { "!dino hp",
                delegate(string msg, TwitchIRCClient client)
                {
                    string userName=client.getUserName(msg);
                    Console.WriteLine(userName);
                    var dino = dinoWorld.dinozavrs.FirstOrDefault(x => x.Name == userName);
                    client.SendMessage(dino.HP.ToString());
                }
            },
        };
        public TwitchIRCClient(TextBox outputTextBox, string channelName, string botName, string token)
        {
            emotions = new Emotion();
            dinoWorld = new DinoWorld();
            commands = new List<string>();
            textBox = outputTextBox;
            client = new TcpClient(TwitchInit.Host, TwitchInit.Port);
            reader = new StreamReader(client.GetStream());
            writer = new StreamWriter(client.GetStream());
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
                    Task.Run(() =>
                    {
                        pair.Value.Invoke(msg, this);
                    });
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
                        if (message == "PING :tmi.twitch.tv\r\n")
                        {
                            SendCommand("PONG", ":tmi.twitch.tv\r\n");
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
        private string getUserName(string msg)
        {
            int indexOfSubstring = msg.IndexOf("!");
            return msg.Substring(1, indexOfSubstring - 1);
        }
        public void SendMessage(string message)
        {
            Console.WriteLine(message);
            commands.Add("PRIVMSG "+ string.Format("#{0} :{1}", channelName, message.ToString()));
            SendCommands();
        }
        public void SendCommands()
        {
            for (int i = commands.Count-1; i>=0;i--)
            {
                writer.WriteLine(commands[i]);
                commands.RemoveAt(i);
                writer.Flush();
                Thread.Sleep(1000);
            }
            
        }
        public void SendCommand(string cmd, string param)
        {
            writer.WriteLine(cmd + " " + param);
            writer.Flush();
        }
    }
}
