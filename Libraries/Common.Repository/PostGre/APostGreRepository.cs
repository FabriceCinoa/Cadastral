using Common.Repository.DBContext;
using Common.Repository.Interfaces;


namespace Common.Repository.PostGre
{
    public abstract class APostGreRepository<PostGreyContext> : IRepository<PostGreyContext> where PostGreyContext :  APostGreContext
    {
        private bool disposedValue;


        protected PostGreyContext? Context { get; set; }
        PostGreyContext IRepository<PostGreyContext>.Context { get => Context; set => Context = value; }


        protected APostGreRepository(PostGreyContext Context)
        {
            this.Context = Context;
        }


        public bool SaveChanges()
        {
            return this.Context.SaveChanges() > 0;
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: supprimer l'état managé (objets managés)
                }

                // TODO: libérer les ressources non managées (objets non managés) et substituer le finaliseur
                // TODO: affecter aux grands champs une valeur null
                disposedValue = true;
            }
        }

        // // TODO: substituer le finaliseur uniquement si 'Dispose(bool disposing)' a du code pour libérer les ressources non managées
        // ~APostGreRepository()
        // {
        //     // Ne changez pas ce code. Placez le code de nettoyage dans la méthode 'Dispose(bool disposing)'
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Ne changez pas ce code. Placez le code de nettoyage dans la méthode 'Dispose(bool disposing)'
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public int Count<TOut>() where TOut : class, ICoreEntity
        {
            return this.Context.Set<TOut>().Count();
        }


        void IDisposable.Dispose()
        {
            Context?.Dispose();
        }
    }
}
