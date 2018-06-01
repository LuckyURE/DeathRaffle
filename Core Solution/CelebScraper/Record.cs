using FileHelpers;

namespace CelebScraper
{
    [DelimitedRecord("\t")]
    [IgnoreFirst]
    public class Record
    {
        public int CuratedId;
        public string Name;
        public int NumLangs;
        public string City;
        public string State;
        public string Country;
        public string CountryCode;
        public string CountryCode3;
        public string Lat;
        public string Lon;
        public string Continent;
        public int BirthYear;
        public string Gender;
        public string Occupation;
        public string Industry;
        public string Domain;
        public string TotalPageViews;
        public string LStar;
        public string DevPageViews;
        public string EnglishPageViews;
        public string NonEnglishViews;
        public string AverageViews;
        public string Hpi;
    }
}