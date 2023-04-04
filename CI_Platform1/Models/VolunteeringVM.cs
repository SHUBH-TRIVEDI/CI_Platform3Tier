using CI_Entities1.Models;

namespace CI_Platform1.Models
{
    public class VolunteeringVM
    {
        public long MissionId { get; set; }

        public long CityId { get; set; }
        public string Cityname { get; set; }
        public long CountryId { get; set; }
        public string Countryname { get; set; }

        public long ThemeId { get; set; }
        public string Themename { get; set; }


        public string Title { get; set; } = null!;

        public string? ShortDescription { get; set; }

        public string? Description { get; set; }

        public string? StartDate { get; set; }

        public string? EndDate { get; set; }
        public string MissionType { get; set; } = null!;
        public string? OrganizationName { get; set; }

        public string? OrganizationDetail { get; set; }

        public string? Availability { get; set; }
        public string? GoalObjectiveText { get; set; }

        public string GoalValue { get; set; } = null!;
        public string UserPrevRating { get; set; }

        // ..............comment
        public List<Comment> comments  { get; set; }

        public long user_id { get; set; }
        public int mission_id { get; set; }
        public string content { get; set; }
        public DateTime created_at { get; set; }


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

        public long? Rating { get; set; }

        public int? isapplied { get; set; }


    }
}
