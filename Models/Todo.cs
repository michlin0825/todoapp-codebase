using System.ComponentModel.DataAnnotations;

namespace TodoListApp.Models
{
    public class Todo
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Description { get; set; } = string.Empty;
        
        public bool IsCompleted { get; set; }
    }
}
