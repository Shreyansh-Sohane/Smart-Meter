using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.ExchangeDeclareAsync(exchange: "logs", type: ExchangeType.Fanout);

DateTime startDate = new DateTime(2025, 10, 29, 0, 0, 0);
DateTime endDate = new DateTime(2025, 10, 31, 0, 0, 0);

int startMeter = 0;
int totalMeters = 2000;

Console.WriteLine(" Starting to send meter data...");

DateTime currentReadingDate = startDate;

while (currentReadingDate < endDate)
{
    for (int i = startMeter; i <= totalMeters; i++)
    {
        double energyConsumed = Random.Shared.NextDouble() * 10; 
        var messageObject = new
        {
            meterid = $"MTR{i:D5}",
            meterreadingdate = currentReadingDate.ToString("yyyy-MM-dd HH:mm:ss"),
            energyconsumed = Math.Round(energyConsumed, 2),
            voltage = Math.Round(220 + Random.Shared.NextDouble() * 10, 2),
            current = Math.Round(5 + Random.Shared.NextDouble() * 2, 2)
        };

        string messageJson = JsonSerializer.Serialize(messageObject);
        byte[] body = Encoding.UTF8.GetBytes(messageJson);

        await channel.BasicPublishAsync(exchange: "logs", routingKey: string.Empty, body: body);
        Console.WriteLine($" [x] Sent: {messageJson}");

        //await Task.Delay(100); 
    }

    currentReadingDate = currentReadingDate.AddMinutes(15);
}

Console.WriteLine("All meter data sent successfully.");
