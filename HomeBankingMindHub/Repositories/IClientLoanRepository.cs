using HomeBankingMindHub.Models;

namespace HomeBanking.Repositories
{
    public interface IClientLoanRepository
    {
        void Save(ClientLoan clientLoan);
    }
}