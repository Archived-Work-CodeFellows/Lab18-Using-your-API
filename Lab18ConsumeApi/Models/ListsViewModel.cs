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
        public IEnumerable<ToDoItem> ToDoItems { get; set; }
        public ToDoList List { get; set; }
        public ToDoItem ToDo { get; set; }
        /// <summary>
        /// Method used to take in a JSON string and deserialize it to a useable
        /// object
        /// </summary>
        /// <param name="id">ID of a list</param>
        /// <param name="value">JSON string</param>
        /// <returns>A ListViewModel object for use on Views</returns>
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
        /// <summary>
        /// Method used to take in a JSON string and deserialize it for one specific list
        /// </summary>
        /// <param name="id">ID of ToDoList</param>
        /// <param name="value">JSON string</param>
        /// <returns>A ListViewModel object for use on Views</returns>
        public static ListsViewModel GetAList(long id, string value)
        {
            ListsViewModel lvm = new ListsViewModel();
            string jToken = JToken.Parse(value).ToString();
            lvm.List = JsonConvert.DeserializeObject<ToDoList>(jToken);
           
            return lvm;
        }
    }
}
