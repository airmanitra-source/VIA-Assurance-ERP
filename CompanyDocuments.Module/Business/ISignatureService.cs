namespace CompanyDocuments.Module.Business
{
    public interface ISignatureService
    {
        Task<byte[]> SignPdfAsync(byte[] pdfBytes, string signerName, string? signatureImageBase64 = null);
    }
}
