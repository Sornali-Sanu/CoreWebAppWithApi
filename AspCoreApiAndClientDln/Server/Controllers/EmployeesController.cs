using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Server.Models;
using Server.Models.DTOs;
using System.Text.Json.Serialization;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _web;

        public EmployeesController(AppDbContext db, IWebHostEnvironment web)
        {
            _db = db;
            _web = web;
        }
        [HttpGet]
        public IActionResult GetEmployees()
        {
            List<Employee> employees = _db.Employees.Include(e => e.Experiences).ToList();
            string jsonString = JsonConvert.SerializeObject(employees, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,

            });
            return Content(jsonString, "application/json");
            //return  new JsonResult(employees);
            // return Ok(employees);
        }

        [HttpGet("{id}")]
        public IActionResult GetEmployeeById(int id) 
        {
            Employee employee = _db.Employees.Include(e => e.Experiences).SingleOrDefault(e => e.EmployeeId == id);
            if (id == null)
            { return NotFound(); }
            string jsonString = JsonConvert.SerializeObject(employee, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling=ReferenceLoopHandling.Ignore,
            });
            return Content(jsonString, "application/json");
        }
        [HttpPost]
        public async Task< IActionResult> PostEmployee([FromBody] Common obj)
        {
            string imgName = obj.ImageName + ".png";
            ImageUpload imgData= new ImageUpload();

            imgData.ImgName = "\\images\\" + imgName;
            if (obj.ImageFile?.Length > 0)
            {
                if (!Directory.Exists(_web.WebRootPath + "\\images"))
                {
                    Directory.CreateDirectory(_web.WebRootPath + "\\images\\");
                }
                string filePath=_web.WebRootPath+"\\images\\"+ imgName;
                using (FileStream stream=System.IO.File.Create(filePath))
                {
                    obj.ImageFile.CopyToAsync(stream);
                    stream.Flush();

                }
                imgData.ImgName = "/images/" + imgName;
            }
            Employee employee = new Employee();
            employee.Name = obj.Name;
            employee.IsActive = obj.IsActive;
            employee.JoinDate = obj.JoinDate;
            employee.ImageName = imgData.ImgName;
            employee.ImageUrl= imgData.ImgName;

            List<Experience> experienceList = JsonConvert.DeserializeObject<List<Experience>>(obj.Experiences!);
            employee.Experiences = experienceList;
            _db.Employees.Add(employee);
            await _db.SaveChangesAsync();
            return Ok(employee);
        }
    }
}
