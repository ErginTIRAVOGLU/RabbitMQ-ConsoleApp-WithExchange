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

channel.QueueDeclare("hello-queue", true, false, false);

//channel.BasicQos(0, 3, false); // Her bir istemci üç üç alır
//channel.BasicQos(0, 6, false); // Kaç istemci varsa 6'ya böler (3 istemci varsa 2-2-2 şeklinde alır)

channel.BasicQos(0, 1, false); // Her bir istemci bir bir alır

var consumer = new EventingBasicConsumer(channel);

//channel.BasicConsume("hello-queue", true, consumer); // Alınan mesajı otomatik siler
channel.BasicConsume("hello-queue", false, consumer); // Mesajı RabbitMQ'den silmek için aşağıdaki komuta ihtiyacı var

consumer.Received += (object? sender, BasicDeliverEventArgs e) =>
{
    var message = Encoding.UTF8.GetString(e.Body.ToArray());
    Thread.Sleep(1500);
    Console.WriteLine("Gelen Mesaj : " + message);
    channel.BasicAck(e.DeliveryTag, false); // Mesajı siler
};



Console.ReadLine();
