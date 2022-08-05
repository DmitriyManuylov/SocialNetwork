using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Models
{
    /// <summary>
    /// Комната чата, не требующего регистрации
    /// </summary>
    public class LiteChatRoom
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public List<SimpleMessage> Messages { get; set; }

    }
}
