using System.ComponentModel;
using ClientApp.Controllers;
using ClientApp.Models;
using ModelContextProtocol.Server;

namespace ClientApp
{
    [McpServerToolType]
    public class McpTools
    {
        private readonly EmployeeController _employeeController;

        public McpTools(EmployeeController employeeController)
        {
            _employeeController = employeeController;
        }

        [McpServerTool, Description("Ajoute un nouvel employé dans le système")]
        public async Task<string> AddEmployeeAsync(
            [Description("Nom de l'employé")] string nom,
            [Description("Prénom de l'employé")] string prenom,
            [Description("Numéro de matricule")] string matricule,
            [Description("Sexe (Homme (M) ou Femme (F))")] string sexe,
            [Description("Age de l'employé")] int age,
           
            [Description("Poste occupé")] string? poste = null)
        {
            var vm = new EmployeeViewModel
            {
                Age = age,
                Nom = nom,
                Prenom = prenom,
                NumeroMatricule = matricule,
                Sexe = sexe,
                NomPoste = poste,
                StatutEmploye = "CDI",
                EntrepriseId = 3
            };

            var res = await _employeeController.Store(vm, null, 3, new(), new());

            if (res.Success)
            {
                return res.Message ?? "Employé ajouté avec succès.";
            }
            else
            {
                return "Erreur lors de l'ajout : " + string.Join(" | ", res.Errors);
            }
        }
    }
}
