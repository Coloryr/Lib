using ColorLib.Log;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace ColorLib.Database
{
    public class MysqlCon
    {
        private ILog log;
        private SQLConfig config;
        private string conn;

        public MysqlCon(ILog log, SQLConfig config)
        {
            this.log = log;
            this.config = config;
        }

        /// <summary>
        /// 连接测试
        /// </summary>
        /// <param name="item">连接池项目</param>
        /// <returns>是否测试成功</returns>
        private bool Test(MySqlConnection item)
        {
            try
            {
                item.Open();
                new MySqlCommand("select TOP 1 id from test", item).ExecuteNonQuery();
                item.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 1146:
                    case 1046:
                        item.Close();
                        return true;
                    default:
                        log.LogError(ex);
                        return false;
                }
            }
        }

        /// <summary>
        /// 尝试重连数据库
        /// </summary>
        /// <param name="id">数据库ID</param>
        /// <returns>是否能连接</returns>
        public bool Contains()
        {
            if (!config.Enable)
                return false;
            var pass = Encoding.UTF8.GetString(Convert.FromBase64String(config.Password));
            conn = string.Format(config.Conn, config.IP, config.User, pass);
            var Conn = new MySqlConnection(conn);
            if (Test(Conn))
            {
                log.LogOut($"Mysql数据库已连接");
            }
            else
            {
                log.LogError($"Mysql数据库连接失败");
            }
            return true;
        }

        /// <summary>
        /// MSCon初始化
        /// </summary>
        public void Start()
        {
            log.LogOut($"正在连接Mysql数据库");
            if (!config.Enable)
                return;
            var pass = Encoding.UTF8.GetString(Convert.FromBase64String(config.Password));
            conn = string.Format(config.Conn, config.IP, config.User, pass);
            var Conn = new MySqlConnection(conn);
            if (Test(Conn))
            {
                log.LogOut($"Mysql数据库已连接");
            }
            else
            {
                log.LogError($"Mysql数据库连接失败");
            }
        }

        /// <summary>
        /// 关闭Ms数据库连接
        /// </summary>
        public void Stop()
        {
            MySqlConnection.ClearAllPools();
            log.LogOut("Mysql数据库已断开");
        }

        /// <summary>
        /// 获取数据库链接
        /// </summary>
        /// <param name="ID">数据库ID</param>
        /// <returns>链接</returns>
        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(conn);
        }
    }
}
