using Bogus;
using Confluent.Kafka;
using Efferent.HL7.V2;
using Microsoft.Extensions.Logging;
using RandomADTEventProducer.Entities;

namespace RandomADTEventProducer.Services;
internal class Hl7Sender(
  ILogger<Hl7Sender> logger,
  IProducer<string, string> kafkaProducer,
  Faker<Patient> fakePatients) : IHl7Sender
{
  private const string Topic = "hl7";

  /// <summary>
  ///   here are two sample ADT messages.  The first is from a hospital system, the second is from a clinic system.
  /// 
  ///  using bogus to generate fake patient data create ADT^A01 messages, and publish them into the kafka stream
  /// 
  /// MSH|^~\&|AcmeHIS|StJohn|CATH|StJohn|20061019172719||ADT^O01|MSGID12349876|P|2.3
  /// PID||0493575^^^2^ID 1|454721||DOE^JOHN^^^^|DOE^JOHN^^^^|19480203|M||B|254 MYSTREET AVE^^MYTOWN^OH^44123^USA||(216)123-4567|||M|NON|400003403~1129086|
  /// NK1||ROE^MARIE^^^^|SPO||(216)123-4567||EC|||||||||||||||||||||||||||
  /// NK1||DOE^JHON^^^^|FTH||(216)123-4567||EC|||||||||||||||||||||||||||
  /// PV1||O|OP^PAREG^^^^^^^^||||277^MYLASTNAME^BONNIE^^^^|||||||||| ||2688684|||||||||||||||||||||||||199912271408||||||002376853
  /// 
  /// MSH|^~\&|MS4_AZ|UNV|PREMISE|UNV|20180301010000||ADT^A04|IHS-20180301010000.00120|P|2.1
  /// EVN|A04|20180301010000
  /// PID|1||19050114293307.1082||BUNNY^BUGS^RABBIT^^^MS||19830215|M|||1234 LOONEY RD^APT A^CRAIGMONT^ID^83523^USA|||||||111-11-1111|111-11-1111
  /// PV1|1|E|ED^^^UNV|C|||999-99-9999^MUNCHER^CHANDRA^ANDRIA^MD^DR|888-88-8888^SMETHERS^ANNETTA^JONI^MD^DR||||||7||||REF||||||||||||||||||||||||||20180301010000
  /// </summary>
  /// <param name="stoppingToken"></param>
  /// <returns></returns>
  public async Task Execute(CancellationToken stoppingToken)
  {
    logger.LogInformation("Hl7Sender.Execute has been called.");

    // generate a unique message control id for the HL7 message, used for deduplication if a dulpcate message is sent
    var uniqueMessageControlId = Guid.NewGuid().ToString("N");

    // generate a fake patient suing bogus
    var patient = fakePatients.Generate(1).First();


    var message = new Efferent.HL7.V2.Message();

    string version = "2.5.1";
    string processingId = "T"; //training
    string messageControlId = uniqueMessageControlId;
    string messageType = "ADT^01"; //admit                   https://pkbdev.atlassian.net/wiki/spaces/api/pages/3365077971/ADT+A01
    string security = string.Empty;
    string receivingFacility = "Facility1";
    string receivingApplication = "CareNavigatorSparrow";
    string sendingFacility = "facility1";
    string sendingApplication = "badassEngine";

    message.AddSegmentMSH(sendingApplication, sendingFacility, receivingApplication, receivingFacility, security, messageType, messageControlId, processingId, version);

    var enc = new HL7Encoding();

    Segment EVN = new Segment("EVN", enc);
    EVN.AddNewField((DateTime.UtcNow - TimeSpan.FromMinutes(-2)).ToString("yyyyMMddHHmmss"), 2); //recorded time
    EVN.AddNewField((DateTime.UtcNow - TimeSpan.FromMinutes(-3)).ToString("yyyyMMddHHmmss"), 6); //occured time

    Segment PID = new Segment("PID", enc);
    PID.AddNewField($"{patient.FirstName} {patient.MiddleName} {patient.LastName}", 5); //name field
    PID.AddNewField($"{patient.address} {patient.city} {patient.state} {patient.zipcode}", 11); //addr field

    Segment PV1 = new Segment("PV1", enc);
    PV1.AddNewField(" floor 1  room 101 bed 1", 3); //patient location (floor, room, bed)  //TODO: make this a randomly selected bed from the RTLS feed
    PV1.AddNewField(Guid.NewGuid().ToString("N"), 19); //visit/encounter id

    message.AddNewSegment(EVN);
    message.AddNewSegment(PID);
    message.AddNewSegment(PV1);

    var adtHl7Msg = message.SerializeMessage();

    try
    {
      var deliveryResult = await kafkaProducer.ProduceAsync(Topic, new Message<string, string> { Key = uniqueMessageControlId, Value = adtHl7Msg }, cancellationToken: stoppingToken);
      //DeliveryResult is not well defined, we cant use @ to get all its field
      logger.LogInformation("Delivered message: {Status} UTC:{Timestamp} Partition:{Partition}", deliveryResult.Status, deliveryResult.Timestamp.UtcDateTime, deliveryResult.Partition);
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Error sending HL7 message to Kafka"); //we should look for a retry mechanism
    }
  }
}
