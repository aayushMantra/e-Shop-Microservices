using Microsoft.EntityFrameworkCore;
using ToDoAPI;

var builder = WebApplication.CreateBuilder(args);

// Add DI - AddService
builder.Services.AddDbContext<ToDoDb>(options => options.UseInMemoryDatabase("ToDoList"));

var app = builder.Build();

// Configure pipeline - UseMethod..

app.MapGet("/todoitems", async (ToDoDb db) => await db.ToDoSet.ToListAsync());

app.MapGet("/todoitems/{id}", async (int id, ToDoDb db) => await db.ToDoSet.FindAsync(id));

app.MapPost(
    "/todoitems",
    async (ToDoItem todo, ToDoDb db) =>
    {
        db.ToDoSet.Add(todo);
        await db.SaveChangesAsync();
        return Results.Created($"/todoitems/{todo.Id}", todo);
    }
);

app.MapPut(
    "/todoitems/{id}",
    async (int id, ToDoItem inputTodo, ToDoDb db) =>
    {
        var todo = await db.ToDoSet.FindAsync(id);
        if (todo is null)
            return Results.NotFound();
        todo.Name = inputTodo.Name;
        todo.IsCompleted = inputTodo.IsCompleted;
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
);

app.MapDelete(
    "/todoitems/{id}",
    async (int id, ToDoDb db) =>
    {
        if (await db.ToDoSet.FindAsync(id) is ToDoItem todo)
        {
            db.ToDoSet.Remove(todo);
            await db.SaveChangesAsync();
            return Results.NoContent();
        }
        return Results.NoContent();
    }
);

app.Run();
