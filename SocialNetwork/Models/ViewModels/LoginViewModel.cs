using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Models.ViewModels
{
    public class LoginViewModel
    {
        [Display(Name = "Логин")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Пароль")]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
