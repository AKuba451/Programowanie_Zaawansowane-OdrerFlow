using System.Collections.Generic;

namespace ConsoleApp1.Models;

public class NbpResponse
{
    public List<NbpRate> Rates { get; set; } = new();
}

public class NbpRate
{
    public decimal Mid { get; set; }
}