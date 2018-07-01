using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Lab18ConsumeApi.Models;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;

namespace Lab18ConsumeApi.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            string value = await APIListGet();
            string jToken = JToken.Parse(value).ToString();
            var lists = JsonConvert.DeserializeObject<List<ToDoList>>(jToken);
            
            return View(lists);
        }

        public async Task<string> APIListGet()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://eisj-todoapi.azurewebsites.net");
                var response = client.GetAsync("/api/todolist").Result;

                if (response.EnsureSuccessStatusCode().IsSuccessStatusCode)
                {
                    string todolistResult = await response.Content.ReadAsStringAsync();
                    return todolistResult;
                }
                return "";
            }
        }
    }
}
