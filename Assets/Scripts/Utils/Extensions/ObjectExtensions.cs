namespace Utils.Extensions
{
    using System;
    using System.Reflection;
    using System.Text;

    public static class ObjectExtensions
    {
        public static string ToStringReflection(this object obj)
        {
            if (obj == null) return "null";

            Type type = obj.GetType();
            StringBuilder sb = new StringBuilder();
            sb.Append($"{type.Name}(");

            bool first = true;

            foreach (PropertyInfo property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (ShouldIgnoreMember(property.Name))
                {
                    continue;
                }

                try
                {
                    var value = property.GetValue(obj, null);
                    if (!first) sb.Append(", ");
                    sb.Append($"{property.Name}({property.PropertyType.Name}): {FormatValue(value)}");
                    first = false;
                }
                catch (Exception)
                {
                    // 무시: Colyseus의 내부 특성에 의해 발생하는 예외
                }
            }

            foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                if (ShouldIgnoreMember(field.Name))
                {
                    continue;
                }

                try
                {
                    var value = field.GetValue(obj);
                    if (!first) sb.Append(", ");
                    sb.Append($"{field.Name}({field.FieldType.Name}): {FormatValue(value)}");
                    first = false;
                }
                catch (Exception)
                {
                    // 무시: Colyseus의 내부 특성에 의해 발생하는 예외
                }
            }

            sb.Append(")");
            return sb.ToString();
        }

        private static bool ShouldIgnoreMember(string memberName)
        {
            // 내부 Colyseus 필드를 무시
            return memberName.StartsWith("__") || memberName == "$changes" || memberName == "$callbacks";
        }

        private static string FormatValue(object value)
        {
            if (value is string)
            {
                return $"\"{value}\"";
            }
            return value.ToString();
        }
    }
}