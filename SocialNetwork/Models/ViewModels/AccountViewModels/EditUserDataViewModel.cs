using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System;
using SocialNetwork.Models.UserInfoModels;

namespace SocialNetwork.Models.ViewModels.AccountViewModels
{
    public class EditUserDataViewModel
    {
        [Display(Name = "Логин")]
        public string Name { get; set; }

        [Display(Name = "Имя")]
        [RegularExpression(@"[a-zA-Zа-яА-Я-]+", ErrorMessage = "Имя может содержать буквы и символ дефиса")]
        public string FirstName { get; set; }

        [Display(Name = "Фамилия")]
        [RegularExpression(@"[a-zA-Zа-яА-Я_ ]+", ErrorMessage = "Фамилия может содержать буквы, символы пробела и дефиса")]
        public string LastName { get; set; }

        [Display(Name = "Адрес электронной почты")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Дата рождения")]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "Пол")]
        public Gender Gender { get; set; }

        [Display(Name = "Город")]
        [RegularExpression(@"[a-zA-Zа-яА-Я- ]+", ErrorMessage = "Название города может содержать буквы, символы пробела и дефиса")]
        public string City { get; set; }

        [Display(Name = "Страна")]
        [RegularExpression(@"[a-zA-Zа-яА-Я- ]+", ErrorMessage = "Название страны может содержать буквы, символы пробела и дефиса")]
        public string Country { get; set; }
    }
}
