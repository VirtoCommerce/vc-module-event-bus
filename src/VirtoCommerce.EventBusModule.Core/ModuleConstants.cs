namespace VirtoCommerce.EventBusModule.Core
{
    public static class ModuleConstants
    {
        public static class Security
        {
            public static class Permissions
            {
                public const string Access = "eventbus:subscriptions:access";
                public const string Create = "eventbus:subscriptions:Create";
                public const string Read = "eventbus:subscriptions:read";
                public const string ReadEvent = "eventbus:events:read";
                public const string Update = "eventbus:subscriptions:update";
                public const string Delete = "eventbus:subscriptions:delete";

                public static string[] AllPermissions { get; } = { Access, Create, Read, ReadEvent, Update, Delete };
            }
        }
    }
}
