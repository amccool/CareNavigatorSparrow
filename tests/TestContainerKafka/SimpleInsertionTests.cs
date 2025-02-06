using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Testcontainers.Kafka;
using Xunit;
using Xunit.Abstractions;

namespace TestContainerKafka;

public class SimpleInsertionTests : IAsyncLifetime
{
    private readonly KafkaContainer _kafkaContainer;

    public SimpleInsertionTests(ITestOutputHelper output)
    {
        ILogger<SimpleInsertionTests> logger = output.ToLogger<SimpleInsertionTests>();

        _kafkaContainer = new KafkaBuilder()
            .WithCleanUp( cleanUp: true)
            .WithLogger( logger)
            .Build();

        ArgumentNullException.ThrowIfNull(_kafkaContainer);
    }


    public Task InitializeAsync()
    {
        return _kafkaContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _kafkaContainer.DisposeAsync().AsTask();
    }


    [Fact]
    public async Task InsertStuffIntoThisHereKafka()
    {
        // Given
        const string topic = "test1";

        var bootstrapServer = _kafkaContainer.GetBootstrapAddress();

        var producerConfig = new ProducerConfig();
        producerConfig.BootstrapServers = bootstrapServer;

        var consumerConfig = new ConsumerConfig();
        consumerConfig.BootstrapServers = bootstrapServer;
        consumerConfig.GroupId = "test1-consumer";
        consumerConfig.AutoOffsetReset = AutoOffsetReset.Earliest;

        var message = new Message<string, string>() { Key = Guid.NewGuid().ToString(), Value = Guid.NewGuid().ToString("D"), Timestamp = Timestamp.Default }; //, Headers =  };

        // When
        using var producer = new ProducerBuilder<string, string>(producerConfig).Build();
        _ = await producer.ProduceAsync(topic, message)
            .ConfigureAwait(true);

        using var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
        consumer.Subscribe(topic);

        var result = consumer.Consume(timeout: TimeSpan.FromSeconds(15));

        // Then
        Assert.NotNull(result);
        Assert.Equal(message.Value, result.Message.Value);

    }

    [Fact]
    public async Task ConsumerReturnsProducerMessage()
    {
        // Given
        const string topic = "sample";

        var bootstrapServer = _kafkaContainer.GetBootstrapAddress();

        var producerConfig = new ProducerConfig();
        producerConfig.BootstrapServers = bootstrapServer;

        var consumerConfig = new ConsumerConfig();
        consumerConfig.BootstrapServers = bootstrapServer;
        consumerConfig.GroupId = "sample-consumer";
        consumerConfig.AutoOffsetReset = AutoOffsetReset.Earliest;

        var message = new Message<string, string>();
        message.Value = Guid.NewGuid().ToString("D");

        // When
        using var producer = new ProducerBuilder<string, string>(producerConfig).Build();
        _ = await producer.ProduceAsync(topic, message)
            .ConfigureAwait(true);

        using var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
        consumer.Subscribe(topic);

        var result = consumer.Consume(timeout: TimeSpan.FromSeconds(15));

        // Then
        Assert.NotNull(result);
        Assert.Equal(message.Value, result.Message.Value);
    }

}
