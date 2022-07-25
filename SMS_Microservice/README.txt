# SMS_MIcroservice

## Service Overview

Service Type: Microservice / Worker

### Description and Architecture Overview

This is the Test Exercise for James Rogers

To Run the project please open a command window on a machine that has .net v 6.0.302 installed. 

navigate to the folder of the solution and run "dotnet build" 

You can run the Project using dotnet run --project SMS_Microservice, this however will not do a lot as the command consumer/event publish source and SMS provider do not have conrete implementations at this time. 

You can run the unit tests using "dotnet test"

This solution Was built in Visual Studio 2022 using .net 6.0 and should be able to be opened in both VS and rider or any other IDE of your choice. 

I've used a Flurl client in this testing as it provides easy and well documented testing strategies out of the box. 
Similarly I have used a InMemory Test Bus to simulate the message queue so the Command can be sent and recieved by the consumer. 

I hope to hear from you in the near future for the technical interview. 

### Dependencies

Below are a list of dependencies consumed by this service.

| Name             | Used For                             | Failure Mode                                                                                                         |
| ---------------- | ------------------------------------ | -------------------------------------------------------------------------------------------------------------------- |
| .Net             | Running and Building                 | .Net 6 Release notes [Link]https://github.com/dotnet/core/blob/main/release-notes/6.0/6.0.3/6.0.3.md?WT.mc_id=dotnet-35129-website



