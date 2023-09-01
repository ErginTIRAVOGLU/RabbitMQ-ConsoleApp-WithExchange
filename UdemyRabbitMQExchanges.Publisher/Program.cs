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

channel.ExchangeDeclare("logs-topic", type: ExchangeType.Topic, durable: true); //durable:false Uygulama restart atarsa Exchageler silinir

Random rnd = new Random();

Enumerable.Range(1, 50).ToList().ForEach(x =>
{ 
    LogNames log1 = (LogNames)rnd.Next(1, 5);
    LogNames log2 = (LogNames)rnd.Next(1, 5);
    LogNames log3 = (LogNames)rnd.Next(1, 5);

    string message = $"Log Type {log1}.{log2}.{log3} -> Log Message {x}";

    var messageBody = Encoding.UTF8.GetBytes(message);

    var routeKey = $"{log1}.{log2}.{log3}";

    channel.BasicPublish("logs-topic", routeKey, null, messageBody);

    Console.WriteLine($"Mesaj Log Gönderilmiştir : {message}");

});

Console.ReadLine();

public enum LogNames
{
    Critical = 1,
    Error = 2,
    Warning = 3,
    Info = 4
}