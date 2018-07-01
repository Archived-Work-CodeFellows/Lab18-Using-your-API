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

        [HttpGet]
        public async Task<IActionResult> Details(long? id)
        {
            if (id.HasValue)
            {
                return View(ListsViewModel.GetAList(id.Value, await APIListGetOne(id.Value)));
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(long? id)
        {
            if (id.HasValue)
            {
                return View(ListsViewModel.GetAList(id.Value, await APIListGetOne(id.Value)));
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Update(long id, [Bind("ID,Name,IsDone")]ToDoList list)
        {
            ListsViewModel lvm = ListsViewModel.GetAList(id, await APIListGetOne(id));
            if (ModelState.IsValid)
            {
                list.ID = (int)id;
                await APIListPut(id,list);
                return RedirectToAction("Index", "Home");
            }
            return View(ListsViewModel.GetAList(id, await APIListGetOne(id)));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(long id)
        {
            if(id == 1)
            {
                return RedirectToAction("DeleteDefault");
            }
            await APItodoListDelete(id);
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult DeleteDefault() => View();

        public async Task<HttpStatusCode> APIListPost(ToDoList list)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://eisj-todoapi.azurewebsites.net");
                var response = await client.PostAsJsonAsync("/api/todolist", list);
                return response.StatusCode;
            }
        }

        public async Task<string> APIListGetOne(long? id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://eisj-todoapi.azurewebsites.net");
                var response = client.GetAsync($"/api/todolist/{id.Value}").Result;

                if (response.EnsureSuccessStatusCode().IsSuccessStatusCode)
                {
                    string todolistResult = await response.Content.ReadAsStringAsync();
                    return todolistResult;
                }
                return "";
            }
        }

        public async Task<HttpStatusCode> APIListPut(long id, ToDoList list)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://eisj-todoapi.azurewebsites.net");
                var response = await client.PutAsJsonAsync($"/api/todolist/{id}", list);
                return response.StatusCode;
            }
        }

        public async Task<string> APItodoListDelete(long? id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://eisj-todoapi.azurewebsites.net");
                var response = client.DeleteAsync($"/api/todolist/{id.Value}").Result;

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
