using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Lab18ConsumeApi.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Lab18ConsumeApi.Controllers
{
    public class ToDoController : Controller
    {
        public async Task<IActionResult> Index()
        {
            string value = await APItodoItemsGet();
            string jToken = JToken.Parse(value).ToString();
            var toDos = JsonConvert.DeserializeObject<List<ToDoItem>>(jToken);

            return View(toDos);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            HomeController hc = new HomeController();
            return View(ListsViewModel.GetLists(0, await hc.APIListGet()));
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("ID,Name,IsDone,ToDoList")]ToDoItem toDo)
        {
            HomeController hc = new HomeController();
            ListsViewModel lvm = ListsViewModel.GetLists(toDo.ID, await hc.APIListGet());
            ToDoList getList = lvm.ToDoLists.FirstOrDefault(i => i.Name == toDo.ToDoList);
            toDo.ListID = getList.ID;

            if (ModelState.IsValid)
            {

                await APITaskPost(toDo);
                return RedirectToAction("Index");
            }
            return View(lvm);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id.HasValue)
            {
                HomeController hc = new HomeController();
                return View(TaskViewModel.GetDetails(id.Value, await hc.APIListGet(), await APItodoGetOne(id.Value)));
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, [Bind("ID,Name,IsDone,ToDoList")]ToDoItem toDo)
        {
            HomeController hc = new HomeController();
            ListsViewModel lvm = ListsViewModel.GetLists(id, await hc.APIListGet());
            ToDoList getList = lvm.ToDoLists.FirstOrDefault(i => i.Name == toDo.ToDoList);
            toDo.ListID = getList.ID;

            if (ModelState.IsValid)
            {
                toDo.ID = id;
                await APITaskPut(toDo.ID, toDo);
                return RedirectToAction("Index", "ToDo");
            }
            return View(TaskViewModel.GetDetails(id, await hc.APIListGet(), await APItodoGetOne(id)));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            HomeController hc = new HomeController();
            TaskViewModel tvm = TaskViewModel.GetDetails(id, await hc.APIListGet(), await APItodoGetOne(id));
            
            if(tvm.ToDo == null)
            {
                return NotFound();
            }
            await APItodoDelete(id);
            return RedirectToAction("Index");
        }

        public async Task<HttpStatusCode> APITaskPost(ToDoItem item)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://eisj-todoapi.azurewebsites.net");
                var response = await client.PostAsJsonAsync("/api/todo", item);
                return response.StatusCode;
            }
        }
        public async Task<HttpStatusCode> APITaskPut(long id, ToDoItem item)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://eisj-todoapi.azurewebsites.net");
                var response = await client.PutAsJsonAsync($"/api/todo/{id}", item);
                return response.StatusCode;
            }
        }

        public async Task<string> APItodoItemsGet()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://eisj-todoapi.azurewebsites.net");
                var response = client.GetAsync("/api/todo").Result;

                if (response.EnsureSuccessStatusCode().IsSuccessStatusCode)
                {
                    string todoItemResult = await response.Content.ReadAsStringAsync();
                    return todoItemResult;
                }
                return "";
            }
        }
        public async Task<string> APItodoGetOne(long? id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://eisj-todoapi.azurewebsites.net");
                var response = client.GetAsync($"/api/todo/{id.Value}").Result;

                if (response.EnsureSuccessStatusCode().IsSuccessStatusCode)
                {
                    string todoItemResult = await response.Content.ReadAsStringAsync();
                    return todoItemResult;
                }
                return "";
            }
        }
        public async Task<string> APItodoDelete(long? id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://eisj-todoapi.azurewebsites.net");
                var response = client.DeleteAsync($"/api/todo/{id.Value}").Result;

                if (response.EnsureSuccessStatusCode().IsSuccessStatusCode)
                {
                    string todoItemResult = await response.Content.ReadAsStringAsync();
                    return todoItemResult;
                }
                return "";
            }
        }
    }
}
