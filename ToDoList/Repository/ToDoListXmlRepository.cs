﻿using System.Xml;
using ToDoList.Models;
using ToDoList.Models.Domain;

namespace ToDoList.Data
{
    public class ToDoListXmlRepository : IToDoListRepository
    {
        private readonly XmlStorageContext _xmlStorageContext;

        public ToDoListXmlRepository(XmlStorageContext xmlStorageContext)
        {
            this._xmlStorageContext = xmlStorageContext;
        }

        public async Task<List<ToDo>> GetAllToDos()
        {
            XmlDocument document = new XmlDocument();
            document.Load(_xmlStorageContext.XmlStoragePath);
            XmlNodeList? nodes = document.SelectNodes("/database/todos/todo");

            List<ToDo> todos = new();
            if (nodes != null)
                foreach (XmlNode node in nodes)
                {
                    var id = node.SelectSingleNode("id")?.InnerText;
                    var task = node.SelectSingleNode("task")?.InnerText;
                    var categoryName = node.SelectSingleNode("categoryName")?.InnerText;
                    var isPerformed = node.SelectSingleNode("isPerformed")?.InnerText;
                    var dateToPerform = node.SelectSingleNode("dateToPerform")?.InnerText;
                    if (task != null && categoryName != null && isPerformed != null && id != null)
                    {
                        ToDo todo = new ToDo
                        {
                            Id = Guid.Parse(id),
                            Task = task,
                            CategoryName = categoryName,
                            IsPerformed = bool.Parse(isPerformed),
                            DateToPerform = !string.IsNullOrEmpty(dateToPerform)
                                ? DateTime.Parse(dateToPerform)
                                : (DateTime?)null,
                        };
                        todos.Add(todo);
                    }
                }

            return todos;
        }

        public async Task<List<Category>> GetAllCategories()
        {
            XmlDocument document = new XmlDocument();
            document.Load(_xmlStorageContext.XmlStoragePath);
            XmlNodeList? nodes = document.SelectNodes("/database/categories/category");

            List<Category> categories = new();

            if (nodes != null)
                foreach (XmlNode node in nodes)
                {
                    var name = node.SelectSingleNode("name")?.InnerText;

                    if (name != null)
                    {
                        Category category = new Category
                        {
                            Name = name,
                        };
                        categories.Add(category);
                    }
                }

            return categories;
        }

        public async Task AddToDo(AddToDoRequest addToDoRequest)
        {
            XmlDocument document = new XmlDocument();

            document.Load(_xmlStorageContext.XmlStoragePath);

            XmlElement root = (XmlElement)document.SelectSingleNode("/database/todos")!;

            if (root != null)
            {
                XmlElement todo = document.CreateElement("todo");
                XmlElement id = document.CreateElement("id");
                XmlElement task = document.CreateElement("task");
                XmlElement isPerformed = document.CreateElement("isPerformed");
                XmlElement categoryName = document.CreateElement("categoryName");

                id.InnerText = addToDoRequest.Id.ToString();
                task.InnerText = addToDoRequest.Task;
                isPerformed.InnerText = addToDoRequest.IsPerformed.ToString();
                categoryName.InnerText = addToDoRequest.CategoryName;

                todo.AppendChild(id);
                todo.AppendChild(task);
                todo.AppendChild(isPerformed);
                todo.AppendChild(categoryName);

                if (addToDoRequest.DateToPerform != null)
                {
                    XmlElement? dateToPerform = document.CreateElement("dateToPerform");
                    dateToPerform.InnerText = addToDoRequest.DateToPerform.ToString();
                    todo.AppendChild(dateToPerform);
                }

                root.AppendChild(todo);
                SaveXml(
                    _xmlStorageContext.XmlStoragePath,
                    document);
            }
        }

        public async Task AddCategory(AddCategoryRequest addCategoryRequest)
        {
            XmlDocument document = new XmlDocument();

            document.Load(_xmlStorageContext.XmlStoragePath);

            XmlElement root = (XmlElement)document.SelectSingleNode("/database/categories")!;

            if (root != null)
            {
                XmlElement category = document.CreateElement("category");
                XmlElement id = document.CreateElement("id");
                XmlElement name = document.CreateElement("name");

                id.InnerText = addCategoryRequest.Id.ToString();
                name.InnerText = addCategoryRequest.Name;

                category.AppendChild(id);
                category.AppendChild(name);

                root.AppendChild(category);

                SaveXml(
                    _xmlStorageContext.XmlStoragePath,
                    document);
            }
        }

        public async Task PerformToDo(HandleTodoRequest handleTodoRequest)
        {
            XmlDocument document = new XmlDocument();
            document.Load(_xmlStorageContext.XmlStoragePath);

            string idString = handleTodoRequest.Id.ToString();

            XmlNode? todoNode = document.SelectSingleNode($"/database/todos/todo[id='{idString}']");

            if (todoNode != null)
            {
                XmlElement isPerformed = (XmlElement)todoNode.SelectSingleNode("isPerformed")!;

                isPerformed.InnerText = true.ToString();
                document.Save(_xmlStorageContext.XmlStoragePath);
            }
        }

        public async Task UnperformToDo(HandleTodoRequest handleTodoRequest)
        {
            XmlDocument document = new XmlDocument();
            document.Load(_xmlStorageContext.XmlStoragePath);

            string idString = handleTodoRequest.Id.ToString();

            XmlNode? todoNode = document.SelectSingleNode($"/database/todos/todo[id='{idString}']");

            if (todoNode != null)
            {
                XmlElement isPerformed = (XmlElement)todoNode.SelectSingleNode("isPerformed")!;

                isPerformed.InnerText = false.ToString();
                document.Save(_xmlStorageContext.XmlStoragePath);
            }
        }

        public async Task DeleteToDo(DeleteToDoRequest deleteToDoRequest)
        {
            XmlDocument document = new XmlDocument();
            document.Load(_xmlStorageContext.XmlStoragePath);

            string idString = deleteToDoRequest.Id.ToString();

            XmlNode? todoToDelete = document.SelectSingleNode($"/database/todos/todo[id='{idString}']");

            if (todoToDelete != null)
            {
                todoToDelete.ParentNode?.RemoveChild(todoToDelete);
                SaveXml(
                    _xmlStorageContext.XmlStoragePath,
                    document);
            }
        }

        public void SaveXml(string path, XmlDocument document)
        {
            document.Save(path);
        }
    }
}