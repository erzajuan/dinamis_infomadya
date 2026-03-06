using BlazorAuthApi.Middlewares;
using StackExchange.Redis;

namespace BlazorAuthApi.Hellpers
{
    public class RedisRateLimit
    {
        private readonly IDatabase _redis;

        public RedisRateLimit(IConnectionMultiplexer redis)
        {
            _redis = redis.GetDatabase();
        }

        public async Task CheckLoginRateLimitAsync(string username)
        {
            username = username.ToLower();
            var key = $"redis:login_attempts:{username}";

            var attempts = await _redis.StringIncrementAsync(key);

            if (attempts == 1)
            {
                await _redis.KeyExpireAsync(key, TimeSpan.FromMinutes(1));
            }

            if (attempts > 3)
            {
                throw new BaseException("Too many login attempts. Please try again later.", 429);
            }
        }
    }
}
