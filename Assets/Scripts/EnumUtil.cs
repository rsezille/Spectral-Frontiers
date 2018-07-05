using System;

public class EnumUtil {
    public static T ParseEnum<T>(string value, T defaultValue) where T : struct {
        if (string.IsNullOrEmpty(value)) {
            return defaultValue;
        }

        T result;

        return Enum.TryParse<T>(value, true, out result) ? result : defaultValue;
    }
}
