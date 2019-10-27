using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MySqlHelpWebTest
{
    [Serializable]
    public class Result
    {
        public Result()
        {

        }

        public Result(Boolean isDone,int code,String msg)
        {
            this.isDone = isDone;
            this.code = code;
            this.msg = msg;
        }

        public Boolean isDone { set; get; }
        public int code { set; get; }
        public String msg { set; get; }
    }
}