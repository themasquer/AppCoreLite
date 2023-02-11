using AppCoreLite.Records.Bases;

namespace AppCoreLite.Entities
{
    public class AccountUser : Record, ISoftDelete, IModifiedBy
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public int AccountRoleId { get; set; }
        public AccountRole? AccountRole { get; set; }

        public bool? IsDeleted { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class AccountRole : Record, ISoftDelete, IModifiedBy
    {
        public string RoleName { get; set; }
        public List<AccountUser>? AccountUsers { get; set; }

        public bool? IsDeleted { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
