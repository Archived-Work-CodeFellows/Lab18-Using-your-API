using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lab18ConsumeApi.Models
{
    public class ToDoList
    {
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
        public IEnumerable<ToDoItem> ToDoItems { get; set; }
        [Required]
        public bool IsDone { get; set; }
    }
}
