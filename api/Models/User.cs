namespace api.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Biography { get; set; }
        public string Created_At { get; set; }
        public string Removed_At { get; set; }
        public string Avatar { get; set; }
    }
}

