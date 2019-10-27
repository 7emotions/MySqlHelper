using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using MySql.Data.MySqlClient;

namespace MySqlHelpWebTest
{
    /// <summary>
    /// MySqlHelp 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class MySqlHelp : System.Web.Services.WebService
    {
        private String source = "server=localhost;userid=root;password=toor;database=test";

        /// <summary>
        /// 判断用户是否存在
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="auth">身份验证码</param>
        /// <returns>
        ///     true:存在
        ///     false:不存在
        /// </returns>
        [WebMethod(Description="MySQL Query")]
        public Result IsUserExist(String username,String auth)
        {
            #region IsUserExist Body
            if (Authentication.Authentic(auth))
            {
                DataSet ds = new DataSet();
                ds = MySqlQuery();

                foreach(DataRow dr in ds.Tables["user_info"].Rows)
                {
                    if(dr["account"].Equals(username))
                    {
                        return new Result(true, 0, "");
                    }
                }
                return new Result(false, 3, "NoUser");
            }
            return new Result(false,1,"AuthError");
            #endregion
        }

        /// <summary>
        /// 查询用户信息
        /// </summary>
        /// <returns>用户数据集合</returns>
        public DataSet MySqlQuery()
        {
            #region MySqlQuery Body
            DataSet ds = new DataSet();
            try
            {
                String sql = "select * from user_info";
                MySqlConnection msc = new MySqlConnection(source);
                msc.Open();
                MySqlDataAdapter mda = new MySqlDataAdapter(sql, msc);
                mda.Fill(ds, "user_info");
                msc.Close();
            }
            catch
            {
                return null;
            }
            return ds;
            #endregion
        }



        /// <summary>
        /// 验证登陆
        /// </summary>
        /// <param name="account">用户名</param>
        /// <param name="pwd">密码</param>
        /// <param name="auth">身份验证码</param>
        /// <returns>
        /// ...isDone=(True:登陆成功|False:登陆失败)
        /// </returns>
        [WebMethod(Description="Authentic User")]
        public Result Authenic(String account,String pwd,String auth)
        {
            #region 验证

            if (Authentication.Authentic(auth))
            {
                account = Disinfection(account);
                pwd = Disinfection(pwd);
                try
                {
                    DataSet ds = new DataSet();
                    ds = MySqlQuery();

                    foreach (DataRow dr in ds.Tables["user_info"].Rows)
                    {
                        if (dr["account"].Equals(account) && dr["password"].Equals(pwd))
                        {
                            return new Result(true, 0, "");
                        }
                    }
                }
                catch(Exception e)
                {
                    return new Result(false, 2, e.Message);
                }
            }
            return new Result(false, 1, "AuthError");
        
            #endregion
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="name">用户名</param>
        /// <param name="pwd">密码</param>
        /// <param name="auth">身份验证码</param>
        /// <returns>
        ///     Result{
        ///         isDone:(True:注册成功|False:注册失败)
        ///         code:(0:成功|1:认证失败|2:未知错误|3:用户存在)
        ///         msg:[错误信息]
        ///     }
        /// </returns>
        [WebMethod(Description="MySQL Insert")]
        public Result MySqlInsert(String name,String pwd,String auth)
        {
            #region 注册
            if (Authentication.Authentic(auth))
            {
                try
                {
                    name = Disinfection(name);
                    pwd = Disinfection(pwd);
                    if(IsUserExist(name,auth).isDone)
                    {
                        return new Result(false, 3, "RE_USER");
                    }

                    String sql = "insert into user_info(account,password)value('"
                        + name + "','"
                        + pwd + "')";
                    using(var msc=new MySqlConnection(source))
                    {
                        try
                        {
                            msc.Open();
                            MySqlCommand cmd = new MySqlCommand(sql, msc);
                            cmd.ExecuteNonQuery();
                            msc.Close();
                            return new Result(true, 0, "");
                        }
                        catch (Exception e) 
                        {
                            return new Result(false, 2, e.Message);
                        }
                    }
                }
                catch (Exception e)
                {
                    return new Result(false, 2, e.Message);
                }
            }
            return new Result(false,1,"AuthError");
            #endregion
        }

        /// <summary>
        /// 更改用户信息
        /// </summary>
        /// <param name="id">用户名</param>
        /// <param name="key">更改字段</param>
        /// <param name="value">更改值</param>
        /// <param name="auth">身份验证码</param>
        /// <returns>
        ///     Result{
        ///         isDone:(True:修改成功|False:修改失败)
        ///         code:(0:成功|1:认证失败|2:未知错误|3:用户不存在)
        ///         msg:[错误信息]
        ///     }
        /// </returns>
        [WebMethod(Description="MySQL Update")]
        public Result MySqlUpdate(String username,String key,String value,String auth)
        {
            #region 修改 
            if(Authentication.Authentic(auth))
            {
                username=Disinfection(username);
                value=Disinfection(value);
                key=Disinfection(key);
                try
                {
                    if (IsUserExist(username, auth).isDone)
                    {
                        String sql = "update user_info set "
                        + key + "='"
                        + value + "' where account='"
                        + username + "'";

                        using(var msc=new MySqlConnection(source))
                        {
                            try
                            {
                                msc.Open();
                                MySqlCommand cmd=new MySqlCommand(sql,msc);
                                cmd.ExecuteNonQuery();
                                msc.Close();
                                return new Result(true,0,"");
                            }
                            catch(Exception e)
                            {
                                return new Result(false, 2, e.Message);
                            }
                        }
                    }
                    return new Result(false, 3, "No_User");
                }
                catch(Exception e)
                {
                    return new Result(false, 2, e.Message);
                }
            }
            return new Result(false, 1, "AuthError");  
#endregion
        }

        /// <summary>
        /// 对Sql语句中的值消毒
        /// </summary>
        /// <param name="str">未消毒的String对象</param>
        /// <returns>已消毒的String对象</returns>
        public String Disinfection(String str)
        {
            return str.Replace("'", "").Replace("\"", "").Replace("/", "");
        }
    }
}