﻿using System.ComponentModel.DataAnnotations;

namespace Server.Models.DTOs
{
    public class Common
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode =true,DataFormatString ="{0:yyyy-MM-dd}")]
        public DateTime JoinDate { get; set; }
        public IFormFile? ImageFile { get; set; }
        public string? ImageName { get; set; }
        public string? Experiences { get; set; } = null!;
    }
}
