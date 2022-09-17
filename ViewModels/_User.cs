using Godch.Models;
using System.ComponentModel.DataAnnotations;

namespace Godch.ViewModels
{
    public class vLogin : ViewBase
    {
        [Required]
        [Display(Name = "Account")]
        public string? Account { get; set; }
        [Required]
        [Display(Name = "Password")]
        public string? Password { get; set; }
        public bool RememberMe { get; set; }
    }
    public class UserConfig
    {
        
    }
    public class NewUser : ViewBase
    {
        
        [Required]
        [MinLength(3)]
        [MaxLength(30)]
        public string? Account { get; set; }
        [Required]
        [MinLength(3)]
        [MaxLength(48)]
        public string? Password { get; set; }
        [Required]
        [MinLength(1)]
        [MaxLength(64)]
        public string? FullName { get; set; }
        public string? NickName { get; set; }
        [Required]
        [EmailAddress]
        public string? EmailAddress { get; set; }
        public string? Description { get; set; }
        public IFormFile? Photo { get; set; }

    }
    public class sUser : ViewBase
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Account { get; set; }
        public string? Photo { get; set; }
        public sUser() { }
        public sUser(User u)
        {
            Id = u.UserId;
            Name = u.UserName;
            Account = u.Account;
            if (!GodchDirectories.UserData.Exist($"{Id}\\photo.jpg"))
            {
                Photo = "/_GodchData/UserData/default.png";
            }
            else
            {
                Photo = $"/_GodchData/UserData/{u.UserId}/photo.jpg";
            }
        }
    }
    public class dUser : sUser
    {
        public string? Contact { get; set; }
        public string? SelfIntroduction { get; set; }
        public List<dynamic>? Followers { get; set; }
        public List<dynamic>? Friends { get; set; }
        public List<dynamic>? Forums { get; set; }
        public List<dynamic>? Works { get; set; }
        public dUser() { }
        public dUser(User u):base(u) 
        {
            SelfIntroduction = u.Description;
        }
    }
}
