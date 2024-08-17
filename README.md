# Users Api with net 8

It is a user system, with registration, login, logout.

## Installation

Use dotnet cli with the command: dotnet build.
To start the server you type the code: dotnet run.

## Use

Endpoints: 

POST: /api/user/regiter: 
    
    To register the user.

    JSON:
        username: string;
        email: string, Only accept valid email;
        password: string, Only accepts strong passwords with lowercase, uppercase, number and special characters.

POST: /api/user/login:
    
    To login the user. To log in, the admin user must log in to this endpoint.
    generated token jwt.
    
    JSON:
        emailOrUserName: string, Accept username or email;
        password: string.

GET: /api/user/generate-password-reset: 
        
        This endpoint server to generate the token to reset the password.
        Need a login token to change password. To get the token you have to log into the system with any user

POST: /api/user/reset-password:

        JSON:

            Email: string;
            Token: string, Token generated for reseting password;
            OldPassword: string;
            NewPassword: string
        
GET: /api/user/logout: 
    
    To log out of user.
    
    
GET: /api/admin/users-stats: 
    
    You must have the admin role with the jwt token to log in to this endpoint
    It has a paging system with queries: pageNumber, pageSize.
    It returns all users on the system.
