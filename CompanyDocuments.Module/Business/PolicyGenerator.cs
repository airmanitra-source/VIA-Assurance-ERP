using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using CompanyDocuments.Module.Business;

namespace CompanyDocuments.Module.Business
{
    public class PolicyGenerator : IPolicyGenerator
    {
        public byte[] GeneratePolicyPdf(PolicyConfirmationModel model)
        {
            // QuestPDF requirement: License configuration (Community is free for small/OSS)
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Arial));

                    page.Header().Column(header =>
                    {
                        header.Item().Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text("ERP ASSUR").FontSize(20).SemiBold().FontColor("#0051ba");
                                col.Item().Text("Votre partenaire confiance").FontSize(10).Italic();
                            });
                        });

                        header.Item().PaddingTop(10).AlignCenter().Text(model.Title.ToUpper()).FontSize(16).SemiBold();
                    });

                    page.Content().PaddingVertical(10).Column(col =>
                    {
                        col.Item().PaddingBottom(10).Text("Important : Ce document n'est pas le contrat d'assurance. Il est émis à titre d'information, sous réserve de toutes les dispositions, exclusions et conditions du contrat.").FontSize(8).Italic();

                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text(t => { t.Span("Police d'assurance numéro : ").SemiBold(); t.Span(model.PolicyNumber ?? "N/A"); });
                                c.Item().Text(t => { t.Span("En vigueur du : ").SemiBold(); t.Span($"{model.StartDate:yyyy-MM-dd} au {model.EndDate:yyyy-MM-dd}"); });
                            });
                        });

                        col.Item().PaddingVertical(10).Row(row =>
                        {
                            row.RelativeItem().Border(0).Column(c =>
                            {
                                c.Item().Text("Assuré désigné").SemiBold();
                                c.Item().Text(model.InsuredName ?? "N/A");
                            });

                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("Adresse").SemiBold();
                                c.Item().Text(model.Address ?? "N/A");
                            });
                        });

                        col.Item().PaddingVertical(5).Text("Informations sur l'objet assuré").SemiBold().Underline();
                        col.Item().Row(row =>
                        {
                            row.ConstantItem(150).Text("Description :");
                            row.RelativeItem().Text(model.VehicleDescription ?? "N/A");
                        });
                        
                        if (!string.IsNullOrEmpty(model.LicensePlate) && model.LicensePlate != "Non renseignée")
                        {
                            col.Item().Row(row =>
                            {
                                row.ConstantItem(150).Text("Plaque d'immatriculation :");
                                row.RelativeItem().Text(model.LicensePlate);
                            });
                        }
                        
                        if (!string.IsNullOrEmpty(model.VIN) && model.VIN != "N/A" && model.VIN != "Non renseigné")
                        {
                            col.Item().Row(row =>
                            {
                                row.ConstantItem(150).Text("Numéro VIN :");
                                row.RelativeItem().Text(model.VIN);
                            });
                        }

                        if (model.Coverages != null && model.Coverages.Any())
                        {
                            RenderCoverageTable(col, "Garanties applicables", model.Coverages);
                        }

                        if (model.VehicleCoverages != null && model.VehicleCoverages.Any())
                        {
                            RenderCoverageTable(col, "Garanties applicables au véhicule", model.VehicleCoverages);
                        }

                        if (model.PolicyCoverages != null && model.PolicyCoverages.Any())
                        {
                            RenderCoverageTable(col, "Garanties applicables à la police", model.PolicyCoverages);
                        }
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Page ");
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                });
            });

            var bytes = document.GeneratePdf();
            if (bytes == null || bytes.Length == 0)
            {
                throw new Exception("QuestPDF generated an empty document.");
            }
            return bytes;
        }

        private void RenderCoverageTable(ColumnDescriptor column, string title, List<CoverageModel> coverages)
        {
            column.Item().PaddingTop(15).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn();
                    columns.ConstantColumn(120);
                    columns.ConstantColumn(120);
                });

                table.Header(header =>
                {
                    header.Cell().Background("#0051ba").Padding(5).Text(title).SemiBold().FontColor(Colors.White);
                    header.Cell().Background("#0051ba").Padding(5).AlignRight().Text("Franchise (Ar)").SemiBold().FontColor(Colors.White);
                    header.Cell().Background("#0051ba").Padding(5).AlignRight().Text("Limite (Ar)").SemiBold().FontColor(Colors.White);
                });

                foreach (var item in coverages)
                {
                    table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(item.Description);
                    var deductibleText = !string.IsNullOrWhiteSpace(item.DeductibleDisplay)
                        ? item.DeductibleDisplay
                        : (item.Deductible > 0 ? $"{item.Deductible:N0} Ar" : "-");
                    table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignRight()
                        .Text(deductibleText);
                    table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignRight()
                        .Text(item.Amount > 0 ? $"{item.Amount:N0} Ar" : "Illimitée");
                }
            });
        }
    }
}
