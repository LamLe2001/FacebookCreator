using FacebookCreator.Entities;
using Microsoft.EntityFrameworkCore;
using Serilog;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace FacebookCreator.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public AccountRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Add(Account account)
        {
            try
            {
                account.CreateDate = DateTime.Now;
                _dbContext.Accounts.Add(account);
                _dbContext.SaveChanges();
            }catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
            }
           
        }

        public void Delete(Guid id)
        {
            try
            {
                var delete = _dbContext.Accounts.FirstOrDefault(x => x.Id == id);
                if (delete != null)
                {
                    _dbContext.Accounts.Remove(delete);
                    _dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
            }
          
        }

        public bool DeleteRange(List<string> deleteList)
        {
            try
            {
                foreach (var id in deleteList)
                {
                    var idAccount = new Guid(id);
                    var delete = _dbContext.Accounts.FirstOrDefault(x => x.Id == idAccount);
                    if (delete != null)
                    {
                        _dbContext.Accounts.Remove(delete);
                    }
                }
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return false;
            }
        }

        public List<Account> GetAll()
        {
            try
            {
                lock (_dbContext)
                {
                    return _dbContext.Accounts.ToList().OrderByDescending(x=>x.CreateDate).ToList();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
            }
            return null;
        }

        public Account GetById(Guid id)
        {
            try
            {
                lock (_dbContext)
                {
                    return _dbContext.Accounts.FirstOrDefault(s => s.Id == id);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
            }
            return null;
           
        }
        public void Update(Account account)
        {
            try
            {
                lock(_dbContext)
                {
                    Log.Information($"Start {nameof(AccountRepository)}, params:  {nameof(Update)}, idAccount  {account.Id}, status {account} ");
                    var acc = _dbContext.Accounts.Where(x => x.Id == account.Id).FirstOrDefault();
                    if (acc != null)
                    {
                        acc.FullName = account.FullName;
                        acc.Status = account.Status;
                        acc.CreateDate = account.CreateDate;
                        acc.Password = account.Password;
                        acc.Backup = account.Backup;
                        acc.Gender = account.Gender;
                        acc.Proxy = account.Proxy;
                        _dbContext.Accounts.Attach(acc);
                        _dbContext.Entry(acc).State = EntityState.Modified;
                        _dbContext.SaveChanges();
                    }
                    Log.Information($"End {nameof(AccountRepository)}, params; {nameof(UpdateStatusByIdAccount)}, acc {acc} ");
                }
              
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
            }
        }
        public bool UpdateStatusByIdAccount(Guid idAccount, string status)
        {
            try
            {
                lock (_dbContext)
                {
                    Log.Information($"Start {nameof(AccountRepository)}, params:  {nameof(UpdateStatusByIdAccount)}, idAccount  {idAccount}, status {status} ");
                    var acc = _dbContext.Accounts.Where(x => x.Id == idAccount).FirstOrDefault();
                    if (acc != null)
                    {
                        acc.Status = status;
                        _dbContext.Accounts.Attach(acc);
                        _dbContext.Entry(acc).State = EntityState.Modified;
                        _dbContext.SaveChanges();
                    }
                    Log.Information($"End {nameof(AccountRepository)}, params; {nameof(UpdateStatusByIdAccount)}, acc {acc} ");
                    return true;
                }
                  
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return false;
            }
        }
        public bool UpdateFullnameByIdAccount(Guid idAccount, string? fullname)
        {
            try
            {
                lock(_dbContext)
                {
                    Log.Information($"Start {nameof(AccountRepository)}, params:  {nameof(UpdateFullnameByIdAccount)}, idAccount  {idAccount}, status {fullname} ");
                    var acc = _dbContext.Accounts.Where(x => x.Id == idAccount).FirstOrDefault();
                    if (acc != null)
                    {
                        acc.FullName = fullname;
                        _dbContext.Accounts.Attach(acc);
                        _dbContext.Entry(acc).State = EntityState.Modified;
                        _dbContext.SaveChanges();
                    }
                    Log.Information($"End {nameof(AccountRepository)}, params; {nameof(UpdateFullnameByIdAccount)}, acc {acc} ");
                    return true;
                }
              
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return false;
            }
        }
        public bool UpdateUidByIdAccount(Guid idAccount, string? uid)
        {
            try
            {
                lock (_dbContext)
                {
                    Log.Information($"Start {nameof(AccountRepository)}, params:  {nameof(UpdateUidByIdAccount)}, idAccount  {idAccount}, status {uid} ");
                    var acc = _dbContext.Accounts.Where(x => x.Id == idAccount).FirstOrDefault();
                    if (acc != null)
                    {
                        acc.Uid = uid;
                        _dbContext.Accounts.Attach(acc);
                        _dbContext.Entry(acc).State = EntityState.Modified;
                        _dbContext.SaveChanges();
                    }
                    Log.Information($"End {nameof(AccountRepository)}, params; {nameof(UpdateUidByIdAccount)}, acc {acc} ");
                    return true;
                }
                   
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return false;
            }
        }
        public bool UpdatePasswordByIdAccount(Guid idAccount, string? password)
        {
            try
            {
                lock (_dbContext)
                {
                    Log.Information($"Start {nameof(AccountRepository)}, params:  {nameof(UpdatePasswordByIdAccount)}, idAccount  {idAccount}, status {password} ");
                    var acc = _dbContext.Accounts.Where(x => x.Id == idAccount).FirstOrDefault();
                    if (acc != null)
                    {
                        acc.Password = password;
                        _dbContext.Accounts.Attach(acc);
                        _dbContext.Entry(acc).State = EntityState.Modified;
                        _dbContext.SaveChanges();
                    }
                    Log.Information($"End {nameof(AccountRepository)}, params; {nameof(UpdatePasswordByIdAccount)}, acc {acc} ");
                    return true;
                }
              
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return false;
            }
        }
        public bool UpdateProxyByIdAccount(Guid idAccount, string? proxy)
        {
            try
            {
                lock (_dbContext)
                {
                    Log.Information($"Start {nameof(AccountRepository)}, params:  {nameof(UpdateProxyByIdAccount)}, idAccount  {idAccount}, status {proxy} ");
                    var acc = _dbContext.Accounts.Where(x => x.Id == idAccount).FirstOrDefault();
                    if (acc != null)
                    {
                        acc.Proxy = proxy;
                        _dbContext.Accounts.Attach(acc);
                        _dbContext.Entry(acc).State = EntityState.Modified;
                        _dbContext.SaveChanges();
                    }
                    Log.Information($"End {nameof(AccountRepository)}, params; {nameof(UpdateProxyByIdAccount)}, acc {acc} ");
                    return true;
                }
                  
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return false;
            }
        }
        public bool UpdateTowFAByIdAccount(Guid idAccount, string? towFA)
        {
            try
            {
                lock (_dbContext)
                {

                    Log.Information($"Start {nameof(AccountRepository)}, params:  {nameof(UpdateTowFAByIdAccount)}, idAccount  {idAccount}, status {towFA} ");
                    var acc = _dbContext.Accounts.Where(x => x.Id == idAccount).FirstOrDefault();
                    if (acc != null)
                    {
                        acc.TowFA = towFA;
                        _dbContext.Accounts.Attach(acc);
                        _dbContext.Entry(acc).State = EntityState.Modified;
                        _dbContext.SaveChanges();
                    }
                    Log.Information($"End {nameof(AccountRepository)}, params; {nameof(UpdateTowFAByIdAccount)}, acc {acc} ");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return false;
            }
        }
        public bool UpdateTokenByIdAccount(Guid idAccount, string? token)
        {
            try
            {
                lock (_dbContext)
                {
                    Log.Information($"Start {nameof(AccountRepository)}, params:  {nameof(UpdateTokenByIdAccount)}, idAccount  {idAccount}, status {token} ");
                    var acc = _dbContext.Accounts.Where(x => x.Id == idAccount).FirstOrDefault();
                    if (acc != null)
                    {
                        acc.Token = token;
                        _dbContext.Accounts.Attach(acc);
                        _dbContext.Entry(acc).State = EntityState.Modified;
                        _dbContext.SaveChanges();
                    }
                    Log.Information($"End {nameof(AccountRepository)}, params; {nameof(UpdateTokenByIdAccount)}, acc {acc} ");
                    return true;
                }
                  
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return false;
            }
        }
        public bool UpdateBackupByIdAccount(Guid idAccount, string? backup)
        {
            try
            {
                lock (_dbContext)
                {
                    Log.Information($"Start {nameof(AccountRepository)}, params:  {nameof(UpdateBackupByIdAccount)}, idAccount  {idAccount}, status {backup} ");
                    var acc = _dbContext.Accounts.Where(x => x.Id == idAccount).FirstOrDefault();
                    if (acc != null)
                    {
                        acc.Backup = backup;
                        _dbContext.Accounts.Attach(acc);
                        _dbContext.Entry(acc).State = EntityState.Modified;
                        _dbContext.SaveChanges();
                    }
                    Log.Information($"End {nameof(AccountRepository)}, params; {nameof(UpdateBackupByIdAccount)}, acc {acc} ");
                    return true;
                }
                  
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return false;
            }
        }
        public bool UpdateAvatarByIdAccount(Guid idAccount, string? avatar)
        {
            try
            {
                lock (_dbContext)
                {
                    Log.Information($"Start {nameof(AccountRepository)}, params:  {nameof(UpdateAvatarByIdAccount)}, idAccount  {idAccount}, status {avatar} ");
                    var acc = _dbContext.Accounts.Where(x => x.Id == idAccount).FirstOrDefault();
                    if (acc != null)
                    {
                        acc.Avatar = avatar;
                        _dbContext.Accounts.Attach(acc);
                        _dbContext.Entry(acc).State = EntityState.Modified;
                        _dbContext.SaveChanges();
                    }
                    Log.Information($"End {nameof(AccountRepository)}, params; {nameof(UpdateAvatarByIdAccount)}, acc {acc} ");
                    return true;
                }
                
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return false;
            }
        }
        public bool UpdateGenderByIdAccount(Guid idAccount, int? gender)
        {
            try
            {
                lock (_dbContext)
                {
                    Log.Information($"Start {nameof(AccountRepository)}, params:  {nameof(UpdateGenderByIdAccount)}, idAccount  {idAccount}, status {gender} ");
                    var acc = _dbContext.Accounts.Where(x => x.Id == idAccount).FirstOrDefault();
                    if (acc != null)
                    {
                        acc.Gender = gender;
                        _dbContext.Accounts.Attach(acc);
                        _dbContext.Entry(acc).State = EntityState.Modified;
                        _dbContext.SaveChanges();
                    }
                    Log.Information($"End {nameof(AccountRepository)}, params; {nameof(UpdateGenderByIdAccount)}, acc {acc} ");
                    return true;
                }
              
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return false;
            }
        }
        public bool UpdateCreaAtByIdAccount(Guid idAccount)
        {
            try
            {

                lock (_dbContext)
                {
                    Log.Information($"Start {nameof(AccountRepository)}, params:  {nameof(UpdateCreaAtByIdAccount)}, idAccount  {idAccount}, status {DateTime.Now} ");
                    var acc = _dbContext.Accounts.Where(x => x.Id == idAccount).FirstOrDefault();
                    if (acc != null)
                    {
                        acc.CreateDate = DateTime.Now;
                        _dbContext.Accounts.Attach(acc);
                        _dbContext.Entry(acc).State = EntityState.Modified;
                        _dbContext.SaveChanges();
                    }
                    Log.Information($"End {nameof(AccountRepository)}, params; {nameof(UpdateCreaAtByIdAccount)}, acc {acc} ");
                    return true;
                }
               
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return false;
            }
        }
        public void DeleteAll()
        {
            try
            {

                lock (_dbContext)
                {
                    var accounts = _dbContext.Accounts;
                    _dbContext.Accounts.RemoveRange(accounts);
                    _dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
            }
        
        }
    }
}
