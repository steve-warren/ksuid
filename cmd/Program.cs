using System.Threading.Channels;
using KsuidDotNet;

Console.ForegroundColor = ConsoleColor.Cyan;

var set = new HashSet<string>();

var channel = Channel.CreateUnbounded<string>(
    new UnboundedChannelOptions { SingleReader = true, SingleWriter = true }
);

var flag = 1;

_ = Task.Run(async () =>
{
    var count = 0;

    await foreach (var id in channel.Reader.ReadAllAsync())
    {
        count++;

        if (!set.Add(id))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"error: duplicate ksuid {id}");
            Volatile.Write(ref flag, 0);
            break;
        }

        Console.WriteLine($"{count} - {id}");
    }
});

while (Volatile.Read(ref flag) == 1)
{
    var id = Ksuid.NewKsuid("cust_");

    channel.Writer.TryWrite(id);
}
