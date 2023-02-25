﻿using System.Globalization;
using CsvHelper.Configuration;

public class CSVProcessing
{
    public void Reading(string filename)
    {
        var configCsvHelper = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";",
            HasHeaderRecord = true,
            IgnoreBlankLines = true,
        };


    }
}

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }
    public double HourValue { get; set; }
    public DateTime Date { get; set; }
    public DateTime EntryTime { get; set; }
    public DateTime ExitTime { get; set; }
    public List<DateTime> LunchTime { get; set; }
}

public sealed class EmployeeCsvMap : ClassMap<Employee>
{
    public EmployeeCsvMap()
    {
        Map(e => e.Id).Name("C�digo");
        Map(e => e.Name).Name("Name");
        Map(e => e.HourValue).Name("Valor hora");
        Map(e => e.Date).Name("Data");
        Map(e => e.EntryTime).Name("Entrada");
        Map(e => e.ExitTime).Name("Sa�da");
        Map(e => e.LunchTime).Name("Almo�o");
    }
}