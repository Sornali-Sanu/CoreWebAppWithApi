using System.Text.Json.Serialization;

namespace Client.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public DateTime JoinDate { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageName { get; set; }
        public ICollection<Experience> Experiences { get; set; } = new List<Experience>();
    }
    public class Experience
    {
        public int ExperienceId { get; set; }
        public string Title { get; set; }
        public int Duration { get; set; }
        public int EmployeeId { get; set; }
        [JsonIgnore]
        public virtual Employee? Employee { get; set; } = null!;
    }
}
