using System;

namespace BookOrganizer2.Domain.Helpers.Extensions
{
    public static class GenericExtensions
    {
        public static bool IsDefault<T>(this T value)
        {
            if (value is null)
            {
                return true;
            }

            var @type = value.GetType();

            return type.IsValueType && value.Equals(Activator.CreateInstance(value.GetType()));
        }

        public static bool HasNonDefaultValue<T>(this T value)
        {
            if (value is null)
            {
                return false;
            }

            var @type = value.GetType();

            return !type.IsValueType || !value.Equals(Activator.CreateInstance(value.GetType()));
        }
    }
}
