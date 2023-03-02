using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

public class CSVProcessing
{
    public async Task CsvMapAndProcessing(string filename, string outputJsonDirectory)
    {
        var configCsvHelper = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";",
            HasHeaderRecord = true,
            IgnoreBlankLines = true,
        };

        var nameOfCurrentFile = Path.GetFileNameWithoutExtension(filename);
        string[] metaDataFromFile = nameOfCurrentFile.Split('-');

        var employees = new List<Employee>();

        using (var reader = new StreamReader(filename))
        using (var csv = new CsvReader(reader, configCsvHelper))
        {
            await csv.ReadAsync();
            csv.ReadHeader();

            csv.Context.RegisterClassMap<EmployeeCsvMap>();

            while (await csv.ReadAsync())
            {
                var employee = csv.GetRecord<Employee>();
                employees.Add(employee);
            }
            
            var util = new UtilsProcessing();
            var completeList = util.GetListOfEmployesById(employees);
            var jsonGenerated = util.GenerateCompleteReport(completeList,
                                        metaDataFromFile[0],
                                        metaDataFromFile[1],
                                        Convert.ToInt32(metaDataFromFile[2]));

            File.WriteAllText(outputJsonDirectory + 
                                @"\" + $"{metaDataFromFile[0]}-{metaDataFromFile[1]}-{metaDataFromFile[2]}.json", 
                                jsonGenerated);
            Console.WriteLine("Finished file: " + filename);
        }
    }
}