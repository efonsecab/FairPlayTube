using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Common.Interfaces
{
    public interface IOriginatorInfo
    {
        string SourceApplication { get; set; }
        string OriginatorIpaddress { get; set; }
        DateTimeOffset RowCreationDateTime { get; set; }
        string RowCreationUser { get; set; }
    }
}
