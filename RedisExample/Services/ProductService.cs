using NRediSearch;
using RedisExample.Helpers;
using RedisExample.Models;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedisExample.Services
{
    public class ProductService
    {
        private const string PRODUCT_INDEX_NAME = "products-idx";
        private readonly IDatabase _db;
        private readonly Client _searchClient;

        public ProductService(IConnectionMultiplexer multiplexer)
        {
            _db = multiplexer.GetDatabase();
            _searchClient = new Client(PRODUCT_INDEX_NAME, _db);

        }

        public async Task<Product> Add(Product product)
        {
            _db.HashSet(GetKey(product.Id), product.AsHashEntries().ToArray());
            return await Get(product.Id);
        }
        public async Task<bool> Delete(string productId)
        {
            string key = GetKey(productId);
            _searchClient.DeleteDocument(key);
            return true;
        }
        public async Task<Product> Update(Product product)
        {
            string key = GetKey(product.Id);

            // according to schema
            Dictionary<string, RedisValue> dictDocRedis = new Dictionary<string, RedisValue>();
            dictDocRedis.Add("id", product.Id);
            dictDocRedis.Add("name", product.Name);
            dictDocRedis.Add("price", product.Price);
            dictDocRedis.Add("categoryId", product.CategoryId);
            //

            await _searchClient.UpdateDocumentAsync(key,dictDocRedis);

            return product;
        }
        public async Task<IEnumerable<Product>> GetAll()
        {
            var q = new Query("*");
            var results = await _searchClient.SearchAsync(q);
            return results.AsList<Product>();
        }
        public async Task<IEnumerable<Product>> NameQuery(string name)
        {
 
            var q = new Query(name);
            q.SortAscending = true;
            var result = await _searchClient.SearchAsync(q);
            return result.AsList<Product>();

        }
        public async Task<IEnumerable<Product>> Range(double min,double max)
        {
                                                                     //Schema property name
            var q = new Query("*").AddFilter(new Query.NumericFilter("price",min,max));
            var results = await _searchClient.SearchAsync(q);
            return results.AsList<Product>();
        }
        public async Task<Product> Get(string id)
        {
            var query = new Query($"@id:{{{id}}}");
            var result = await _searchClient.SearchAsync(query);
            if (result.TotalResults == 0)
            {
                return null;
            }
            return result.AsList<Product>().FirstOrDefault();
        }

        public async Task<IEnumerable<Product>> GetByCategoryId(int categoryId)
        {
            var q = new Query("*").AddFilter(new Query.NumericFilter("categoryId",categoryId,categoryId));
            var result = await _searchClient.SearchAsync(q);
            return result.AsList<Product>();
        }

        private static RedisKey GetKey(string id)
        {
            return new RedisKey(new Product().GetType().Name + ":" + id);
        }

        public async Task CreateProductIndex()
        {
            // drop the index, if it doesn't exists, that's fine
            try
            {
                await _db.ExecuteAsync("FT.DROPINDEX", PRODUCT_INDEX_NAME);
            }
            catch (Exception)
            {
            }

            var schema = new Schema();
            schema.AddTagField("id");
            schema.AddNumericField("categoryId");
            schema.AddTextField("name");
            schema.AddSortableNumericField("price");
            var options = new Client.ConfiguredIndexOptions(Client.IndexOptions.Default);
            await _searchClient.CreateIndexAsync(schema, options);
        }
    }
}
