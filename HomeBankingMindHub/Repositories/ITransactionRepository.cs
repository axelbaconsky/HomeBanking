using HomeBankingMindHub.Models;
using System.Collections.Generic;

namespace HomeBanking.Repositories
{
    public interface ITransactionRepository
    {
        void Save(Transaction transaction);
        Transaction FindByNumber(long id);
    }
}