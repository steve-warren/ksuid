using KsuidDotNet;

Console.ForegroundColor = ConsoleColor.Cyan;
var id = Ksuid.NewKsuid("cu_");
Console.WriteLine(id);