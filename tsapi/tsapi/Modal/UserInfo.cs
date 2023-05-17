using System.ComponentModel.DataAnnotations;

namespace tsapi.Modal
{
    public class UserInfo
    {
        [Required]
        public string UniqueIdentityKey { get; set; }

        [Required]
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

    }

    public class Users
    {
        public List<UserInfo> UserList { get; set; }
    }



}
