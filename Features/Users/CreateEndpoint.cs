using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Endpoint;

public class CreateEndpoint : IEndpoint
{
    private readonly AppDbContext dbContext;
    private readonly PasswordHasher passwordHasher;
    private readonly JwtTokenGenerator jwtTokenGenerator;
    private readonly IMapper mapper;

    public CreateEndpoint(
        AppDbContext dbContext, 
        PasswordHasher passwordHasher, 
        JwtTokenGenerator jwtTokenGenerator,
        IMapper mapper)
    {
        this.dbContext = dbContext;
        this.passwordHasher = passwordHasher;
        this.jwtTokenGenerator = jwtTokenGenerator;
        this.mapper = mapper;
    }

    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("/users/register", HandleAsync);
    }

    public async Task<User> HandleAsync(CreateRequest request, CancellationToken cancellationToken)
    {
        var userExists = await this.dbContext.Persons.Where(p => p.Email == request.Email).AnyAsync(cancellationToken);
        if (userExists)
        {
            throw new Exception("User already exists");
        }

        var salt = Guid.NewGuid().ToByteArray();
        var person = new Person
        {
            Email = request.Email,
            Salt = salt,
            Hash = await this.passwordHasher.Hash(request.Password, salt)
        };

        await this.dbContext.Persons.AddAsync(person, cancellationToken);
        await this.dbContext.SaveChangesAsync(cancellationToken);

        var token = this.jwtTokenGenerator.CreateToken(person.Email);
        var user = this.mapper.Map<Person, User>(person);
        user.Token = token;
        return user;
    }
}