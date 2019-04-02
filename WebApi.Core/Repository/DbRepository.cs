using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Core.Repository
{
    public class DbRepository<TEntity> : IDbRepository<TEntity> where TEntity : class
    {
        Type entityType;
        readonly IDbSet<TEntity> dbSet;
        readonly IQueryable<TEntity> dbSetIQueryable;
        readonly bool permanentDelete;
        readonly bool getDeleted;

        #region Implements

        public DbRepository(IDbSet<TEntity> dbSet, bool getDeleted = false, bool permanentDelete = false)
        {
            if (dbSet == null) throw new ArgumentNullException("dbSet");

            this.dbSet = dbSet;
            this.getDeleted = getDeleted;
            this.permanentDelete = permanentDelete;
            this.dbSetIQueryable = dbSet as IQueryable<TEntity>;
            this.entityType = typeof(TEntity);
        }


        public Type ElementType
        {
            get { return dbSetIQueryable.ElementType; }
        }

        public Expression Expression
        {
            get { return dbSetIQueryable.Expression; }
        }

        public ObservableCollection<TEntity> Local
        {
            get { return dbSet.Local; }
        }

        public IQueryProvider Provider
        {
            get { return dbSetIQueryable.Provider; }
        }

        public TEntity Add(TEntity entity)
        {
            return dbSet.Add(entity);
        }

        public TEntity Attach(TEntity entity)
        {
            return dbSet.Attach(entity);
        }

        public TEntity Create()
        {
            return dbSet.Create();
        }

        public TDerivedEntity Create<TDerivedEntity>() where TDerivedEntity : class, TEntity
        {
            return dbSet.Create<TDerivedEntity>();
        }

        public TEntity Find(params object[] keyValues)
        {
            return dbSet.Find(keyValues);
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return dbSetIQueryable.GetEnumerator();
        }

        public TEntity Remove(TEntity entity)
        {
            return dbSet.Remove(entity);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
