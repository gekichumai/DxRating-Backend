using System.Globalization;
using System.Text.Json;
using DxRating.Common.Enums;
using DxRating.Common.Extensions;

namespace DxRating.Common.Services;

public class I18N
{
    private readonly Dictionary<Language, JsonElement> _translations = [];

    public I18N(Dictionary<Language, string> translations)
    {
        foreach (var (language, json) in translations)
        {
            var element = JsonDocument.Parse(json).RootElement;
            _translations[language] = element;
        }
    }

    public string Get(string key, CultureInfo cultureInfo)
    {
        return Get(key, cultureInfo.ParseLanguage());
    }

    public string Get(string key, Language language)
    {
        var json = _translations[language].Clone();

        if (key.Contains('.'))
        {
            var keys = key.Split('.');

            foreach (var k in keys)
            {
                if (json.TryGetProperty(k, out var element))
                {
                    json = element;
                }
                else
                {
                    return key;
                }
            }

            return json.GetString() ?? key;
        }

        return json.GetProperty(key).GetString() ?? key;
    }
}
