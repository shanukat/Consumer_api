using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using tsapi.Modal;
using tsapi.Util;

namespace tsapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TokenController : Controller
    {
        private readonly JwtBearerSettings _jwtsettings;

        public TokenController(IOptions<JwtBearerSettings> options)
        {
            _jwtsettings = options.Value;
        }



        //[HttpPost]
        //public async Task<IActionResult> Post(UserInfo _userData)
        //{


        //    if (_userData != null && _userData.UniqueIdentityKey != null && _userData.Email != null)
        //    {
        //        var user = await GetUser(_userData.UniqueIdentityKey, _userData.Email);

        //        if (user != null)
        //        {
        //            //create claims details based on the user information
        //            var claims = new[] {
        //            //new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
        //            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        //            new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
        //            new Claim("UniqueIdentityKey", user.UniqueIdentityKey.ToString()),
        //            new Claim("FirstName", user.FirstName),
        //            new Claim("LastName", user.LastName),
        //            new Claim("Email", user.Email)
        //           };

        //            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtsettings.Key));
        //            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        //            var token = new JwtSecurityToken(_jwtsettings.Issuer, _jwtsettings.Audience, claims, expires: DateTime.UtcNow.AddDays(1), signingCredentials: signIn);

        //            return Ok(new JwtSecurityTokenHandler().WriteToken(token));
        //        }
        //        else
        //        {
        //            return BadRequest("Invalid credentials");
        //        }
        //    }
        //    else
        //    {
        //        return BadRequest();
        //    }
        //}



        [HttpPost]
        public async Task<IActionResult> Post(string uniqueIdentityKey)
        {


            if (uniqueIdentityKey != null)
            {
                var user = await GetUser(uniqueIdentityKey);

                if (user != null)
                {
                    //create claims details based on the user information
                    var claims = new[] {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                    new Claim("UniqueIdentityKey", user.UniqueIdentityKey.ToString()),
                    new Claim("FirstName", user.FirstName),
                    new Claim("LastName", user.LastName),
                    new Claim("Email", user.Email)
                   };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtsettings.Key));
                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(_jwtsettings.Issuer, _jwtsettings.Audience, claims, expires: DateTime.UtcNow.AddDays(1), signingCredentials: signIn);

                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
                else
                {
                    return BadRequest("Invalid Identity Key");
                }
            }
            else
            {
                return BadRequest();
            }
        }
        /// <summary>
        /// Validating the User
        /// </summary>
        /// <param name="uniqueIdentity"></param>
        /// <returns></returns>
        private async Task<UserInfo> GetUser(string uniqueIdentityKey)
        {
            UserInfo info = new UserInfo();
            try
            {
                
                //Use a JSON file to keep User Information without using a database
                string JSON = System.IO.File.ReadAllText("Helper/Users.json");
                var userLst = Newtonsoft.Json.JsonConvert.DeserializeObject<Users>(JSON).UserList;

                info = userLst.Where(x => x.UniqueIdentityKey == uniqueIdentityKey).FirstOrDefault();
                
            }
            catch (Exception ex)
            {

            }
            return info;
        }



    }
}
