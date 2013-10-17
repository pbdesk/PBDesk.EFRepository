using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PBDesk.EFRepository
{
    public interface IRepository : IDisposable
    {

        IQueryable<T> GetAll<T>() where T : class;

        T GetSingle<T>(object Id) where T : class;
        T GetSingle<T>(Expression<Func<T, bool>> expression) where T : class;

        IQueryable<T> Filter<T>(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "") where T : class;
        IQueryable<T> Filter<T>(Expression<Func<T, bool>> filter, out int total, int index = 0, int size = 50) where T : class;

        bool Contains<T>(Expression<Func<T, bool>> predicate) where T : class;

        T Find<T>(params object[] keys) where T : class;
        T Find<T>(Expression<Func<T, bool>> predicate) where T : class;

        T Insert<T>(T t) where T : class;

        int Delete<T>(object id) where T : class;
        int Delete<T>(T t) where T : class;
        int Delete<T>(Expression<Func<T, bool>> predicate) where T : class;

        int Update<T>(T t) where T : class;

        void InsertLite<T>(T t) where T : class;
        bool UpdateLite<T>(T t) where T : class;
        void DeleteLite<T>(object id) where T : class;
        void DeleteLite<T>(T t) where T : class;
        void DeleteLite<T>(Expression<Func<T, bool>> predicate) where T : class;

        int SaveChanges();

    }
}
