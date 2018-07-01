using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab18ConsumeApi.Models
{
    public class ListsViewModel
    {
        public IEnumerable<SelectListItem> ToDoListNames { get; set; }
        public IEnumerable<ToDoList> ToDoLists { get; set; }
        public ToDoItem ToDo { get; set; }

        public static ListsViewModel GetLists(long id, string value)
        {
            ListsViewModel lvm = new ListsViewModel();
            string jToken = JToken.Parse(value).ToString();
            var toDos = JsonConvert.DeserializeObject<List<ToDoList>>(jToken);

            lvm.ToDoListNames = new SelectList(
                toDos.Select(n => n.Name)
                );
            lvm.ToDoLists = toDos;
            return lvm;
        }
    }
}
