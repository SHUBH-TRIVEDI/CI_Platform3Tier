using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI_Entities1.Models.ViewModel
{
    public class StoryShareVM
    {

        public long StoryId { get; set; }

        public long MissionId { get; set; }

        public long UserId { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? Status { get; set; }

        public DateTime? PublishedAt { get; set; }

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

    }
}
