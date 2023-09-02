// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using Shared;
using System.Text;
using System.Text.Json;

Console.WriteLine("RabbitMQ Publisher");

IConfigurationRoot config = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();
var section = config.GetSection("Settings");
var rabbitMQUrl = section.GetValue<string>("RabbbitMQUrl");


var factory = new ConnectionFactory();

factory.Uri = new Uri(rabbitMQUrl);

using var connection = factory.CreateConnection();

var channel = connection.CreateModel();

channel.QueueDeclare("hello-queue", true, false, false);

var properties = channel.CreateBasicProperties();
properties.Persistent = true; //Gönderilen Mesaj RabbitMQ restart edildiğinde silinmez.

//Enumerable.Range(1, 50).ToList().ForEach(x =>
//{
//string message = $"Message {x}";

var product = new Product { Id = 1, Name = "Kalem", Price = 100, Stock = 10 };

var productJsonString=JsonSerializer.Serialize(product);

//var messageBody = Encoding.UTF8.GetBytes(message);
var messageBody = Encoding.UTF8.GetBytes(productJsonString);

channel.BasicPublish(string.Empty, "hello-queue",properties, messageBody);

//Console.WriteLine($"Mesaj Gönderilmiştir : {message}");
Console.WriteLine($"Mesaj Gönderilmiştir : {productJsonString}");

//});

Console.ReadLine();