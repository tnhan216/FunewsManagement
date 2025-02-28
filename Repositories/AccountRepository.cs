using BusinessObjects;
using DataAccessObjects;
using System.Collections.Generic;

namespace Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly AccountDAO _dao;

        public AccountRepository(FunewsManagementContext context)
        {
            _dao = new AccountDAO(context);
        }

        public List<SystemAccount> GetAllAccounts() => _dao.GetAllAccounts();

        public SystemAccount GetAccountById(short? id) => _dao.GetAccountById(id);

        public SystemAccount GetAccountByEmail(string email) => _dao.GetAccountByEmail(email);

        public void AddAccount(SystemAccount account) => _dao.AddAccount(account);

        public void UpdateAccount(SystemAccount account) => _dao.UpdateAccount(account);

        public void DeleteAccount(SystemAccount account) => _dao.DeleteAccount(account);
        public List<SystemAccount> SearchAccount(string query) =>  _dao.SearchAccount(query);
        public List<SystemAccount> SortAccounts(string sortBy) => _dao.SortAccounts(sortBy);
    }
}
