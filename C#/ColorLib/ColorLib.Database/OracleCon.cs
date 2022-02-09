using ColorLib.Log;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace ColorLib.Database
{
    public class OracleCon
    {
        private ILog log;
        private SQLConfig config;
        private string conn;

        public OracleCon(ILog log, SQLConfig config)
        {
            this.log = log;
            this.config = config;
        }

        /// <summary>
        /// 连接测试
        /// </summary>
        /// <param name="item">连接池项目</param>
        /// <returns>是否测试成功</returns>
        private bool Test(OracleConnection item)
        {
            try
            {
                item.Open();
                new OracleCommand("select id from test where rownum<=1", item).ExecuteNonQuery();
                item.Close();
                return true;
            }
            catch (OracleException ex)
            {
                switch (ex.Number)
                {
                    case 942:
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
        internal bool Contains(int id)
        {
            if (!config.Enable)
                return false;
            var pass = Encoding.UTF8.GetString(Convert.FromBase64String(config.Password));
            conn = string.Format(config.Conn, config.IP, config.Port, config.User, pass);
                var Conn = new OracleConnection(conn);
            if (Test(Conn))
            {
                log.LogOut($"Oracle数据库已连接");
                return true;
            }
            else
            {
                log.LogError($"Oracle数据库连接失败");
                return false;
            }
        }

        /// <summary>
        /// Oracle初始化
        /// </summary>
        /// <returns>是否连接成功</returns>
        public void Start()
        {
            log.LogOut($"正在连接Mysql数据库");
            if (!config.Enable)
                return;
            var pass = Encoding.UTF8.GetString(Convert.FromBase64String(config.Password));
            conn = string.Format(config.Conn, config.IP, config.User, pass);
            var Conn = new OracleConnection(conn);
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
        /// 关闭Oracle数据库连接
        /// </summary>
        public void Stop()
        {
            OracleConnection.ClearAllPools();
            log.LogOut("Oracle数据库已断开");
        }

        /// <summary>
        /// 获取数据库链接
        /// </summary>
        /// <param name="ID">数据库ID</param>
        /// <returns>链接</returns>
        public OracleConnection GetConnection()
        {
            return new OracleConnection(conn);
        }
    }
}
