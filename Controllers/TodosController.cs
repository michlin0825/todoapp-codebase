using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TodoListApp.Data;
using TodoListApp.Models;

namespace TodoListApp.Controllers
{
    public class TodosController : Controller
    {
        private readonly TodoContext _context;
        private readonly ILogger<TodosController> _logger;

        public TodosController(TodoContext context, ILogger<TodosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Todos
        public async Task<IActionResult> Index()
        {
            try
            {
                // Log the database path for debugging
                _logger.LogInformation("Database connection string: {ConnectionString}", 
                    _context.Database.GetConnectionString() ?? "No connection string available");
                
                // Ensure database exists
                if (!await _context.Database.CanConnectAsync())
                {
                    _logger.LogWarning("Cannot connect to database, attempting to create it");
                    await _context.Database.EnsureCreatedAsync();
                    
                    // Check if we need to seed data
                    if (!_context.Todos.Any())
                    {
                        _logger.LogInformation("Seeding initial data");
                        SeedData.Initialize(_context);
                    }
                }
                
                var todos = await _context.Todos.ToListAsync();
                _logger.LogInformation("Successfully retrieved {Count} todos", todos.Count);
                return View(todos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving todos");
                
                // Add error to ModelState
                ModelState.AddModelError(string.Empty, "An error occurred while retrieving the todo list.");
                return View(new List<Todo>());
            }
        }

        // GET: Todos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Todos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Description,IsCompleted")] Todo todo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(todo);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(todo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating todo");
                ModelState.AddModelError(string.Empty, "An error occurred while creating the todo item.");
                return View(todo);
            }
        }

        // GET: Todos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var todo = await _context.Todos.FindAsync(id);
                if (todo == null)
                {
                    return NotFound();
                }
                return View(todo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving todo for edit");
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Todos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,IsCompleted")] Todo todo)
        {
            if (id != todo.Id)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(todo);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        if (!TodoExists(todo.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            _logger.LogError(ex, "Concurrency error while updating todo");
                            throw;
                        }
                    }
                    return RedirectToAction(nameof(Index));
                }
                return View(todo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating todo");
                ModelState.AddModelError(string.Empty, "An error occurred while updating the todo item.");
                return View(todo);
            }
        }

        // POST: Todos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var todo = await _context.Todos.FindAsync(id);
                if (todo != null)
                {
                    _context.Todos.Remove(todo);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting todo");
                ModelState.AddModelError(string.Empty, "An error occurred while deleting the todo item.");
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Todos/ToggleStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                var todo = await _context.Todos.FindAsync(id);
                if (todo == null)
                {
                    return NotFound();
                }

                todo.IsCompleted = !todo.IsCompleted;
                _context.Update(todo);
                await _context.SaveChangesAsync();
                
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while toggling todo status");
                ModelState.AddModelError(string.Empty, "An error occurred while updating the todo status.");
                return RedirectToAction(nameof(Index));
            }
        }

        private bool TodoExists(int id)
        {
            return _context.Todos.Any(e => e.Id == id);
        }
    }
}
