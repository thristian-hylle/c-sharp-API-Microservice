using Confluent.Kafka;
using System.Text.Json;

public class KafkaService : IDisposable
{
    private readonly IProducer<string, string> _producer;

    public KafkaService()
    {
        var config = new ProducerConfig
        {
            BootstrapServers = "localhost:9092"
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task PublishProductCreated(Product product)
    {
        var message = JsonSerializer.Serialize(product);

        await _producer.ProduceAsync("product.created", new Message<string, string>
        {
            Key = product.Id.ToString(),
            Value = message
        });
    }

    public void Dispose()
    {
        _producer.Flush(TimeSpan.FromSeconds(5));
        _producer.Dispose();
    }
}