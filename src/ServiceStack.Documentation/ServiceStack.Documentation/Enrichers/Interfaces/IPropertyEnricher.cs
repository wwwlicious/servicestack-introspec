namespace ServiceStack.Documentation.Enrichers.Interfaces
{
    using System.Reflection;

    /// <summary>
    /// Methods for populating a documentatino resource parameter
    /// </summary>
    public interface IPropertyEnricher : IEnrich
    {
        string GetTitle(PropertyInfo pi);
        string GetDescription(PropertyInfo pi);
        string GetNotes(PropertyInfo pi);
        bool? GetAllowMultiple(PropertyInfo pi);
        string[] GetExternalLinks(PropertyInfo pi);
        string GetContraints(PropertyInfo pi);
        bool? GetIsRequired(PropertyInfo pi);
        string GetParamType(PropertyInfo pi);
    }
}