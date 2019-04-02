using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Core.Repository
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces", Justification = "IDbRepository will be used in reflection to determine repository without knowing TEntity")]
    public interface IDbRepository
    {
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "The name Repository is self explain it was a collection, but RepositoryCollection sounds like 'collection of repository' (repositories) which has different meaning.")]
    public interface IDbRepository<TEntity> : IDbRepository, IDbSet<TEntity>, IQueryable<TEntity>, IEnumerable<TEntity>, IQueryable, IEnumerable where TEntity : class
    {
    }
}
