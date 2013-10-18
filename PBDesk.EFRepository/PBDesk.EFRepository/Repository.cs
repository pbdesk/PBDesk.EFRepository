using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using PBDesk.EFRepository.Exceptions;

namespace PBDesk.EFRepository
{
    public class Repository<T> : IRepository where T : IEntity
    {

        protected internal DbContext context = null;

        public Repository(DbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Gets all objects from database
        /// </summary>
        /// <returns></returns>
        public IQueryable<T> GetAll<T>() where T : Entity
        {
            try
            {
                return context.Set<T>().AsQueryable().Cast<T>();
            }
            catch(Exception ex)
            {
                throw new EFRepositoryException("Error while reading data from database via DbContext.", "Repository.GetAll()", ex);
            }
        }

        /// <summary>
        /// Gets object from database by Id
        /// </summary>
        /// <returns></returns>
        public T GetSingle<T>(object Id) where T : Entity
        {
            try
            {
                return context.Set<T>().Find(Id) as T;
            }
            catch (Exception ex)
            {
                throw new EFRepositoryException("Error while reading data from database via DbContext.", "Repository.GetSingle(Id)", ex);
            }
        }

        /// <summary>
        /// Select Single Item by specified expression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public T GetSingle<T>(System.Linq.Expressions.Expression<Func<T, bool>> expression) where T : Entity
        {
            try
            {
                return GetAll<T>().FirstOrDefault(expression);
            }
            catch (Exception ex)
            {
                throw new EFRepositoryException("Error while reading data from database via DbContext.", "Repository.GetSingle(expression)", ex);
            }
            
        }

        //public IQueryable<T> Filter<T>(System.Linq.Expressions.Expression<Func<T, bool>> predicate) where T : Entity
        //{
        //    return context.Set<T>().Where<T>(predicate).AsQueryable<T>();            
        //}

        /// <summary>
        /// Gets objects from database by filter.
        /// </summary>
        /// <param name="predicate">Specified a filter</param>
        /// <returns></returns>
        public IQueryable<T> Filter<T>(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "") where T : Entity
        {
            try
            {
                IQueryable<T> query = context.Set<T>();

                if (filter != null)
                {
                    query = query.Where(filter);
                }

                foreach (var includeProperty in includeProperties.Split
                    (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }

                if (orderBy != null)
                {
                    return orderBy(query).AsQueryable<T>();
                }
                else
                {
                    return query.AsQueryable<T>();
                }
            }
            catch(Exception ex)
            {
                throw new EFRepositoryException("Error while reading data from database via DbContext.", "Repository.Filter(filter,orderBy,includeProperties)", ex);
            }
        }

        /// <summary>
        /// Gets objects from database with filting and paging.
        /// </summary>
        /// <typeparam name="Key"></typeparam>
        /// <param name="filter">Specified a filter</param>
        /// <param name="total">Returns the total records count of the filter.</param>
        /// <param name="index">Specified the page index.</param>
        /// <param name="size">Specified the page size</param>
        /// <returns></returns>
        public IQueryable<T> Filter<T>(Expression<Func<T, bool>> filter, out int total, int index = 0, int size = 50) where T : Entity
        {
            try
            {
                int skipCount = index * size;
                var _resetSet = filter != null ? context.Set<T>().Where<T>(filter).AsQueryable() : context.Set<T>().AsQueryable();
                _resetSet = skipCount == 0 ? _resetSet.Take(size) : _resetSet.Skip(skipCount).Take(size);
                total = _resetSet.Count();
                return _resetSet.AsQueryable();
            }
            catch(Exception ex)
            {
                throw new EFRepositoryException("Error while reading data from database via DbContext.", "Repository.Filter(filter,total,index, size)", ex);
            }
        }

        /// <summary>
        /// Gets the object(s) is exists in database by specified filter.
        /// </summary>
        /// <param name="predicate">Specified the filter expression</param>
        /// <returns></returns>
        public bool Contains<T>(System.Linq.Expressions.Expression<Func<T, bool>> predicate) where T : Entity
        {
            try
            {
                return context.Set<T>().Count<T>(predicate) > 0;
            }
            catch (Exception ex)
            {
                throw new EFRepositoryException("Error while reading data from database via DbContext.", "Repository.Contains(predicate)", ex);
            }
        }

        /// <summary>
        /// Find object by keys.
        /// </summary>
        /// <param name="keys">Specified the search keys.</param>
        /// <returns></returns>
        public T Find<T>(params object[] keys) where T : Entity
        {
            try
            {
                return (T)context.Set<T>().Find(keys);
            }
            catch (Exception ex)
            {
                throw new EFRepositoryException("Error while reading data from database via DbContext.", "Repository.Find(keys)", ex);
            }
        }

        /// <summary>
        /// Find object by specified expression.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public T Find<T>(System.Linq.Expressions.Expression<Func<T, bool>> predicate) where T : Entity
        {
            try
            {
                return context.Set<T>().FirstOrDefault<T>(predicate);
            }
            catch (Exception ex)
            {
                throw new EFRepositoryException("Error while reading data from database via DbContext.", "Repository.Find(predicate)", ex);
            }
            
        }

        /// <summary>
        /// Create/Insert a new object to database.
        /// </summary>
        /// <param name="obj">Specified a new object to create.</param>
        /// <returns></returns>
        public T Insert<T>(T obj) where T : Entity
        {
            try
            {
                UpdateAuditInfo<T>(obj);
                var newEntry = context.Set<T>().Add(obj);
                SaveChanges();
                return newEntry;
            }
            catch(Exception ex)
            {
                throw new EFRepositoryException("Error while inserting data to database via DbContext.", "Repository.Insert(obj)", ex);
            }
        }

        /// <summary>
        /// Delete the object from database.
        /// </summary>
        /// <param name="t">Specified a existing object to delete.</param>
        public int Delete<TEntity>(object id) where TEntity : Entity
        {
            try
            {
                DeleteLite <TEntity>(id);  //context.Set<T>().Remove(GetSingle<T>(id));
                return SaveChanges();
            }
            catch (Exception ex)
            {
                throw new EFRepositoryException("Error while deleting data from database via DbContext.", "Repository.Delete(id)", ex);
            }
        }

        /// <summary>
        /// Delete the object from database.
        /// </summary>
        /// <param name="obj">Specified a existing object to delete.</param>
        public int Delete<T>(T obj) where T : Entity
        {
            try
            {
                DeleteLite<T>(obj); // context.Set<T>().Remove(t);
                return SaveChanges();
            }
            catch (Exception ex)
            {
                throw new EFRepositoryException("Error while deleting data from database via DbContext.", "Repository.Delete(obj)", ex);
            }
        }

        /// <summary>
        /// Delete objects from database by specified filter expression.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public int Delete<T>(System.Linq.Expressions.Expression<Func<T, bool>> predicate) where T : Entity
        {
            try
            {
                DeleteLite<T>(predicate);
                return SaveChanges();
            }
            catch (Exception ex)
            {
                throw new EFRepositoryException("Error while deleting data from database via DbContext.", "Repository.Delete(predicate)", ex);
            }
        }

        /// <summary>
        /// Update object changes and save to database.
        /// </summary>
        /// <param name="obj">Specified the object to save.</param>
        /// <returns></returns>
        public int Update<T>(T obj, string primaryKeyName = "Id") where T : Entity
        {
            try
            {
                if (UpdateLite<T>(obj) == true)
                {
                    return context.SaveChanges();
                }
                else
                    return 0;
            }
            catch (Exception ex)
            {
                throw new EFRepositoryException("Error while updating data to database via DbContext.", "Repository.Update(obj, primaryKeyName)", ex);
            }

        }

        public int SaveChanges()
        {
            if (context != null)
            {
                try
                {
                    return context.SaveChanges();
                }
                catch (DbEntityValidationException vEx)
                {
                    throw new EFRepositoryException("DbEntityValidationException Error while saving.", "Repository.SaveChanges()", vEx);
                }
                catch (Exception ex)
                {
                    throw new EFRepositoryException("Error while saving.", "Repository.SaveChanges()", ex);
                }
            }
            else
            {
                // throw new EFRepositoryException("Error while saving.", "Repository.SaveChanges()", ex);
                throw new EFRepositoryException("'context' object is null.", "Repository.SaveChanges()");
            }
        }

        public void Dispose()
        {
            if (context != null)

                context.Dispose();
        }

        public void DeleteLite<TEntity>(object id) where TEntity : Entity
        {
            try
            {
                context.Set<TEntity>().Remove(GetSingle<TEntity>(id));
            }
            catch (Exception ex)
            {
                throw new EFRepositoryException("Error while deleting data from database via DbContext.", "Repository.DeleteLite(id)", ex);
            }
        }

        public void DeleteLite<T>(T obj) where T : Entity
        {
            try
            {
                context.Set<T>().Remove(obj);
            }
            catch (Exception ex)
            {
                throw new EFRepositoryException("Error while deleting data from database via DbContext.", "Repository.DeleteLite(obj)", ex);
            }
        }

        public void DeleteLite<T>(Expression<Func<T, bool>> predicate) where T : Entity
        {
            try
            {
                var objects = Filter<T>(predicate);
                foreach (var obj in objects)
                {
                    context.Set<T>().Remove(obj);
                }
            }
            catch (Exception ex)
            {
                throw new EFRepositoryException("Error while deleting data from database via DbContext.", "Repository.DeleteLite(predicate)", ex);
            }
        }


        public void InsertLite<T>(T obj) where T : Entity
        {
            try
            {
                UpdateAuditInfo<T>(obj);
                context.Set<T>().Add(obj);
            }
            catch (Exception ex)
            {
                throw new EFRepositoryException("Error while inserting data to database via DbContext.", "Repository.InsertLite(obj)", ex);
            }
        }

        public bool UpdateLite<T>(T obj, string primaryKeyName = "Id") where T : Entity
        {
            bool success = false;
            try
            {
                if (String.IsNullOrWhiteSpace(primaryKeyName))
                {
                    primaryKeyName = "Id";
                }
                if (obj == null)
                {
                    throw new ArgumentException("Cannot add or update a null entity.");
                }

                UpdateAuditInfo<T>(obj);
                var entry = context.Entry<T>(obj);

                if (entry.State == EntityState.Detached)
                {
                    var set = context.Set<T>();
                    var pkey = set.Create().GetType().GetProperty(primaryKeyName).GetValue(obj);
                    T attachedEntity = set.Find(pkey);  // You need to have access to key

                    if (attachedEntity != null)
                    {
                        var attachedEntry = context.Entry(attachedEntity);
                        attachedEntry.CurrentValues.SetValues(obj);
                    }
                    else
                    {
                        entry.State = EntityState.Modified; // This should attach entity
                    }
                }
                success = true;
            }
            catch (OptimisticConcurrencyException ex)
            {
                success = false;
                throw new EFRepositoryException("Error while updating data to database via DbContext. OptimisticConcurrencyException.", "Repository.UpdateLite(obj, primaryKeyName)", ex);
            }
            catch (Exception ex)
            {
                success = false;
                throw new EFRepositoryException("Error while updating data to database via DbContext.", "Repository.UpdateLite(obj, primaryKeyName)", ex);
            }
            return success;
        }

        internal void UpdateAuditInfo<T>(T obj) where T: Entity
        {
            if (obj != null)
            {
                if(obj.LastUpdDate == default(DateTime) )
                {
                    obj.LastUpdDate = DateTime.Now;
                }
             }
            
        }

    }
}
