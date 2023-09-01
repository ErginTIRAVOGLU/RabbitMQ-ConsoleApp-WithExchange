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

//channel.QueueDeclare("hello-queue", true, false, false);

//channel.ExchangeDeclare("logs-fanout", type: ExchangeType.Fanout, durable: true); //durable:false Uygulama restart atarsa Exchageler silinir

////var randomQueueName = channel.QueueDeclare().QueueName; //

//var randomQueueName = "logs-database-save-queue";

//channel.QueueDeclare(randomQueueName, true, false, false);

//channel.QueueBind(randomQueueName, "logs-fanout", "", null);



channel.BasicQos(0, 1, false); // Her bir istemci bir bir alır

var consumer = new EventingBasicConsumer(channel);

//channel.BasicConsume("hello-queue", true, consumer); // Alınan mesajı otomatik siler
//channel.BasicConsume("hello-queue", false, consumer); // Mesajı RabbitMQ'den silmek için aşağıdaki komuta ihtiyacı var

var queueName = "direct-queue-Critical";
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

