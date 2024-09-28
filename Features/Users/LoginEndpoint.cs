using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Endpoint;

class LoginEndpoint : IEndpoint
{
    private readonly AppDbContext appDbContext;
    private readonly PasswordHasher passwordHasher;
    private readonly JwtTokenGenerator jwtTokenGenerator;

    public LoginEndpoint(
        AppDbContext appDbContext, 
        PasswordHasher passwordHasher,
        JwtTokenGenerator jwtTokenGenerator)
    {
        this.appDbContext = appDbContext;
        this.passwordHasher = passwordHasher;
        this.jwtTokenGenerator = jwtTokenGenerator;
    }

    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("/users/login", Handle);
    }

    public async Task<IResult> Handle(LoginRequest loginRequest, CancellationToken cancellationToken)
    {
        var user = await appDbContext.Persons.Where(p => p.Email == loginRequest.Email).FirstOrDefaultAsync();
        if (user == null)
        {
            return Results.NotFound("User not found");
        }

        var passwordHash = await this.passwordHasher.Hash(loginRequest.Password, user.Salt);
        if(!user.Hash.SequenceEqual(passwordHash))
        {
            return Results.BadRequest("Invalid email or password");
        }
        var token = this.jwtTokenGenerator.CreateToken(user.Email ?? throw new InvalidOperationException());

        var userDto = new UserDto{
            Email = loginRequest.Email,
            Token = token,
        };
        
        return Results.Ok(userDto);
    }
}