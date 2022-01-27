using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
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
        public string UserName { get; set; }
        public string Name { get; set; }
        public int XP { get; set; } //опыт

        public readonly static int[] levels = new int[]
        {
            5,20,40,65,90,130,170,215,260,340,500,720,960,1200,1470,1800,2230,2700,3250,3900,4700,6200,8700,12000,16500,23700,29500,35000,42000,50000,60000,70000,80000,90000,100000,120000,140000,160000,180000,200000
        };
        public int Level { get; set; }
        public int HP { get; set; } //очки здоровья
        public bool Busy { get; set; }
        public int MaxHP { get; set; }
        public Dinozavr() { }
        public Dinozavr(string userName, string name)
        {
            XP = 1;
            Level = 0;
            HP = 10;
            UserName = userName;
            Name = name;
            Busy = false;
        }
        public abstract string dinner();
        public abstract string dinner(Dinozavr prey);
        public  string getLevel()
        {
            for (int i = 0; i < levels.Count(); i++)
            {
                if (levels[i] > XP)
                {
                    if (i > Level)
                    {
                        return UserName + ", у вашего динозавра сейчас " + Level + " уровень и заработано " + XP + " опыта, вы уже можете поднять свой уровень!";
                    }
                    else
                    {
                        return UserName + ", у вашего динозавра сейчас " + Level + " уровень и заработано " + XP + " опыта, до следующего уровня не хватает " + (levels[i] - XP) + " опыта";
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
                        UpdateDinoInDB(this);
                        return UserName + ", поздравляю, ваш динозавр стал " + Level + " уровня " + Emotion.emotions["joy"];
                    }
                    else
                    {
                        return UserName + ", к сожалению, вы ещё не можете поднять уровень своего динозавра " + Emotion.emotions["dropping"];
                    }
                }
            }
            return "Ошибка логики поднятия уровня";
        }
        protected async void DeleteDinoFromDB(string dinoName)
        {
            IFirebaseConfig config = new FirebaseConfig
            {
                AuthSecret = "QEko1K97XZMZtzKKhc77Eh8EmAXwGEUSxtpie53H",
                BasePath = "https://dinoworld-474aa-default-rtdb.europe-west1.firebasedatabase.app/"
            };
            IFirebaseClient client = new FirebaseClient(config);
            FirebaseResponse response = await client.DeleteAsync("Dinozavrs/" + dinoName);
        }
        protected async void UpdateDinoInDB(Dinozavr dino)
        {
            IFirebaseConfig config = new FirebaseConfig
            {
                AuthSecret = "QEko1K97XZMZtzKKhc77Eh8EmAXwGEUSxtpie53H",
                BasePath = "https://dinoworld-474aa-default-rtdb.europe-west1.firebasedatabase.app/"
            };
            IFirebaseClient client = new FirebaseClient(config);
            FirebaseResponse response = await client.UpdateAsync("Dinozavrs/" + dino.Name, dino);
        }
        protected async Task<Dinozavr> GetDinoFromBD(string dinoName)
        {
            IFirebaseConfig config = new FirebaseConfig
            {
                AuthSecret = "QEko1K97XZMZtzKKhc77Eh8EmAXwGEUSxtpie53H",
                BasePath = "https://dinoworld-474aa-default-rtdb.europe-west1.firebasedatabase.app/"
            };
            IFirebaseClient client = new FirebaseClient(config);
            FirebaseResponse response = await client.GetAsync("Dinozavrs/" + dinoName);
            return response.ResultAs<Dinozavr>();
        }
    }
    public class Herbivore: Dinozavr
    {
        public int Fruits { get; set; }
        public Herbivore() { }
        public Herbivore(string userName, string name) : base(userName, name)
        {
            MaxHP = 300;
            Fruits = 0;
        }

        public override string dinner()
        {
            if (!Busy)
            {
                Busy = true;
                UpdateDinoInDB(this);
                Random rnd = new Random();
                int value = rnd.Next(0, 10);
                Thread.Sleep(720000);
                Random rnd2 = new Random();
                int xp = rnd2.Next(4, 10);
                XP += xp;
                if (value >= 8)
                {
                    Fruits++;
                    Busy = false;
                    if (HP<=MaxHP-10)
                    {
                        HP += 10-1;
                    }
                    else
                    {
                        HP = MaxHP;
                    }
                    UpdateDinoInDB(this);
                    return UserName + ", ваш динозавр смог найти замечательный фрукт, поэтому восполнил себе здоровье! А также получил " + xp + " опыта " + Emotion.emotions["joy"];
                }
                else
                {
                    Busy = false;
                    HP--;
                    UpdateDinoInDB(this);
                    if (HP<=0)
                    {
                        DeleteDinoFromDB(Name);
                        return "Динозавра " + Name + "больше нет с нами " + Emotion.emotions["sadness"];
                    }
                    return UserName + ", в этот раз фрукты найти не удалось, но мы смогли получить " + xp + " опыта " + Emotion.emotions["dropping"];

                }
            }
            if (Busy)
            {
                return UserName + ", ваш динозавр ещё не вернулся, необходимо подождать"; 
            }
            return "что-то пошло не так";
        }

        public override string dinner(Dinozavr prey)
        {
            throw new NotImplementedException();
        }
    }
    public class Predator: Dinozavr
    {
        public int Preys { get; set; }
        public Predator() { }
        public Predator(string userName, string name) : base(userName, name)
        {
            MaxHP = 15;
            Preys = 0;
        }

        public override string dinner(Dinozavr prey)
        {
            if (!Busy)
            {
                Busy = true;
                UpdateDinoInDB(this);
                Random rnd = new Random();
                int huntValue = rnd.Next(0, 70) + Level;
                int preyValue = rnd.Next(0, 90) + prey.Level;
                Thread.Sleep(1800000);
                Random rnd2 = new Random();
                int xp = rnd2.Next(14, 28);
                XP += xp;
                UpdateDinoInDB(this);
                if (huntValue >= preyValue)
                {
                    Preys++;
                    if (HP <= MaxHP - 5)
                    {
                        HP += 5-1;
                    }
                    else
                    {
                        HP += MaxHP;
                    }
                    prey.HP -= 5;
                    Busy = false;
                    UpdateDinoInDB(this);
                    UpdateDinoInDB(prey);
                    if (prey.HP < 0)
                    {
                        DeleteDinoFromDB(prey.Name);
                        return "Динозавра " + prey.Name + "больше нет с нами " + Emotion.emotions["sadness"];
                    }
                    return UserName + ", ваш динозавр смог съесть динозавра " + prey.Name + "! Он восполнил себе немножко здоровья и получил "+xp+" опыта " + Emotion.emotions["predator"];
                }
                else
                {
                    HP--;
                    Busy = false;
                    UpdateDinoInDB(this);
                    if (HP < 0)
                    {
                        DeleteDinoFromDB(Name);
                        return "Динозавра " + Name + "больше нет с нами " + Emotion.emotions["sadness"];
                    }
                    Busy = false;
                    UpdateDinoInDB(this);
                    return UserName + ", ваш динозавр не смог поймать динозавра " + prey.Name + "! Но получил "+xp+" опыта " + Emotion.emotions["dropping"];
                }
            }
            if (Busy)
            {
                return UserName + ", ваш динозавр ещё не вернулся, необходимо подождать"; 
            }
            return "что-то пошло не так";
        }

        public override string dinner()
        {
            throw new NotImplementedException();
        }
        
    }
}
