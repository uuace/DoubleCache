﻿using DoubleCache;
using DoubleCache.Redis;
using DoubleCache.Serialization;
using RandomUser;
using StackExchange.Redis;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net.Http;
using System.Net;

namespace CacheSample
{
    [RoutePrefix("rediscachebinaryformatter")]
    public class RedisBinaryFormatterCacheController : ApiController, IUserController
    {
        private static ICacheAside _redisCache;
        private RandomUserRepository _repo;

        static RedisBinaryFormatterCacheController()
        {
            _redisCache = new RedisCache(ConnectionMultiplexer.Connect("localhost").GetDatabase(), new BinaryFormatterItemSerializer()); ;
        }

        public RedisBinaryFormatterCacheController()
        {
            _repo = new RandomUserRepository();
        }

        [Route("single")]
        public async Task<IHttpActionResult> GetSingle()
        {
            return Ok(await _redisCache.GetAsync(Request.RequestUri.PathAndQuery, () => _repo.GetSingleDummyUser()));
        }

        [Route("many")]
        public async Task<IHttpActionResult> GetMany()
        {
            return Ok(await _redisCache.GetAsync(Request.RequestUri.PathAndQuery, () => _repo.GetManyDummyUser(2000)));
        }

        [HttpDelete]
        [Route("single")]
        [Route("many")]
        public IHttpActionResult Remove()
        {
            _redisCache.Remove(Request.RequestUri.PathAndQuery);

            return ResponseMessage(new HttpResponseMessage(HttpStatusCode.NoContent));
        }
    }
}
