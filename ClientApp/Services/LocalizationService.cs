using Microsoft.Extensions.Localization;
using System.Globalization;

namespace ClientApp.Services
{
    public class LocalizationService
    {
        private readonly IStringLocalizer<SharedResource> _stringLocalizer;
        private string _currentCulture = "fr-FR";

        public LocalizationService(IStringLocalizer<SharedResource> stringLocalizer)
        {
            _stringLocalizer = stringLocalizer;
        }

        public string CurrentCulture => _currentCulture;

        public void SetCulture(string culture)
        {
            _currentCulture = culture;
            CultureInfo.CurrentCulture = new CultureInfo(culture);
            CultureInfo.CurrentUICulture = new CultureInfo(culture);
        }

        public string GetLocalizedString(string key)
        {
            return _stringLocalizer[key].Value;
        }

        public IEnumerable<(string Code, string Name)> GetSupportedCultures()
        {
            return new[]
            {
                ("en-US", "English"),
                ("fr-FR", "Français")
            };
        }
    }
}
