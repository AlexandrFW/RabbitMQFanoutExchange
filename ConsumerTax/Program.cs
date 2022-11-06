using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

internal class Program
{
    static double _totalHold;

    private static void Main(string[] args)
    {
        Console.WriteLine("RabbitMQ Default Exchange Consumer");

        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.ExchangeDeclare("notifier", ExchangeType.Fanout);

        var queueName = channel.QueueDeclare().QueueName;

        channel.QueueBind(queue: queueName,
                          exchange: "notifier",
                          routingKey: string.Empty);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (sender, e) =>
        {
            var body = e.Body;
            var message = Encoding.UTF8.GetString(body.ToArray());

            var payment = GetPayment(message);
            _totalHold += payment * 0.01;

            Console.WriteLine($"Payment received for amount of ${payment}");
            Console.WriteLine($"${_totalHold} total hold");
        };

        channel.BasicConsume(queue: queueName,
                             autoAck: true,
                             consumer: consumer);

        Console.WriteLine($"Subscribed to the queue '{queueName}'");

        Console.WriteLine($"Listening...");

        Console.ReadKey();
    }

    static double GetPayment(string message)
    {
        var messageWords = message.Split(' ');

        return double.Parse(messageWords[^1]);
    }
}