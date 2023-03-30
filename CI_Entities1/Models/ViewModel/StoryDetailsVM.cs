using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI_Entities1.Models.ViewModel
{
    public class StoryDetailsVM
    {

        public int user_id { get; set; }
        public int mission_id { get; set; }

        public string username { get; set; }
        public string? ShortDescription { get; set; }

    }
}
