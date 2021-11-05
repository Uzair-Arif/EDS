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

        public string Name { get; set; }

        public string PersonalWebAddress { get; set; }

        public string Headings { get; set; }

        public virtual ICollection<Friend> MemberFriends { get; set; }

        //public virtual ICollection<Friend> MemberFriendsOf { get; set; }
    }
}
