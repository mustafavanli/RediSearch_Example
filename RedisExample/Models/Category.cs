using RedisExample.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedisExample.Models
{
    public class Category
    {

        [RedisHashField("id")]
        public string Id { get; set; }
        [RedisHashField("name")]
        public string Name { get; set; }
    }
}
