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

channel.ExchangeDeclare("header-exchange", type: ExchangeType.Headers, durable: true); //durable:false Uygulama restart atarsa Exchageler silinir
Dictionary<string,object> headers = new Dictionary<string, object>();

headers.Add("format", "pdf");
headers.Add("shape", "a4");

var properties = channel.CreateBasicProperties();
properties.Headers=headers;

Console.WriteLine("Mesaj Gönderilmiştir");

channel.BasicPublish("header-exchange", string.Empty, properties,Encoding.UTF8.GetBytes("Header Mesajım"));
 
Console.ReadLine();
