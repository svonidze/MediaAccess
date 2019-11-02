using System.Linq;
using System.Text;

namespace Common.Text
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder AppendIf(
            this StringBuilder builder, 
            bool conditional, 
            string valueIfConditionalIsTrue, 
            string valueIfConditionalIsFalse = null)
        {
            if (conditional)
            {
                builder.Append(valueIfConditionalIsTrue);
            }
            else if (valueIfConditionalIsFalse != null)
            {
                builder.Append(valueIfConditionalIsFalse);
            }

            return builder;
        }

        public static StringBuilder AppendLineIf(
            this StringBuilder builder,
            bool conditional,
            string valueIfConditionalIsTrue,
            string valueIfConditionalIsFalse = null)
        {
            if (conditional)
            {
                builder.AppendLine(valueIfConditionalIsTrue);
            }
            else if (valueIfConditionalIsFalse != null)
            {
                builder.AppendLine(valueIfConditionalIsFalse);
            }

            return builder;
        }

        public static StringBuilder AppendIfValueIsNotEmpty(this StringBuilder builder, string value)
        {
            return value.IsNullOrEmpty()
                       ? builder
                       : builder.Append(value);
        }

        public static StringBuilder AppendFormatNotEmptyParameters(
            this StringBuilder builder, 
            string format, 
            params object[] parameters)
        {
            if (parameters == null || !parameters.Any() || parameters.Any(IsValid) == false)
            {
                return builder;
            }

            return builder.AppendFormat(format, parameters);
        }

        public static StringBuilder Clone(this StringBuilder builder)
        {
            return new StringBuilder(builder.ToString());
        }

        private static bool IsValid(object parameter)
        {
            switch (parameter)
            {
                case null:
                    return false;
                case string s:
                    return !s.IsNullOrEmpty();
                default:
                    return true;
            }
        }
    }
}