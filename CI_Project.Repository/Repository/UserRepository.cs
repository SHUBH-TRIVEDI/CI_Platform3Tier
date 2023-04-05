using CI_Entities1.Data;
using CI_Entities1.Models;
using CI_Platform1.Models;
using CI_Project.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI_Project.Repository.Repository
{
    public class UserRepository : IUser
    {

        private readonly CiPlatformContext _CiPlatformContext;

        public UserRepository(CiPlatformContext CiPlatformContext)
        {
            _CiPlatformContext= CiPlatformContext;
        }

        public void AddStoryMedia(string mediaType, string mediaPath, long missionId, long userId, long storyId, long sId)
        {

            StoryMedium sm = new StoryMedium();
            sm.StoryId = sId;
            sm.StoryType = mediaType;
            sm.StoryPath = mediaPath;
            _CiPlatformContext.Add(sm);
            _CiPlatformContext.SaveChanges();

        }

        public MissionApplication ApplyMission(int missonid, int userid)
        {
            MissionApplication ma = new MissionApplication();
            ma.MissionId = missonid;
            ma.UserId= userid;
            ma.ApprovalStatus = "1";
            ma.CreatedAt = DateTime.Now;
            ma.AppliedAt = DateTime.Now;

            _CiPlatformContext.Add(ma);
            _CiPlatformContext.SaveChanges();
            return ma;
        }


        public User UserByEmail(String email)
        {
            return _CiPlatformContext.Users.FirstOrDefault(x => x.Email == email);
        }

        public User UserByEmailPassword(String email, String password)
        {
            return _CiPlatformContext.Users.Where(u => u.Email == email && u.Password == password).FirstOrDefault();
        }

        public PasswordReset Reset(String email, String token)
        {
            // return _CiPlatformContext.Resetpassword.FirstOrDefault(u => u.Email == email && u.Token == token);
            return _CiPlatformContext.PasswordResets.FirstOrDefault(u => u.Email == email && u.Token == token);
        }
        public User addUser(User user)
        {
            _CiPlatformContext.Users.Add(user);
            _CiPlatformContext.SaveChanges();
            return user;
        }

        public PasswordReset token(string email, string token)
        {
            PasswordReset pr = new PasswordReset
            //ResetPassword pr = new ResetPassword
            {
                Email = email,
                Token = token
            };
            _CiPlatformContext.Add(pr);
            _CiPlatformContext.SaveChanges();
            return pr;
        }


        //Favroite mission
        public FavoriteMission FavMission(int missionId, int userId)
        {
            var tempFav = _CiPlatformContext.FavoriteMissions.Where(e => (e.MissionId == missionId) && (e.UserId == Convert.ToInt32(userId))).FirstOrDefault();
            if (tempFav == null)
            {
                FavoriteMission fav = new FavoriteMission
                {
                    MissionId = missionId,
                    UserId = userId,
                };
                _CiPlatformContext.Add(fav);

            }
            else
            {
                _CiPlatformContext.FavoriteMissions.Remove(tempFav);
            }
            _CiPlatformContext.SaveChanges();
            return tempFav;
        }
        public List<MissionRating> missionRatings()
        {
            return _CiPlatformContext.MissionRatings.ToList();
        }

        public MissionRating MissionratingByUserid_Missionid(long userid, long missionid)
        {
            return _CiPlatformContext.MissionRatings.FirstOrDefault(fm => fm.UserId == userid && fm.MissionId == missionid);
        }

        public MissionRating updaterating(MissionRating ratingExists, int rating)
        {
            ratingExists.Rating = rating;
            _CiPlatformContext.Update(ratingExists);
            _CiPlatformContext.SaveChanges();
            return ratingExists;
        }
        public MissionRating addratings(int rating, long id, long missionid)
        {
            var ratingele = new MissionRating();
            ratingele.Rating = rating;
            ratingele.UserId = id;
            ratingele.MissionId = missionid;
            _CiPlatformContext.Add(ratingele);
            _CiPlatformContext.SaveChanges();
            return ratingele;
        }
    }
}
