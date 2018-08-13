using System.Collections.Generic;

public static class LanguageList {
    public static Language French = new Language("fr", "Français");
    public static Language English = new Language("en", "English");
    public static Language Japanese = new Language("jp", "日本語");
    public static Language German = new Language("de", "Deutsch");

    public static Dictionary<string, Language> allLanguages = new Dictionary<string, Language> {
        { English.isoCode, English },
        { French.isoCode, French },
        { Japanese.isoCode, Japanese },
        { German.isoCode, German }
    };

    public static List<string> availableLanguages = new List<string> {
        English.isoCode,
        French.isoCode
    };
}
