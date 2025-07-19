using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
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
        public async Task< IActionResult> PostEmployee([FromForm] Common obj)
        {
            ImageUpload fileApi = new ImageUpload();
            string fileName = obj.ImageName + ".png";
            fileApi.ImgName = "\\images\\" + fileName;
            if (obj.ImageFile?.Length > 0)
            {
                if (!Directory.Exists(_web.WebRootPath + "\\images"))
                {
                    Directory.CreateDirectory(_web.WebRootPath + "\\images\\");
                }
                string filePath = _web.WebRootPath + "\\images\\" + fileName;
                using (FileStream stream = System.IO.File.Create(filePath))
                {
                    obj.ImageFile.CopyTo(stream);
                    stream.Flush();
                }
                fileApi.ImgName = "/images/" + fileName;
            }
            Employee employee = new Employee();
            employee.Name = obj.Name;
            employee.IsActive = obj.IsActive;
            employee.JoinDate = obj.JoinDate;
            employee.ImageName = fileApi.ImgName;
            employee.ImageUrl= fileApi.ImgName;

            List<Experience> experienceList = JsonConvert.DeserializeObject<List<Experience>>(obj.Experiences);
            employee.Experiences = experienceList;
            _db.Employees.Add(employee);
            await _db.SaveChangesAsync();
            return Ok(employee);
        }
        //[HttpPut()]
        //public async Task<IActionResult> PutEmployee(int id, [FromForm] Common objCommon)
        //{
        //    var empObj = await _db.Employees.FindAsync(id);
        //    if (empObj == null)
        //    {
        //        return NotFound("No Employee Found");
        //    }
        //   ImageUpload fileApi= new ImageUpload();
        //    if (objCommon.ImageFile?.Length > 0)
        //    {
        //        string fileName = objCommon.ImageName + ".png";
        //        fileApi.ImgName = "\\images\\" + fileName;
        //        if (!Directory.Exists(_web.WebRootPath + "\\images\\"))
        //        {
        //            Directory.CreateDirectory(_web.WebRootPath + "\\images\\");

        //        }
        //        string filePath = _web.WebRootPath + "\\images\\" + fileName;
        //        using (FileStream stream = System.IO.File.Create(filePath))
        //        {
        //            objCommon.ImageFile.CopyTo(stream);
        //            stream.Flush();

        //        }
        //        fileApi.ImgName = "/images/" + fileName;
        //    }
        //    else {
        //        fileApi.ImgName = objCommon.ImageName;
        //    }
        //    empObj.Name= objCommon.Name;
        //    empObj.IsActive = objCommon.IsActive;
        //    empObj.JoinDate = objCommon.JoinDate;
        //    empObj.ImageName= objCommon.ImageName;
        //    empObj.ImageUrl = fileApi.ImgName;
        //    List<Experience> expList = JsonConvert.DeserializeObject<List<Experience>>(objCommon.Experiences);
        //    var existingExperiences=_db.Experiences.Where(x=>x.EmployeeId==id);
        //    _db.Experiences.RemoveRange(existingExperiences);

        //    if(expList.Any())
        //    {
        //        foreach (Experience ex in expList) {
        //            Experience Exp = new Experience
        //            {
        //                EmployeeId = ex.EmployeeId,
        //                Title = ex.Title,
        //                Duration
        //                = ex.Duration,
        //            };
        //            _db.Experiences.Add(Exp);
        //        }
        //        await _db.SaveChangesAsync();

        //    }
        //    return Ok("Employee Updated Successfully");
        //}
        [HttpPut()]
        public async Task<IActionResult> PutEmployee(int id, [FromForm] Common objCommon)
        {
            try
            {
                var empObj = await _db.Employees.FindAsync(id);
                if (empObj == null)
                {
                    return NotFound("No Employee Found");
                }

                ImageUpload fileApi = new ImageUpload();

                if (objCommon.ImageFile?.Length > 0)
                {
                    string fileName = objCommon.ImageName + ".png";
                    fileApi.ImgName = "\\images\\" + fileName;

                    string imagePath = Path.Combine(_web.WebRootPath, "images");

                    if (!Directory.Exists(imagePath))
                    {
                        Directory.CreateDirectory(imagePath);
                    }

                    string filePath = Path.Combine(imagePath, fileName);

                    using (FileStream stream = new FileStream(filePath, FileMode.Create))
                    {
                        await objCommon.ImageFile.CopyToAsync(stream);
                        stream.Flush();
                    }

                    fileApi.ImgName = "/images/" + fileName;
                }
                else
                {
                    fileApi.ImgName = objCommon.ImageName;
                }

                empObj.Name = objCommon.Name;
                empObj.IsActive = objCommon.IsActive;
                empObj.JoinDate = objCommon.JoinDate;
                empObj.ImageName = objCommon.ImageName;
                empObj.ImageUrl = fileApi.ImgName;

                // Deserialize Experiences
                List<Experience> expList = JsonConvert.DeserializeObject<List<Experience>>(objCommon.Experiences);

                var existingExperiences = _db.Experiences.Where(x => x.EmployeeId == id);
                _db.Experiences.RemoveRange(existingExperiences);

                if (expList != null && expList.Any())
                {
                    foreach (var ex in expList)
                    {
                        Experience newExp = new Experience
                        {
                            EmployeeId = id,
                            Title = ex.Title,
                            Duration = ex.Duration
                        };

                        _db.Experiences.Add(newExp);
                    }
                }

                await _db.SaveChangesAsync();
                return Ok("Employee Updated Successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Server Error: " + ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var obj =await _db.Employees.FindAsync(id);
            if (obj == null)
            
                return NotFound("Employee Not Found");
            
            _db.Employees.Remove(obj);
            await _db.SaveChangesAsync();
            return Ok("Employee Deleted Successfully");

        }
      


    }
   
}
