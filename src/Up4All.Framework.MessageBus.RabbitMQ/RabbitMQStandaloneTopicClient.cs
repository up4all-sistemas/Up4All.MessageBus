﻿using RabbitMQ.Client;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Up4All.Framework.MessageBus.Abstractions;
using Up4All.Framework.MessageBus.Abstractions.Interfaces;
using Up4All.Framework.MessageBus.Abstractions.Messages;
using Up4All.Framework.MessageBus.RabbitMQ.Extensions;
using Up4All.Framework.MessageBus.RabbitMQ.Options;

namespace Up4All.Framework.MessageBus.RabbitMQ
{
    public class RabbitMQStandaloneTopicClient : MessageBusStandaloneTopicClient, IRabbitMQClient, IMessageBusStandalonePublisher
    {
        private readonly string _topicName;
        private readonly int _connectionAttempts;
        private readonly ExchangeDeclareOptions _declareOpts;
        private readonly string _type;

        public RabbitMQStandaloneTopicClient(string connectionString, string topicName, int connectionAttemps = 8, string type = "topic"
            , ExchangeDeclareOptions declareOpts = null) : base(connectionString, topicName)
        {
            _topicName = topicName;
            _connectionAttempts = connectionAttemps;
            _declareOpts = declareOpts;
            _type = type;
        }

        public IConnection Connection { get; set; }

        public override async Task SendAsync<TModel>(TModel model, CancellationToken cancellation = default)
        {
            var message = model.CreateMessagebusMessage();
            await SendAsync(message, cancellation);
        }

        public override Task SendAsync(MessageBusMessage message, CancellationToken cancellation = default)
        {
            using (var channel = ConfigureChannel(this.GetConnection(ConnectionString, _connectionAttempts)))
                channel.SendMessage(_topicName, string.Empty, message, cancellation);

            return Task.CompletedTask;
        }

        public override Task SendAsync(IEnumerable<MessageBusMessage> messages, CancellationToken cancellation = default)
        {
            using (var channel = ConfigureChannel(this.GetConnection(ConnectionString, _connectionAttempts)))
            {
                foreach (var message in messages)
                    channel.SendMessage(_topicName, string.Empty, message, cancellation);
            }

            return Task.CompletedTask;
        }

        public override async Task SendManyAsync<TModel>(IEnumerable<TModel> list, CancellationToken cancellation = default)
        {
            await SendAsync(list.Select(x => x.CreateMessagebusMessage()), cancellation);
        }

        private IModel ConfigureChannel(IConnection connection)
        {
            var channel = this.CreateChannel(connection);

            if (_declareOpts == null) return channel;

            channel.ExchangeDeclare(exchange: _topicName, type: _type, durable: _declareOpts.Durable, _declareOpts.AutoComplete, _declareOpts.Args);
            return channel;
        }
    }
}
