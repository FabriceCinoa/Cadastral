using System.Linq.Expressions;

namespace Common.Repository.Interfaces
{
    public interface IRepository<IDBContext> : IDisposable 
    {
         IDBContext Context { get; set; }

        /*
        public IQueryable<TOut> Find<TOut>(Expression<Func<TOut, bool>> where) where TOut : class, ICoreEntity;

        public IQueryable<TOut> All<TOut>() where TOut : class, ICoreEntity;

        public TOut First<TOut>(Expression<Func<TOut, bool>> where = null) where TOut : class, ICoreEntity;*/

        public int Count<TOut>() where TOut : class, ICoreEntity;
        /*
        public bool Delete<TOut>(TOut entity) where TOut : class, ICoreEntity;

        public IQueryable<TOut> Set<TOut>() where TOut : class, ICoreEntity;*/


        public bool SaveChanges();

    }
}
