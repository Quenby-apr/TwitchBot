using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchBot
{
    public class DinoDeserialization
    {
        public Dinozavr Deserialize(string JSDino) 
        {
            string[] dinoParams = JSDino.Split(',');
            if (JSDino.Contains("Fruits"))
            {
                Herbivore dino = new Herbivore();
                dino.Fruits = Convert.ToInt32(dinoParams[1].Substring(9));
                dino.Busy = Convert.ToBoolean(dinoParams[0].Substring(8));
                dino.HP = Convert.ToInt32(dinoParams[2].Substring(5));
                dino.Level = Convert.ToInt32(dinoParams[3].Substring(8));
                dino.MaxHP = Convert.ToInt32(dinoParams[4].Substring(8));
                dino.Name = dinoParams[5].Substring(8, dinoParams[5].Length - 8 - 1);
                dino.UserName = dinoParams[6].Substring(12, dinoParams[6].Length - 12 - 1);
                dino.XP = Convert.ToInt32(dinoParams[7].Substring(5, dinoParams[7].Length - 6));
                return dino;
            } else if (JSDino.Contains("Preys"))
            {
                Predator dino = new Predator();
                dino.Preys = Convert.ToInt32(dinoParams[5].Substring(8));
                dino.Busy = Convert.ToBoolean(dinoParams[0].Substring(8));
                dino.HP = Convert.ToInt32(dinoParams[1].Substring(5));
                dino.Level = Convert.ToInt32(dinoParams[2].Substring(8));
                dino.MaxHP = Convert.ToInt32(dinoParams[3].Substring(8));
                dino.Name = dinoParams[4].Substring(8, dinoParams[4].Length - 8 - 1);
                dino.UserName = dinoParams[6].Substring(12, dinoParams[6].Length - 12 - 1);
                dino.XP = Convert.ToInt32(dinoParams[7].Substring(5, dinoParams[7].Length - 6));
                return dino;
            } else
            {
                return null;
            }
        }
        public string FindPrey(string JSDinos)
        {
            string substring = "Name";
            var indices = new List<int>();

            int index = JSDinos.IndexOf(substring, 0);
            while (index > -1)
            {
                indices.Add(index);
                index = JSDinos.IndexOf(substring, index + substring.Length);
            }
            Random rnd = new Random();
            int value = rnd.Next(0, indices.Count());
            int val = JSDinos.IndexOf(",", indices[value]);
            var str = JSDinos.Substring(indices[value] + 7, val - indices[value] - 8);
            string answer = JSDinos.Substring(indices[value] + 7, val - indices[value] - 8);
            return answer;
        }
    }
}
