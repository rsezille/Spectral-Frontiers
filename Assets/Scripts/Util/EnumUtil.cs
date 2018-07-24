using System;

namespace SF {
    public class EnumUtil {
        /**
         * Retrieve the T type of the given string
         */
        public static T ParseEnum<T>(string value, T defaultValue) where T : struct {
            if (string.IsNullOrEmpty(value)) {
                return defaultValue;
            }

            T result;

            return Enum.TryParse<T>(value, true, out result) ? result : defaultValue;
        }
    }
}
