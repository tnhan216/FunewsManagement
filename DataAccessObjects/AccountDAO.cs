using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;
using Microsoft.EntityFrameworkCore;
namespace DataAccessObjects
{
    public class AccountDAO
    {
        private readonly FunewsManagementContext _context;

        public AccountDAO(FunewsManagementContext context)
        {
            _context = context;
        }

        public List<SystemAccount> GetAllAccounts()
        {
            return _context.SystemAccounts.ToList();
        }

        public SystemAccount GetAccountById(short? id)
        {
            return _context.SystemAccounts.FirstOrDefault(a => a.AccountId == id);
        }

        public SystemAccount GetAccountByEmail(string email)
        {
            return _context.SystemAccounts.FirstOrDefault(a => a.AccountEmail.Equals(email));
        }

        public void AddAccount(SystemAccount account)
        {
            _context.SystemAccounts.Add(account);
            _context.SaveChanges();
        }

        public void UpdateAccount(SystemAccount account)
        {
            _context.SystemAccounts.Update(account);
            _context.SaveChanges();
        }

        public void DeleteAccount(SystemAccount account)
        {
            if (account != null)
            {
                _context.SystemAccounts.Remove(account);
                _context.SaveChanges();
            }
        }
        public List<SystemAccount> SearchAccount(string? query)
        {
            var accounts = _context.SystemAccounts.AsQueryable();

            if (!string.IsNullOrEmpty(query))
            {
                accounts = accounts.Where(a => a.AccountName.Contains(query) ||
                                               a.AccountEmail.Contains(query) ||
                                               a.AccountRole.ToString().Contains(query));
            }
            return  accounts.ToList(); 
        }
        public List<SystemAccount> SortAccounts(string sortBy)
        {
            var accounts = _context.SystemAccounts.AsQueryable(); 
            switch (sortBy?.ToLower())
            {
                case "name":
                    accounts = accounts.OrderBy(a => a.AccountName);
                    break;
                case "email":
                    accounts = accounts.OrderBy(a => a.AccountEmail);
                    break;
                case "role":
                    accounts = accounts.OrderBy(a => a.AccountRole);
                    break;
            };
            return accounts.ToList();
        }

    }
}
