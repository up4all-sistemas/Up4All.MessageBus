﻿using System;
using System.Collections.Generic;

namespace Up4All.Framework.MessageBus.RabbitMQ.Options
{
    public class QueueDeclareOptions
    {
        public bool Exclusive { get; set; }

        public bool Durable { get; set; }

        public bool AutoComplete { get; set; }

        public Dictionary<string, object> Args { get; set; }

        public ICollection<QueueBindOptions> Bindings { get; set; }

        public string Type { get; set; }

        internal QueueDeclareOptions()
        {
            Bindings = new List<QueueBindOptions>();
            Args = new Dictionary<string, object>();       
            Type = "classic";
            Durable = true;
        }
                
        public void AddBinding(Action<QueueBindOptions> createBinding)
        {
            var binding = new QueueBindOptions();
            createBinding(binding);
            Bindings.Add(binding);
        }

    }

    public class QueueBindOptions
    {
        public string ExchangeName { get; set; }

        public string RoutingKey { get; set; }

        public Dictionary<string, object> Args { get; set; }

        internal QueueBindOptions()
        {
            Args = new Dictionary<string, object>();
        }

        public QueueBindOptions(string exchangeName, string defaultArgKey, object defaultArgValue, string routingkey = null) : this()
        {
            ExchangeName = exchangeName;
            RoutingKey = routingkey;
            Args.Add(defaultArgKey, defaultArgValue);
        }
    }
}
