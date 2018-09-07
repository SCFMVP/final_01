using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace final_01
{
    class SqlDesigner
    {
        private static string connStr = "server=localhost;User Id=root;password=root;Database=final;SslMode = none;";
        /// 返回受影响的数据行数
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static int ExecuteNoQuery(string sql)
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    return cmd.ExecuteNonQuery();

                }
            }
        }
        /// <summary>
        /// 返回一个数据集
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static DataSet ExecuteDataSet(string sql)
        {
            using (MySqlConnection xonn = new MySqlConnection(connStr))
            {
                xonn.Open();
                using (MySqlCommand cmd = xonn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataSet dataset = new DataSet();
                    adapter.Fill(dataset);
                    return dataset;
                }
            }
        }
        public static object ExecuteScalar(string sql)
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    return cmd.ExecuteScalar();
                }
            }
        }
        /// <summary>
        /// md5加密
        /// </summary>
        /// <param name="strPwd"></param>
        /// <returns></returns>
        public static string GetMD5(string strPwd)
        {
            string pwd = "";
            //实例化一个md5对象
            MD5 md5 = MD5.Create();
            // 加密后是一个字节类型的数组
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(strPwd));
            //翻转生成的MD5码        
            s.Reverse();
            //通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            //只取MD5码的一部分，这样恶意访问者无法知道取的是哪几位
            for (int i = 3; i < s.Length - 1; i++)
            {
                //将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符
                //进一步对生成的MD5码做一些改造
                pwd = pwd + (s[i] < 198 ? s[i] + 28 : s[i]).ToString("X");
            }
            return pwd;
        }
    }
}
