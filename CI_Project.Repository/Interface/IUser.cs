using CI_Entities1.Models;

namespace CI_Project.Repository.Interface
{
    public interface IUser
    {
        public User UserByEmail(string email);

        public User UserByEmailPassword(String email, string password);

        public PasswordReset Reset(String email, String token);

        public User addUser(User user); 

        public PasswordReset token( string email ,string token);

        public FavoriteMission FavMission(int missionId, int userId);

        public List<MissionRating> missionRatings();

        public MissionRating MissionratingByUserid_Missionid(long userid, long missionid);

        public MissionRating updaterating(MissionRating ratingExists, int rating);

        public MissionRating addratings(int rating, long id, long missionid);


    }
}
