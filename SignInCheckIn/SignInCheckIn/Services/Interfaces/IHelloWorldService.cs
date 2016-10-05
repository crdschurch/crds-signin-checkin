using SignInCheckIn.Models;

namespace SignInCheckIn.Services.Interfaces
{
    public interface IHelloWorldService
    {
        HelloWorldOutputDto Greet(HelloWorldInputDto input);
    }
}
