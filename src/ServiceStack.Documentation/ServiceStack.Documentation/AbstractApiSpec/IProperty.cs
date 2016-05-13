namespace ServiceStack.Documentation.AbstractApiSpec
{
    public interface IProperty
    {
        string Title { get; set; }
        string Description { get; set; }
        bool? IsRequired { get; set; }
    }
}