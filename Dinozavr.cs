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
        public int XP { get; protected set; } //опыт
        public bool Alive { get; set; } //НАДО ИЗМЕНИТЬ НА УДАЛЕНИЕ ИЗ БД
        public int[] levels = new int[]
        {
            5,20,40,65,90,130,170,215,260,340,500,720,960,1200,1470,1800,2230,2700
        };
        public int Level { get; protected set; }
        public int HP { get; set; } //очки здоровья
        public int Wins { get; set; }
        public bool Busy { get; set; }

        public Dinozavr(string name)
        {
            XP = 1;
            Level = 0;
            HP = 10;
            Wins = 0;
            Name = name;
            Busy = false;
            emotion = new Emotion();
        }
        public abstract string dinner();
        public abstract string dinner(List<Dinozavr> dinozavrs);
        public  string getLevel()
        {
            for (int i = 0; i < levels.Count(); i++)
            {
                if (levels[i] > XP)
                {
                    if (i > Level)
                    {
                        return userName + ", у вашего динозавра сейчас " + Level + " уровень и заработано " + XP + " опыта, вы уже можете поднять свой уровень!";
                    }
                    else
                    {
                        return userName + ", у вашего динозавра сейчас " + Level + " уровень и заработано " + XP + " опыта, до следующего уровня не хватает " + (levels[i] - XP) + " опыта";
                    }
                }
            }
            return "Ошибка логики уровня";
        }
        public string lvlUp()
        {
            for (int i = 0; i < levels.Count(); i++)
            {
                if (levels[i] > XP)
                {
                    if (i > Level)
                    {
                        Level++;
                        return userName + ", поздравляю, ваш динозавр стал " + Level + " уровня " + emotion.emotions["joy"];
                    }
                    else
                    {
                        return userName + ", к сожалению, вы ещё не можете поднять уровень своего динозавра " + emotion.emotions["dropping"];
                    }
                }
            }
            return "Ошибка логики поднятия уровня";
        }
    }

    public class Herbivore: Dinozavr
    {
        public int MaxHP = 300;
        public int Fruits { get; private set; }
        public Herbivore(string name):base(name)
        {
            Fruits = 0;
            Name = name;
        }

        public override string dinner()
        {
            if (!Busy)
            {
                Busy = true;
                Random rnd = new Random();
                int value = rnd.Next(0, 10);
                Thread.Sleep(10000);
                Random rnd2 = new Random();
                int xp = rnd2.Next(4, 10);
                XP += xp;
                if (value >= 8)
                {
                    Fruits++;
                    Busy = false;
                    if (HP<=MaxHP-10)
                    {
                        HP += 10;
                    }
                    else
                    {
                        HP = MaxHP;
                    }
                    return userName + ", ваш динозавр смог найти замечательный фрукт, поэтому восполнил себе здоровье! А также получил " + xp + " опыта " + emotion.emotions["joy"];
                }
                else
                {
                    Busy = false;
                    HP--;
                    if (HP<=0)
                    {
                        Alive = false;
                        return "Динозавра " + Name + "больше нет с нами " +emotion.emotions["sadness"];
                    }
                    return userName + ", в этот раз фрукты найти не удалось, но мы смогли получить " + xp + " опыта " + emotion.emotions["dropping"];

                }
            }
            if (Busy)
            {
                return Name + ", ваш динозавр ещё не вернулся, необходимо подождать"; //dinoName почему-то не работает, хоть и инициализирован
            }
            return "что-то пошло не так";
        }

        public override string dinner(List<Dinozavr> dinozavrs)
        {
            throw new NotImplementedException();
        }
    }

    public class Predator: Dinozavr
    {
        public int MaxHP = 15;
        public int Preys { get; private set; }
        public Predator(string name) : base(name)
        {
            Name = name;
        }

        public override string dinner(List<Dinozavr> dinozavrs)
        {
            if (!Busy)
            {
                Busy = true;
                Random rnd = new Random();
                int preyId = rnd.Next(0, dinozavrs.Count());
                var prey = dinozavrs[preyId];
                if (prey.Name == Name)
                {
                    if (preyId != dinozavrs.Count() - 1)
                    {
                        prey = dinozavrs[preyId + 1];
                    }
                    else if (preyId != 0)
                    {
                        prey = dinozavrs[preyId - 1];
                    }
                    else
                    {
                        return userName+" , в нашем мире вы единственный динозавр " + emotion.emotions["sadness"];
                    }
                }
                int huntValue = rnd.Next(0, 70) + Level;
                int preyValue = rnd.Next(0, 90) + prey.Level;
                Random rnd2 = new Random();
                int xp = rnd2.Next(14, 28);
                XP += xp;
                if (huntValue >= preyValue)
                {
                    Preys++;
                    if (HP <= MaxHP - 5)
                    {
                        HP += 5;
                    }
                    else
                    {
                        HP += MaxHP;
                    }
                    prey.HP -= 5;
                    if (prey.HP < 0)
                    {
                        prey.Alive = false;
                        return "Динозавра " + prey.Name + "больше нет с нами " + emotion.emotions["sadness"];
                    }
                    return userName + ", ваш динозавр смог съесть динозавра " + prey.Name + "! Но он восполнил себе немножко здоровья и получил "+XP+" опыта " + emotion.emotions["predator"];
                }
                else
                {
                    HP--;
                    if (HP < 0)
                    {
                        Alive = false;
                        return "Динозавра " + Name + "больше нет с нами " + emotion.emotions["sadness"];
                    }
                    return userName + ", ваш динозавр не смог поймать динозавра " + prey.Name + "! Но получил "+XP+" опыта " + emotion.emotions["dropping"];
                }
            }
            if (Busy)
            {
                return Name + ", ваш динозавр ещё не вернулся, необходимо подождать"; //dinoName почему-то не работает, хоть и инициализирован
            }
            return "что-то пошло не так";
        }

        public override string dinner()
        {
            throw new NotImplementedException();
        }
    }
}
