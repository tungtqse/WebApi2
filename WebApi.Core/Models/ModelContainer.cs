using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Core.Models
{
    /// <summary>
    /// To include Fluent Valiation process into AutoMapper
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ModelContainer<T>
    {
        public T Value { get; set; }
    }
}
