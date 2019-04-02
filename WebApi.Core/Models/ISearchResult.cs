using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Core.Models
{
    public interface ISearchResult<TResult>
        where TResult : class
    {
        IList<TResult> SearchResultItems { get; set; }
        int Count { get; set; }
    }
}
