using EDS.Api.ApiModels;
using EDS.Api.ApiModels.UserAccountModels;
using EDS.Domain.Models;
using EDS.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EDS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IJWTAuthenticationManager jWTAuthenticationManager;

        public AccountController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager, IJWTAuthenticationManager jWTAuthenticationManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.jWTAuthenticationManager = jWTAuthenticationManager;
        }

        // GET: api/<AccountController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<AccountController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<AccountController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<AccountController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<AccountController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        [HttpPost]
        public async Task<BaseModel> Register(Register model)
        {
            if (ModelState.IsValid)
            {
                // Copy data from RegisterViewModel to IdentityUser
                var user = new IdentityUser
                {
                    UserName = model.Email,
                    Email = model.Email
                };

                // Store user data in AspNetUsers database table
                var result = await userManager.CreateAsync(user, model.Password);

                // If user is successfully created, sign-in the user using
                // SignInManager and redirect to index action of HomeController
                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return new BaseModel { data = user, message = "Registered", success = true };
                }

                // If there are any errors, add them to the ModelState object
                // which will be displayed by the validation summary tag helper
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return new BaseModel{ success=false, data=null, message="Invalid Data" };
        }

        [HttpPost]
        public async Task<BaseModel> Login(Login model)
        {
            if (ModelState.IsValid)
            {


                var token=jWTAuthenticationManager.Authenticate(signInManager,model.Email, model.Password,model.RememberMe);

                if (token == null) 
                {
                    ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
                    return new BaseModel { success = false, data = null, message = "Login Failed" };
                }
                

                else 
                {
                    return new BaseModel { success = true, data = token, message = "Login Success" };
                }
                    
            }

            return new BaseModel { success = false, data = null, message = "Invalid Data" };
        }

    }
}
