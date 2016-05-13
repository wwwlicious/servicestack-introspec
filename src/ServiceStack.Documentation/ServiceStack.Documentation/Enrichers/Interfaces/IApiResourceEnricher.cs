namespace ServiceStack.Documentation.Enrichers.Interfaces
{
    using Host;
    using Models;

    /// <summary>
    /// Basic operations for implementing a new documentation enricher
    /// </summary>
    public interface IApiResourceEnricher
    {
        /// <summary>
        /// Enriches ApiResourceDocumentation with information from specified Operation
        /// </summary>
        /// <param name="resourceSpecification"></param>
        /// <param name="operation"></param>
        void Enrich(ApiResourceDocumentation resourceSpecification, Operation operation);
    }
}