using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MySqlHelpWebTest
{
    public class Authentication
    {
        static private String pwd = "LongestChina!";
        static public Boolean Authentic(String code)
        {
            if(code.Equals(pwd))
            {
                return true;
            }
            return false;
        }
    }
}