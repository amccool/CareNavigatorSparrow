var builder = DistributedApplication.CreateBuilder(args);

//resources
var db = builder.AddPostgres("postgres")
  .WithPgAdmin()
  .AddDatabase("carenavigator");

var kafka = builder.AddKafka("kafka")
  .WithKafkaUI(kafkaUI => kafkaUI.WithHostPort(9100))
  //.WithLifetime(ContainerLifetime.Persistent); //healcheck topics are occasionally failing
  .WithLifetime(ContainerLifetime.Session);

//projects
builder.AddProject<Projects.CareNavigatorSparrow_Web>("web")
  .WithReference(db).WaitFor(db)
  .WithReference(kafka).WaitFor(kafka);

builder.AddProject<Projects.RandomADTEventProducer>("randomadteventproducer")
  .WithReference(kafka).WaitFor(kafka);

await builder.Build().RunAsync();
