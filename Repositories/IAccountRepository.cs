using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessObjects;
namespace Repositories
{
    public interface IAccountRepository
    {
        List<SystemAccount> GetAllAccounts();
        SystemAccount GetAccountById(short? id);
        SystemAccount GetAccountByEmail(string email);
        void AddAccount(SystemAccount account);
        void UpdateAccount(SystemAccount account);
        void DeleteAccount(SystemAccount account);
        List<SystemAccount> SearchAccount(string query);
        List<SystemAccount> SortAccounts(string sortBy);
    }
}
