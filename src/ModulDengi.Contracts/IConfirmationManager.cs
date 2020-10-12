namespace ModulDengi.Contracts
{
    using System;

    public interface IConfirmationManager
    {
        bool CheckConfirmationReceived(DateTime checkSinceUtc, string projectNumber, out string confirmationCode);
    }
}