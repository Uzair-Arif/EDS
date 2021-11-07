using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EDS.Domain.Models;
using EM.Infrastructure.Data;
using EDS.Api.ApiModels;
using EDS.Domain.Interfaces;

namespace EDS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendsController : ControllerBase
    {
        private readonly IGenericRepository<Friend> friendRepository = null;
        public FriendsController(IGenericRepository<Friend> friendRepository)
        {
            this.friendRepository = friendRepository;
        }


        [HttpPost("AddFriend")]
        public async Task<BaseModel> AddFriend(Friend friend)
        {

            try
            {

                if (ModelState.IsValid)
                {
                    //Adding two records for each friendship
                    //1st friendship
                    var newFriendship1 = new Domain.Models.Friend
                    {
                        Member1Id = friend.Member1Id,
                        Member2Id = friend.Member2Id

                    };
                    //2nd friendship
                    var newFriendship2 = new Domain.Models.Friend
                    {
                        Member1Id = friend.Member2Id,
                        Member2Id = friend.Member1Id

                    };


                    var result1 = await friendRepository.Insert(newFriendship1);
                    var result2 = await friendRepository.Insert(newFriendship2);

                    var newFriendships = new List<Friend>();
                    newFriendships.Add(result1);
                    newFriendships.Add(result2);




                    return new BaseModel { data = newFriendships, message = "Friendship Added", success = true };
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
    }
}
