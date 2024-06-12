namespace TaskManagementSystem.Web.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public ICollection<Task> Tasks { get; set; } = new List<Task>();
    }
}
