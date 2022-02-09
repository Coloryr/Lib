using ColorLib.Log;
using StackExchange.Redis;

namespace ColorLib.Database
{
    public class RedisCon
    {
        private ILog log;
        private string conn;
        private RedisConfig config;

        public RedisCon(ILog log, RedisConfig config) 
        {
            this.log = log;
            this.config = config;
        }

        /// <summary>
        /// 连接测试
        /// </summary>
        /// <param name="item">连接池项目</param>
        /// <returns>是否测试成功</returns>
        private bool Test(ConnectionMultiplexer item)
        {
            try
            {
                if (item.IsConnected == false)
                {
                    return false;
                }
                item.GetDatabase().KeyExists("test");
                return true;
            }
            catch (Exception ex)
            {
                log.LogError(ex);
                return false;
            }
        }

        /// <summary>
        /// Redis初始化
        /// </summary>
        /// <returns>是否连接成功</returns>
        public void Start()
        {
            log.LogOut($"正在连接Redis数据库");
            if (!config.Enable)
                return;
            conn = string.Format(config.Conn, config.IP, config.Port);
            var Conn = ConnectionMultiplexer.Connect(conn);
            if (Test(Conn))
            {
                log.LogOut($"Redis数据库已连接");
            }
            else
            {
                log.LogError($"Redis数据库连接失败");
            }
        }

        /// <summary>
        /// 关闭Redis数据库连接
        /// </summary>
        public void Stop()
        {
            log.LogOut("Redis数据库已断开");
        }

        /// <summary>
        /// 根据key获取缓存对象
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>值</returns>
        public RedisValue Get(string key, int id)
        {
            return ConnectionMultiplexer.Connect(conn).GetDatabase().StringGet(key);
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expireMinutes">存在秒</param>
        public bool Set(string key, string value, int expireMinutes, int id)
        {
            return ConnectionMultiplexer.Connect(conn).GetDatabase().StringSet(key, value);
        }
        /// <summary>
        /// 判断在缓存中是否存在该key的缓存数据
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>是否存在</returns>
        public bool Exists(string key)
        {
            return ConnectionMultiplexer.Connect(conn).GetDatabase().KeyExists(key);
        }
        /// <summary>
        /// 移除指定key的缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>是否移除</returns>
        public bool Remove(string key)
        {
            return ConnectionMultiplexer.Connect(conn).GetDatabase().KeyDelete(key);
        }
        /// <summary>
        /// 数据累加
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>累加后的值</returns>
        public long Increment(string key, long val = 1)
        {
            return ConnectionMultiplexer.Connect(conn).GetDatabase().StringIncrement(key, val);
        }
    }
}
