using System.Collections.Generic;
using System.Linq;

namespace Mifs.Extensions
{
    public static class String_Extensions
    {
        public static bool IsNullOrWhiteSpace(this string? value)
            => string.IsNullOrWhiteSpace(value);

        public static string Join(this string value, string separator, params string[] newValues)
        {
            if (value is null)
            {
                return string.Join(separator, newValues);
            }

            if (newValues?.Any() != true)
            {
                return value;
            }

            var joinValues = new List<string>(newValues.Length + 1) { value };
            foreach (var newValue in newValues)
            {
                if (newValue.IsNullOrWhiteSpace())
                {
                    continue;
                }

                joinValues.Add(newValue);
            }

            return string.Join(separator, joinValues);
        }
    }
}
