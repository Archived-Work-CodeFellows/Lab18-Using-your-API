using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab18ConsumeApi.Models
{
    public class TaskViewModel
    {
        public IEnumerable<SelectListItem> ToDoListNames { get; set; }
        public IEnumerable<ToDoList> ToDoLists { get; set; }
        public ToDoItem ToDo { get; set; }
        /// <summary>
        /// Method used to take in JSON strings to deserialize into useable
        /// objects
        /// </summary>
        /// <param name="id">ID to target specific toDos</param>
        /// <param name="list">JSON string</param>
        /// <param name="task">JSON string</param>
        /// <returns>A TaskViewModel object to be used on Views</returns>
        public static TaskViewModel GetDetails(long id, string list, string task)
        {
            TaskViewModel tvm = new TaskViewModel();

            string jToken = JToken.Parse(list).ToString();
            var lists = JsonConvert.DeserializeObject<List<ToDoList>>(jToken);

            string jToken2 = JToken.Parse(task).ToString();
            tvm.ToDo = JsonConvert.DeserializeObject<ToDoItem>(jToken2);

            tvm.ToDoListNames = new SelectList(
                lists.Select(n => n.Name)
                );
            tvm.ToDoLists = lists;
            return tvm;
        }
    }
}
