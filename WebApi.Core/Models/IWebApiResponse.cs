using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Core.Models
{
    public interface IWebApiResponse
    {
        int Code { get; set; }
        bool IsSuccessful { get; set; }
        List<string> Messages { get; set; }
    }
}
