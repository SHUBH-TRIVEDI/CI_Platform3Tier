using CI_Project.Repository.Interface;
using CI_Entities1.Data;
using CI_Entities1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI_Project.Repository.Repository
{
    public class LandingRepository : ILanding
    {
        private readonly CiPlatformContext _CiPlatformContext;

        public LandingRepository(CiPlatformContext CiPlatformContext)
        {
            _CiPlatformContext = CiPlatformContext;
        }

        public List<User> users()
        {
            return _CiPlatformContext.Users.ToList();
        }

        public List<Mission> missions()
        {
            return _CiPlatformContext.Missions.ToList();
        }


        public List<MissionRating> missionRatings()
        {
            return _CiPlatformContext.MissionRatings.ToList();
        }

        public List<City> city()
        {
            return _CiPlatformContext.Cities.ToList();
        }

        public List<Country> country()
        {
            return _CiPlatformContext.Countries.ToList();
        }

        public List<MissionTheme> missionThemes()
        {
            return _CiPlatformContext.MissionThemes.ToList();
        }

        public List<GoalMission> goalMissions()
        {
            return _CiPlatformContext.GoalMissions.ToList();
        }

        public List<MissionApplication> missionApplications()
        {
            return _CiPlatformContext.MissionApplications.ToList();
        }

        public List<FavoriteMission> favoriteMissions()
        {
            return _CiPlatformContext.FavoriteMissions.ToList();
        }

    }
}
