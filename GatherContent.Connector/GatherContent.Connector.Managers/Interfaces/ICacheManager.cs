namespace GatherContent.Connector.Managers.Interfaces
{
    /// <summary>
    /// </summary>
    public interface ICacheManager
    {
        /// <summary>
        ///     The get.
        /// </summary>
        /// <param name="key">
        ///     The key.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="T" />.
        /// </returns>
        T Get<T>(string key);

        /// <summary>
        ///     The is set.
        /// </summary>
        /// <param name="key">
        ///     The key.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        bool IsSet(string key);

        /// <summary>
        ///     The set.
        /// </summary>
        /// <param name="key">
        ///     The key.
        /// </param>
        /// <param name="data">
        ///     The data.
        /// </param>
        /// <param name="cacheTime">
        ///     The cache time.
        /// </param>
        void Set(string key, object data, int cacheTime);
    }
}