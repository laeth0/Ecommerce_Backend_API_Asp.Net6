using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Core.ILog
{
    public interface ILogging
    {
        public void Log(string message,string type);
    }
}
