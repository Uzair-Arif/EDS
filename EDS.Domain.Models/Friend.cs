using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EDS.Domain.Models
{
   public class Friend
    {
        public Member Member1 { get; set; }
        public Member Member2 { get; set; }
        public int Member1Id { get; set; }
       
        public int Member2Id { get; set; }
      
        
        
    }
}
