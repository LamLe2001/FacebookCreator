using FacebookCreator.Entities;

namespace FacebookCreator.Repositories
{
    public interface IAccountRepository
    {
        void Add(Account account);
        List<Account> GetAll();
        void Update(Account account);
        Account GetById(Guid id);
        void Delete(Guid id);
        bool DeleteRange(List<string> deleteList);
        void DeleteAll();
        bool UpdateStatusByIdAccount(Guid idAccount, string status);
        bool UpdateFullnameByIdAccount(Guid idAccount, string? fullname);
        bool UpdateUidByIdAccount(Guid idAccount, string? uid);
        bool UpdatePasswordByIdAccount(Guid idAccount, string? password);
        bool UpdateProxyByIdAccount(Guid idAccount, string? proxy);
        bool UpdateTowFAByIdAccount(Guid idAccount, string? towFA);
        bool UpdateTokenByIdAccount(Guid idAccount, string? token);
        bool UpdateBackupByIdAccount(Guid idAccount, string? backup);
        bool UpdateAvatarByIdAccount(Guid idAccount, string avatar);
        bool UpdateGenderByIdAccount(Guid idAccount, int? gender);
        bool UpdateCreaAtByIdAccount(Guid idAccount);
    }
}
