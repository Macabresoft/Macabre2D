namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Linq;

    public class JsonColorConverter : JsonConverter {

        public override bool CanConvert(Type objectType) {
            return (objectType == typeof(Color));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            var jsonObject = JObject.Load(reader);
            var properties = jsonObject.Properties().ToList();
            return new Color(
                (byte)properties.First(x => x.Name == "R"),
                (byte)properties.First(x => x.Name == "G"),
                (byte)properties.First(x => x.Name == "B"),
                (byte)properties.First(x => x.Name == "A"));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            var color = (Color)value;
            writer.WriteStartObject();
            writer.WritePropertyName("R");
            serializer.Serialize(writer, color.R);
            writer.WritePropertyName("G");
            serializer.Serialize(writer, color.G);
            writer.WritePropertyName("B");
            serializer.Serialize(writer, color.B);
            writer.WritePropertyName("A");
            serializer.Serialize(writer, color.A);
            writer.WriteEndObject();
        }
    }
}