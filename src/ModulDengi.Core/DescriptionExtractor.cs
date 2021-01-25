namespace ModulDengi.Core
{
    using System;
    using System.Text.RegularExpressions;

    using ModulDengi.Contracts;

    public static class DescriptionExtractor
    {
        private static readonly Regex ProjectIdRegex = new Regex("(проекта|займа) (?<value>\\d{6})");

        private static readonly Regex BorrowerNameRegex = new Regex("Заемщик (?<value>([^\\.])+)\\.");

        private static readonly Regex PercentRegex = new Regex("Процентная ставка (?<value>\\d{1,2})%");

        private static readonly Regex TimePeriodInDaysRegex = new Regex("Срок (?<value>\\d+)");

        public static Description Extract(string input)
        {
            void MatchAndSetUp(Regex regex, Action<string> setUp)
            {
                if (Match(input, regex, out var result))
                {
                    setUp(result);
                }
            }

            void MatchAndSetUpInt(Regex regex, Action<int> setUp)
            {
                if (!Match(input, regex, out var stringResult))
                    return;

                if (!int.TryParse(stringResult, out var intResult))
                {
                    throw new FormatException($"Cannot convert '{stringResult}' into {nameof(Int32)}");
                }

                setUp(intResult);
            }

            var description = new Description();

            MatchAndSetUpInt(ProjectIdRegex, projectId => description.ProjectId = projectId);
            MatchAndSetUp(BorrowerNameRegex, borrowerName => description.BorrowerName = borrowerName);
            MatchAndSetUpInt(PercentRegex, percent => description.Percent = percent);
            MatchAndSetUpInt(
                TimePeriodInDaysRegex,
                timePeriodInDays => description.TimePeriodInDays = timePeriodInDays);

            return description;
        }

        private static bool Match(string input, Regex regex, out string result)
        {
            var match = regex.Match(input);
            if (!match.Success)
            {
                result = null;
                return false;
            }

            result = match.Groups["value"].Value;
            return true;
        }
    }
}