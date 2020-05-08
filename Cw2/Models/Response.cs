using System;

namespace Cw2.DTO
{
    public class Response
    {
        public string Type { get; set; }
        public string Message { get; set; }
        public Object ResponseObject { get; set; }

        public Response(string Type, string Message)
        {
            this.Type = Type;
            this.Message = Message;
        }

        public Response(string Type, string Message, Object ResponseObject)
        {
            this.Type = Type;
            this.Message = Message;
            this.ResponseObject = ResponseObject;
        }

    }
}
