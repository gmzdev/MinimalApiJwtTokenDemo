using Microsoft.AspNetCore.Mvc;
using MinimalApi.Endpoint;

class LoginEndpoint : IEndpoint
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("/users/login", Handle);
    }

    public IResult Handle(){
        return Results.Ok();
    }
}