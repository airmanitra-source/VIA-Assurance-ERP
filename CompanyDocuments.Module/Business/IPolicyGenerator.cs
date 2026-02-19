namespace CompanyDocuments.Module.Business
{
    public interface IPolicyGenerator
    {
        byte[] GeneratePolicyPdf(PolicyPdfModel model);
    }
}
