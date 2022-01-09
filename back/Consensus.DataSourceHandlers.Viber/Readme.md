## Reverse engeneering Viber DB

```
dotnet-ef dbcontext scaffold "Data Source=D:/tmp/viber/viber.db" Microsoft.EntityFrameworkCore.Sqlite --context-dir Db --output-dir Db/Entities --context ViberDbContext --table ChatInfo --table Events --table Messages --table Contact
```
