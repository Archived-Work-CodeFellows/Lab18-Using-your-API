using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Lab18ConsumeApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lab18ConsumeApi.Controllers
{
    public class ToDoListController : Controller
    {
        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create([Bind("ID,Name,IsDone")]ToDoList toDoList)
        {
            if (ModelState.IsValid)
            {
                await APIListPost(toDoList);
                return RedirectToAction("Index","Home");
            }
            return View();
        }

        public async Task<HttpStatusCode> APIListPost(ToDoList list)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://eisj-todoapi.azurewebsites.net");
                var response = await client.PostAsJsonAsync("/api/todolist", list);
                return response.StatusCode;
            }
        }
    }
}
