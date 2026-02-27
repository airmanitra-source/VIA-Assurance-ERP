using ClientApp.Components.Shared;
using ClientApp.Controllers;
using ClientApp.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace ClientApp.Components.Pages.Routed
{
    public partial class PayrollDashboardComponent : AuthenticatedComponentBase
    {
        [Inject] protected PayrollController Controller { get; set; } = default!;
        [Inject] protected IJSRuntime JSRuntime { get; set; } = default!;

        protected EntrepriseViewModel? currentCompany;
        protected List<EmployeeViewModel> employees = new();
        protected List<string> errors = new();
        protected HashSet<int> expandedPaySlipIds = new();
        protected string activeTab = "draft";
        protected bool isGenerating = false;
        protected bool isLoading = true;
        protected bool isLoadingSaved = false;
        protected bool isSavingEdits = false;
        protected bool isSubmittingDraft = false;
        protected bool isPaySlipSaved = false;
        protected Dictionary<long, PaySlipModificationRequestViewModel> modificationRequests = new();
        protected DateTime newPeriodEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
        protected DateTime newPeriodStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        protected PaySlipInputViewModel paySlipInput = new();
        protected PaySlipViewModel? paySlipPreview;
        protected List<PayrollPeriodViewModel> periods = new();
        protected List<PaySlipViewModel> savedPaySlips = new();
        protected long? selectedEmployeeId;
        protected int? selectedPeriodId;
        protected string successMessage = string.Empty;

        protected override async Task OnInitializedAuthenticatedAsync()
        {
            await LoadDataAsync();
        }

        protected async Task CreatePeriodAsync()
        {
            if (currentCompany == null) return;

            try
            {
                var periodId = await Controller.Store(currentCompany.Id, newPeriodStart, newPeriodEnd);
                successMessage = "Période de paie créée avec succès";
                await LoadPeriodsAsync();
                selectedPeriodId = periodId;
                await OnPeriodSelectedAsync();
            }
            catch (Exception ex)
            {
                errors.Add($"Erreur: {ex.Message}");
            }
        }

        protected async Task GeneratePreviewAsync()
        {
            if (currentCompany == null || !selectedEmployeeId.HasValue || !selectedPeriodId.HasValue) return;

            isGenerating = true;
            isPaySlipSaved = false;
            try
            {
                paySlipPreview = await Controller.ShowPreviewAsync(
                    selectedEmployeeId.Value,
                    selectedPeriodId.Value,
                    currentCompany.Id,
                    paySlipInput);
            }
            catch (Exception ex)
            {
                errors.Add($"Erreur: {ex.Message}");
            }
            finally
            {
                isGenerating = false;
            }
        }

        protected void OnEmployeeSelected(ChangeEventArgs e)
        {
            if (long.TryParse(e.Value?.ToString(), out var id))
            {
                selectedEmployeeId = id;
                var emp = employees.FirstOrDefault(x => x.EmployeeID == id);
                paySlipInput = new PaySlipInputViewModel
                {
                    EmployeeID = id,
                    EmployeeName = emp != null ? $"{emp.Prenom} {emp.Nom}" : string.Empty
                };
                paySlipPreview = null;
                isPaySlipSaved = false;
            }
        }

        protected Task OnEmployeeSelectedAsync()
        {
            if (!selectedEmployeeId.HasValue)
                return Task.CompletedTask;

            var emp = employees.FirstOrDefault(x => x.EmployeeID == selectedEmployeeId.Value);
            paySlipInput = new PaySlipInputViewModel
            {
                EmployeeID = selectedEmployeeId.Value,
                EmployeeName = emp != null ? $"{emp.Prenom} {emp.Nom}" : string.Empty
            };
            paySlipPreview = null;
            isPaySlipSaved = false;
            return Task.CompletedTask;
        }

        protected async Task SavePaySlipAsync()
        {
            if (currentCompany == null || !selectedEmployeeId.HasValue || !selectedPeriodId.HasValue) return;

            try
            {
                await Controller.StorePaySlipAsync(
                    selectedEmployeeId.Value,
                    selectedPeriodId.Value,
                    currentCompany.Id,
                    paySlipInput);
                successMessage = "Bulletin de paie enregistré avec succès";
                isPaySlipSaved = true;

                if (selectedPeriodId.HasValue)
                {
                    await LoadSavedPaySlipsAsync();
                }
            }
            catch (Exception ex)
            {
                errors.Add($"Erreur: {ex.Message}");
            }
        }

        protected async Task SubmitDraftToEmployeeAsync()
        {
            if (currentCompany == null || !selectedEmployeeId.HasValue || !selectedPeriodId.HasValue || paySlipPreview == null)
                return;

            isSubmittingDraft = true;
            errors.Clear();
            successMessage = string.Empty;

            try
            {
                await Controller.SubmitDraftToEmployeeAsync(
                    selectedEmployeeId.Value,
                    selectedPeriodId.Value,
                    currentCompany.Id);
                successMessage = $"Bulletin envoyé par email à {paySlipPreview.EmployeeName}. L'employé sera notifié de vérifier son portail.";
            }
            catch (Exception ex)
            {
                errors.Add($"Erreur lors de l'envoi: {ex.Message}");
            }
            finally
            {
                isSubmittingDraft = false;
            }
        }

        protected async Task SetActiveTabAsync(string tab)
        {
            activeTab = tab;
            successMessage = string.Empty;
            errors.Clear();

            if (activeTab == "saved" && selectedPeriodId.HasValue)
            {
                await LoadSavedPaySlipsAsync();
            }
        }

        protected Task SetDraftTabAsync()
        {
            return SetActiveTabAsync("draft");
        }

        protected Task SetSavedTabAsync()
        {
            return SetActiveTabAsync("saved");
        }

        protected void TogglePaySlipExpansion(int payrollId)
        {
            if (!expandedPaySlipIds.Add(payrollId))
            {
                expandedPaySlipIds.Remove(payrollId);
            }
        }

        protected bool IsPaySlipExpanded(int payrollId)
        {
            return expandedPaySlipIds.Contains(payrollId);
        }

        protected static bool IsPaySlipLineEditable(PaySlipLineViewModel line)
        {
            return line.LineType == "Gain";
        }

        protected async Task SaveEditedPaySlipAsync(PaySlipViewModel paySlip)
        {
            if (isSavingEdits) return;
            isSavingEdits = true;
            errors.Clear();
            successMessage = string.Empty;

            try
            {
                await Controller.StoreSavedPaySlipAsync(currentCompany.Id, selectedPeriodId.Value, paySlip);
                successMessage = $"Bulletin mis à jour pour {paySlip.EmployeeName}.";
                await LoadSavedPaySlipsAsync();
            }
            catch (Exception ex)
            {
                errors.Add($"Erreur lors de la mise à jour: {ex.Message}");
            }
            finally
            {
                isSavingEdits = false;
            }
        }

        protected async Task DeletePaySlipAsync(PaySlipViewModel paySlip)
        {
            if (!await JSRuntime.InvokeAsync<bool>("confirm", $"Êtes-vous sûr de vouloir supprimer le bulletin de {paySlip.EmployeeName} ?"))
                return;

            try
            {
                await Controller.RemovePaySlipAsync(paySlip.PayrollID);
                successMessage = "Bulletin supprimé avec succès.";
                await LoadSavedPaySlipsAsync();
                
                // Reload available employees for draft tab since one is now available again
                if (activeTab == "saved" && selectedPeriodId.HasValue)
                {
                    await LoadEmployeesForPeriodAsync();
                }
            }
            catch (Exception ex)
            {
                errors.Add($"Erreur lors de la suppression : {ex.Message}");
            }
        }

        protected async Task OnPeriodSelectedAsync()
        {
            paySlipPreview = null;
            isPaySlipSaved = false;

            if (activeTab == "saved" && selectedPeriodId.HasValue)
            {
                await LoadSavedPaySlipsAsync();
                return;
            }

            savedPaySlips.Clear();

            if (activeTab == "draft" && selectedPeriodId.HasValue)
            {
                await LoadEmployeesForPeriodAsync();
            }
        }

        protected Task OpenSavedPaySlipAsync(PaySlipViewModel paySlip)
        {
            selectedEmployeeId = paySlip.EmployeeID;
            selectedPeriodId = paySlip.PeriodID;
            paySlipPreview = paySlip;
            paySlipInput = new PaySlipInputViewModel
            {
                EmployeeID = paySlip.EmployeeID,
                EmployeeName = paySlip.EmployeeName
            };
            isPaySlipSaved = true;
            activeTab = "draft";
            return Task.CompletedTask;
        }

        private async Task LoadDataAsync()
        {
            isLoading = true;
            try
            {
                var company = await GetOrLoadCurrentCompanyAsync();
                if (company != null)
                {
                    currentCompany = EntrepriseViewModel.FromBusinessModel(company);
                    await Task.WhenAll(LoadPeriodsAsync(), LoadEmployeesAsync());
                }
            }
            catch (Exception ex)
            {
                errors.Add($"Erreur: {ex.Message}");
            }
            finally
            {
                isLoading = false;
            }
        }

        private async Task LoadEmployeesAsync()
        {
            if (currentCompany == null) return;
            employees = await Controller.IndexEmployeesAsync(currentCompany.Id);
        }

        private async Task LoadPeriodsAsync()
        {
            if (currentCompany == null) return;
            periods = await Controller.Index(currentCompany.Id);
            
            if (!selectedPeriodId.HasValue && periods.Any())
            {
                var currentPeriod = periods
                    .Where(p => p.Status == "Draft")
                    .OrderByDescending(p => p.PeriodStart)
                    .FirstOrDefault();

                if (currentPeriod != null)
                {
                    selectedPeriodId = currentPeriod.PeriodID;
                    await OnPeriodSelectedAsync();
                }
            }
        }

        private async Task LoadSavedPaySlipsAsync()
        {
            if (currentCompany == null || !selectedPeriodId.HasValue)
                return;

            isLoadingSaved = true;
            try
            {
                savedPaySlips = await Controller.IndexSavedPaySlipsAsync(currentCompany.Id, selectedPeriodId.Value);
                modificationRequests = await Controller.IndexModificationRequestsAsync(selectedPeriodId.Value, currentCompany.Id);
                EnrichPaySlipsWithMissingRequests();
                expandedPaySlipIds.Clear();
            }
            catch (Exception ex)
            {
                errors.Add($"Erreur: {ex.Message}");
            }
            finally
            {
                isLoadingSaved = false;
            }
        }

        private async Task LoadEmployeesForPeriodAsync()
        {
            if (currentCompany == null || !selectedPeriodId.HasValue)
                return;

            try
            {
                employees = await Controller.IndexEmployeesWithoutPaySlipAsync(currentCompany.Id, selectedPeriodId.Value);
            }
            catch (Exception ex)
            {
                errors.Add($"Erreur: {ex.Message}");
            }
        }

        private void EnrichPaySlipsWithMissingRequests()
        {
            if (savedPaySlips == null || modificationRequests == null) return;

            foreach (var paySlip in savedPaySlips)
            {
                if (modificationRequests.TryGetValue(paySlip.EmployeeID, out var modifReq))
                {
                    var extraRubriques = new[] 
                    { 
                        ("15000", "Heures supplémentaires"),
                        ("17000", "Bonus"),
                        ("17100", "Prime de scolarité"),
                        ("17200", "13ème mois"),
                        ("19400", "Indemnité de déplacement"),
                        ("19500", "Indemnité de logement")
                    };

                    foreach (var (rub, label) in extraRubriques)
                    {
                        var reqVal = GetRequestedValueForRubrique(modifReq, rub);
                        // Show if employee requested a value (only positive values) and it's missing from the slip
                        if (reqVal.HasValue && reqVal.Value > 0 && !paySlip.Lines.Any(l => l.Rubrique == rub))
                        {
                            paySlip.Lines.Add(new PaySlipLineViewModel
                            {
                                Rubrique = rub,
                                Libelle = label,
                                LineType = "Gain",
                                GainAmount = 0,
                                Nombre = 0,
                                Base = 0,
                                Taux = 0,
                                IsRequestedMissingLine = true
                            });
                        }
                    }

                    // Sort everything to ensure logical grouping (Indemnités together, Gains at top, etc.)
                    var sortedLines = paySlip.Lines
                        .OrderBy(l => l.LineType == "Gain" ? 0 : (l.LineType == "Impot" ? 2 : 1))
                        .ThenBy(l => int.TryParse(l.Rubrique, out var r) ? r : 99999)
                        .ToList();

                    // Re-assign SortOrder so the Razor's .OrderBy(l => l.SortOrder) reflects this update
                    for (int i = 0; i < sortedLines.Count; i++)
                    {
                        sortedLines[i].SortOrder = i + 1;
                    }
                    paySlip.Lines = sortedLines;
                }
            }
        }

        protected decimal? GetCurrentValueFromPaySlip(PaySlipViewModel paySlip, string rubrique)
        {
            var line = paySlip.Lines.FirstOrDefault(l => l.Rubrique == rubrique);
            return line?.GainAmount;
        }

        protected decimal? GetCurrentOvertimeFromPaySlip(PaySlipViewModel paySlip)
        {
            var line = paySlip.Lines.FirstOrDefault(l => l.Rubrique == "15000");
            return line?.Nombre;
        }

        protected bool HasValueChanged(decimal? currentValue, decimal? requestedValue)
        {
            if (!requestedValue.HasValue) return false;
            if (!currentValue.HasValue) return true;
            return currentValue.Value != requestedValue.Value;
        }

        protected decimal? GetRequestedValueForRubrique(PaySlipModificationRequestViewModel? req, string rubrique)
        {
            if (req == null) return null;
            return rubrique switch
            {
                "15000" => req.OvertimeHours,
                "17000" => req.Bonus,
                "17100" => req.PrimeScolarite,
                "17200" => req.TreiziemeMois,
                "19400" => req.IndemniteTransport,
                "19500" => req.IndemniteLogement,
                _ => null
            };
        }

        protected bool HasModificationsToDisplay(PaySlipViewModel paySlip, PaySlipModificationRequestViewModel? req)
        {
            if (req == null) return false;
            
            if (req.Bonus.HasValue && HasValueChanged(GetCurrentValueFromPaySlip(paySlip, "17000"), req.Bonus)) return true;
            if (req.PrimeScolarite.HasValue && HasValueChanged(GetCurrentValueFromPaySlip(paySlip, "17100"), req.PrimeScolarite)) return true;
            if (req.TreiziemeMois.HasValue && HasValueChanged(GetCurrentValueFromPaySlip(paySlip, "17200"), req.TreiziemeMois)) return true;
            if (req.IndemniteTransport.HasValue && HasValueChanged(GetCurrentValueFromPaySlip(paySlip, "19400"), req.IndemniteTransport)) return true;
            if (req.IndemniteLogement.HasValue && HasValueChanged(GetCurrentValueFromPaySlip(paySlip, "19500"), req.IndemniteLogement)) return true;
            if (req.OvertimeHours.HasValue && HasValueChanged(GetCurrentOvertimeFromPaySlip(paySlip), req.OvertimeHours)) return true;
            
            return false;
        }

        protected decimal CalculateProjectedNetAPayer(PaySlipViewModel paySlip, PaySlipModificationRequestViewModel? req)
        {
            if (req == null || !HasModificationsToDisplay(paySlip, req)) return paySlip.NetAPayer;

            decimal gainDiff = 0;
            decimal taxableDiff = 0;
            
            // Taxable items (usually Bonus, TreiziemeMois, Overtime)
            if (req.Bonus.HasValue) taxableDiff += req.Bonus.Value - (GetCurrentValueFromPaySlip(paySlip, "17000") ?? 0);
            if (req.TreiziemeMois.HasValue) taxableDiff += req.TreiziemeMois.Value - (GetCurrentValueFromPaySlip(paySlip, "17200") ?? 0);
            
            var overtimeLine = paySlip.Lines.FirstOrDefault(l => l.Rubrique == "15000");
            if (overtimeLine != null && req.OvertimeHours.HasValue)
            {
                decimal hourlyRate = overtimeLine.Taux ?? 0;
                taxableDiff += (req.OvertimeHours.Value - (overtimeLine.Nombre ?? 0)) * hourlyRate;
            }

            // High probability of exemption items (Indemnités, Prime Scolarité)
            if (req.PrimeScolarite.HasValue) gainDiff += req.PrimeScolarite.Value - (GetCurrentValueFromPaySlip(paySlip, "17100") ?? 0);
            if (req.IndemniteTransport.HasValue) gainDiff += req.IndemniteTransport.Value - (GetCurrentValueFromPaySlip(paySlip, "19400") ?? 0);
            if (req.IndemniteLogement.HasValue) gainDiff += req.IndemniteLogement.Value - (GetCurrentValueFromPaySlip(paySlip, "19500") ?? 0);
            
            // Estimation: Taxable items get a 22% haircut (Social (CNAPS 1% + OSIE 1%) + IRSA ~20%)
            // This is an estimation for summary purposes.
            return paySlip.NetAPayer + gainDiff + (taxableDiff * 0.78m);
        }
    }
}
