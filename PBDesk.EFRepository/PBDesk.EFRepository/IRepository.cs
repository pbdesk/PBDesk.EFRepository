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

        IQueryable<T> GetAll<T>() where T : Entity;

        T GetSingle<T>(object Id) where T : Entity;
        T GetSingle<T>(Expression<Func<T, bool>> expression) where T : Entity;

        IQueryable<T> Filter<T>(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "") where T : Entity;
        IQueryable<T> Filter<T>(Expression<Func<T, bool>> filter, out int total, int index = 0, int size = 50) where T : Entity;

        bool Contains<T>(Expression<Func<T, bool>> predicate) where T : Entity;

        T Find<T>(params object[] keys) where T : Entity;
        T Find<T>(Expression<Func<T, bool>> predicate) where T : Entity;

        #endregion

        #region Insert Methods

        #region Insert Light Methods

        void InsertLite<T>(T obj) where T : Entity;

        #endregion

        T Insert<T>(T obj) where T : Entity;

        #endregion

        #region Update Methods

        #region Update Light Methods

        bool UpdateLite<T>(T obj, string primaryKeyName) where T : Entity;

        #endregion

        int Update<T>(T obj, string primaryKeyName) where T : Entity;

        #endregion

        #region Delete Methods

        #region Delete Light Methods

        void DeleteLite<T>(object id) where T : Entity;
        void DeleteLite<T>(T obj) where T : Entity;
        void DeleteLite<T>(Expression<Func<T, bool>> predicate) where T : Entity;

        #endregion

        int Delete<T>(object id) where T : Entity;
        int Delete<T>(T obj) where T : Entity;
        int Delete<T>(Expression<Func<T, bool>> predicate) where T : Entity;

        #endregion

        #region Save

        int SaveChanges();

        #endregion

    }
}
