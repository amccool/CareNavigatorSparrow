namespace RandomADTEventProducer.Services;
internal interface IHl7Sender
{
  Task Execute(CancellationToken stoppingToken);
}
