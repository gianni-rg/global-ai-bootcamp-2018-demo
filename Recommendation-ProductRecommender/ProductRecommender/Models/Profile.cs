namespace ProductRecommender.Models
{
    using System.Collections.Generic;

    public class Profile
    {
        public int ProfileID { get; set; }
        public string ProfileImageName { get; set; }
        public string ProfileName { get; set; }
        public List<int> ProfileProductBought { get; set; }
    }
}
