using System.Runtime.InteropServices.JavaScript;
using GraphQL;
using GraphQL.Types;
using ToDoList.Models.Domain;
using ToDoList.Repository;
using ToDoListAPI.Type;

namespace ToDoListAPI.Mutation;

public sealed class ToDoMutation : ObjectGraphType
{
    public ToDoMutation(IToDoListRepository todoListRepository)
    {
        Field<ToDoType>("addToDo").Arguments(new QueryArguments(new QueryArgument<ToDoInputType>
            {
                Name = "todo"
            }
        )).Resolve(context =>
            {
                var todo = context.GetArgument<ToDo>("todo");
                return todoListRepository.AddToDo(todo);
            }
        );

        Field<ToDoType>("handlePerformed").Arguments(new QueryArguments(new QueryArgument<ToDoInputType>
            {
                Name = "handlePerformed"
            }
        )).Resolve(context =>
        {
            var handledToDo = context.GetArgument<ToDo>("handlePerformed");
            
            return todoListRepository.HandlePerformed(handledToDo);
        });

        Field<StringGraphType>("deleteToDo").Arguments(new QueryArguments(new QueryArgument<IdGraphType>
            {
                Name = "id"
            }
        )).Resolve(context =>
        {
            var todoId = context.GetArgument<Guid>("id");
            var todoToDelete = new ToDo
            {
                Id = todoId,
            };

            todoListRepository.DeleteToDo(todoToDelete);

            return "ToDo with id " + todoId + " has been deleted";
        });
    }
}