using IronPdf;
using IronPdf.Editing;

namespace CompanyDocuments.Module.Business
{
    public class SignatureService : ISignatureService
    {
        public async Task<byte[]> SignPdfAsync(byte[] pdfBytes, string signerName, string? signatureImageBase64 = null)
        {
            // Load PDF from bytes
            using var pdf = new PdfDocument(pdfBytes);

            string signatureContent;
            if (!string.IsNullOrEmpty(signatureImageBase64))
            {
                signatureContent = $@"
                    <div style='text-align: center;'>
                        <img src='{signatureImageBase64}' style='max-width: 200px; max-height: 100px;' />
                        <p style='margin: 0; font-size: 12px;'><b>Signed by:</b> {signerName}</p>
                        <p style='margin: 0; font-size: 10px; color: #666;'>{DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>
                    </div>";
            }
            else
            {
                signatureContent = $@"
                    <h4 style='color: #0051ba; margin: 0 0 5px 0;'>ELECTRONIC SIGNATURE</h4>
                    <p style='margin: 0; font-size: 14px;'><b>Signed by:</b> {signerName}</p>
                    <p style='margin: 0; font-size: 12px;'><b>Date:</b> {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>
                    <p style='margin: 5px 0 0 0; font-size: 10px; color: #666;'>Verified by Ny Havana Dashboard</p>";
            }

            // Create a simple signature stamp
            var stamper = new HtmlStamper($@"
                <div style='border: 2px solid #0051ba; padding: 10px; width: 250px; background-color: #f8f9fa; font-family: Arial, sans-serif;'>
                    {signatureContent}
                </div>");

            // Apply stamp to the last page (bottom right)
            stamper.VerticalAlignment = VerticalAlignment.Bottom;
            stamper.HorizontalAlignment = HorizontalAlignment.Right;
            stamper.VerticalOffset = new Length(-20, MeasurementUnit.Percentage); // Adjust as needed
            stamper.HorizontalOffset = new Length(-5, MeasurementUnit.Percentage);

            pdf.ApplyStamp(stamper);

            // Return signed PDF as bytes
            return pdf.BinaryData;
        }
    }
}
