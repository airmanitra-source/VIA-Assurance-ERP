namespace ClientApp.Models
{
    public class PaySlipLineViewModel
    {
        public decimal? Base { get; set; }
        public decimal EmployeeDeduction { get; set; }
        public decimal EmployerContribution { get; set; }
        public decimal GainAmount { get; set; }
        public string Libelle { get; set; } = string.Empty;
        public string LineType { get; set; } = string.Empty;
        public decimal? Nombre { get; set; }
        public string Rubrique { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public decimal? Taux { get; set; }
    }
}
