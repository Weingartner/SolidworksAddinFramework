using ReactiveUI;

namespace SolidworksAddinFramework
{
    /// <summary>
    /// Provides a clone of the original data to use in 
    /// a PMP. When commit is called the data is copied
    /// from the clone into the original. Rollback undoes
    /// any changes.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class ReactiveTransaction<T> where T : ReactiveObject
    {
        private readonly T _Original;
        public T Data { get; private set; }

        public ReactiveTransaction(T original)
        {
            _Original = original;
            Data = Json.Clone(_Original);
        }

        public void Rollback()
        {
            Json.Copy(_Original, Data);
        }

        public void Commit()
        {
            using (_Original.DelayChangeNotifications())
            {
                Json.Copy(Data, _Original);
            }
        }
    }
}