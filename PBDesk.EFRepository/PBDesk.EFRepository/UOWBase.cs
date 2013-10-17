using PBDesk.EFRepository.Exceptions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBDesk.EFRepository
{
    public class UOWBase : IDisposable
    {
        protected DbContext context = null;

        #region Dispose

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (context != null)
                    {
                        context.Dispose();
                    }
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Save

        public int SaveChanges()
        {
            if (context != null)
            {
                try
                {
                    return context.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw new EFRepositoryException("Error while saving.", "UOWBase.SaveChanges()", ex);
                }
            }
            else
            {
                throw new EFRepositoryException("'context' object is null.", "UOWBase.SaveChanges()");
            }
        }

        #endregion
    }
}
