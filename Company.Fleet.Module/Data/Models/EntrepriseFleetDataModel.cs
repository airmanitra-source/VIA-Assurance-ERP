namespace Company.Fleet.Module.Data.Models
{
    public class EntrepriseFleetDataModel
    {
        public long EntrepriseId { get; set; }
        public int? FiscalPower { get; set; }
        public long Id { get; set; }
        public DateTime? InsuranceEndDate { get; set; }
        public DateTime? InsuranceStartDate { get; set; }
        public bool IsInsured { get; set; }
        public bool IsWorking { get; set; }
        public string Make { get; set; } = string.Empty;
        public int Mileage { get; set; }
        public string Model { get; set; } = string.Empty;
        public string? PolicyNumber { get; set; }
        public int? Power { get; set; }
        public string Type { get; set; } = string.Empty; // 'Auto' or 'Moto'
        public bool WantsInsurance { get; set; }
        public int Year { get; set; }
    }
}
