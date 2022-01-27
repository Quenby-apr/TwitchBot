using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TwitchBot
{ // КРУТАЯ ШТУКА, ЖАЛЬ НЕ ИСПОЛЬЗУЕТСЯ
    public class DinozavrConverterDiscriminator : JsonConverter<Dinozavr>
    {
        public string UserName { get; set; }
        public string DinoName { get; set; }
        enum TypeDiscriminator
        {
            Herbivore = 1,
            Predator = 2
        }

        public override bool CanConvert(Type typeToConvert) =>
            typeof(Dinozavr).IsAssignableFrom(typeToConvert);

        public override Dinozavr Read(
            ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }

            string propertyName = reader.GetString();
            if (propertyName != "TypeDiscriminator")
            {
                throw new JsonException();
            }

            reader.Read();
            if (reader.TokenType != JsonTokenType.Number)
            {
                throw new JsonException();
            }

            TypeDiscriminator typeDiscriminator = (TypeDiscriminator)reader.GetInt32();
            Dinozavr dino;
            if (typeDiscriminator == TypeDiscriminator.Herbivore)
            {
                dino = new Herbivore(UserName, DinoName);
            }
            else if (typeDiscriminator == TypeDiscriminator.Predator)
            {
                dino = new Predator(UserName, DinoName);
            }
            else
            {
                throw new JsonException();
            }

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return dino;
                }

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    propertyName = reader.GetString();
                    reader.Read();
                    switch (propertyName)
                    {
                        case "Fruits":
                            int fruits = reader.GetInt32();
                            ((Herbivore)dino).Fruits = fruits;
                            break;
                        case "Preys":
                            int preys = reader.GetInt32();
                            ((Predator)dino).Preys = preys;
                            break;
                        case "UserName":
                            string userName = reader.GetString();
                            dino.UserName = userName;
                            break;
                        case "Name":
                            string name = reader.GetString();
                            dino.Name = name;
                            break;
                        case "Level":
                            int level = reader.GetInt32();
                            dino.Level = level;
                            break;
                        case "HP":
                            int hp = reader.GetInt32();
                            dino.HP = hp;
                            break;
                        case "XP":
                            int xp = reader.GetInt32();
                            dino.XP = xp;
                            break;
                        case "MaxHP":
                            int maxHP = reader.GetInt32();
                            dino.MaxHP = maxHP;
                            break;
                        case "Busy":
                            bool busy = reader.GetBoolean();
                            dino.Busy = busy;
                            break;
                    }
                }
            }

            throw new JsonException();
        }

        public override void Write(
            Utf8JsonWriter writer, Dinozavr dino, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            if (dino is Herbivore herb)
            {
                writer.WriteNumber("TypeDiscriminator", (int)TypeDiscriminator.Herbivore);
                writer.WriteNumber("Fruits", herb.Fruits);
            }
            else if (dino is Predator pred)
            {
                writer.WriteNumber("TypeDiscriminator", (int)TypeDiscriminator.Predator);
                writer.WriteNumber("Preys", pred.Preys);
            }

            writer.WriteString("UserName", dino.UserName);
            writer.WriteString("Name", dino.Name);
            writer.WriteNumber("Level", dino.Level);
            writer.WriteNumber("HP", dino.HP);
            writer.WriteNumber("XP", dino.XP);
            writer.WriteNumber("MaxHP", dino.MaxHP);
            writer.WriteBoolean("Busy", dino.Busy);

            writer.WriteEndObject();
        }
    }
}
