using Client.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using static System.Net.WebRequestMethods;
using Newtonsoft.Json;
using Client.Models.viewModels;

namespace Client.Controllers
{
    public class EmployeesController : Controller
    {
        private string apiServer = "http://localhost:5193/api/Employees";
        public async Task<IActionResult> Index()
        {
            ViewBag.ApiServer = "http://localhost:5193";
            List<Employee>employees= new List<Employee>();
            using (var httpClient = new HttpClient())
            {
                using (var res = await httpClient.GetAsync(apiServer))
                { 
                    string apiResponse=await res.Content.ReadAsStringAsync();
                    employees=JsonConvert.DeserializeObject<List<Employee>>(apiResponse);
                }
            }

                return View(employees);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            EmployeeViewModel model = new EmployeeViewModel();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Create(EmployeeViewModel employee)
        {
            common obj = new common();
            obj.Experiences = JsonConvert.SerializeObject(employee.Experiences, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(employee.EmployeeId.ToString()), "EmployeeId");
            content.Add(new StringContent(employee.Name.ToString()), "Name");
            content.Add(new StringContent(employee.IsActive.ToString()), "IsActive");
            content.Add(new StringContent(employee.JoinDate.ToString("yyyy-MM-dd")), "JoinDate");
            if (employee.ProfileFile != null)
            {
                content.Add(new StreamContent(employee.ProfileFile.OpenReadStream()), "ImageFile", employee.ProfileFile.FileName);
                content.Add(new StringContent(employee.ProfileFile.FileName), "ImageName");
            }
            content.Add(new StringContent(obj.Experiences), "Experiences");
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.PostAsync(apiServer, content);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return StatusCode((int)response.StatusCode, "Error occurred while creating employee.");
                    }
                }
            }
            catch (Exception ex)
            {

                return StatusCode((500), ex.Message);
            }

        }
    }
}
