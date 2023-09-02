// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

Console.WriteLine("RabbitMQ Subscriber");

IConfigurationRoot config = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();
var section = config.GetSection("Settings");
var rabbitMQUrl = section.GetValue<string>("RabbbitMQUrl");

var factory = new ConnectionFactory();

factory.Uri = new Uri(rabbitMQUrl);

using var connection = factory.CreateConnection();

var channel = connection.CreateModel();

channel.ExchangeDeclare("header-exchange", type: ExchangeType.Headers, durable: true); //durable:false Uygulama restart atarsa Exchageler silinir


channel.BasicQos(0, 1, false); // Her bir istemci bir bir alır

var consumer = new EventingBasicConsumer(channel);


var queueName = channel.QueueDeclare().QueueName;

Dictionary<string,object> headers=new Dictionary<string, object>();

headers.Add("format", "pdf");
headers.Add("shape", "a4");
headers.Add("x-match", "all"); //all yukarıda eklenen parametre adları aynı olmalı
//headers.Add("x-match", "any"); //any yukarıda eklenen parametre adlarından en az biri aynı olmalı

channel.QueueBind(queueName, "header-exchange",string.Empty,headers);

channel.BasicConsume(queueName, false, consumer);


Console.WriteLine("Loglar Dinleniyor...");

consumer.Received += (object? sender, BasicDeliverEventArgs e) =>
{
    var message = Encoding.UTF8.GetString(e.Body.ToArray());
    Thread.Sleep(1500);
    Console.WriteLine("Gelen Mesaj : " + message);

 
    channel.BasicAck(e.DeliveryTag, false); // Mesajı siler
};



Console.ReadLine();

