using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TwitchBot
{
    internal class DinoWorld
    {
        public int maxDinos = 1;
        DinoDeserialization myDes;
        public DinoWorld()
        {
            myDes = new DinoDeserialization();
        }
        public string createDino(string userName, string dinoName)
        {
            if (GetDinoFromBDByDinoName(dinoName).Result == null)
            {
                Random rand = new Random();
                int value = rand.Next(0, 4);
                if (value < 3)
                {
                    AddDinoToDB(userName, dinoName, false);
                    return userName + ", Новый динозавр добавлен, и он травоядный " + Emotion.emotions["joy"]; //дино добавлен   
                }
                else if (value == 3)
                {
                    AddDinoToDB(userName, dinoName, true);
                    return userName + ", Новый динозавр добавлен, и он хищный! " + Emotion.emotions["predator"];
                }
            }
            else
            {
                return userName + ", К сожалению у вас уже есть динозавр " + Emotion.emotions["sadness"]; //такое имя уже существует
            }
            return "Ошибка";
        }

        public string killDino(string userName, string dinoName)
        {
            if (GetDinoFromBDByDinoName(dinoName).Result == null)
            {
                return userName + ", у вас и так нет ни единого динозавра " + Emotion.emotions["sadness"];
            }
            else 
            {
                DeleteDinoFromDB(dinoName);
                return "Динозавра " + dinoName + " больше нет с нами";
            }
        }
        private async void AddDinoToDB(string userName, string dinoName, bool predator)
        {
            IFirebaseConfig config = new FirebaseConfig
            {
                AuthSecret = "QEko1K97XZMZtzKKhc77Eh8EmAXwGEUSxtpie53H",
                BasePath = "https://dinoworld-474aa-default-rtdb.europe-west1.firebasedatabase.app/"
            };
            IFirebaseClient client = new FirebaseClient(config);
            if (predator) {
                SetResponse response = await client.SetAsync("Dinozavrs/" + dinoName, new Predator(userName, dinoName));
            }
            else
            {
                SetResponse response = await client.SetAsync("Dinozavrs/" + dinoName, new Herbivore(userName, dinoName));
            }
        }
        private async void DeleteDinoFromDB( string dinoName)
        {
            IFirebaseConfig config = new FirebaseConfig
            {
                AuthSecret = "QEko1K97XZMZtzKKhc77Eh8EmAXwGEUSxtpie53H",
                BasePath = "https://dinoworld-474aa-default-rtdb.europe-west1.firebasedatabase.app/"
            };
            IFirebaseClient client = new FirebaseClient(config);
            FirebaseResponse response = await client.DeleteAsync("Dinozavrs/" + dinoName);
        }
        public async Task<Dinozavr> GetDinoFromBDByDinoName(string dinoName)
        {
            IFirebaseConfig config = new FirebaseConfig
            {
                AuthSecret = "QEko1K97XZMZtzKKhc77Eh8EmAXwGEUSxtpie53H",
                BasePath = "https://dinoworld-474aa-default-rtdb.europe-west1.firebasedatabase.app/"
            };
            IFirebaseClient client = new FirebaseClient(config);
            FirebaseResponse response = await client.GetAsync("Dinozavrs/" + dinoName);
            var resp = response.Body;
            Dinozavr dino=null;
            if (resp != null)
            {
                dino = myDes.Deserialize(resp);
                return dino;
            }  
            return null;
        }
        public async Task<Dinozavr> GetPrey(string dinoName)
        {
            IFirebaseConfig config = new FirebaseConfig
            {
                AuthSecret = "QEko1K97XZMZtzKKhc77Eh8EmAXwGEUSxtpie53H",
                BasePath = "https://dinoworld-474aa-default-rtdb.europe-west1.firebasedatabase.app/"
            };
            IFirebaseClient client = new FirebaseClient(config);
            FirebaseResponse response = await client.GetAsync("Dinozavrs");
            var resp = response.Body;
            var preyName = myDes.FindPrey(resp);
            if (preyName == dinoName)
            {
                preyName = "Prey";
            }
            return GetDinoFromBDByDinoName(preyName).Result;
        }
    }
}
