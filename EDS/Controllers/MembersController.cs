using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EDS.Domain.Models;
using EM.Infrastructure.Data;
using EDS.Core.Utilities;
using EDS.Api.ApiModels;
using EDS.Domain.Interfaces;
using System.Collections.ObjectModel;
using System.Text;

namespace EDS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly IGenericRepository<Domain.Models.Member> memberRepository = null;
        private readonly IGenericRepository<Friend> friendRepository = null;

      
        public MembersController(IGenericRepository<Domain.Models.Member> memberRepository, IGenericRepository<Friend> friendRepository)
        {
            this.memberRepository = memberRepository;
            this.friendRepository = friendRepository;
          
       
        }

        // GET: api/Members
        [HttpGet]
        public async Task<ActionResult<BaseModel>> GetMembers()
        {
            try
            {
                //Get all members including friends
                 var allmembers = memberRepository.GetAll().Include(x => x.MemberFriends).ThenInclude(e =>e.Member2).Include(e=>e.MemberFriendsOf).ThenInclude(e=>e.Member1).ToList();

                //List to return from the call
                var retMembers = new List<MemberWithFriendCount>();
                
                //Iterate to create return list with friends count
                foreach (var member in allmembers)
                {
                    var retMembersToAdded = new MemberWithFriendCount { Id= member.Id, Name=member.Name, PersonalWebAddress=member.PersonalWebAddress , FriendCount=member.MemberFriends.Count };

                    retMembers.Add(retMembersToAdded);
                }


               if(retMembers.Count>0)
                return new BaseModel { data=retMembers, message="Members List",success=true };
               else
                    return new BaseModel { data = null, message = "No members found", success = false };

            }

            catch (Exception ex)
            {
                return new BaseModel { data = ex, success = false };
                
            }
        }

        // GET: api/Members/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BaseModel>> GetMember(int id)
        {
            try
            {

            // Get member with id including friends
            var member = memberRepository.GetAll().Where(x=>x.Id==id).Include(x => x.MemberFriends).ThenInclude(e => e.Member2).Include(e => e.MemberFriendsOf).ThenInclude(e => e.Member1).FirstOrDefault();
            
            if (member == null)
            {
                return new BaseModel { success=false, message=" Member Not Found" };
            }

            //Custom model to return friends pages links as well
            var retMember = new MemberWithFriendsPagesLinks { Name= member.Name, Headings=member.Headings, PersonalWebAddress= member.PersonalWebAddress };

            //Iterate to generate list of tuples for all friends websites links
            foreach (var memberFriend in member.MemberFriends)
            {

                retMember.Links.Add(new Tuple<int,string, string>(memberFriend.Member2.Id,memberFriend.Member2.Name, memberFriend.Member2.PersonalWebAddress));
          
            }

            
            
            return new BaseModel { data = retMember, success=true  };


            }
            catch (Exception ex)
            {
                return new BaseModel { data = ex, success = false };
            }
            

        }

        

       

        [HttpPost("Create")]
        public async Task<BaseModel> Create(ApiModels.Member member)
        {

            try
            {

            
            if (ModelState.IsValid)
            {
                // Get heading for profile
                var headings = WebScraping.GetHeadings(member.PersonalWebAddress);
                
                var newMember = new Domain.Models.Member
                {
                    Name = member.Name,
                    PersonalWebAddress = member.PersonalWebAddress,
                    Headings = headings

                };

                var result=await memberRepository.Insert(newMember);
               
                return new BaseModel { data= result, message = "Member Created", success = true };
            }

            else 
            {
                return new BaseModel { data = null, message = "Invalid data", success = false };
            }
            }
            catch (Exception ex)
            {
                return new BaseModel { data=ex, success=false };
            }


        }

        [HttpGet("{id}/{topic}")]
        public async Task<BaseModel> ExpertSearch(int id,string topic)
        {
            try
            {

            //Get all members who are not friends
            var notFriends = friendRepository.GetAll().Where(x => x.Member1Id != id && x.Member2Id != id);
                if (notFriends.Count() > 0) 
                {
                 
                    StringBuilder sb = new StringBuilder();
                    int experrtId = 0;
                    string dispHeading = string.Empty;

                    //Iterate all non-friend and find experts
                    foreach (var notFriend in notFriends)
                    {

                        if (notFriend.Member1.Headings.Split(',').Contains(topic))
                        {
                            var headings = notFriend.Member1.Headings.Split(',');
                            foreach (var heading in headings)
                            {
                                if (heading.Contains(topic))
                                {
                                    dispHeading = heading;

                                    experrtId = notFriend.Member1.Id;
                                }
                               
                                break;
                            }
                            break;
                        }

                    }
                    //If expert exist with topic
                    if (!string.IsNullOrEmpty(dispHeading)) 
                    {
                        var memberFriend = friendRepository.GetAll().Where(x => x.Member1Id == id && (x.Member2.MemberFriends.Any(x => x.Member2Id == experrtId)) == true).FirstOrDefault();
                        var memberName = memberRepository.GetByIdAsync(id).Result.Name;
                        var expertName = memberRepository.GetByIdAsync(experrtId).Result.Name;

                        var retPath = sb.Append(memberName + "-->" + memberFriend.Member1.Name + "-->" + expertName + "(" + dispHeading + ")").ToString();
                        return new BaseModel { data = retPath, message = "Path to topic expert", success = true };


                    }
                    else
                    {
                        return new BaseModel { message = "No expert available", success = false };
                    }



                }

                else
                {
                    return new BaseModel { message="No other friends available", success=false };
                }


            }
            catch (Exception ex)
            {
                return new BaseModel { data = ex, success = false };
            }

           
           

        }

    }
}
