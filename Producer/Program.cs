
using RabbitMQ.Client;
using System.Text;

Console.WriteLine("RabbitMQ Fanout Exchange Publisher");

var random = new Random();
do
{
    int timeToSleep = random.Next(1000, 3000);
    Thread.Sleep(timeToSleep);

    var factory = new ConnectionFactory { HostName = "localhost" };
    using var connection = factory.CreateConnection();
    using var channel = connection.CreateModel();

    channel.ExchangeDeclare("notifier", ExchangeType.Fanout);

    var moneyCount = random.Next(1000, 10000);

    string message = $"Payment received for the amount of {moneyCount}";

    var body = Encoding.UTF8.GetBytes(message);

    channel.BasicPublish(exchange: "notifier",
                         routingKey: "",
                         basicProperties: null,
                         body: body);

    Console.WriteLine($"Payment received for the amount of {moneyCount}\nNotified by 'notifier' exchange");
}
while (true);
