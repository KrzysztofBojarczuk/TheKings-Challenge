// See https://aka.ms/new-console-template for more information
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

var kings = await GetKings();

Console.WriteLine($"1 Total number of Monarchs: {kings.Count}");

var longestReignKing = LongestReginMonarch(kings);

var reignLength = CalculateReignLength(longestReignKing.Yrs);

Console.WriteLine($"2 The Longest reigning Monarch: {longestReignKing.Nm}, total reign length: {reignLength} years");

var longestRulingHouse = GetLongestRulingHouse(kings);

Console.WriteLine($"3 The Longest ruled house: {longestRulingHouse.Key}, length: {longestRulingHouse.Value} years");

var mostCommonFirstName = GetMostCommonFirstName(kings);

Console.WriteLine($"4 The most common first name: {mostCommonFirstName}");

static async Task<List<King>> GetKings()
{
    using (var httpClient = new HttpClient())
    {
        var response = await httpClient.GetStringAsync("https://gist.githubusercontent.com/SECRETAPI");
        return JsonConvert.DeserializeObject<List<King>>(response);
    }
}

static King LongestReginMonarch(List<King> kings)
{
    var longestReignKing = kings
        .OrderByDescending(k => CalculateReignLength(k.Yrs))
        .FirstOrDefault();

    return longestReignKing;
}

static KeyValuePair<string, int> GetLongestRulingHouse(List<King> kings)
{
    var longestRulingHouse = kings
       .GroupBy(king => king.Hse)
       .ToDictionary(
           group => group.Key,
           group => group.Sum(king => CalculateReignLength(king.Yrs))
       )
       .OrderByDescending(kv => kv.Value)
       .FirstOrDefault();

    return longestRulingHouse;
}

static int CalculateReignLength(string years)
{
    var yearParts = years.Split('-');
    if (yearParts.Length == 2 && int.TryParse(yearParts[0], out int startYear))
    {
        if (int.TryParse(yearParts[1], out int endYear))
        {
            return endYear - startYear;
        }
        else if (yearParts[1] == "")
        {
            return DateTime.Now.Year - startYear;
        }
    }

    return 0;
}

static string GetMostCommonFirstName(List<King> kings)
{
    var firstNameOccurrences = kings
        .GroupBy(k => k.Nm.Split(' ').First(), StringComparer.OrdinalIgnoreCase)
        .ToDictionary(g => g.Key, g => g.Count());

    var mostCommonFirstName = firstNameOccurrences
        .OrderByDescending(kv => kv.Value)
        .FirstOrDefault();

    return mostCommonFirstName.Key;
}

public class King
{
    public  int Id { get; set; }
    public string Nm { get; set; }
    public string Cty { get; set; }
    public string Hse { get; set; }
    public string Yrs { get; set; }
}