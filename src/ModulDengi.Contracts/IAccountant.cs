namespace ModulDengi.Contracts
{
    public interface IAccountant
    {
        bool IsPossibleToInvest(in double availableMoney, Project project, out double investingMoney, out string message);
    }
}