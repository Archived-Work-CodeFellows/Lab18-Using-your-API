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
        /// <summary>
        /// Action that uses the APItodoItemsGet action
        /// to GET all ToDos from the API and deserializes the JSON
        /// to be viewed
        /// </summary>
        /// <returns>Deserialized JSON object</returns>
        public async Task<IActionResult> Index()
        {
            string value = await APItodoItemsGet();
            string jToken = JToken.Parse(value).ToString();
            var toDos = JsonConvert.DeserializeObject<List<ToDoItem>>(jToken);

            return View(toDos);
        }

        /// <summary>
        /// Action that is used to GET the appropriate information from the API
        /// and pass that object to the View
        /// </summary>
        /// <returns>ListsViewModel</returns>
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            HomeController hc = new HomeController();
            return View(ListsViewModel.GetLists(0, await hc.APIListGet()));
        }
        /// <summary>
        /// Action that uses a POST to grab user inputted information that is
        /// them passed to the APITaskPost action to POST to the API
        /// </summary>
        /// <param name="toDo">ToDoItem object</param>
        /// <returns>Either redirects back to Index Action if ModelState is valid or reloads page</returns>
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
        /// <summary>
        /// Action that uses a GET to retreive the details based on the given idea
        /// </summary>
        /// <param name="id">id in route that is used to make request</param>
        /// <returns>A TaskViewModel if the id has value else redirects to Index action</returns>
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
        /// <summary>
        /// Action that uses a POST to update the current ToDoItem being viewed.
        /// </summary>
        /// <param name="id">id of the current ToDoItem</param>
        /// <param name="toDo">ToDoItem object that is park of the TaskViewModel</param>
        /// <returns>Redirects to Index if successful or reloads page if there is an error in the ModelState</returns>
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
        /// <summary>
        /// Action that takes the given id and will delete that task.
        /// </summary>
        /// <param name="id">id of the ToDoItem</param>
        /// <returns>NotFound if the ToDo is null or Redirects to Index if successful deletion</returns>
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
        /// <summary>
        /// Action that uses a POST request to the API 
        /// </summary>
        /// <param name="item">ToDoItem to be created</param>
        /// <returns>Status Code</returns>
        public async Task<HttpStatusCode> APITaskPost(ToDoItem item)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://eisj-todoapi.azurewebsites.net");
                var response = await client.PostAsJsonAsync("/api/todo", item);
                return response.StatusCode;
            }
        }
        /// <summary>
        /// Action that uses a PUT request to the API to update or create
        /// </summary>
        /// <param name="id">ID of the ToDoItem</param>
        /// <param name="item">ToDoItem object to be Updated</param>
        /// <returns>Status Code</returns>
        public async Task<HttpStatusCode> APITaskPut(long id, ToDoItem item)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://eisj-todoapi.azurewebsites.net");
                var response = await client.PutAsJsonAsync($"/api/todo/{id}", item);
                return response.StatusCode;
            }
        }
        /// <summary>
        /// Action that uses a GET request to the API to grab all ToDoItems
        /// </summary>
        /// <returns>JSON string</returns>
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
        /// <summary>
        /// Action that uses a GET request to the API to grab a specific ToDoItem
        /// </summary>
        /// <param name="id">ID of ToDoItem</param>
        /// <returns>JSON string</returns>
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
        /// <summary>
        /// Action that uses a DELETE request to the API to remove specific ToDoItem
        /// </summary>
        /// <param name="id">ID of ToDoItem</param>
        /// <returns>A string status</returns>
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
