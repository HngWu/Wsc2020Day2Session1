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
            public string Email { get; set; } = null!;
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

        public class tempProfile
        {
            public string id { get; set; }
            public int userTypeId { get; set; }
            public string fullName { get; set; } 
            public string email { get; set; } 
            public string password { get; set; }
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


        public class tempAnnouncementToReturn
        {
            public int id { get; set; } 

            public string announcementDate { get; set; }

            public string announcementTitle { get; set; }

            public string announcementDescription { get; set; }
        }
       

        [HttpPost("competitor")]
        public IActionResult AddCompetitor(tempCompetitor competitor)
        {

            try
            {
               
                var id = Guid.NewGuid().ToString().Substring(0, 10);
               

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
        public IActionResult Login(LoginRequest loginRequest)
        {
            try
            {
                // Find user - adjust this based on your User model properties
                var user = context.Users.FirstOrDefault(u =>
                    u.Email == loginRequest.Email &&
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

                var newAnnouncements = new List<tempAnnouncementToReturn>();
                foreach (var announcement in announcements)
                {
                    var newAnnouncement = new tempAnnouncementToReturn
                    {
                        id = announcement.Id,
                        announcementDate = announcement.Announcementdate.ToString(),
                        announcementTitle = announcement.AnnouncementTitle,
                        announcementDescription = announcement.AnnouncementDescription
                    };
                    newAnnouncements.Add(newAnnouncement);
                }

                return Ok(newAnnouncements);
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
                    var newCompetitor = new tempProfile
                    {
                        id = competitor.Id,
                        userTypeId = competitor.UserTypeId,
                        fullName = competitor.FullName,
                        email = competitor.Email,
                        password = competitor.Password,

                    };

                    return Ok(newCompetitor);

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


        public class tempCheckIn
        {
            public string CompetitorId { get; set; } = null!;
            public string? checkInTime { get; set; }
        }


        public class tempCheckInResponse
        {
            public Boolean isCheckedIn { get; set; }
            public string CompetitorEmail { get; set; } = null!;
        }

        [HttpGet("checkin/{competitorId}")]
        public IActionResult checkForCheckIn(string competitorID)
        {


            try
            {
                var ischeckedIn = context.CheckIns.Where(x => x.CompetitorId == competitorID).Any();
                if (ischeckedIn)
                {
                    return Ok();
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {

                return NotFound();
            }
          

        }

        [HttpPost("checkin")]
        public IActionResult checkIn(tempCheckIn checkin)
        {
            try
            {
                var ischeckedIn = context.CheckIns.Where(x => x.CompetitorId == checkin.CompetitorId).Any();

                
                if(context.Users.Where(x => x.Id == checkin.CompetitorId).Any() == false)
                {
                    return NotFound();
                }

                var competitor = context.Users.Where(x => x.Id == checkin.CompetitorId).FirstOrDefault();
                if(ischeckedIn)
                {
                    var checkIn = new CheckIn
                    {
                        CompetitorId = checkin.CompetitorId,
                    };
                    context.CheckIns.Add(checkIn);
                    context.SaveChanges();
                }
          

                var response = new tempCheckInResponse
                {
                    isCheckedIn = ischeckedIn,
                    CompetitorEmail = competitor.Email
                };

                return Ok(response);
                
            }
            catch (Exception)
            {

                NotFound();
            }
            return BadRequest();
           

        }
    }
}
