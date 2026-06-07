using Moq;
using ERP.Analyzers;
using Xunit;

namespace ERP.Analyzers.Tests
{
    public class NamingHelperTests
    {
        private readonly NamingHelper _helper;

        public NamingHelperTests()
        {
            _helper = new NamingHelper();
        }

        [Theory]
        [InlineData(Constants.BusinessModelDiagnosticId, Constants.BusinessModelTitle)]
        [InlineData(Constants.DataModelDiagnosticId, Constants.DataModelTitle)]
        [InlineData(Constants.DataProviderDiagnosticId, Constants.DataProviderTitle)]
        [InlineData("UNEXPECTED_ID", Constants.BusinessModelTitle)]
        public void GetTitle_ReturnsCorrectTitle(string diagnosticId, string expectedTitle)
        {
            var result = _helper.GetTitle(diagnosticId);
            Assert.Equal(expectedTitle, result);
        }

        [Theory]
        [InlineData(Constants.BusinessModelDiagnosticId, Constants.BusinessModelDescription)]
        [InlineData(Constants.DataModelDiagnosticId, Constants.DataModelDescription)]
        [InlineData(Constants.DataProviderDiagnosticId, Constants.DataProviderDescription)]
        [InlineData("UNEXPECTED_ID", Constants.BusinessModelDescription)]
        public void GetDescription_ReturnsCorrectDescription(string diagnosticId, string expectedDescription)
        {
            var result = _helper.GetDescription(diagnosticId);
            Assert.Equal(expectedDescription, result);
        }

        [Theory]
        [InlineData(Constants.BusinessModelDiagnosticId, Constants.BusinessModelCodeFixTitle)]
        [InlineData(Constants.DataModelDiagnosticId, Constants.DataModelCodeFixTitle)]
        [InlineData(Constants.DataProviderDiagnosticId, Constants.DataProviderCodeFixTitle)]
        [InlineData("UNEXPECTED_ID", Constants.BusinessModelCodeFixTitle)]
        public void GetCodeFixTitle_ReturnsCorrectCodeFixTitle(string diagnosticId, string expectedCodeFixTitle)
        {
            var result = _helper.GetCodeFixTitle(diagnosticId);
            Assert.Equal(expectedCodeFixTitle, result);
        }

        [Theory]
        [InlineData(@"Module\Business\Models\Test.cs", Constants.BusinessModelSuffix)]
        [InlineData(@"Module\Data\Models\Test.cs", Constants.DataModelSuffix)]
        [InlineData(@"Module\Data\Providers\Test.cs", Constants.DataProviderSuffix)]
        [InlineData(@"Module/Data/Models/Test.cs", Constants.DataModelSuffix)]
        [InlineData(@"Module/Data/Providers/Test.cs", Constants.DataProviderSuffix)]
        [InlineData(@"SomeOtherFolder\Test.cs", Constants.BusinessModelSuffix)]
        [InlineData(null, Constants.BusinessModelSuffix)]
        [InlineData("", Constants.BusinessModelSuffix)]
        public void GetSuffixForDocumentFilePath_ReturnsCorrectSuffix(string filePath, string expectedSuffix)
        {
            var result = _helper.GetSuffixForDocumentFilePath(filePath);
            Assert.Equal(expectedSuffix, result);
        }
    }
}
