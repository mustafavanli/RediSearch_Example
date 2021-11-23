using NRediSearch;
using RedisExample.Helpers;
using RedisExample.Models;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static NRediSearch.Client;

namespace RedisExample.Services
{
    public class CategoryService
    {
        private const string CATEGORY_INDEX_NAME = "categories-idx";
        private readonly IDatabase _db;
        private readonly Client _searchClient;
        public CategoryService(IConnectionMultiplexer multiplexer)
        {
            _db = multiplexer.GetDatabase();
            _searchClient = new Client(CATEGORY_INDEX_NAME, _db);
        }

        public async Task<Category> Add(Category category)
        {
            _db.HashSet(GetKey(category.Id), category.AsHashEntries().ToArray());
            return await Get(category.Id);

        }
        public async Task<Category> Update(Category category)
        {
            string key = GetKey(category.Id);

            // according to schema
            Dictionary<string, RedisValue> dictDocRedis = new Dictionary<string, RedisValue>();
            dictDocRedis.Add("id", category.Id);
            dictDocRedis.Add("name", category.Name);
            //
            await _searchClient.UpdateDocumentAsync(key, dictDocRedis);
            return category;
        } 
        public async Task<Category> Get(string id)
        {
            var query = new Query($"@id:{{{id}}}");

            var result = await _searchClient.SearchAsync(query);
            if (result.TotalResults == 0)
            {
                return null;
            }
            return result.AsList<Category>().FirstOrDefault();

        }
        public async Task<IEnumerable<Category>> GetAll()
        {
            var query = new Query("*");

            var result = await _searchClient.SearchAsync(query);
            if (result.TotalResults == 0)
            {
                return null;
            }
            return result.AsList<Category>();
        }

        private static RedisKey GetKey(string id)
        {
            return new RedisKey(new Category().GetType().Name + ":" + id);
        }
        public async Task<bool> Delete(string categoryId)
        {
            string key = GetKey(categoryId);
            _searchClient.DeleteDocument(key);
            return true;
        }
        public async Task CreateCategoryIndex()
        {
            try
            {
                await _db.ExecuteAsync("FT.DROPINDEX", CATEGORY_INDEX_NAME);
            }
            catch (Exception)
            {
            }

            var schema = new Schema();
            schema.AddTagField("id");
            schema.AddTextField("name");
            var options = new Client.ConfiguredIndexOptions(Client.IndexOptions.Default);
            await _searchClient.CreateIndexAsync(schema, options);
        }
    }
}
