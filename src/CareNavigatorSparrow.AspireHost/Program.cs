var builder = DistributedApplication.CreateBuilder(args);


var db = builder.AddPostgres("postgres").AddDatabase("carenavigator");

var kafka = builder.AddKafka("kafka")
  .WithKafkaUI(kafkaUI => kafkaUI.WithHostPort(9100))
  .WithDataBindMount(
    source: @"D:\temp\kafkaData",
    isReadOnly: false);

builder.AddProject<Projects.CareNavigatorSparrow_Web>("web")
  .WithReference(db)
  .WithReference(kafka);


await builder.Build().RunAsync();
