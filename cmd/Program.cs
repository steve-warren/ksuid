using KsuidDotNet;

Console.ForegroundColor = ConsoleColor.Cyan;
var id = Ksuid.NewKsuid("c_");
Console.WriteLine(id);