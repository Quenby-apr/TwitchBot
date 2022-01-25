using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TwitchBot
{
    public abstract class Dinozavr
    {
        public Emotion emotion;
        public string userName { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public int Lives { get; set; }
        public int Wins { get; set; }

        public Dinozavr(string name)
        {
            Name = name;
            emotion = new Emotion();
        }
        public abstract string dinner();
    }

    public class Herbivore: Dinozavr
    {
        private int Fruits { get; set; }
        public Herbivore(string name):base(name)
        {
            Name = name;
        }

        public override string dinner()
        {
            Task<bool> task = Task.Run(() =>
            {
                Random rnd = new Random();
                int value = rnd.Next(0, 10);
                Thread.Sleep(10000);
                if (value >= 8)
                {
                    Fruits++;
                    return true;
                }
                return false;
            });
            if (task.Result)
            {
                return "Вы смогли найти замечательный фрукт! "+emotion.emotions["joy"];
            }
            if (!task.Result)
            {
                return "В этот раз ничего найти не удалось " + emotion.emotions["slightPain"];
            }
            return "что-то пошло не так";
        }
    }

    public class Predator: Dinozavr
    {
        private int Kills { get; }
        public Predator(string name) : base(name)
        {
            Name = name;
        }

        public override string dinner()
        {
            throw new NotImplementedException();
        }
    }
}
