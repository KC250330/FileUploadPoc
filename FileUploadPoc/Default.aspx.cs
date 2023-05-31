using System;
using System.Web.UI;

namespace FileUploadPoc
{
    public partial class _Default : Page
    {

        private string _env;

        private string[] _envs;

        public string[] Environments
        {
            get { return _envs; }
        }

        public string Environment
        {
            get { return _env; }
        }
     
        protected void Page_Load(object sender, EventArgs e)
        {
           
            _env = @"Dev1Sql01\\instance2";
            _envs = new string[] {_env, "server2" };
            DataBind();
        }

        //protected void Page_PreRenderComplete(object sender, EventArgs e)
        //{
        //   // DataBind();
        //    _env = @"Dev1Sql01\\instance2";
        //}
    }
}