# scrum-poll

# prerequisites

- Visual studio 2022
- .net 6

# development

## get it running

1. Change MSSQL Server connection string using your database credentials in the `appsetting.json` file
2. Start application using `dotnet` CLI or inside Visual Studio.
3. The application automaticly creates 6 accounts - one for master (login - master) and 5 for users (logins - user1/2/3/4/5) with default password "Qazaq123".
   It can be changes in the `DAL\Entities\AppDbContext.cs` file.


