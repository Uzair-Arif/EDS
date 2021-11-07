using EDS.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDS.Api
{
    public static class FriendsExtension
    {


        //Maintaining Stack for the friends collection
        public static IEnumerable<Domain.Models.Member> GetFriends( this Domain.Models.Member root, List<Member> m)
        {
            var nodes = new Stack<Domain.Models.Member>(new[] { root });
            while (nodes.Any())
            {
                Domain.Models.Member node = nodes.Pop();
               
                m.Add(node);
                yield return node;
                foreach (var n in node.MemberFriends)
                {
                    if (!m.Contains(n.Member2)) 
                    {
                        nodes.Push(n.Member2);
                       
                    }
                    
                }
                
            }
        }

    }
}
