namespace ModulDengi.Core
{
    using System;
    using System.Linq;

    using Common.Serialization.Json;

    using ModulDengi.Contracts;

    public class Accountant : IAccountant
    {
        private const int MinAmount = 500;

        public bool IsPossibleToInvest(
            in double availableMoney,
            Project project,
            out double investingMoney,
            out string message)
        {
            if (availableMoney < MinAmount)
            {
                message = $"{nameof(availableMoney)} {availableMoney} is less than {nameof(MinAmount)} {MinAmount}";

                investingMoney = 0;
                return false;
            }

            var nameParts = project.BorrowerShortName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (!nameParts.Any())
                throw new Exception($"{new { project.BorrowerShortName }.ToJson()} cannot be split into parts");

            investingMoney = nameParts.First() switch
                {
                    "ИП" => MinAmount * 2,
                    "ООО" => MinAmount * 1.2,
                    _ => MinAmount
                };

            message = null;
            return true;
        }
    }
}