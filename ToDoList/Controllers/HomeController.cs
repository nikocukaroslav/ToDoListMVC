using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoList.Data;
using ToDoList.Models;
using ToDoList.Models.Domain;

namespace ToDoList.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ToDoListDbContext toDoListDbContext;

    public HomeController(ILogger<HomeController> logger, ToDoListDbContext toDoListDbContext)
    {
        _logger = logger;
        this.toDoListDbContext = toDoListDbContext;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var model = new HomePageViewModel
        {
            Categories = await toDoListDbContext.Category.ToListAsync(),
            ToDos = await toDoListDbContext.ToDo.Include(x => x.Category).ToListAsync(),
        };
        return View(model);
    }
    
    [HttpPost]
    public async Task<IActionResult> Index(FilterToDosByCategory filterToDosByCategory)
    {
        var filteredToDos = await toDoListDbContext.ToDo
            .Where(x => x.CategoryId == filterToDosByCategory.CategoryId)
            .ToListAsync();
        
        var model = new HomePageViewModel
        {
            Categories = await toDoListDbContext.Category.ToListAsync(), 
            ToDos = filteredToDos,
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> AddToDo(AddToDoRequest addToDoRequest)
    {
        var todo = new ToDo
        {
            Task = addToDoRequest.Task,
            CategoryId = addToDoRequest.CategoryId,
            DateToPerform = addToDoRequest.DateToPerform,
        };
        await toDoListDbContext.ToDo.AddAsync(todo);
        await toDoListDbContext.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> AddCategory(AddCategoryRequest addCategoryRequest)
    {
        var category = new Category
        {
            Name = addCategoryRequest.Name,
        };
        await toDoListDbContext.Category.AddAsync(category);
        await toDoListDbContext.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> PerformToDo(PerformTodoRequest performTodoRequest)
    {
        var todoToPerform = await toDoListDbContext.ToDo.FindAsync(performTodoRequest.Id);

        if (todoToPerform != null)
        {
            todoToPerform.IsPerformed = true;
            await toDoListDbContext.SaveChangesAsync();
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> UnperformToDo(PerformTodoRequest performTodoRequest)
    {
        var todoToUnperform = await toDoListDbContext.ToDo.FindAsync(performTodoRequest.Id);

        if (todoToUnperform != null)
        {
            todoToUnperform.IsPerformed = false;
            await toDoListDbContext.SaveChangesAsync();
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteToDo(DeleteToDoRequest deleteToDoRequest)
    {
        var todoToDelete = await toDoListDbContext.ToDo.FindAsync(deleteToDoRequest.Id);

        if (todoToDelete != null)
        {
            toDoListDbContext.Remove(todoToDelete);
            await toDoListDbContext.SaveChangesAsync();
        }

        return RedirectToAction("Index");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}