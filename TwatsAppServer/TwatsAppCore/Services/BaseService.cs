using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwatsAppCore.Models;

namespace TwatsAppCore.Services
{
    public class BaseService : IDisposable
    {
        protected TwatsAppContext db = new TwatsAppContext();

        public void Dispose()
        {
            db.Dispose();
        }
    }
}
