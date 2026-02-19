namespace FileTable.Infrastructure.FileTableDb.Entities
{
    public class EntrepriseFleetEntity
    {
        public long Id { get; set; }

        public long EntrepriseId { get; set; }

        public bool IsInsured { get; set; }

        public bool IsWorking { get; set; }

        public string Make { get; set; } = string.Empty;

        public int Mileage { get; set; }

        public string Model { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public bool WantsInsurance { get; set; }

        public int Year { get; set; }
    }
}
