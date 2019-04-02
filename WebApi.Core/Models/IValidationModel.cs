using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Core.Models
{
    public interface IValidationModel
    {
        bool IsValid { get; set; }
        List<string> Messages { get; set; }
    }
}
