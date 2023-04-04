using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI_Entities1.Models.ViewModel
{
    public class StoryShareVM
    {
        public List<IFormFile> attachment { get; set; }
        public long StoryId { get; set; }

        public long MissionId { get; set; }

        public long UserId { get; set; }

        public string? Title { get; set; }

        public string? editor1 { get; set; }

        public string? Status { get; set; }

        public DateTime? PublishedAt { get; set; }

        public DateTime? date { get; set; }


        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public long MissionApplicationId { get; set; }

        public DateTime AppliedAt { get; set; }

        public string ApprovalStatus { get; set; } = null!;

        public long StoryMediaId { get; set; }

        public string StoryType { get; set; } = null!;

        public string StoryPath { get; set; } = null!;

        public List<Mission> missions { get; set; }

        public List<MissionApplication> missionApplications { get; set; }

        public Story singleStory { get; set; }

        //StoryList
        public List<Story> Stories { get; set; }

        public List<City> cities { get; set; }

        public List<Country> countries { get; set; }

        public List<MissionTheme> missionThemes { get; set; }

        public List<StoryMedium> storymedia { get; set; }

        public string? FirstName { get; set; }


        //StoryDetails
        public List<User> users { get; set; }

        public int user_id { get; set; }
        public int mission_id { get; set; }

        public string username { get; set; }
        public string? ShortDescription { get; set; }

        //Story Media

        public virtual Story Story { get; set; } = null!;

    }
}
