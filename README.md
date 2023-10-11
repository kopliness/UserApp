## UserApp - ASP.NET Core WEB API CRUD application aimed at managing a list of users. A 3-layer architecture was chosen as the architecture.
## Technology Stack:
 - .Net 7.0
 - Entity Framework Core
 - MS SQL
 - AutoMapper
 - JWTBearer
 - Swagger

## To start: 
Clone the repository
```bash
https://github.com/kopliness/UserApp
```
Go to the project folder
```bash
cd UserApp/UserApp.API
```
Change the connection string
```bash
"ConnectionStrings": {
     "DefaultConnection": "Data Source={your server};Integrated Security=True;Initial Catalog=Users;MultipleActiveResultSets=True;TrustServerCertificate=True"
  }
```
Create migrations
```bash
Add-Migration FirstMigration
```
Apply migrations
```bash
Update-Database
```
Next, you need to start the project with the command
```bash
  dotnet run --project .\UserApp.API\UserApp.API.csproj --launch-profile https
```
## Go to the application page
[https://localhost:7148/swagger/index.html](https://localhost:44322/swagger/index.html)

## Next, you'll need to register an account:
![image](https://github.com/kopliness/UserApp/assets/92124944/d677e29a-8d16-466c-94b1-17f80c0f73fb)

## Get your JWT-Token and copy it:
![image](https://github.com/kopliness/UserApp/assets/92124944/bc5f47f0-a46e-468c-8bff-656986b7db2f)

## Insert the JWT-Token into the authorization form, making sure to write the word "Bearer" or the token will be incorrect and press the "Authorize" button:
![image](https://github.com/kopliness/UserApp/assets/92124944/56967d78-0095-4045-8686-b6fe9b9bc40a)

## Done, now you can use all the functionality of the app.

## Functionality:

For unauthorized users: 
- registration and authorization

For authorized users:
- getting a list of all users(sorting and filtering attributes are optimal)
- new user creation
- get user by Id
- update user properties by Id
- deleting a user by Id
- adding a list of roles to user by Id
- deleting a list of roles for a user by Id
