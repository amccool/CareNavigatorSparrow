var builder = DistributedApplication.CreateBuilder(args);


var db = builder.AddPostgres("postgres")
  .WithPgAdmin()// (pgAdmin => pgAdmin.WithHostPort(5050))
  .AddDatabase("carenavigator");

var kafka = builder.AddKafka("kafka")
  //auto.create.topics.enable=true 
  //.WithCommand("kafka-topics --create --topic carenavigator --partitions 1 --replication-factor 1 --bootstrap-server localhost:9092")
  //.WithBuildArg("KAFKA_AUTO_CREATE_TOPICS_ENABLE", "true")
  .WithKafkaUI(kafkaUI => kafkaUI.WithHostPort(9100))
  //.WithDataBindMount(
  //  source: @"D:\temp\kafkaData",
  //  isReadOnly: false)
  .WithLifetime(ContainerLifetime.Persistent);

builder.AddProject<Projects.CareNavigatorSparrow_Web>("web")
  .WithReference(db).WaitFor(db)
  .WithReference(kafka).WaitFor(kafka);

builder.AddProject<Projects.RandomADTEventProducer>("randomadteventproducer")
  .WithReference(kafka).WaitFor(kafka);

await builder.Build().RunAsync();
