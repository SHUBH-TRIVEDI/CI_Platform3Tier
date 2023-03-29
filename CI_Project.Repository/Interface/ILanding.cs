using CI_Entities1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI_Project.Repository.Interface
{
    public interface ILanding
    {
        public List<User> users();

        public List<Mission> missions();

        public List<MissionRating> missionRatings();

        public List<City> city();

        public List<Country> country();

        public List<MissionTheme> missionThemes();

        public List<GoalMission> goalMissions();

        public List<MissionApplication> missionApplications();

        public List<FavoriteMission> favoriteMissions();

        

    }
}
