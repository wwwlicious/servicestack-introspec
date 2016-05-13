namespace ServiceStack.Documentation.Enrichers.Interfaces
{
    using System;

    /// <summary>
    /// Methods for populating a documentation resource
    /// </summary>
    public interface IResourceEnricher : IEnrich
    {
        string GetTitle(Type type);
        string GetDescription(Type type);
        string GetNotes(Type type);
    }
}