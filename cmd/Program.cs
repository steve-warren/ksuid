using System.Collections.Concurrent;
using KsuidDotNet;

Console.ForegroundColor = ConsoleColor.Cyan;

var bag = new ConcurrentBag<string>();

var tasks = Enumerable.Range(0, 32).Select(taskNumber =>
{
    return Task.Run(() =>
    {
        for (var i = 0; i < 100_000; i++)
        {
            bag.Add(Ksuid.NewKsuid());
        }
    });
});

Task.WaitAll(tasks);

var set = new HashSet<string>(bag);

Console.WriteLine(set.Count == 3_200_000 ? "collision pass" : "collision fail");

var lexicalList = new List<string>(100_000);
var baseTime = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

for (var i = 0; i < 100_000; i++)
{
    var simulatedTime = baseTime.AddSeconds(i);
    lexicalList.Add(Ksuid.NewKsuid(simulatedTime));
}

var shuffledList = lexicalList.Shuffle().ToList();

var orderedList = shuffledList.OrderBy(x => x, StringComparer.Ordinal).ToList();

var expression = orderedList.SequenceEqual(lexicalList);

Console.WriteLine(expression ? "lexical pass" : "lexical fail");
