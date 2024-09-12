using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Task3_ChatApp_vSem2
{
    internal class Message
    {
        public string? FromName { get; set; }
        public DateTime Date { get; set; }
        public string? Text { get; set; }

        // Метод для сериализации в JSON
        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }

        // Статический метод для десериализации JSON в объект Message
        public static Message? FromJson(string json)
        {
            return JsonSerializer.Deserialize<Message>(json);
        }

    }
}