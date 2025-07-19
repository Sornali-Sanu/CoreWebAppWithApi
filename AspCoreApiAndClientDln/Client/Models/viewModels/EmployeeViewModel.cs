using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Client.Models.viewModels
{
    public class EmployeeViewModel
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode =true,DataFormatString ="{0:dd-MM-yyyy}")]
        public DateTime JoinDate { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageName { get; set; }
        public IFormFile ? ProfileFile { get; set; }
        public ICollection<Experience> Experiences { get; set; } = new List<Experience>();
       
        public string Title { get; set; }
        public int Duration { get; set; }

     
    }
}
