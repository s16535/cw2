using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApplication2.Services
{
    public interface IExceptionFilter : IFilterMetadata
    {
        void OnException(ExceptionContext context);
    }
}
