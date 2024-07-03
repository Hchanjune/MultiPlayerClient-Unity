using System;

namespace Utils.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrBlank(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }
        
        
    }
}

