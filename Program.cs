using System.Diagnostics;


var mainProcessing = new CSVProcessing();
var sw = new Stopwatch();

Console.WriteLine("Enter input Path");
var inputDirectory = @"" + Console.ReadLine();
while (!Directory.Exists(inputDirectory))
{
    Console.WriteLine("Path does not exists. Please, enter again:");
    inputDirectory = @"" + Console.ReadLine();
}

Console.WriteLine("Enter Output Path");
var outputDirectory = @"" + Console.ReadLine();
while (!Directory.Exists(outputDirectory))
{
    Console.WriteLine("Path does not exists. Please, enter again:");
    outputDirectory = @"" + Console.ReadLine();
}

string[] directoryFiles = Directory.GetFiles(inputDirectory);

sw.Start();
await Parallel.ForEachAsync(directoryFiles, async (file, ct) =>
{
    await mainProcessing.CsvMapAndProcessing(file, outputDirectory);
});
sw.Stop();

Console.WriteLine($"\nTime: {sw.ElapsedMilliseconds} ms");
Console.WriteLine($"Memory Used: {Process.GetCurrentProcess().WorkingSet64 / 1024 / 1024} mb");
