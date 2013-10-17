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
    public class Repository<T> : IRepository where T : class
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
        public IQueryable<T> GetAll<T>() where T : class
        {
            return context.Set<T>().AsQueryable().Cast<T>();
        }

        /// <summary>
        /// Gets object from database by Id
        /// </summary>
        /// <returns></returns>
        public T GetSingle<T>(object Id) where T : class
        {
            return context.Set<T>().Find(Id) as T;
        }

        /// <summary>
        /// Select Single Item by specified expression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public T GetSingle<T>(System.Linq.Expressions.Expression<Func<T, bool>> expression) where T : class
        {
            return GetAll<T>().FirstOrDefault(expression);
        }

        //public IQueryable<T> Filter<T>(System.Linq.Expressions.Expression<Func<T, bool>> predicate) where T : class
        //{
        //    return context.Set<T>().Where<T>(predicate).AsQueryable<T>();            
        //}

        /// <summary>
        /// Gets objects from database by filter.
        /// </summary>
        /// <param name="predicate">Specified a filter</param>
        /// <returns></returns>
        public IQueryable<T> Filter<T>(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "") where T : class
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

        /// <summary>
        /// Gets objects from database with filting and paging.
        /// </summary>
        /// <typeparam name="Key"></typeparam>
        /// <param name="filter">Specified a filter</param>
        /// <param name="total">Returns the total records count of the filter.</param>
        /// <param name="index">Specified the page index.</param>
        /// <param name="size">Specified the page size</param>
        /// <returns></returns>
        public IQueryable<T> Filter<T>(Expression<Func<T, bool>> filter, out int total, int index = 0, int size = 50) where T : class
        {
            int skipCount = index * size;
            var _resetSet = filter != null ? context.Set<T>().Where<T>(filter).AsQueryable() : context.Set<T>().AsQueryable();
            _resetSet = skipCount == 0 ? _resetSet.Take(size) : _resetSet.Skip(skipCount).Take(size);
            total = _resetSet.Count();
            return _resetSet.AsQueryable();
        }

        /// <summary>
        /// Gets the object(s) is exists in database by specified filter.
        /// </summary>
        /// <param name="predicate">Specified the filter expression</param>
        /// <returns></returns>
        public bool Contains<T>(System.Linq.Expressions.Expression<Func<T, bool>> predicate) where T : class
        {
            return context.Set<T>().Count<T>(predicate) > 0;
        }

        /// <summary>
        /// Find object by keys.
        /// </summary>
        /// <param name="keys">Specified the search keys.</param>
        /// <returns></returns>
        public T Find<T>(params object[] keys) where T : class
        {
            return (T)context.Set<T>().Find(keys);
        }

        /// <summary>
        /// Find object by specified expression.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public T Find<T>(System.Linq.Expressions.Expression<Func<T, bool>> predicate) where T : class
        {
            return context.Set<T>().FirstOrDefault<T>(predicate);
        }

        /// <summary>
        /// Create/Insert a new object to database.
        /// </summary>
        /// <param name="t">Specified a new object to create.</param>
        /// <returns></returns>
        public T Insert<T>(T t) where T : class
        {
            var newEntry = context.Set<T>().Add(t);
            SaveChanges();
            return newEntry;
        }

        /// <summary>
        /// Delete the object from database.
        /// </summary>
        /// <param name="t">Specified a existing object to delete.</param>
        public int Delete<T>(object id) where T : class
        {
            DeleteLite(id);  //context.Set<T>().Remove(GetSingle<T>(id));
            return SaveChanges();
        }

        /// <summary>
        /// Delete the object from database.
        /// </summary>
        /// <param name="t">Specified a existing object to delete.</param>
        public int Delete<T>(T t) where T : class
        {
            DeleteLite<T>(t); // context.Set<T>().Remove(t);
            return SaveChanges();
        }

        /// <summary>
        /// Delete objects from database by specified filter expression.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public int Delete<T>(System.Linq.Expressions.Expression<Func<T, bool>> predicate) where T : class
        {
            //var objects = Filter<T>(predicate);
            //foreach (var obj in objects)
            //{
            //    context.Set<T>().Remove(obj);
            //}
            DeleteLite<T>(predicate);
            return SaveChanges();
        }

        /// <summary>
        /// Update object changes and save to database.
        /// </summary>
        /// <param name="t">Specified the object to save.</param>
        /// <returns></returns>
        public int Update<T>(T t) where T : class
        {
            if (UpdateLite<T>(t) == true)
            {
                return context.SaveChanges();
            }
            else
                return 0;

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

        public void DeleteLite<T>(object id) where T : class
        {
            context.Set<T>().Remove(GetSingle<T>(id));
        }

        public void DeleteLite<T>(T t) where T : class
        {
            context.Set<T>().Remove(t);
        }

        public void DeleteLite<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            var objects = Filter<T>(predicate);
            foreach (var obj in objects)
            {
                context.Set<T>().Remove(obj);
            }
        }


        public void InsertLite<T>(T t) where T : class
        {
            context.Set<T>().Add(t);
        }

        public bool UpdateLite<T>(T t) where T : class
        {
            bool success = false;
            if (t == null)
            {
                throw new ArgumentException("Cannot add a null entity.");
            }
            try
            {


                var entry = context.Entry<T>(t);

                if (entry.State == EntityState.Detached)
                {
                    var set = context.Set<T>();
                    var pkey = set.Create().GetType().GetProperty("Id").GetValue(t);
                    T attachedEntity = set.Find(pkey);  // You need to have access to key

                    if (attachedEntity != null)
                    {
                        var attachedEntry = context.Entry(attachedEntity);
                        attachedEntry.CurrentValues.SetValues(t);
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
                throw ex;
            }
            catch (Exception ex)
            {
                success = false;
                //throw ex;
            }
            return success;
            //var entry = context.Entry(t);
            //context.Set<T>().Attach(t);
            //entry.State = EntityState.Modified;
        }
    }
}
