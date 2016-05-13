namespace ServiceStack.Documentation.Settings
{
    public enum EnrichmentStrategy
    {
        /// <summary>
        /// Each enricher will be called and result will be union of all
        /// </summary>
        Union = 0,

        /// <summary>
        /// Lower priority enrichers will only be called if property null/empty
        /// </summary>
        SetIfEmpty = 1
    }
}