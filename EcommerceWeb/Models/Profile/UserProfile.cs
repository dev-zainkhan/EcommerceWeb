namespace EcommerceWeb.Models.Profile
{
    public class UserProfile
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public string ProfileImage { get; set; }
        public string Role { get; set; }
    }
}