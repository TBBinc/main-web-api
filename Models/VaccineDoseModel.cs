namespace SiteWebApi.Models
{
    public class VaccineDoseModel
    {
        public string UserHash {get; set;}
        public string SiteId {get; set;}
        public string Vaccine {get; set;}
        public long timestamp {get; set;}
    }
}