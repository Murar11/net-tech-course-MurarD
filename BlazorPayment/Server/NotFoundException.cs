using System;

namespace BlazorPayment.Server
{
    public class NotFoundException : ApplicationException
    {
        public NotFoundException() : base("Object not found")
        {
        }
    }
}
