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

        #region Get/Read Methods

        IQueryable<T> GetAll<T>() where T : class;

        T GetSingle<T>(object Id) where T : class;
        T GetSingle<T>(Expression<Func<T, bool>> expression) where T : class;

        IQueryable<T> Filter<T>(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "") where T : class;
        IQueryable<T> Filter<T>(Expression<Func<T, bool>> filter, out int total, int index = 0, int size = 50) where T : class;

        bool Contains<T>(Expression<Func<T, bool>> predicate) where T : class;

        T Find<T>(params object[] keys) where T : class;
        T Find<T>(Expression<Func<T, bool>> predicate) where T : class;

        #endregion

        #region Insert Methods

        #region Insert Light Methods

        void InsertLite<T>(T obj) where T : class;

        #endregion

        T Insert<T>(T obj) where T : class;

        #endregion

        #region Update Methods

        #region Update Light Methods

        bool UpdateLite<T>(T obj, string primaryKeyName) where T : class;

        #endregion

        int Update<T>(T obj, string primaryKeyName) where T : class;

        #endregion

        #region Delete Methods

        #region Delete Light Methods

        void DeleteLite<T>(object id) where T : class;
        void DeleteLite<T>(T obj) where T : class;
        void DeleteLite<T>(Expression<Func<T, bool>> predicate) where T : class;

        #endregion

        int Delete<T>(object id) where T : class;
        int Delete<T>(T obj) where T : class;
        int Delete<T>(Expression<Func<T, bool>> predicate) where T : class;

        #endregion

        #region Save

        int SaveChanges();

        #endregion

    }
}
