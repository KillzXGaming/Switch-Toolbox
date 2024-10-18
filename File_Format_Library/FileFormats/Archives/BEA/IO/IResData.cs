namespace BezelEngineArchive_Lib
{
    /// <summary>
    /// Represents the common interface for <see cref="ResFile"/> data instances.
    /// </summary>
    public interface IFileData
    {
        // ---- METHODS ------------------------------------------------------------------------------------------------

        /// <summary>
        /// Loads raw data from the <paramref name="loader"/> data stream into instances.
        /// </summary>
        /// <param name="loader">The <see cref="ResFileLoader"/> to load data with.</param>
        void Load(FileLoader loader);
        
        /// <summary>
        /// Saves header data of the instance and queues referenced data in the given <paramref name="saver"/>.
        /// </summary>
        /// <param name="saver">The <see cref="ResFileSaver"/> to save headers and queue data with.</param>
        void Save(FileSaver saver);
    }
}
