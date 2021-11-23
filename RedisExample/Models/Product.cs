using RedisExample.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedisExample.Models
{
    public class Product
    {
        [RedisHashField("id")]
        public string Id { get; set; }

        [RedisHashField("categoryId")]
        public int CategoryId { get; set; }

        [RedisHashField("name")]
        public string Name { get; set; }

        [RedisHashField("price")]
        public double Price { get; set; }
    }
}
