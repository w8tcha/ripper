namespace PGRipper.Objects
{
    /// <summary>
    /// The Cache Object Item.
    /// </summary>
    public class CacheObject : object
    {
        /// <summary>
        /// Gets or sets FilePath.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets Url.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsDownloaded.
        /// </summary>
        public bool IsDownloaded { get; set; }
    }
}