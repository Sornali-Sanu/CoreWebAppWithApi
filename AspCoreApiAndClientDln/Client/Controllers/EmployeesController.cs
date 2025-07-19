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
        public async Task<IActionResult> Delete(int id)
        {
            using (var client = new HttpClient())
            {
                using (var response = await client.DeleteAsync(apiServer + $"?id={id}"))
                {
                    if (response.IsSuccessStatusCode)  
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.ApiUrl = "http://localhost:5193";
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(apiServer + $"/{id}"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        var employee = JsonConvert.DeserializeObject<Employee>(apiResponse);
                        if (employee != null)
                        {
                            var employeeViewModel = new EmployeeViewModel
                            {
                                EmployeeId = employee.EmployeeId,
                                Name = employee.Name,
                                IsActive = employee.IsActive,
                                JoinDate = employee.JoinDate,
                                ImageUrl = employee.ImageUrl,
                                Experiences = employee.Experiences.ToList()
                            };
                            return View(employeeViewModel);
                        }
                        else
                        {
                            return StatusCode((int)response.StatusCode, "Employee not found.");
                        }
                    }
                }
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Edit(EmployeeViewModel employee)
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
            else
            {
                if (!string.IsNullOrEmpty(employee.ImageUrl))
                {
                    content.Add(new StringContent(employee.ImageUrl), "ImageName");
                }
            }
            content.Add(new StringContent(obj.Experiences), "Experiences");
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.PutAsync(apiServer + $"?id={employee.EmployeeId}", content);
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
        public async Task<IActionResult> Details(int id)
        {
            ViewBag.ApiServer = "http://localhost:5193";

            Employee employee = null;

            using (var httpClient = new HttpClient())
            {
               
                var response = await httpClient.GetAsync($"{ViewBag.ApiServer}/api/employees/{id}");

                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    employee = JsonConvert.DeserializeObject<Employee>(apiResponse);
                }
            }

            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

    }


}
