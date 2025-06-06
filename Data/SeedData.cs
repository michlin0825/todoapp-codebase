using TodoListApp.Models;

namespace TodoListApp.Data
{
    public static class SeedData
    {
        public static void Initialize(TodoContext context)
        {
            // Look for any existing todos
            if (context.Todos.Any())
            {
                return; // DB has been seeded
            }

            context.Todos.AddRange(
                new Todo
                {
                    Description = "Learn ASP.NET Core MVC",
                    IsCompleted = true
                },
                new Todo
                {
                    Description = "Build a Todo List application",
                    IsCompleted = true
                },
                new Todo
                {
                    Description = "Deploy to AWS Elastic Beanstalk",
                    IsCompleted = true
                },
                new Todo
                {
                    Description = "Add more features to the application",
                    IsCompleted = false
                },
                new Todo
                {
                    Description = "Write comprehensive tests",
                    IsCompleted = false
                }
            );

            context.SaveChanges();
        }
    }
}
