using CI_Entities1.Data;
using CI_Entities1.Models;
using CI_Entities1.Models.NewFolder;
using CI_Entities1.Models.ViewModel;
using CI_Platform1.Models;
using CI_Project.Repository.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace CI_Platform1.Controllers
{
    public class HomeController : Controller
    {
        int i = 0, i1 = 0, j = 0, j1 = 0, k = 0, k1 = 0;
        private readonly ILogger<HomeController> _logger;
        private readonly CiPlatformContext _CiPlatformContext;

        private readonly IUser _Interface;

        public dynamic SearchingMission { get; private set; }

        public HomeController(CiPlatformContext CiPlatformContext, IUser UserInterface)
        {
            _CiPlatformContext = CiPlatformContext;
            //_logger = logger     ILogger<HomeController> logger,;
            _Interface = UserInterface;
        }

        //login method
        public IActionResult Login()
        {
            HttpContext.Session.Clear();
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _Interface.UserByEmailPassword(model.Email, model.Password);
                var username = model.Email.Split('@')[0];
                if (user == null)
                {
                    ViewBag.Error = "Email or Password has not Matched to the registered Credentials";
                }
                else
                {
                    HttpContext.Session.SetString("userID", user.UserId.ToString());
                    HttpContext.Session.SetString("Firstname", user.FirstName);

                    return RedirectToAction("LandingPage", "Home", new { @id = user.UserId });
                }
            }

            return View();
        }

        //Registration 
        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registration(User user)
        {
            var obj = _Interface.UserByEmail(user.Email);

            if (obj == null)
            {
                _Interface.addUser(user);
                //_CiPlatformContext.Users.Add(user);
                //_CiPlatformContext.SaveChanges();
                return RedirectToAction("Login", "Home");
            }
            else
            {
                ViewBag.Email = "Email already exists";
            }
            return View();
        }


        //Forget Password Model
        public IActionResult Forget()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Forget(ForgetModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _Interface.UserByEmail(model.Email);
                if (user == null)
                {
                    return RedirectToAction("ForgetPass", "Home");
                }


                var token = Guid.NewGuid().ToString();

                //var passwordReset = new PasswordReset
                //{
                //    Email = model.Email,
                //    Token = token
                //};

                _Interface.token(model.Email, token);

                //_CiPlatformContext.PasswordResets.Add(passwordReset);
                //_CiPlatformContext.SaveChanges();


                var resetLink = Url.Action("Resetpass", "Home", new { email = model.Email, token }, Request.Scheme);


                var fromAddress = new MailAddress("officehl1881@gmail.com", "Sender Name");
                var toAddress = new MailAddress(model.Email);
                var subject = "Password reset request";
                var body = $"Hi,<br /><br />Please click on the following link to reset your password:<br /><br /><a href='{resetLink}'>{resetLink}</a>";
                var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                var smtpClient = new SmtpClient("smtp.gmail.com", 587)
                {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("officehl1881@gmail.com", "vrbxqqayjlbvoihx"),
                    EnableSsl = true
                };
                smtpClient.Send(message);

                return RedirectToAction("Login", "Home");
            }

            return View();
        }


        //Reset Password
        [HttpGet]
        public IActionResult Resetpass(string email, string token)
        {
            var passwordReset = _Interface.Reset(email, token);
            if (passwordReset == null)
            {
                return RedirectToAction("Login", "Home");
            }
            // Pass the email and token to the view for resetting the password
            var model = new PasswordReset
            {
                Email = email,
                Token = token
            };
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult Resetpass(Resetpassword model)
        {
            if (ModelState.IsValid)
            {
                // Find the user by email
                var user = _Interface.UserByEmail(model.Email);
                if (user == null)
                {
                    return RedirectToAction("Forget", "User");
                }

                // Find the password reset record by email and token
                var passwordReset = _Interface.Reset(model.Email, model.Token);
                if (passwordReset == null)
                {
                    return RedirectToAction("Login", "Home");
                }

                // Update the user's password
                user.Password = model.Password;
                _CiPlatformContext.SaveChanges();

            }

            return RedirectToAction("Login", "Home");
        }


        //Landing Page
        public IActionResult LandingPage(long id, string SearchingMission, int? pageIndex, int cntry, int Order)
        {
            //User Admin Name
            var userid = HttpContext.Session.GetString("userID");
            ViewBag.UserId = int.Parse(userid);

            List<User> Alluser = _CiPlatformContext.Users.ToList();
            ViewBag.alluser = Alluser;

            List<VolunteeringVM> relatedlist = new List<VolunteeringVM>();
            VolunteeringVM volunteeringVM = new VolunteeringVM();
            ViewBag.Missiondetail = volunteeringVM;

            LandingPageVM lp = new LandingPageVM();
            lp.missions = _CiPlatformContext.Missions.ToList();
            lp.cities = _CiPlatformContext.Cities.ToList();
            lp.countries = _CiPlatformContext.Countries.ToList();
            lp.missionThemes = _CiPlatformContext.MissionThemes.ToList();
            lp.users = _CiPlatformContext.Users.ToList();
            lp.goalMissions = _CiPlatformContext.GoalMissions.ToList();
            lp.favoriteMissions = _CiPlatformContext.FavoriteMissions.ToList();
            lp.missionRatings = _CiPlatformContext.MissionRatings.ToList();
            lp.skills = _CiPlatformContext.Skills.ToList();

            return View(lp);
        }
        [HttpGet]
        public IActionResult _Missions(long id, int jpg, string SearchingMission, int? pageIndex, int cntry, int order, string country, string city, string theme, long missionId)
        {
            var userid = HttpContext.Session.GetString("userID");
            ViewBag.UserId = int.Parse(userid);

            LandingPageVM lp = new LandingPageVM();
            lp.missions = _CiPlatformContext.Missions.ToList();
            lp.cities = _CiPlatformContext.Cities.ToList();
            lp.countries = _CiPlatformContext.Countries.ToList();
            lp.missionThemes = _CiPlatformContext.MissionThemes.ToList();
            lp.users = _CiPlatformContext.Users.ToList();
            lp.goalMissions = _CiPlatformContext.GoalMissions.ToList();
            lp.favoriteMissions = _CiPlatformContext.FavoriteMissions.ToList();
            lp.missionRatings = _CiPlatformContext.MissionRatings.ToList();
            lp.missionApplications = _CiPlatformContext.MissionApplications.Where(e => e.UserId == Convert.ToInt32(userid)).ToList();


            if (userid == null)
            {
                return RedirectToAction("Login", "Home");
            }

            //Search Mission
            if (SearchingMission != null)
            {
                lp.missions = _CiPlatformContext.Missions.Where(m => m.Title.Contains(SearchingMission)).ToList();

                if (lp.missions.Count() == 0)
                {
                    return RedirectToAction("NoMissionFound", "Home");
                }
            }

            //filter
            if (country != null)
            {
                string[] countryText = country.Split(',');
                lp.missions = lp.missions.Where(m => countryText.Contains(m.Country.Name)).ToList();
            }

            if (city != null)
            {
                string[] cityText = city.Split(',');
                lp.missions = lp.missions.Where(m => cityText.Contains(m.City.Name)).ToList();
            }

            if (theme != null)
            {
                string[] themeText = theme.Split(',');
                lp.missions = lp.missions.Where(m => themeText.Contains(m.Theme.Title)).ToList();
            }


            //Order By
            switch (order)
            {
                case 1:
                    lp.missions = lp.missions.OrderBy(e => e.Title).ToList();
                    break;
                case 2:
                    lp.missions = lp.missions.OrderByDescending(e => e.StartDate).ToList();
                    break;
                case 3:
                    lp.missions = lp.missions.OrderBy(e => e.EndDate).ToList();
                    break;

                case 4:
                    lp.missions = lp.missions.OrderBy(e => e.Availability).ToList();
                    break;

                case 5:
                    lp.missions = lp.missions.OrderBy(e => e.ThemeId).ToList();
                    break;
                    //default:
                    //    mission = _CiPlatformContext.Missions.OrderBy(e => e.Theme).ToList();
                    //    break;
            }

            //Pagination
            ViewBag.missionCount = lp.missions.Count();
            const int pageSize = 9;
            if (jpg < 1)
            {
                jpg = 1;
            }
            int recsCount = lp.missions.Count();
            var pager = new Pager(recsCount, jpg, pageSize);
            int recSkip = (jpg - 1) * pageSize;
            var data = lp.missions.Skip(recSkip).Take(pager.PageSize).ToList();
            this.ViewBag.pager = pager;
            ViewBag.missionTempDate = data;
            lp.missions = data.ToList();
            ViewBag.TotalMission = recsCount;

            //Average Rating
            //int finalrating = 0;
            ////var volmission = _CiPlatformContext.Missions.FirstOrDefault(m => m.MissionId == missionId);
            ////var ratinglist = _Interface.missionRatings().Where(m => m.MissionId == volmission.MissionId).ToList();
            //if (ratinglist.Count() > 0)
            //{
            //    int rat = 0;
            //    foreach (var m in ratinglist)
            //    {
            //        rat = rat + Convert.ToInt32(m.Rating);


            //    }
            //    finalrating = rat / ratinglist.Count();
            //}
            //ViewBag.finalrating = finalrating;
            //ViewBag.totalvol = ratinglist.Count();
            ////ViewBag.Missiondetail = volunteeringVM;
            ///



            //Add to favroite
            var fav=lp.favoriteMissions.FirstOrDefault(u=> u.UserId== Convert.ToInt32(userid)  && u.MissionId== missionId);
            ViewBag.fav = fav;

            return PartialView("_Missions", lp);
        }


        //Volunteering Missions Model
        public IActionResult volunteering(long missionid, long id, long missionId)
        {
            List<Mission> mission = _CiPlatformContext.Missions.ToList();
            ViewBag.listofmission = mission;

            var userid = HttpContext.Session.GetString("userID");
            ViewBag.UserId = int.Parse(userid);

            List<User> Alluser = _CiPlatformContext.Users.ToList();
            ViewBag.alluser = Alluser;

            var ratings =_Interface.missionRatings().FirstOrDefault(MR => MR.MissionId == missionId && MR.UserId == int.Parse(userid));

            ViewBag.user = _CiPlatformContext.Users.FirstOrDefault(e => e.UserId == id);
            List<VolunteeringVM> relatedlist = new List<VolunteeringVM>();

            IEnumerable<Mission> objMis = _CiPlatformContext.Missions.ToList();
            IEnumerable<Comment> objComm = _CiPlatformContext.Comments.ToList();
            IEnumerable<Mission> selected = _CiPlatformContext.Missions.Where(m => m.MissionId == missionid).ToList();

            var volmission = _CiPlatformContext.Missions.FirstOrDefault(m => m.MissionId == missionid);
            var theme = _CiPlatformContext.MissionThemes.FirstOrDefault(m => m.MissionThemeId == volmission.ThemeId);
            var City = _CiPlatformContext.Cities.FirstOrDefault(m => m.CityId == volmission.CityId);
            var prevRating = _CiPlatformContext.MissionRatings.Where(e => e.MissionId == missionid && e.UserId == id).FirstOrDefault();

            var themeobjective = _CiPlatformContext.GoalMissions.FirstOrDefault(m => m.MissionId == missionid);
            string[] Startdate = volmission.StartDate.ToString().Split(" ");
            string[] Enddate = volmission.EndDate.ToString().Split(" ");
            VolunteeringVM volunteeringVM = new VolunteeringVM();
            volunteeringVM.MissionId = missionid;
            volunteeringVM.Title = volmission.Title;
            volunteeringVM.ShortDescription = volmission.ShortDescription;
            volunteeringVM.OrganizationName = volmission.OrganizationName;
            volunteeringVM.Description = volmission.Description;
            volunteeringVM.OrganizationDetail = volmission.OrganizationDetail;
            volunteeringVM.Availability = volmission.Availability;
            volunteeringVM.MissionType = volmission.MissionType;
            volunteeringVM.Cityname = City.Name;
            volunteeringVM.Themename = theme.Title;
            volunteeringVM.EndDate = Enddate[0];
            volunteeringVM.StartDate = Startdate[0];
            volunteeringVM.favoriteMissions= _CiPlatformContext.FavoriteMissions.ToList();    
            volunteeringVM.Rating = ratings != null ? Convert.ToInt64(ratings.Rating) : 0;
            volunteeringVM.GoalObjectiveText = themeobjective.GoalObjectiveText;
            var fav = volunteeringVM.favoriteMissions.FirstOrDefault(u => u.UserId == Convert.ToInt32(userid) && u.MissionId == missionId);
            ViewBag.fav = fav;

            //Average Rating
            int finalrating = 0;
            var ratinglist = _Interface.missionRatings().Where(m => m.MissionId == volmission.MissionId).ToList();
            if (ratinglist.Count() > 0)
            {
                int rat = 0;
                foreach (var m in ratinglist)
                {
                    rat = rat + Convert.ToInt32(m.Rating);


                }
                finalrating = rat / ratinglist.Count();
            }
            ViewBag.finalrating=finalrating;
            ViewBag.totalvol = ratinglist.Count();
            ViewBag.Missiondetail = volunteeringVM;

            //Recent Volunteers
            //List<VolunteeringVM> recentvolunteredlist = new List<VolunteeringVM>();

            //var recentvoluntered = from U in IUser.users() join MA in IUser.applications() on U.UserId equals MA.UserId where MA.MissionId == missionid select U;
            //foreach (var item in recentvoluntered)
            //{


            //    recentvolunteredlist.Add(new VolunteeringVM
            //    {
            //        username = item.FirstName,
            //    });

            //}
            //ViewBag.recentvolunteered = recentvolunteredlist;


            //Related Missions
            var relatedmission = _CiPlatformContext.Missions.Where(m => m.ThemeId == volmission.ThemeId && m.MissionId != missionid).ToList();
            foreach (var item in relatedmission.Take(3))
            {

                var relcity = _CiPlatformContext.Cities.FirstOrDefault(m => m.CityId == item.CityId);
                var reltheme = _CiPlatformContext.MissionThemes.FirstOrDefault(m => m.MissionThemeId == item.ThemeId);
                var relgoalobj = _CiPlatformContext.GoalMissions.FirstOrDefault(m => m.MissionId == item.MissionId);
                string[] Startdate1 = item.StartDate.ToString().Split(" ");
                string[] Enddate2 = item.EndDate.ToString().Split(" ");

                relatedlist.Add(new VolunteeringVM
                {
                    MissionId = item.MissionId,
                    Cityname = relcity.Name,
                    Themename = reltheme.Title,
                    Title = item.Title,
                    ShortDescription = item.ShortDescription,
                    StartDate = Startdate1[0],
                    EndDate = Enddate2[0],
                    Availability = item.Availability,
                    OrganizationName = item.OrganizationName,
                    GoalObjectiveText = relgoalobj.GoalObjectiveText,
                    MissionType = item.MissionType,
                }
                );
                ;
            }
            ViewBag.relatedmission = relatedlist.Take(3);
            return View(selected);
        }

        //Story Listing
        public IActionResult Storylisting()
        {

            Storylist storylist = new Storylist();
            //storylist is the object of class Storylist(it's in CIentities1)
            storylist.Stories = _CiPlatformContext.Stories.ToList();

            storylist.User8 = _CiPlatformContext.Users.ToList();
            //User8 is the name defined in class by us which equals ton the context file's -> Users 
            //so all the data from context file comes into variable or object called User8

            storylist.Mission8 = _CiPlatformContext.Missions.ToList();
            return View(storylist);
        }

        //Story Detail
        public IActionResult StoryDetail()
        {
            //Reccomend to Coworker
            List<User> Alluser = _CiPlatformContext.Users.ToList();
            ViewBag.alluser = Alluser;

            List<VolunteeringVM> relatedlist = new List<VolunteeringVM>();
            VolunteeringVM volunteeringVM = new VolunteeringVM();
            ViewBag.Missiondetail = volunteeringVM;
            return View();
        }

        public IActionResult nomissionfound()
        {
            return View();
        }

        public IActionResult Storyshare()
        {
            return View();
        }

        private void ToList()
        {
            throw new NotImplementedException();
        }

        public IActionResult Privacy()
        {
            return View();
        }


        //--------------------Error--------------------
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //Add to favrouite
        public IActionResult addToFavourite(int missonid)
        {
            var userId = HttpContext.Session.GetString("userID");
            ViewBag.UserId = int.Parse(userId);

            //add favourite mission data
            if (missonid != null)
            {
                //var tempFav = _IUser.favoriteMissions().Where(e => (e.MissionId == missonid) && (e.UserId == Convert.ToInt32(userId))).FirstOrDefault();
                _Interface.FavMission(missonid, Convert.ToInt32(userId));

                //if (tempFav == null)
                //{
                // _IUser.addfav(missonid, Convert.ToInt32(userId));
                // FavoriteMission fm = new FavoriteMission();
                // fm.UserId = Convert.ToInt32(userId);
                // fm.MissionId = missonid;
                //_CIDbContext.Add(fm);
                //}
                //else
                //{
                // _CIDbContext.Remove(tempFav);
                //}
                //_CIDbContext.SaveChanges();

            }
            return RedirectToAction("Volunteering", new { id = int.Parse(userId), missionid = missonid });

        }

        //favroite landing page
        public IActionResult addToFavouriteLanding(int missonid)
        {
            var userId = HttpContext.Session.GetString("userID");
            ViewBag.UserId = int.Parse(userId);

            //add favourite mission data
            if (missonid != null)
            {
                //var tempFav = _IUser.favoriteMissions().Where(e => (e.MissionId == missonid) && (e.UserId == Convert.ToInt32(userId))).FirstOrDefault();
                _Interface.FavMission(missonid, Convert.ToInt32(userId));

                //if (tempFav == null)
                //{
                // _IUser.addfav(missonid, Convert.ToInt32(userId));
                // FavoriteMission fm = new FavoriteMission();
                // fm.UserId = Convert.ToInt32(userId);
                // fm.MissionId = missonid;
                //_CIDbContext.Add(fm);
                //}
                //else
                //{
                // _CIDbContext.Remove(tempFav);
                //}
                //_CIDbContext.SaveChanges();

            }
            return RedirectToAction("Volunteering", new { id = int.Parse(userId), missionid = missonid });

        }

        //---------------------Comments---------------------------
        public IActionResult PostComment(int missionId, string Content)
        {
            Comment objComment = new Comment();
            objComment.UserId = Convert.ToInt64(HttpContext.Session.GetString("userID"));
            objComment.MissionId = missionId;
            objComment.Comment1 = Content;
            objComment.CreatedAt = DateTime.Now;
            _CiPlatformContext.Comments.Add(objComment);
            _CiPlatformContext.SaveChanges();
            return RedirectToAction("volunteering", new { id = Convert.ToInt64(HttpContext.Session.GetString("userID")), missionid = missionId });
        }


        ////user details
        //[HttpPost]
        //public IActionResult GetDetails()
        //{
        //    LandingPageVM umodel = new LandingPageVM();

        //    //umodel.Name = HttpContext.Request.Form["txtName"].ToString();
        //    //umodel.Age = Convert.ToInt32(HttpContext.Request.Form["txtAge"]);
        //    //umodel.City = HttpContext.Request.Form["txtCity"].ToString();
        //    umodel.stories = _CiPlatformContext.Stories.ToList();
        //    var lp = umodel.stories;

        //    lp.

        //    int result = umodel.SaveDetails();
        //    if (result > 0)
        //    {
        //        ViewBag.Result = "Data Saved Successfully";
        //    }
        //    else
        //    {
        //        ViewBag.Result = "Something Went Wrong";
        //    }
        //    return View("Profile");
        //}

        //-----------------Reccomend to coworker----------------------

        [HttpPost]
        public async Task<IActionResult> Sendmail(long[] userid, int id)
        {


            foreach (var item in userid)
            {
                var user = _CiPlatformContext.Users.FirstOrDefault(u => u.UserId == item);
                var resetLink = Url.Action("Volunteering", "Home", new { user = user.UserId, missionId = id }, Request.Scheme);

                var fromAddress = new MailAddress("officehl1882@gmail.com", "Sender Name");
                var toAddress = new MailAddress(user.Email);
                var subject = "Password reset request";
                var body = $"Hi,<br /><br />This is to <br /><br /><a href='{resetLink}'>{resetLink}</a>";
                var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                var smtpClient = new SmtpClient("smtp.gmail.com", 587)
                {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("officehl1882@gmail.com", "yedkuuhuklkqfzwx"),
                    EnableSsl = true
                };
                smtpClient.Send(message);

            }
            return Json(new { success = true });
        }

        //-----------------Rating-----------------
        public IActionResult Addrating(int rating, long missionId, long Id )
        {
            MissionRating ratingExists = _Interface.MissionratingByUserid_Missionid(Id, missionId);
            if (ratingExists != null)
            {
                _Interface.updaterating(ratingExists, rating);
                // return Json(new { success = true, ratingExists, isRated = true });
            }
            else
            {
                _Interface.addratings(rating, Id, missionId);
                //return Json(new { success = true, ratingele, isRated = true });
            }
            return RedirectToAction("Volunteering", new { id = Id, missionId = missionId });
        }


    }
}