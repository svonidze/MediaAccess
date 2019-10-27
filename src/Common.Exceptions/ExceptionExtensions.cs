namespace Common.Exceptions
{
    using System;
    using System.Linq;
    using System.Text;

    public static class ExceptionExtensions
    {
        private enum ExceptionDescriptionFullnessType
        {
            Full, 

            Short
        }

        public static string GetFullDescription(this Exception exception, string customMessage = null)
        {
            return GetDescription(exception, customMessage, ExceptionDescriptionFullnessType.Full);
        }

        public static string GetShortDescription(this Exception exception, string customMessage = null)
        {
            return GetDescription(exception, customMessage, ExceptionDescriptionFullnessType.Short);
        }

        private static string GetDescription(Exception exception, string customMessage, ExceptionDescriptionFullnessType fullnessType)
        {
            var descriptionBuilder = new StringBuilder();
            if (!string.IsNullOrEmpty(customMessage))
            {
                descriptionBuilder.AppendLine(customMessage);
            }

            CollectDescription(descriptionBuilder, exception, fullnessType);

            return descriptionBuilder.ToString();
        }

        private static void CollectDescription(
            StringBuilder descriptionBuilder, 
            Exception exception, 
            ExceptionDescriptionFullnessType fullnessType)
        {
            descriptionBuilder.Append($"[{exception.GetType().Name}]");
            switch (fullnessType)
            {
                case ExceptionDescriptionFullnessType.Full:
                    descriptionBuilder.AppendLine(exception.Message).Append(exception.StackTrace);
                    break;
                case ExceptionDescriptionFullnessType.Short:
                    descriptionBuilder.Append(
                        exception.Message.Split(new[] { Environment.NewLine },
                            StringSplitOptions.RemoveEmptyEntries).First());
                    break;
            }

            if (exception.InnerException != null)
            {
                descriptionBuilder.AppendLine();
                CollectDescription(descriptionBuilder, exception.InnerException, fullnessType);
            }
        }
    }
}