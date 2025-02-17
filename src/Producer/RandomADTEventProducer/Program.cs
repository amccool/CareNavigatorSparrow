using Bogus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RandomADTEventProducer;
using RandomADTEventProducer.Entities;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

//no more builder.Services.AddHostedService<RandomEventProducerHostedService>();

builder.Services.AddHostedService<RandomADTEventProducer.Services.Hl7PeriodicSenderService>();
builder.Services.AddScoped<RandomADTEventProducer.Services.IHl7Sender, RandomADTEventProducer.Services.Hl7Sender>();


//inject a bogus data into the hl7 sender
var listOfFakePatient = new Faker<Patient>()
  .CustomInstantiator(f => new Patient(
    f.Person.FirstName,
    f.Person.LastName,
    f.Person.FirstName,
    f.Date.Past(yearsToGoBack: 18),
    f.Address.StreetAddress(useFullAddress: false),
    f.Address.City(),
    f.Address.StateAbbr(),
    f.Address.ZipCode().Substring(0, 5)));
  

builder.Services.AddSingleton(listOfFakePatient);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();
//builder.AddNpgsqlDbContext<TodoDbContext>(connectionName: "apidb-postgres");
//builder.AddSqlServerDbContext<BlogContext>(connectionName: "apidb-sqlserver");
builder.AddKafkaProducer<string, string>("kafka");

IHost host = builder.Build();
await host.RunAsync();
