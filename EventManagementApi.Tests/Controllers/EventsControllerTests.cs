
using System;
using System.Reflection;
using System.Threading.Tasks;
using EventManagementApi.Controllers;
using EventManagementApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace EventManagementApi.Tests.Controllers;

public class EventsControllerTests
{
    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var service = DispatchProxy.Create<
            IEventService, FakeEventService>();

        var controller = new EventsController(service);

        var action = await controller.GetAll();

        Assert.IsType<OkObjectResult>(action.Result);
    }

    [Fact]
    public async Task GetById_UnknownEvent_ReturnsNotFound()
    {
        var service = DispatchProxy.Create<
            IEventService, FakeEventService>();

        var controller = new EventsController(service);

        var action = await controller.GetById(999);

        Assert.IsType<NotFoundObjectResult>(action.Result);
    }

    public class FakeEventService : DispatchProxy
    {
        protected override object? Invoke(
            MethodInfo? targetMethod,
            object?[]? args)
        {
            var returnType = targetMethod!.ReturnType;

            if (returnType == typeof(Task))
                return Task.CompletedTask;

            if (returnType.IsGenericType &&
                returnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                var resultType = returnType.GetGenericArguments()[0];

                object? value = resultType == typeof(bool)
                    ? false
                    : null;

                return typeof(Task)
                    .GetMethod(nameof(Task.FromResult))!
                    .MakeGenericMethod(resultType)
                    .Invoke(null, new[] { value });
            }

            throw new NotSupportedException(
                $"Unsupported return type: {returnType}");
        }
    }
}
