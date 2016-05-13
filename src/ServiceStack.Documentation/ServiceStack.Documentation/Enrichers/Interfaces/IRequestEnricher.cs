namespace ServiceStack.Documentation.Enrichers.Interfaces
{
    using Host;
    using Models;

    /// <summary>
    /// Methods for populating a documentation response
    /// </summary>
    public interface IRequestEnricher : IEnrich
    {
        string[] GetVerbs(Operation operation);
        StatusCode[] GetStatusCodes(Operation operation);
        string GetRelativePath(Operation operation);
    }
}