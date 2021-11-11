using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(BalajiInstitute.Startup))]

namespace BalajiInstitute
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //configure things here
            ConfigureAuth(app);
        }
    }
}
