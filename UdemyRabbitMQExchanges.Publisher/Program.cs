// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;

Console.WriteLine("RabbitMQ Publisher");

IConfigurationRoot config = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();
var section = config.GetSection("Settings");
var rabbitMQUrl = section.GetValue<string>("RabbbitMQUrl");


var factory = new ConnectionFactory();

factory.Uri = new Uri(rabbitMQUrl);

using var connection = factory.CreateConnection();

var channel = connection.CreateModel();

//channel.QueueDeclare("hello-queue", true, false, false); // Kuyruğu Subscriber oluştursun

//channel.ExchangeDeclare("logs-fanout", type: ExchangeType.Fanout, durable: true); //durable:false Uygulama restart atarsa Exchageler silinir
channel.ExchangeDeclare("logs-direct", type: ExchangeType.Direct, durable: true); //durable:false Uygulama restart atarsa Exchageler silinir

Enum.GetNames(typeof(LogNames)).ToList().ForEach(name =>
{
    var routeKey = $"route-{name}";
    var queueName = $"direct-queue-{name}";
    channel.QueueDeclare(queueName, true, false, false);
    channel.QueueBind(queueName, "logs-direct",routeKey,null);

});

Enumerable.Range(1, 50).ToList().ForEach(x =>
{
    LogNames logName = (LogNames)new Random().Next(1, 5);

    //string message = $"Log Message {x}";
    string message = $"Log Type {logName} -> Log Message {x}";

    var messageBody = Encoding.UTF8.GetBytes(message);

    var routeKey=$"route-{logName}";

    //channel.BasicPublish(string.Empty, "hello-queue", null, messageBody);
    channel.BasicPublish("logs-direct", routeKey, null, messageBody);

    Console.WriteLine($"Mesaj Log Gönderilmiştir : {message}");

});

Console.ReadLine();

public enum LogNames
{
    Critical=1,
    Error=2,
    Warning=3,
    Info=4
}