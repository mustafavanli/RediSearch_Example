using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedisExample.Exceptions
{
    public class RedisKeyNotFoundException : Exception
    {
        public RedisKeyNotFoundException(string message) : base(message) { }
    }
}
