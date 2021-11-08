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
    // [ApiController]
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
                var allmembers = memberRepository.GetAll().Include(x => x.MemberFriends).ThenInclude(e => e.Member2).Include(e => e.MemberFriendsOf).ThenInclude(e => e.Member1).ToList();

                //List to return from the call
                var retMembers = new List<MemberWithFriendCount>();

                //Iterate to create return list with friends count
                foreach (var member in allmembers)
                {
                    var retMembersToAdded = new MemberWithFriendCount { Id = member.Id, Name = member.Name, PersonalWebAddress = member.PersonalWebAddress, FriendCount = member.MemberFriends.Count };

                    retMembers.Add(retMembersToAdded);
                }


                if (retMembers.Count > 0)
                    return new BaseModel { data = retMembers, message = "Members List", success = true };
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
                var member = memberRepository.GetAll().Where(x => x.Id == id).Include(x => x.MemberFriends).ThenInclude(e => e.Member2).Include(e => e.MemberFriendsOf).ThenInclude(e => e.Member1).FirstOrDefault();

                if (member == null)
                {
                    return new BaseModel { success = false, message = " Member Not Found" };
                }

                //Custom model to return friends pages links as well
                var retMember = new MemberWithFriendsPagesLinks { Name = member.Name, Headings = member.Headings, PersonalWebAddress = member.PersonalWebAddress };

                //Iterate to generate list of tuples for all friends websites links
                foreach (var memberFriend in member.MemberFriends)
                {

                    retMember.Links.Add(new Tuple<int, string, string>(memberFriend.Member2.Id, memberFriend.Member2.Name, memberFriend.Member2.PersonalWebAddress));

                }



                return new BaseModel { data = retMember, success = true };


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

                    var result = await memberRepository.Insert(newMember);

                    return new BaseModel { data = result, message = "Member Created", success = true };
                }

                else
                {
                    return new BaseModel { data = null, message = "Invalid data", success = false };
                }
            }
            catch (Exception ex)
            {
                return new BaseModel { data = ex, success = false };
            }


        }

        [HttpGet("{id}/{topic}")]
        public async Task<BaseModel> ExpertSearch(int id, string topic)
        {
            try
            {
                var member = await memberRepository.GetByIdAsync(id);

                if (member == null)
                {
                    return new BaseModel { success = false, message = " Member Not Found" };
                }
                //Get all members who are not friends
                var notFriends = friendRepository.GetAll().Include(x => x.Member1).ThenInclude(x => x.MemberFriends).Include(x => x.Member2).ThenInclude(x => x.MemberFriendsOf).Where(x => x.Member1Id != id && x.Member2Id != id).ToList();
                if (notFriends.Count() > 0)
                {


                    int experrtId = 0;
                    string dispHeading = string.Empty;

                    //Get expert MemberId
                    if (!string.IsNullOrEmpty(topic))
                        experrtId = FindExpert(notFriends, topic, out dispHeading);



                    ////        Approach 1- When 1 degree separation between member and expert as given in document Alan --> Bart --> Claudia /////


                    ////If expert exist with topic
                    //if (!string.IsNullOrEmpty(dispHeading))
                    //{
                    //    StringBuilder sb = new StringBuilder();
                    //    var memberFriend = friendRepository.GetAll().Where(x => x.Member1Id == id && (x.Member2.MemberFriends.Any(x => x.Member2Id == experrtId)) == true).FirstOrDefault();
                    //    var memberName = memberRepository.GetByIdAsync(id).Result.Name;
                    //    var expertName = memberRepository.GetByIdAsync(experrtId).Result.Name;

                    //    var retPath = sb.Append(memberName + "-->" + memberFriend.Member2.Name + "-->" + expertName + "(" + dispHeading + ")").ToString();
                    //    return new BaseModel { data = retPath, message = "Path to topic expert", success = true };


                    //}
                    //else
                    //{
                    //    return new BaseModel { message = "No expert available", success = false };
                    //}



                    ///////////////////// Approach 2 if n degree of separation between member and expert e.g Alan-->Bart-->Nick-->Claudia  ////////////////
                    //Will enter the searching mechanism only if topic expert is available
                    if (experrtId > 0 && !string.IsNullOrEmpty(dispHeading))
                    {

                        string pathToRet = GetPathFromExpertToMember(experrtId, id, dispHeading);

                        if (!string.IsNullOrEmpty(pathToRet))
                        {
                            return new BaseModel { data = pathToRet, message = "Path found", success = true };

                        }
                        else
                        {
                            return new BaseModel { message = "Sorry no friendship chain available", success = false };
                        }



                    }

                    else
                    {
                        return new BaseModel { message = "No expert found", success = false };
                    }

                }

                else
                {
                    return new BaseModel { message = "No other friends available", success = false };
                }


            }
            catch (Exception ex)
            {
                return new BaseModel { data = ex, success = false };
            }
        }
        public int FindExpert(IEnumerable<Friend> friends, string topic, out string dispHeading)
        {
            try
            {

                int expertId = 0;
                //Iterate all non-friend and find expert
                foreach (var notFriend in friends)
                {

                    if (notFriend.Member1.Headings.Split(',').Contains(topic))
                    {
                        var headings = notFriend.Member1.Headings.Split(',');
                        foreach (var heading in headings)
                        {
                            if (heading.Contains(topic))
                            {
                                dispHeading = heading;

                                expertId = notFriend.Member1.Id;
                                return expertId;
                            }


                        }

                    }



                }
                dispHeading = string.Empty;
                return expertId;

            }
            catch (Exception)
            {
                dispHeading = string.Empty;
                return 0;
            }

        }

        public string GetPathFromExpertToMember(int expertId, int memberId, string dispHeading)
        {

            try
            {

                StringBuilder sb = new StringBuilder();
                var pathList = new List<Domain.Models.Member>();

                var expert = memberRepository.GetByIdAsync(expertId).Result;

                //A list to act as global for the extension method for finding descendants(friends) it will insure that their is no cycle of friendship being added
                //Hence once a member is marked as friend it will not be marked again and avoiding infinte looping
                var membersList = new List<Domain.Models.Member>();

                //Now we will track back from the expert upto member   
                var friendIterator = FriendsExtension.GetFriends(expert, membersList);
                bool possiblePath = false;
                foreach (var f in friendIterator)
                {

                    pathList.Add(f);
                    if (f.MemberFriends.Any(x => x.Member2Id == memberId))
                    {
                        // Add member to list as well
                        var member = memberRepository.GetByIdAsync(memberId).Result;
                        pathList.Add(member);
                        possiblePath = true;
                        break;
                    }

                }
                // If path found from expert to member
                if (possiblePath)
                {
                    //Reverse the list and fromat the output
                    for (int i = pathList.Count - 1; i >= 0; i--)
                    {
                        sb.Append(pathList[i].Name + "-->");
                    }

                    //Remove last -->
                    sb.Length = sb.Length - 3;
                    sb.Append("(" + dispHeading + ")");

                    return sb.ToString();

                }


            }
            catch (Exception ex)
            {
                // Log exception here
            }


            return string.Empty;
        }

    }
}
