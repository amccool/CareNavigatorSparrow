namespace RandomADTEventProducer.Entities;
internal record Patient(string FirstName, string LastName, string? MiddleName, DateTimeOffset DateOfBirth, string address, string city, string state, string zipcode);


