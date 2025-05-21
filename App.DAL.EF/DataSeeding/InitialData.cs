namespace App.DAL.EF.DataSeeding;

public static class InitialData
{
    public static readonly (string roleName, Guid? id)[]
        Roles =
        [
            ("admin", null),
        ];

    public static readonly (string name, string firstName, string lastName, string password, Guid? id, string[] roles)[]
        Users =
        [
            ("admin@taltech.ee", "admin", "taltech", "Foo.Bar.1", null, ["admin"]),
            ("user@taltech.ee", "user", "taltech", "Foo.Bar.2", null, []),
        ];
}