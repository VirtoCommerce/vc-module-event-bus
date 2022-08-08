using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.EventBusModule.Core.Options
{
    public class AzureEventGridOptions 
    {
        public string ConnectionString { get; set; }
        public string AccessKey { get; set; }
    }
}
