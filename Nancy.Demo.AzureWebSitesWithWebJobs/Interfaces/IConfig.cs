using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nancy.Demo.AzureWebSitesWithWebJobs.Interfaces
{
    public interface IConfig
    {
        string StorageCdn { get; }
    }
}
