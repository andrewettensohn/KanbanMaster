namespace KanbanMaster.Client
{
    public static class Constants
    {
        public static class ApiRoutes
        {
            public static string BaseApiRoute = "api";

            public static string Project = $"{BaseApiRoute}/projectItem";
            public static string ProjectList = $"{BaseApiRoute}/projectItem/list";
            public static string ProjectListArchived = $"{BaseApiRoute}/projectItem/listArchived";
            public static string ProjectListHistory = $"{BaseApiRoute}/projectItem/listHistory";
            public static string ProjectDelete = $"{BaseApiRoute}/projectItem/delete";
            public static string ProjectArchive = $"{BaseApiRoute}/projectItem/archive";
            public static string ProjectSetActive = $"{BaseApiRoute}/projectItem/setActive";

            public static string TodoItem = $"{BaseApiRoute}/todoItem";
            public static string TodoItemList = $"{BaseApiRoute}/todoItem/list";
            public static string TodoItemDelete = $"{BaseApiRoute}/todoItem/delete";
        }
    }
}
