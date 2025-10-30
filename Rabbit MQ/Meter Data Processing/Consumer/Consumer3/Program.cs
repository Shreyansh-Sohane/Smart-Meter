using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using SharedLibrary.Models;
using SharedLibrary.Utils;

namespace Consumer3
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.ExchangeDeclareAsync(exchange: "logs", type: ExchangeType.Fanout);
            string queueName = "meter_readings_queue";
            await channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false);
            await channel.QueueBindAsync(queue: queueName, exchange: "logs", routingKey: string.Empty);
            await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);

            Console.WriteLine("Waiting for meter readings. Press [enter] to exit.");

            var dbService = new DatabaseService("Host=localhost;Username=postgres;Password=1234;Database=SmartMeterDB");

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var messageJson = Encoding.UTF8.GetString(body);

                try
                {
                    var data = JsonSerializer.Deserialize<MeterReading>(messageJson);
                    Console.WriteLine($"Received -> Meter: {data.meterid}, Energy: {data.energyconsumed}");

                    await dbService.InsertMeterReadingAsync(data);
                    Console.WriteLine("✅ Inserted into database.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[!] Error: {ex.Message}");
                }
            };

            await channel.BasicConsumeAsync(queue: queueName, autoAck: true, consumer: consumer);
            Console.ReadLine();
        }
    }
}
