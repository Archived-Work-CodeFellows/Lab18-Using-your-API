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
        /// <summary>
        /// Action to just grab a View
        /// </summary>
        /// <returns>A View</returns>
        [HttpGet]
        public IActionResult Create() => View();
        /// <summary>
        /// Action that uses a POST to create a new ToDoList to send to the API.
        /// </summary>
        /// <param name="toDoList">ToDoList objce</param>
        /// <returns>Redirect to Index on the Home controller or reload create view</returns>
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
        /// <summary>
        /// Action that uses a GET request to grab the specific ToDoList from the API
        /// with its id
        /// </summary>
        /// <param name="id">id of ToDoList Object</param>
        /// <returns>A view passed with  ListsViewModel or Redirect to Index on the Home Controller</returns>
        [HttpGet]
        public async Task<IActionResult> Details(long? id)
        {
            if (id.HasValue)
            {
                return View(ListsViewModel.GetAList(id.Value, await APIListGetOne(id.Value)));
            }
            return RedirectToAction("Index", "Home");
        }
        /// <summary>
        /// Action that uses a GET to grab the id of ToDoList to confirm deletion
        /// </summary>
        /// <param name="id">id of ToDoList object</param>
        /// <returns>Confirm Delete View or a redirect if no id value</returns>
        [HttpGet]
        public async Task<IActionResult> Delete(long? id)
        {
            if (id.HasValue)
            {
                return View(ListsViewModel.GetAList(id.Value, await APIListGetOne(id.Value)));
            }
            return RedirectToAction("Index", "Home");
        }
        /// <summary>
        /// Action to update the current ToDoList object and use a PUT
        /// request to the API
        /// </summary>
        /// <param name="id">id of current ToDoList object</param>
        /// <param name="list">ToDoList object</param>
        /// <returns>Redirect to index on the home controller or reload page if ModelState is not valid</returns>
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
        /// <summary>
        /// Action that uses a DELETE request to remove the specific ToDoList object from the
        /// API
        /// </summary>
        /// <param name="id">id of ToDoList object</param>
        /// <returns>
        /// Redirect to a view if user attempts to remove the default list, 
        /// else Deletes list and all tasks with list
        /// </returns>
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
        /// <summary>
        /// Action for the Default List Delete catch
        /// </summary>
        /// <returns>View</returns>
        [HttpGet]
        public IActionResult DeleteDefault() => View();
        /// <summary>
        /// Action that makes a POST call to the API to create a new List
        /// </summary>
        /// <param name="list">ToDoList object</param>
        /// <returns>Status code</returns>
        public async Task<HttpStatusCode> APIListPost(ToDoList list)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://eisj-todoapi.azurewebsites.net");
                var response = await client.PostAsJsonAsync("/api/todolist", list);
                return response.StatusCode;
            }
        }
        /// <summary>
        /// Action that uses a GET to the API to retrieve a specific ToDoList
        /// </summary>
        /// <param name="id">ToDoList object ID</param>
        /// <returns>JSON string</returns>
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
        /// <summary>
        /// Action that makes a PUT request to the API to update the current
        /// ToDoList or create a new one
        /// </summary>
        /// <param name="id">Id of the ToDoList object</param>
        /// <param name="list">ToDoList object</param>
        /// <returns>Status code</returns>
        public async Task<HttpStatusCode> APIListPut(long id, ToDoList list)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://eisj-todoapi.azurewebsites.net");
                var response = await client.PutAsJsonAsync($"/api/todolist/{id}", list);
                return response.StatusCode;
            }
        }
        /// <summary>
        /// Action that makes a DELETE request to the API based on the specific
        /// ToDoList id
        /// </summary>
        /// <param name="id">ID of a specific ToDoList objec</param>
        /// <returns>JSON string</returns>
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
