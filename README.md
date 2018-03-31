# SmtpServerStub

Allows you to check sent E-mails in your .NET project component tests without installing SMTP on your test machine.
Just add nugget package to your project.

## Getting Started

Just install Nuget package using Package Manager Console in VS
```
Install-Package SmtpServerStub -Version 1.1.0
```
Or using NuGet Package Manager search for SmtpServerStub package.

## Writing Tests with SmtpServerStub package

Firs of all you will need to initialize and start SmtpServerStub server. The simplest way to do this is to write following code in one time setup in a base test. Which will run once in the begining of your test run.
By default stub will start on 25 port of localhost.
```csharp
[OneTimeSetUp]
public void RunBeforeAllTest()
{
    var settings = new SmtpServerSettings();
    Server = new SmtpServer(settings);
    Server.Start();
    Console.WriteLine("Server Started");
}
```
To make your tests more independent you may need to reset server stub state before each test. Do this by adding "before each" method to base test.
Reseting server state is required to cleare list of already received E-mails and finish all asynchronous tasks, which can be created for receiving other messages.
```csharp
[SetUp]
public void RunBeforeAnyTest()
{
    Server.ResetState();
    Console.WriteLine("Server has been reset");
}
```

## Running the tests
[Synchronous check exapmples](https://github.com/ValentinKostiuk/SmtpServerStub/tree/DocumentationUpdate/SmtpServerStubIntegrationTests/Sync)  
[Asynchronous check exapmples](https://github.com/ValentinKostiuk/SmtpServerStub/tree/DocumentationUpdate/SmtpServerStubIntegrationTests/Async)  
[Simplified tests without required SSL encryption](https://github.com/ValentinKostiuk/SmtpServerStub/tree/DocumentationUpdate/SmtpServerStubIntegrationTests/NoSsl)  


### Break down into end to end tests

### And coding style tests

## Built With

* [AppVeyor](https://www.appveyor.com/) - Continuous Integration solution for Windows

## Code examples unit and component testing is made using flowing frameworks
* [NUnit](http://nunit.org/)
* [NSubstitute](http://nsubstitute.github.io/)
* [Fluent Assertions](https://fluentassertions.com/)

## Authors

* **Valentin Kostiuk** - [GitHub Page](https://github.com/ValentinKostiuk)

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/ValentinKostiuk/SmtpServerStub/blob/master/LICENSE) file for details
