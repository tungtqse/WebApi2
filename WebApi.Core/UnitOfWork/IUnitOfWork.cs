using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.Core.Repository;

namespace WebApi.Core.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        int SaveChanges();
        IDbRepository<TEntity> Repository<TEntity>() where TEntity : class;
    }
}
