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

 

channel.BasicQos(0, 1, false); // Her bir istemci bir bir alır

var consumer = new EventingBasicConsumer(channel);


var queueName = channel.QueueDeclare().QueueName;

var routeKey = "*.Error.*";

channel.QueueBind(queueName, "logs-topic",routeKey);

channel.BasicConsume(queueName, false, consumer);

Console.WriteLine("Loglar Dinleniyor...");

consumer.Received += (object? sender, BasicDeliverEventArgs e) =>
{
    var message = Encoding.UTF8.GetString(e.Body.ToArray());
    Thread.Sleep(1500);
    Console.WriteLine("Gelen Mesaj : " + message);

    //File.AppendAllText("logs-critical.txt",message + "\n",Encoding.UTF8);
    channel.BasicAck(e.DeliveryTag, false); // Mesajı siler
};



Console.ReadLine();

