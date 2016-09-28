using System;
using SignInCheckIn.Models;
using SignInCheckIn.Services.Interfaces;

namespace SignInCheckIn.Services
{
    public class HelloWorldService : IHelloWorldService
    {
        public HelloWorldOutputDto Greet(HelloWorldInputDto input)
        {
            // V1
            if (input.GetType() == typeof (HelloWorldInputDto))
            {
                return new HelloWorldOutputDto
                {
                    Message = $"Hello {input.Name}!",
                    Name = input.Name
                };
            }

            // V2
            if (input.GetType() == typeof (HelloWorldV2InputDto))
            {
                var v2 = (HelloWorldV2InputDto) input;
                return new HelloWorldV2OutputDto
                {
                    Message = $"Hello {v2.FirstName} {v2.LastName}!",
                    FirstName = v2.FirstName,
                    LastName = v2.LastName
                };
            }

            throw new InvalidOperationException("API version not supported");
        }
    }
}