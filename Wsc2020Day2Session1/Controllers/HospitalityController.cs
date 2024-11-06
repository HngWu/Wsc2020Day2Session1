using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wsc2020Day2Session1.Models;

namespace Wsc2020Day2Session1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HospitalityController : Controller
    {
       Wsc2020Day2Session1Context context = new Wsc2020Day2Session1Context();

        public string adminSecret = "aldfjaslkfajkllnjl1350-49ru0wdfgjvlsdkj";
        public string competitorSecret = "alsdfkjalsdfalsdfn12094eerwfdknxcx";


        public class LoginRequest
        {
            public string Username { get; set; } = null!;
            public string Password { get; set; } = null!;
        }

        public class LoginResponse
        {
            public string Token { get; set; } = null!;
            public string Role { get; set; } = null!;
            public string UserId { get; set; } = null!;
        }

        public class tempCompetitor
        {
            public string FullName { get; set; } = null!;

            public string Email { get; set; } = null!;

            public string Password { get; set; } = null!;
        }

        public class Auth
        {
            public string? token { get; set; }
        }

        public class tempAnnouncement
        {


            public string title { get; set; } = null!;

            public string description { get; set; } = null!;
        }

        [HttpPost("competitor")]
        public IActionResult AddCompetitor(tempCompetitor competitor)
        {

            try
            {
                var id = Guid.NewGuid().ToString().Take(10).ToString();

                var newCompetitor = new User
                {
                    Id = id,
                    UserTypeId = 1,
                    FullName = competitor.FullName,
                    Email = competitor.Email,
                    Password = competitor.Password,
                };

                context.Users.Add(newCompetitor);
                context.SaveChanges();

                return Ok();
            }
            catch (Exception)
            {

                return StatusCode(404);
            }
        }

        [HttpPost("announcement")]
        public IActionResult addAnnouncement(tempAnnouncement announcement)
        {

            try
            {
                var newAnnouncement = new Announcement
                {
                    Announcementdate = DateTime.Now,
                    AnnouncementTitle = announcement.title,
                    AnnouncementDescription = announcement.description,
                };
                context.Announcements.Add(newAnnouncement);
                context.SaveChanges();

                return Ok();
            }
            catch (Exception)
            {

                return StatusCode(404);
            }
        }

        [HttpPost("AuthAdmin")]
        public IActionResult AuthAdmin(LoginResponse loginResponse)
        {
            try
            {
                if (loginResponse.Token == adminSecret)
                {
                    return Ok();

                }
                else
                {
                    return Unauthorized();
                }


            }
            catch (Exception)
            {

                Unauthorized();
            }


            return Ok();
        }


        [HttpPost("AuthCompetitor")]
        public IActionResult AuthCompetitor(Auth token)
        {
            try
            {
                if (token.token == competitorSecret)
                {
                    return Ok();

                }
                else
                {
                    return Unauthorized();
                }


            }
            catch (Exception)
            {

                Unauthorized();
            }


            return Ok();
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            try
            {
                // Find user - adjust this based on your User model properties
                var user = context.Users.FirstOrDefault(u =>
                    u.Id == loginRequest.Username &&
                    u.Password == loginRequest.Password);

                if (user == null)
                    return Unauthorized(new { message = "Invalid credentials" });

                // Determine role based on UserTypeId
                string role = user.UserTypeId == 1 ? "competitor" : "admin";

                // Generate JWT token
                var token = user.UserTypeId == 1 ? competitorSecret : adminSecret;

                return Ok(new LoginResponse
                {
                    Token = token,
                    Role = role,
                    UserId = user.Id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error" });
            }
        }



        [HttpGet("announcements")]
        public IActionResult getAccouncements()
        {
            try
            {
                var announcements = context.Announcements.OrderBy(x => x.Announcementdate).ToList();
                return Ok(announcements);
            }
            catch (Exception)
            {

                return NotFound() ;
            }
            
        }


        [HttpGet("competitor/{competitorId}")]
        public IActionResult getCompetitor(string competitorId)
        {

            try
            {
                var competitor = context.Users.Where(x => x.Id == competitorId).FirstOrDefault();

                if (competitor != null)
                {
                    return Ok(competitor);

                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {

                NotFound();
            }

            return BadRequest();
        }


        [HttpGet("checkin/{id}")]
        public IActionResult checkIn(string id)
        {
            try
            {
                var ischeckedIn = context.CheckIns.Where(x => x.CompetitorId == id).Any();

                if (ischeckedIn)
                {
                    return StatusCode(201);

                }
                else
                {
                    var checkIn = new CheckIn
                    {
                        CompetitorId = id,
                    };
                    return Ok(checkIn);
                }
            }
            catch (Exception)
            {

                NotFound();
            }
            return BadRequest();
           

        }
    }
}
