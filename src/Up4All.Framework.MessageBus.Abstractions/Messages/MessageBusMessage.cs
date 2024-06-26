﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

using Up4All.Framework.MessageBus.Abstractions.Interfaces;

namespace Up4All.Framework.MessageBus.Abstractions.Messages
{
    public class MessageBusMessage : IMessage
    {
        public byte[] Body { get; private set; }

        public IDictionary<string, object> UserProperties { get; private set; }

        public bool IsJson { get; private set; }

        public MessageBusMessage()
        {
            IsJson = false;
            UserProperties = new Dictionary<string, object>();
        }

        public MessageBusMessage(byte[] body) : this()
        {
            IsJson = false;
            Body = body;
        }

        public void AddBody(byte[] body)
        {
            Body = body;
        }

        public void AddBody(string body)
        {
            AddBody(Encoding.UTF8.GetBytes(body));
        }

        public void AddBody(BinaryData data, bool isJsonData = false)
        {
            IsJson = isJsonData;
            AddBody(data.ToArray());
        }

        public void AddBody<T>(T obj, JsonSerializerOptions opts = null) where T : class
        {
            opts = opts ?? new JsonSerializerOptions(JsonSerializerDefaults.Web) { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault };

            IsJson = true;
            AddBody(JsonSerializer.Serialize(obj, opts));
        }

        public void AddUserProperty(KeyValuePair<string, object> prop)
        {
            UserProperties.Add(prop);
        }

        public void AddUserProperty(string key, object value)
        {
            UserProperties.Add(new KeyValuePair<string, object>(key, value));
        }

        public void AddUserProperties(IDictionary<string, object> props)
        {
            foreach (var prop in props)
                AddUserProperty(prop);
        }
    }
}
