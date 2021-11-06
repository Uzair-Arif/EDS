using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EDS.Api.ApiModels
{
    public class Member
    {

        public int Id { get; set; }


        [Required]
        public string Name { get; set; }

        [Required]
        public string PersonalWebAddress { get; set; }
    }

    public class MemberWithFriendCount
    {

        public int Id { get; set; }


        [Required]
        public string Name { get; set; }

        [Required]
        public string PersonalWebAddress { get; set; }

        public int FriendCount { get; set; }
    }

    public class MemberWithFriendsPagesLinks
    {

        public int Id { get; set; }


        [Required]
        public string Name { get; set; }

        [Required]
        public string PersonalWebAddress { get; set; }

        public string Headings { get; set; }

        public List<Tuple<int,string, string>> Links = new List<Tuple<int,string, string>>();

        //public List<string> Links { get; set; } = new List<string>();
    }
}
