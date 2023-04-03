using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI_Entities1.Models.ViewModel
{
    public class LandingPageVM
    {
        public List<Mission> missions { get; set; }
        public List<City> cities { get; set; }
        public List<Country> countries { get; set; }
        public List<MissionTheme> missionThemes { get; set; }
        public List<Skill> skills { get; set; }
        public List<FavoriteMission> favoriteMissions { get; set; }
        public List<MissionRating> missionRatings { get; set; }
        public List<User> users { get; set; }
        public List<GoalMission> goalMissions { get; set; }
        public List<MissionApplication> missionApplications { get; set; }

        public List<Comment> comments { get; set; }

        public List<Story> stories { get; set; }
    }
}
