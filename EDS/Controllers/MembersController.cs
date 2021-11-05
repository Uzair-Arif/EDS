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
                var allmembers = await memberRepository.GetAll();
                var retMembers = new List<MemberWithFriendCount>();
                
                foreach (var member in allmembers)
                {
                    var retMembersToAdded = new MemberWithFriendCount { Name=member.Name, PersonalWebAddress=member.PersonalWebAddress , FriendCount=member.MemberFriends.Count };

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
            var member = await memberRepository.GetById(id);

            if (member == null)
            {
                return new BaseModel { success=false, message=" Member Not Found" };
            }

            var retMember = new MemberWithFriendsPagesLinks { Name= member.Name, Headings=member.Headings, PersonalWebAddress= member.PersonalWebAddress };

            var memberFriends = friendRepository.GetAll().Result.Where(x=>x.Member1Id==member.Id);

            foreach (var memberFriend in memberFriends) 
            {
                var webAddressLink = memberRepository.GetById(memberFriend.Member2Id).Result.PersonalWebAddress;
                retMember.Links.Add(webAddressLink);
            }


            return new BaseModel { }; 

        }

        // PUT: api/Members/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMember(int id, Domain.Models.Member member)
        {
            if (id != member.Id)
            {
                return BadRequest();
            }

           // _context.Entry(member).State = EntityState.Modified;

            try
            {
              //  await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                //if (!MemberExists(id))
                //{
                //    return NotFound();
                //}
                //else
                //{
                //    throw;
                //}
            }

            return NoContent();
        }

        // POST: api/Members
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Domain.Models.Member>> PostMember(Domain.Models.Member member)
        {
          //  _context.Members.Add(member);
          //  await _context.SaveChangesAsync();

            return CreatedAtAction("GetMember", new { id = member.Id }, member);
        }

       
        //private bool MemberExists(int id)
        //{
        //   // return _context.Members.Any(e => e.Id == id);
        //}

        [HttpPost("Create")]
        public async Task<BaseModel> Create(ApiModels.Member member)
        {
            if (ModelState.IsValid)
            {
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

       
    }
}
