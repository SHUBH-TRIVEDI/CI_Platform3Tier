﻿using CI_Entities1.Data;
using CI_Entities1.Models;
using CI_Entities1.Models.ViewModel;
using CI_Platform1.Models;
using CI_Project.Repository.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace CI_Platform1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CiPlatformContext _CiPlatformContext;
        private readonly IUser _Interface;
        private readonly ILanding _Landing;

        public dynamic SearchingMission { get; private set; }

        public HomeController(CiPlatformContext CiPlatformContext, IUser UserInterface, ILanding landing)
        {
            _CiPlatformContext = CiPlatformContext;
            //_logger = logger     ILogger<HomeController> logger,;
            _Interface = UserInterface;
            _Landing = landing;
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
                ViewBag.user = user;
                if (user == null)
                {
                    return RedirectToAction("ForgetPass", "Home");
                }

                var token = Guid.NewGuid().ToString();
                _Interface.token(model.Email, token);

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
        public IActionResult LandingPage()
        {
            //User Admin Name
            if (ViewBag.user != null)
            {
                var userid = HttpContext.Session.GetString("userID");
                ViewBag.UserId = int.Parse(userid);
            }
            List<User> Alluser = _CiPlatformContext.Users.ToList();
            ViewBag.alluser = Alluser;

            List<VolunteeringVM> relatedlist = new List<VolunteeringVM>();
            VolunteeringVM volunteeringVM = new VolunteeringVM();
            ViewBag.Missiondetail = volunteeringVM;

            LandingPageVM lp = new LandingPageVM();

            lp.missions = _Landing.missions();
            lp.users = _Landing.users();
            lp.missionRatings = _Landing.missionRatings();
            lp.cities = _Landing.city();
            lp.countries = _Landing.country();
            lp.missionThemes = _Landing.missionThemes();
            lp.goalMissions = _Landing.goalMissions();
            lp.favoriteMissions = _Landing.favoriteMissions();
            lp.missionApplications = _Landing.missionApplications();

            return View(lp);
        }
        [HttpGet]
        public IActionResult _Missions(int jpg, string SearchingMission, int order, string country, string city, string theme, long missionId)
        {
            LandingPageVM lp = new LandingPageVM();

            if (ViewBag.user != null)
            {
                var userid = HttpContext.Session.GetString("userID");
                ViewBag.UserId = int.Parse(userid);

                if (userid == null)
                {
                    return RedirectToAction("Login", "Home");
                }

                var fav = lp.favoriteMissions.FirstOrDefault(u => u.UserId == Convert.ToInt32(userid) && u.MissionId == missionId);
                ViewBag.fav = fav;
            }


            lp.missions = _Landing.missions();
            lp.users = _Landing.users();
            lp.missionRatings = _Landing.missionRatings();
            lp.cities = _Landing.city();
            lp.countries = _Landing.country();
            lp.missionThemes = _Landing.missionThemes();
            lp.goalMissions = _Landing.goalMissions();
            lp.favoriteMissions = _Landing.favoriteMissions();
            lp.missionApplications = _Landing.missionApplications();





            //Search Mission
            if (SearchingMission != null)
            {
                //char.ToUpper(str[0]) + str.Substring(1)
                var search = char.ToUpper(SearchingMission[0]) + SearchingMission.Substring(1);
                lp.missions = lp.missions.Where(m => m.Title.Contains(search)).ToList();

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

            if (lp.missions.Count() == 0)
            {
                return RedirectToAction("NoMissionFound", "Home");
            }

            //Add to favrouite

            if (ViewBag.user != null)
            {

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

                    //case 6:
                    //    lp.missions = lp.missions.Where(m => m.fav == true).ToList();
                    //    break;
                    //    //lp.missions = lp.missions.OrderByDescending(e => e.fav).ToList();
                    //    //lp.missions = lp.missions.Where(m => m.fav == true).ToList();

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



            return PartialView("_Missions", lp);
        }


        //Volunteering Missions Model
        public IActionResult volunteering(long missionid, long id, long missionId)
        {
            LandingPageVM lp = new LandingPageVM();
            lp.missions = _Landing.missions();
            lp.users = _Landing.users();
            lp.cities = _Landing.city();
            lp.countries= _Landing.country();
            lp.missionThemes = _Landing.missionThemes();
            lp.missionRatings= _Landing.missionRatings();
            lp.goalMissions= _Landing.goalMissions();
            lp.missionApplications = _Landing.missionApplications();

            ViewBag.listofmission = lp.missions;
            ViewBag.alluser = lp.users;

            var userid = HttpContext.Session.GetString("userID");
            ViewBag.UserId = int.Parse(userid);


            var ratings = _Interface.missionRatings().FirstOrDefault(MR => MR.MissionId == missionId && MR.UserId == int.Parse(userid));

            ViewBag.user = lp.users.FirstOrDefault(e => e.UserId == id);
            List<VolunteeringVM> relatedlist = new List<VolunteeringVM>();

            //IEnumerable<Mission> objMis = _CiPlatformContext.Missions.ToList();
            //IEnumerable<Comment> objComm = _CiPlatformContext.Comments.ToList();
            IEnumerable<Mission> selected = lp.missions.Where(m => m.MissionId == missionid).ToList();

            var volmission = lp.missions.FirstOrDefault(m => m.MissionId == missionid);
            var theme = lp.missionThemes.FirstOrDefault(m => m.MissionThemeId == volmission.ThemeId);
            var City = lp.cities.FirstOrDefault(m => m.CityId == volmission.CityId);
            var prevRating = lp.missionRatings.Where(e => e.MissionId == missionid && e.UserId == id).FirstOrDefault();
            var themeobjective = lp.goalMissions.FirstOrDefault(m => m.MissionId == missionid);
            
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
            volunteeringVM.favoriteMissions = _CiPlatformContext.FavoriteMissions.ToList();
            volunteeringVM.Rating = ratings != null ? Convert.ToInt64(ratings.Rating) : 0;
            volunteeringVM.GoalObjectiveText = themeobjective.GoalObjectiveText;

            //volunteeringVM.missionApplications= _CiPlatformContext.MissionApplications.ToList();

            var app = lp.missionApplications.Where(u=>u.UserId==Convert.ToInt32(userid) && u.MissionId== missionid).ToList();
            if (app.Count()!=0)
            {
                volunteeringVM.isapplied = 1;
            }
            else
            {
                volunteeringVM.isapplied = 0;
            }
            ViewBag.isapplied = volunteeringVM.isapplied;

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
            ViewBag.finalrating = finalrating;
            ViewBag.totalvol = ratinglist.Count();
            ViewBag.Missiondetail = volunteeringVM;

            //Recent Volunteers
            //Related Missions
            var relatedmission =lp.missions.Where(m => m.ThemeId == volmission.ThemeId && m.MissionId != missionid).ToList();
            foreach (var item in relatedmission.Take(3))
            {

                var relcity = lp.cities.FirstOrDefault(m => m.CityId == item.CityId);
                var reltheme = lp.missionThemes.FirstOrDefault(m => m.MissionThemeId == item.ThemeId);
                var relgoalobj = lp.goalMissions.FirstOrDefault(m => m.MissionId == item.MissionId);
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
        public IActionResult Storylisting(int jpg)
        {
            StoryShareVM storylist = new StoryShareVM();
            storylist.Stories = _CiPlatformContext.Stories.ToList();

            storylist.users = _CiPlatformContext.Users.ToList();
            storylist.missions = _CiPlatformContext.Missions.ToList();
            storylist.storymedia= _CiPlatformContext.StoryMedia.ToList();


            

            //Pagination
            ViewBag.missionCount = storylist.Stories.Count();
            const int pageSize = 6;
            if (jpg < 1)
            {
                jpg = 1;
            }
            int recsCount = storylist.Stories.Count();
            var pager = new Pager(recsCount, jpg, pageSize);
            int recSkip = (jpg - 1) * pageSize;
            var data = storylist.Stories.Skip(recSkip).Take(pager.PageSize).ToList();
            this.ViewBag.pager = pager;
            ViewBag.missionTempDate = data;
            storylist.Stories = data.ToList();
            ViewBag.TotalMission = recsCount;

            return View(storylist);
        }

        //Applied
        public IActionResult Applied(int missonid)
        {
            var userid = HttpContext.Session.GetString("userID");
            ViewBag.UserId = int.Parse(userid);

            _Interface.ApplyMission(missonid, Convert.ToInt32(userid));
            //return View();
            return RedirectToAction("volunteering", new { id = Convert.ToInt64(HttpContext.Session.GetString("userID")), missionid = missonid });

        }



        public IActionResult nomissionfound()
        {
            return View();
        }


        public IActionResult _StoryList(int jpg)
        {
            StoryShareVM storylist = new StoryShareVM();

            storylist.Stories = _CiPlatformContext.Stories.ToList();

            storylist.users = _CiPlatformContext.Users.ToList();

            storylist.missions = _CiPlatformContext.Missions.ToList();
            storylist.missionThemes = _CiPlatformContext.MissionThemes.ToList();
            storylist.storymedia = _CiPlatformContext.StoryMedia.ToList();

            ViewBag.missionCount = storylist.Stories.Count();
            const int pageSize = 6;
            if (jpg < 1)
            {
                jpg = 1;
            }
            int recsCount = storylist.Stories.Count();
            var pager = new Pager(recsCount, jpg, pageSize);
            int recSkip = (jpg - 1) * pageSize;
            var data = storylist.Stories.Skip(recSkip).Take(pager.PageSize).ToList();
            this.ViewBag.pager = pager;
            ViewBag.missionTempDate = data;
            storylist.Stories = data.ToList();
            ViewBag.TotalMission = recsCount;
            return PartialView("_StoryList",storylist);
        }

        public IActionResult Storyshare(long StoryId)
        {
            var userid = HttpContext.Session.GetString("userID");
            ViewBag.UserId = int.Parse(userid);
            StoryShareVM ss = new StoryShareVM();
            ss.missions = _Landing.missions();
            ss.missionApplications = _CiPlatformContext.MissionApplications.ToList();

                StoryShareVM sl = new StoryShareVM();
            if(StoryId != 0)
            {

                sl.storymedia = _CiPlatformContext.StoryMedia.Where(e => e.StoryId == StoryId).ToList();
                sl.missionApplications = _CiPlatformContext.MissionApplications.Where(m => m.UserId == Convert.ToInt32(userid)).ToList();

                var story = _CiPlatformContext.Stories.Where(e => e.StoryId == StoryId).FirstOrDefault();
                sl.MissionId = story.MissionId;
                sl.Title = story.Title;
                sl.editor1 = story.Description;
                sl.StoryId = story.StoryId;
                sl.date = story.PublishedAt;
                return View(sl);

            }
            else
            {
                return View(sl);

            }
        }

        [HttpPost]
        public IActionResult Storyshare(StoryShareVM ss, string action, IFormFileCollection? dragdrop)
        {
            if (action == "submit")
            {
                var userid = HttpContext.Session.GetString("userID");
                ViewBag.UserId = int.Parse(userid);
                ss.missions = _Landing.missions();
                ss.missionApplications = _CiPlatformContext.MissionApplications.Where(u => u.UserId == Convert.ToInt32(userid)).ToList();

                Story stories = new Story();
                stories.UserId = Convert.ToInt64(HttpContext.Session.GetString("userID"));
                stories.MissionId = ss.MissionId;
                stories.Title = ss.Title;
                stories.Description = ss.editor1;
                stories.Status = "1";
                stories.CreatedAt = DateTime.Now;
                _CiPlatformContext.Stories.Add(stories);
                _CiPlatformContext.SaveChanges();

            //Image view
                foreach (IFormFile file in dragdrop)
                {
                    if (file != null)
                    {
                        //Set Key Name
                        string ImageName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                        //Get url To Save
                        string SavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\storyuserimg", ImageName);

                        using (var stream = new FileStream(SavePath, FileMode.Create))
                        {
                            //StoryShareVM sm = new StoryShareVM();
                            ss.StoryType = file.ContentType.ToString().Replace("image/", "");
                            ss.StoryPath = ImageName;
                            ss.CreatedAt = DateTime.Now;
                            //stories.StoryMedia.Add(sm);
                            //stories.StoryMedia.Add(ss);
                            file.CopyTo(stream);
                        }
                    }
                }
                return View(ss);
            }

            else if(action == "save")
            {
                var userid = HttpContext.Session.GetString("userID");
                ViewBag.UserId = int.Parse(userid);
                ss.missions = _Landing.missions();
                ss.missionApplications = _CiPlatformContext.MissionApplications.Where(u => u.UserId == Convert.ToInt32(userid)).ToList();

                Story stories = new Story();
                stories.UserId = Convert.ToInt64(HttpContext.Session.GetString("userID"));
                stories.MissionId = ss.MissionId;
                stories.Title = ss.Title;
                stories.Description = ss.editor1;
                stories.Status = "Draft";
                stories.CreatedAt = DateTime.Now;
                _CiPlatformContext.Stories.Add(stories);
                _CiPlatformContext.SaveChanges();
                return View(ss);

            }

            else
            {
                return View(ss);
            }
            return View(ss);
        }

        public IActionResult Draft()
        {
            var userid = HttpContext.Session.GetString("userID");
            ViewBag.UserId = int.Parse(userid);

            StoryShareVM sl= new StoryShareVM();
            sl.Stories=  _CiPlatformContext.Stories.Where(u=> u.Status=="draft" && u.UserId==Convert.ToInt64(userid)).ToList();
            sl.users = _CiPlatformContext.Users.ToList();
            sl.missions=_CiPlatformContext.Missions.ToList();
            sl.missionThemes = _CiPlatformContext.MissionThemes.ToList();
            sl.storymedia=_CiPlatformContext.StoryMedia.ToList();

            return View(sl);


            return View();
        }

        //Story Detail
        public IActionResult StoryDetail(int missionid)
        {
            var userid = HttpContext.Session.GetString("userID");
            var user = Convert.ToInt64(userid);
            ViewBag.UserId = int.Parse(userid);

            //Reccomend to Coworker
            List<User> Alluser = _CiPlatformContext.Users.ToList();
            ViewBag.alluser = Alluser;

            List<VolunteeringVM> relatedlist = new List<VolunteeringVM>();
            VolunteeringVM volunteeringVM = new VolunteeringVM();
            ViewBag.Missiondetail = volunteeringVM;

            //var volmission = lp.missions.FirstOrDefault(m => m.MissionId == missionid);
            StoryShareVM sd = new StoryShareVM();
            sd.UserId = Convert.ToInt64(HttpContext.Session.GetString("userID"));

            var users = _CiPlatformContext.Users.FirstOrDefault(u=> u.UserId == user);
            var mission = _CiPlatformContext.Missions.FirstOrDefault(u=> u.MissionId == missionid);

            sd.username = users.FirstName;

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

            }
            return RedirectToAction("Volunteering", new { id = int.Parse(userId), missionid = missonid });

        }

        //favroite landing page
        public IActionResult addToFavouriteLanding(int missonid)
        {
            if (ViewBag.user != null)
            {
                var userId = HttpContext.Session.GetString("userID");
                ViewBag.UserId = int.Parse(userId);

                //add favourite mission data
                if (missonid != null)
                {
                    //var tempFav = _IUser.favoriteMissions().Where(e => (e.MissionId == missonid) && (e.UserId == Convert.ToInt32(userId))).FirstOrDefault();
                    _Interface.FavMission(missonid, Convert.ToInt32(userId));

                }
                return RedirectToAction("_Missions", new { id = int.Parse(userId), missionid = missonid });
            }
            return View();
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
        public IActionResult Addrating(int rating, long missionId, long Id)
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