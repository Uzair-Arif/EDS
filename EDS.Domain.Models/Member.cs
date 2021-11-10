using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EDS.Domain.Models
{
   public class Member
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "* Name must be between 3 and 50 character in length.")]
        public string Name { get; set; }

        [Required]
        [RegularExpression(@"^http(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&amp;%\$#_]*)?$", ErrorMessage = "URL format is wrong")]
        public string PersonalWebAddress { get; set; }

        [Required]
        public string Headings { get; set; }

        public virtual ICollection<Friend> MemberFriends { get; set; }

        public virtual ICollection<Friend> MemberFriendsOf { get; set; }
    }
}
