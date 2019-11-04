namespace Macabre2D.Framework {

    /// <summary>
    /// An object pool.
    /// </summary>
    /// <typeparam name="T">The type of object to pool.</typeparam>
    public interface IObjectPool<T> {

        /// <summary>
        /// Gets the next pooled object.
        /// </summary>
        /// <returns></returns>
        T GetNext();

        /// <summary>
        /// Returns the object.
        /// </summary>
        /// <param name="pooledObject">The pooled object.</param>
        void ReturnObject(T pooledObject);
    }
}