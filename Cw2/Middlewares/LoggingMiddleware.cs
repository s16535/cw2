using Cw2.Services;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Cw2.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, IStudentDbService service)
        {
            httpContext.Request.EnableBuffering();

            if (httpContext.Request != null)
            {
                string sciezka = httpContext.Request.Path;
                string querystring = httpContext.Request?.QueryString.ToString();
                string metoda = httpContext.Request.Method.ToString();
                string bodyStr = "";

                using (StreamReader reader
                 = new StreamReader(httpContext.Request.Body, Encoding.UTF8, true, 1024, true))
                {
                    bodyStr = await reader.ReadToEndAsync();
                }

                FileStream fs = new FileStream("requestsLog.txt", FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                StringBuilder sb = new StringBuilder();
                sb.Append("Scieżka: ")
                    .Append(sciezka)
                    .Append("\nZapytanie: ")
                    .Append(querystring)
                    .Append("\nMetoda: ")
                    .Append(metoda)
                    .Append("\nBody: ")
                    .Append(bodyStr)
                    .Append("\n\n");

                sw.Write(sb.ToString());
                sw.Close();
            }

            await _next(httpContext);
        }
    }
}
