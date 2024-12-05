using System.Collections.Generic;

namespace LendingView.Models
{
    public class Country
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string FlagUrl { get; set; } // URL for the flag icon
    }

    public static class CountryData
    {
        public static List<Country> GetCountries() => new List<Country>
        {
            new Country { Name = "United States", Code = "+1", FlagUrl = "https://flagcdn.com/us.svg" },
            new Country { Name = "Canada", Code = "+1", FlagUrl = "https://flagcdn.com/ca.svg" },
            new Country { Name = "United Kingdom", Code = "+44", FlagUrl = "https://flagcdn.com/gb.svg" },
            new Country { Name = "India", Code = "+91", FlagUrl = "https://flagcdn.com/in.svg" },
            new Country { Name = "Germany", Code = "+49", FlagUrl = "https://flagcdn.com/de.svg" }
            // Add more countries as needed
        };
    }
}
    
