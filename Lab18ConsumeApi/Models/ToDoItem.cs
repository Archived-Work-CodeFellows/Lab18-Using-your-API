using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lab18ConsumeApi.Models
{
    public class ToDoItem
    {
        public long ID { get; set; }
        [Required]
        public string Name { get; set; }
        public bool IsDone { get; set; }
        public long ListID { get; set; }
        public string ToDoList { get; set; }
    }
}
