using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KanbanMaster.Client
{
    public static class Constants
    {
        public static class ApiRoutes
        {
            public static string BaseApiRoute = "api";

            public static string Project = $"{BaseApiRoute}/projectItem";
            public static string ProjectList = $"{BaseApiRoute}/projectItem/list";
            public static string ProjectDelete = "{BaseApiRoute}/projectItem/delete";

            public static string TodoItem = $"{BaseApiRoute}/todoItem";
            public static string TodoItemList = $"{BaseApiRoute}/todoItem/list";
            public static string TodoItemDelete = $"{BaseApiRoute}/todoItem/delete";
        }
    }
}
