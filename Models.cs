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

public class EmployeeJson
{
    public string Name { get; set; }
    public int Id { get; set; }
    public double TotalRevenue { get; set; }
    public double ExtraHours { get; set; }
    public double OwedHours { get; set; }
    public int MissingDays { get; set; }
    public int ExtraDays { get; set; }
    public int TotalDaysOfWork { get; set; }
}

public class Company
{
    public string Departament { get; set; }
    public string Month { get; set; }
    public int Year { get; set; }
    public double TotalToPay { get; set; }
    public double TotalDisccounts { get; set; }
    public double TotalExtras { get; set; }
    public List<EmployeeJson> Employees { get; set; }

}