using Company.Fleet.Module.Business;

namespace ClientApp.Models
{
    public class EntrepriseFleetViewModel
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

        internal static List<EntrepriseFleetViewModel>? FromBusinessModel(List<EntrepriseFleetBusinessModel> entrepriseFleetBusinessModels)
        {
           return entrepriseFleetBusinessModels.Select(x => new EntrepriseFleetViewModel
           {
               Id = x.Id,
               EntrepriseId = x.EntrepriseId,
               Type = x.Type,
               Year = x.Year,
               IsWorking = x.IsWorking,
               Mileage = x.Mileage,
               Make = x.Make,
               Model = x.Model,
               WantsInsurance = x.WantsInsurance,
               IsInsured = x.IsInsured
           }).ToList();
        }
    }
}