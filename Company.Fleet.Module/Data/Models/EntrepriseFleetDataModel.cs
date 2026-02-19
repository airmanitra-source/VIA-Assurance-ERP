namespace Company.Fleet.Module.Data.Models
{
    public class EntrepriseFleetDataModel
    {
        public long Id { get; set; }
        public long EntrepriseId { get; set; }
        public string Type { get; set; } = string.Empty; // 'Auto' or 'Moto'
        public int Year { get; set; }
        public bool IsWorking { get; set; }
        public int Mileage { get; set; }
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public bool WantsInsurance { get; set; }
        public bool IsInsured { get; set; }
        public string? PolicyNumber { get; set; }
    }
}
