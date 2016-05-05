using System;
using System.Configuration;
using StackExchange.Redis;

namespace Pelway.Redis
{
	/// <summary>
	/// Redis 业务类
	/// </summary>
	public class RedisBusiness
	{

		/// <summary>
		/// The connection multiplexer
		/// </summary>
		public static ConnectionMultiplexer connectionMultiplexer;


		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="url">The URL.</param>
		public static void Init(string url)
		{
			var redisServer = ConfigurationManager.AppSettings[url];
			connectionMultiplexer = GetManager(redisServer);
		}

		/// <summary>
		/// The database
		/// </summary>
		private readonly IDatabase db;

		/// <summary>
		/// 
		/// </summary>
		public RedisBusiness()
		{
			db = this.getRedisDB("redisServer");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="redisUrl"></param>
		public RedisBusiness(string redisUrl)
		{
			db = this.getRedisDB(redisUrl);
		}

		/// <summary>
		/// 获得 Redis 数据上下文
		/// </summary>
		/// <param name="redisUrl">The redis URL.</param>
		/// <returns></returns>
		private IDatabase getRedisDB(string redisUrl)
		{
			// 加载 redis 服务器地址
			var redisServer = ConfigurationManager.AppSettings[redisUrl];

			var redis = GetManager(redisServer);

			return redis.GetDatabase();
		}


		private static ConnectionMultiplexer GetManager(string connectionString = "")
		{

			if (connectionMultiplexer != null)
			{
				return connectionMultiplexer;

			}
			// 连接 Redis 对象
			connectionMultiplexer = ConnectionMultiplexer.Connect(connectionString);

			return connectionMultiplexer;

		}



		/// <summary>
		/// Initializes a new instance of the <see cref="RedisBusiness"/> class.
		/// </summary>
		/// <param name="connectionMultiplexer">The connection multiplexer.</param>
		public RedisBusiness(ConnectionMultiplexer connectionMultiplexer)
		{
			db = connectionMultiplexer.GetDatabase();
		}

		/// <summary>
		/// Adds the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <param name="timeSpan">The time span.</param>
		public void Add(string key, string value, TimeSpan? timeSpan = null)
		{
			if (!timeSpan.HasValue)
			{
				var currentTime = DateTime.Now;

				var entTime = currentTime.AddHours(24);

				timeSpan = (entTime - currentTime);
			}

			db.StringSet(key, value, timeSpan);
		}

		///  <summary>
		///  获得视图模型 
		///  </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public string Get(string key)
		{
			return db.StringGet(key);
		}

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void Update(string key, string value)
		{
			db.StringSet(key, value);
		}

		/// <summary>
		/// 删除
		/// </summary>
		/// <param name="key">The veify user token model.</param>
		public void Delete(string key)
		{
			db.KeyDelete(key);
		}

		/// <summary>
		/// 检测用户 Token 值
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public Boolean CheckUserToken(string key, string value)
		{
			var token = Get(key);

			if (string.IsNullOrEmpty(token))
			{
				return false;
			}
			var newToken = token.Split('|')[0];

			if (!String.Equals(value, newToken, StringComparison.CurrentCultureIgnoreCase))
			{
				return false;
			}

			return true;
		}
	}
}