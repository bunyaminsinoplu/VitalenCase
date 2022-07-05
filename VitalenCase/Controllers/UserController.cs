using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VitalenCase.CustomModels;
using VitalenCase.Interface;
using VitalenCase.Models;

namespace VitalenCase.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly VitalenTestContext _context;
        private readonly IConfiguration _config;
        private readonly IFunctions _functions;

        public UserController(VitalenTestContext context, IConfiguration config, IFunctions functions)
        {
            _context = context;
            _config = config;
            _functions = functions;
        }

        [Route("login")]
        [HttpPost]
        public async Task<ObjectResult> Login([FromBody] Credential credentials)
        {
            try
            {
                if (credentials.password != null && credentials.username != null)
                {
                    var UserGuidHash = await _context.Users.FirstOrDefaultAsync(c => c.UserName == credentials.username && c.IsDeleted != true);

                    if (UserGuidHash != null)
                    {
                        string EncryptPassword = await _functions.Encrypt(credentials.password, UserGuidHash.Guid);
                        var user = await _context.Users.FirstOrDefaultAsync(c => c.UserName == credentials.username && c.Password == EncryptPassword);
                        if (user != null)
                        {
                            return Ok(new { token = await _functions.CreateJWT(user), message = "Giriþ Baþarýlý" });
                        }
                        else
                        {
                            return BadRequest(new { message = "Kullanýcý adý veya þifre yanlýþ" });
                        }
                    }
                    else
                    {
                        return BadRequest(new { message = "Kullanýcý adý veya þifre yanlýþ" });
                    }
                }
                else
                {
                    return BadRequest(new { message = "Kullanýcý adý veya þifre yanlýþ" });
                }
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpGet("getUsers")]
        [Authorize]
        public async Task<ObjectResult> GetUsers()
        {
            try
            {
                var contextUser = HttpContext.User;
                int userId = Convert.ToInt32(contextUser.Claims.FirstOrDefault(c => c.Type == "userID").Value);
                var user = await _context.Users.FirstOrDefaultAsync(c => c.Id == userId);
                if (user != null)
                {
                    List<User> userList = await _context.Users.Where(c => c.IsDeleted != true).ToListAsync();

                    return Ok(new { userList });
                }
                else
                {
                    return BadRequest(new { message = "Ýstek göndermeden önce giriþ yapýn" });
                }
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpPut("updateUser")]
        [Authorize]
        public async Task<ObjectResult> UpdateUser([FromBody]UserCRUD model)
        {
            try
            {
                var contextUser = HttpContext.User;
                int userId = Convert.ToInt32(contextUser.Claims.FirstOrDefault(c => c.Type == "userID").Value);
                var user = await _context.Users.FirstOrDefaultAsync(c => c.Id == userId);
                if (user != null)
                {
                    User existUser = await _context.Users.FirstOrDefaultAsync(c => c.Id == model.id && c.IsDeleted != true);

                    if (existUser != null)
                    {
                        existUser.UserName = model.username;
                        existUser.Password = await _functions.Encrypt(model.password, existUser.Guid);
                        existUser.UpdatedDate = DateTime.Now;
                        existUser.UpdatedBy = userId;

                        _context.Entry(existUser).State = EntityState.Modified;
                        await _context.SaveChangesAsync();

                        return Ok(new { message = "Güncelleme baþarýlý" });
                    }
                    else
                    {
                        return BadRequest(new { message = "Güncellemek istediðiniz kullanýcý bulunamadý" });
                    }
                }
                else
                {
                    return BadRequest(new { message = "Ýstek göndermeden önce giriþ yapýn" });
                }
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpDelete("deleteUser")]
        [Authorize]
        public async Task<ObjectResult> DeleteUser([FromBody] int id)
        {
            try
            {
                var contextUser = HttpContext.User;
                int userId = Convert.ToInt32(contextUser.Claims.FirstOrDefault(c => c.Type == "userID").Value);
                var user = await _context.Users.FirstOrDefaultAsync(c => c.Id == userId);
                if (user != null)
                {
                    User existUser = await _context.Users.FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted != true);

                    if (existUser != null)
                    {
                        existUser.IsDeleted = true;
                        existUser.UpdatedDate = DateTime.Now;
                        existUser.UpdatedBy = userId;

                        _context.Entry(existUser).State = EntityState.Modified;
                        await _context.SaveChangesAsync();

                        return Ok(new { message = "Silme iþlemi baþarýlý" });
                    }
                    else
                    {
                        return BadRequest(new { message = "Silmek istediðiniz kullanýcý bulunamadý" });
                    }
                }
                else
                {
                    return BadRequest(new { message = "Ýstek göndermeden önce giriþ yapýn" });
                }
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpPost("createUser")]
        [Authorize]
        public async Task<ObjectResult> CreateUser(UserCRUD model)
        {
            try
            {
                var contextUser = HttpContext.User;
                int userId = Convert.ToInt32(contextUser.Claims.FirstOrDefault(c => c.Type == "userID").Value);
                var user = await _context.Users.FirstOrDefaultAsync(c => c.Id == userId);
                if (user != null)
                {
                    string guid = Guid.NewGuid().ToString();

                    User newUser = new()
                    {
                        CreatedDate = DateTime.Now,
                        Guid = guid,
                        IsDeleted = false,
                        Password = await _functions.Encrypt(model.password, guid),
                        UpdatedBy = userId,
                        UpdatedDate = DateTime.Now,
                        UserName = model.username
                    };

                    await _context.Users.AddAsync(newUser);
                    await _context.SaveChangesAsync();

                    return Ok(new { message = "Ekleme iþlemi baþarýlý" });

                }
                else
                {
                    return BadRequest(new { message = "Ýstek göndermeden önce giriþ yapýn" });
                }
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

    }
}