namespace RandomADTEventProducer.Services;


public class Hl7PeriodicSenderService : BackgroundService
{
  private readonly IServiceScopeFactory _serviceScopeFactory;
  public Hl7PeriodicSenderService(IServiceScopeFactory serviceScopeFactory) => _serviceScopeFactory = serviceScopeFactory;

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    // so long as we havent been cancelled
    while (!stoppingToken.IsCancellationRequested)
    {
      // do async work
      using (var scope = _serviceScopeFactory.CreateScope())
      {
        var hl7Service = scope.ServiceProvider.GetRequiredService<IHl7Sender>();
        await hl7Service.Execute(stoppingToken);
      }
      await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
    }
  }
}
