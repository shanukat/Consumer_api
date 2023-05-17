using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Newtonsoft.Json;

public class KafkaConsumer<TKey, TValue>
{
    private readonly IConsumer<TKey, TValue> _consumer;

    public KafkaConsumer(string bootstrapServers, string topic, string groupId)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = bootstrapServers,
            GroupId = groupId,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        var consumerBuilder = new ConsumerBuilder<TKey, TValue>(config);
        if (typeof(TKey) == typeof(string))
        {
            consumerBuilder.SetKeyDeserializer(new Utf8Deserializer() as IDeserializer<TKey>);
        }
        _consumer = consumerBuilder.Build();
       
        _consumer.Subscribe(topic);
    }

    public async Task<TValue> ConsumeAsync(CancellationToken cancellationToken = default)
    {
        var result = await Task.Run(() => _consumer.Consume(cancellationToken));
        return result.Message.Value;
    }

    public void Dispose()
    {
        _consumer.Dispose();
    }

    //private class JsonDeserializer<T> : IDeserializer<T>
    //{
    //    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    //    {
    //        if (isNull) return default;
    //        return JsonConvert.DeserializeObject<T>(data.ToString());
    //    }
    //}

    public class Utf8Deserializer : IDeserializer<string>
    {
        public string Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            if (isNull) return null;
            return Encoding.UTF8.GetString(data);
        }
    }
}
