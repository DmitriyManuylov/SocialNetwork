using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Models.ViewModels.AccountViewModels
{
    public class RegisterViewModel
    {

        [Required(ErrorMessage = "Введите логин")]
        [Display(Name = "Логин")]
        public string Name { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Required(ErrorMessage = "Введите адрес электронной почты")]
        [Display(Name = "Адрес электронной почты")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Display(Name = "Подтвердите пароль")]
        [Compare(nameof(Password), ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        public string PasswordConfirm { get; set; }

        public int? CityId { get; set; }
    }
}
