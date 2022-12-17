using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructuur.Database
{
    public class StoryZonDbContext
    {
        private MongoClient dbClient = new MongoClient("mongodb+srv://StoryApi:Toyakes08@cluster0.t0jd6kw.mongodb.net/?retryWrites=true&w=majority");
        public async Task<T> GetByIdAsync<T>(Func<T, bool> predicate, string collection) where T : class
        {
            var data = (await GetAllAsync<T>(collection)).FirstOrDefault(predicate);
            return data;
        }

        public async Task<bool> DeleteAsync<T>(string id, string collectionData, Func<T, bool> pred) where T : class
        {
            var database = dbClient.GetDatabase("Storyzon");
            int previousCount = (await GetAllAsync<T>(collectionData)).Count();
            // get the collection in that database
            var collection = database.GetCollection<BsonDocument>(collectionData);

            // make error if this document(employee) does not exist
            var objectId = new ObjectId(id);
            // filter based on the document with employee id equaling employee.IdNumber.
            var filter = Builders<BsonDocument>.Filter.Eq("_id", objectId);
            //delete
            var data = (await GetAllAsync<T>(collectionData)).FirstOrDefault(pred);
            if (data is null)
            {
                return false;
            }
            collection.DeleteOne(filter);
            return (await GetAllAsync<T>("storyzon")).Count() < previousCount;
        }
        // generic
        public async Task<IQueryable<T>> GetAllAsync<T>(string collection) where T : class
        {
            var database = dbClient.GetDatabase("Storyzon");
            var listOb = new List<T>();
            var collectionTeams = database.GetCollection<BsonDocument>(collection);
            // change one onbject for objectid replacer otherwise error in code
            var jsonTeams = (await collectionTeams.FindAsync(new BsonDocument())).ToList();
            foreach (var item in jsonTeams)
            {
                listOb.Add(JsonConvert.DeserializeObject<T>(item
                    .ToString()
                    .Replace("ObjectId(", "")
                    .Replace(")", "")));
            }
            return listOb.AsQueryable();
        }

        // create 
        public async Task<T> CreateAsync<T>(T toAdd, string collectionData, BsonDocument bsonDocument) where T : class
        {
            var database = dbClient.GetDatabase("Storyzon");

            // get the collection in that database
            var collection = database.GetCollection<BsonDocument>(collectionData);
            var serialize = JsonConvert.SerializeObject(toAdd);

            await collection.InsertOneAsync(bsonDocument);

            return toAdd;
        }

        // update
        public async Task<bool> UpdateAsync<T>(string id, Dictionary<string,string> fieldsToUpdate, string collectionData) where T : class
        {
            var database = dbClient.GetDatabase("Storyzon");
            int previousCount = (await GetAllAsync<T>(collectionData)).Count();
            // get the collection in that database
            var collection = database.GetCollection<BsonDocument>(collectionData);

            var objectId = new ObjectId(id);
          
            // filter based on the document with employee id equaling employee.IdNumber.
            var filter = Builders<BsonDocument>.Filter.Eq("_id", objectId);
            // to update
            foreach (var field in fieldsToUpdate)
            {
                var updateName = Builders<BsonDocument>.Update.Set(field.Key, field.Value);
                await collection.UpdateOneAsync(filter, updateName);
            }
            return (await GetAllAsync<T>("storyzon")).Count() > previousCount; 
        }
    }
}
