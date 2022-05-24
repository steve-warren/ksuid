using System.Security.Cryptography;
using Ksuid;

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine(KsuidFactory.New(RandomNumberGenerator.Create(), DateTime.UtcNow, "cu_"));