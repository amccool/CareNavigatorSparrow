var builder = DistributedApplication.CreateBuilder(args);


var db = builder.AddPostgres("postgres")
  .WithPgAdmin()// (pgAdmin => pgAdmin.WithHostPort(5050))
  .AddDatabase("carenavigator");

var kafka = builder.AddKafka("kafka")
  .WithKafkaUI(kafkaUI => kafkaUI.WithHostPort(9100))
  .WithLifetime(ContainerLifetime.Persistent);

builder.AddProject<Projects.CareNavigatorSparrow_Web>("web")
  .WithReference(db).WaitFor(db)
  .WithReference(kafka).WaitFor(kafka);

builder.AddProject<Projects.RandomADTEventProducer>("randomadteventproducer")
  .WithReference(kafka).WaitFor(kafka);

await builder.Build().RunAsync();
