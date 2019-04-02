using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WebApi.Common;
using WebApi.Core.Helper;
using WebApi.Core.Repository;
using WebApi.DataAccess;

namespace WebApi.Core.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        readonly MainContext dbContext;
        readonly string TransactionId;
        readonly Dictionary<Type, IDbRepository> cachedRepositories = new Dictionary<Type, IDbRepository>();

        public UnitOfWork(MainContext dbContext)
        {
            this.dbContext = dbContext;
            this.TransactionId = Convert.ToString(Guid.NewGuid());
        }

        #region Implements

        public IDbRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            var type = typeof(TEntity);

            if (cachedRepositories.ContainsKey(type))
            {
                return cachedRepositories[type] as IDbRepository<TEntity>;
            }
            else
            {
                var repo = new DbRepository<TEntity>(dbContext.Set<TEntity>());
                cachedRepositories[type] = repo;

                return repo;
            }
        }

        public MainContext GetDbContext()
        {
            return dbContext;
        }

        public int SaveChanges()
        {
            Type type;
            var list = new List<DbEntityEntry>();
            string currentUsername = string.Empty;
            string currentUserDisplayname = string.Empty;
            Guid currentUserId = Constant.SystemId;
            var currentDateTime = DateTime.Now;

            #region Get AuditTrail

            // Get AuditTrail DbSet
            var auditType = dbContext
            .GetType()
            .GetProperties()
            .Where(p =>
                p.PropertyType.IsGenericType &&
                p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
            .Select(p => p.PropertyType.GetGenericArguments()[0])
            .FirstOrDefault(t => t.Name == "AuditTrail");
            var dbset = dbContext.Set(auditType);

            #endregion

            var changeSet = dbContext.ChangeTracker.Entries();
            changeSet.Select(t => new
            {
                Original = t.OriginalValues.PropertyNames.ToDictionary(pn => pn, pn => t.OriginalValues[pn]),
                Current = t.CurrentValues.PropertyNames.ToDictionary(pn => pn, pn => t.CurrentValues[pn]),
            });


            if (changeSet != null)
            {
                var user = SessionHelper.GetSessionObject("UserContext") as Models.UserProfileModel;

                if (user != null)
                {
                    currentUserId = user.Id;
                }

                var entries = changeSet.Where(f => f.State == EntityState.Added || f.State == EntityState.Modified);

                foreach (var entry in entries)
                {
                    type = entry.Entity.GetType();
                    var entityName = type.Name;

                    if (entry.State == EntityState.Modified)
                    {
                        #region Add AuditTrail

                        var originalValues = entry.OriginalValues.PropertyNames.ToDictionary(pn => pn, pn => entry.OriginalValues[pn]);
                        var currentValues = entry.CurrentValues.PropertyNames.ToDictionary(pn => pn, pn => entry.CurrentValues[pn]);
                        XElement xml = new XElement("Change");

                        foreach (var value in originalValues)
                        {
                            var oldValue = value.Value != null ? value.Value.ToString() : string.Empty;
                            var newValue = currentValues[value.Key] != null ? currentValues[value.Key].ToString() : string.Empty;
                            if (oldValue != newValue)
                            {
                                var field = new XElement("field");
                                var att = new XAttribute("Name", value.Key);
                                field.Add(att);
                                var oldNode = new XElement("OldValue", oldValue);
                                var newNode = new XElement("NewValue", newValue);
                                field.Add(oldNode);
                                field.Add(newNode);
                                xml.Add(field);

                            }
                        }

                        // Create instance of AuditTrail for edit item
                        var instanceAuditTrail = Activator.CreateInstance(auditType);
                        var itemId = (Guid)type.GetProperty(Constant.AuditTrailProperty.Id).GetValue(entry.Entity, null);
                        var datatable = GetTableName(entry);
                        auditType.GetProperty(Constant.AuditTrailProperty.ItemId).SetValue(instanceAuditTrail, itemId, null);
                        auditType.GetProperty(Constant.AuditTrailProperty.TableName).SetValue(instanceAuditTrail, datatable, null);
                        auditType.GetProperty(Constant.AuditTrailProperty.ModifiedDate).SetValue(instanceAuditTrail, currentDateTime, null);
                        auditType.GetProperty(Constant.AuditTrailProperty.ModifiedBy).SetValue(instanceAuditTrail, currentUserId, null);
                        auditType.GetProperty(Constant.AuditTrailProperty.TrackChange).SetValue(instanceAuditTrail, xml.ToString(), null);
                        auditType.GetProperty(Constant.AuditTrailProperty.TransactionId).SetValue(instanceAuditTrail, TransactionId, null);
                        auditType.GetProperty(Constant.AuditTrailProperty.StatusId).SetValue(instanceAuditTrail, Constant.StatusId.Active, null);
                        auditType.GetProperty(Constant.AuditTrailProperty.CreatedDate).SetValue(instanceAuditTrail, currentDateTime, null);
                        auditType.GetProperty(Constant.AuditTrailProperty.CreatedBy).SetValue(instanceAuditTrail, currentUserId, null);

                        // Insert AuditTrail
                        dbset.Add(instanceAuditTrail);

                        #endregion
                    }
                    else
                    {
                        #region Set status as active/inactive
                        var status = type.GetProperty(Constant.BaseProperty.StatusId).GetValue(entry.Entity, null);
                        if (status == null)
                            type.GetProperty(Constant.BaseProperty.StatusId).SetValue(entry.Entity, Constant.StatusId.Active, null);
                        list.Add(entry);
                        #endregion
                    }

                    #region Update System Fields

                    if (entry.State == EntityState.Added
                        && type.GetProperty(Constant.BaseProperty.CreatedBy) != null && type.GetProperty(Constant.BaseProperty.CreatedDate) != null
                        && type.GetProperty(Constant.BaseProperty.ModifiedBy) != null && type.GetProperty(Constant.BaseProperty.ModifiedDate) != null)
                    {
                        type.GetProperty(Constant.BaseProperty.CreatedDate).SetValue(entry.Entity, currentDateTime, null);
                        type.GetProperty(Constant.BaseProperty.CreatedBy).SetValue(entry.Entity, currentUserId, null);
                        type.GetProperty(Constant.BaseProperty.ModifiedDate).SetValue(entry.Entity, currentDateTime, null);
                        type.GetProperty(Constant.BaseProperty.ModifiedBy).SetValue(entry.Entity, currentUserId, null);
                    }
                    else if (entry.State == EntityState.Modified
                        && type.GetProperty(Constant.BaseProperty.ModifiedBy) != null && type.GetProperty(Constant.BaseProperty.ModifiedDate) != null)
                    {
                        type.GetProperty(Constant.BaseProperty.ModifiedDate).SetValue(entry.Entity, currentDateTime, null);
                        type.GetProperty(Constant.BaseProperty.ModifiedBy).SetValue(entry.Entity, currentUserId, null);
                    }

                    #endregion
                }
            }

            #region Save

            try
            {
                //Save all changes in DB Context
                var result = dbContext.SaveChanges();

                #region  Set & Save AuditTrail for new item

                try
                {
                    foreach (var entry in list)
                    {
                        type = entry.Entity.GetType();
                        var entityName = type.Name;
                        XElement xml = new XElement("Create");

                        var instanceAuditTrail = Activator.CreateInstance(auditType);
                        var itemId = (Guid)type.GetProperty(Constant.AuditTrailProperty.Id).GetValue(entry.Entity, null);
                        var datatable = GetTableName(entry);
                        auditType.GetProperty(Constant.AuditTrailProperty.ItemId).SetValue(instanceAuditTrail, itemId, null);
                        auditType.GetProperty(Constant.AuditTrailProperty.TableName).SetValue(instanceAuditTrail, datatable, null);
                        auditType.GetProperty(Constant.AuditTrailProperty.ModifiedDate).SetValue(instanceAuditTrail, currentDateTime, null);
                        auditType.GetProperty(Constant.AuditTrailProperty.ModifiedBy).SetValue(instanceAuditTrail, currentUserId, null);
                        auditType.GetProperty(Constant.AuditTrailProperty.TrackChange).SetValue(instanceAuditTrail, xml.ToString(), null);
                        auditType.GetProperty(Constant.AuditTrailProperty.TransactionId).SetValue(instanceAuditTrail, TransactionId, null);
                        auditType.GetProperty(Constant.AuditTrailProperty.StatusId).SetValue(instanceAuditTrail, Constant.StatusId.Active, null);
                        auditType.GetProperty(Constant.AuditTrailProperty.CreatedDate).SetValue(instanceAuditTrail, currentDateTime, null);
                        auditType.GetProperty(Constant.AuditTrailProperty.CreatedBy).SetValue(instanceAuditTrail, currentUserId, null);

                        // Insert AuditTrail
                        dbset.Add(instanceAuditTrail);
                    }

                    dbContext.SaveChanges();

                }
                catch (Exception ex) { }

                return result;

                #endregion

            }
            catch (DbEntityValidationException ex)
            {
                foreach (var eve in ex.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }

            #endregion
        }

        public void Dispose()
        {
            dbContext.Dispose();
        }

        private string GetTableName(DbEntityEntry ent)
        {
            ObjectContext objContext = ((IObjectContextAdapter)this.GetDbContext()).ObjectContext;
            Type entityType = ent.Entity.GetType();

            if (entityType.BaseType != null && entityType.Namespace == "System.Data.Entity.DynamicProxies")
            {
                entityType = entityType.BaseType;
            }

            return entityType.Name;
        }

        #endregion

    }
}
