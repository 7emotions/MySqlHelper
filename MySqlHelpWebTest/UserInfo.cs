using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MySqlHelpWebTest
{
    [Serializable]
    public class UserInfo
    {
        public UserInfo()
        {

        }

        public UserInfo(int id,String name,String pwd)
        {
            this.Id = id;
            this.Account = name;
            this.Password = pwd;
        }

        public int Id;
        public String Account;
        public String Password;
    }
}