using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Core.Infrastructure.MediatedControllers
{
    public interface ICommandResponse
    {
        ValidationResult ValidationResult { get; set; }
    }
}
