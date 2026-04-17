using System.Security.Cryptography;

namespace Twitter.WebApi.Atributos;

[AttributeUsage(AttributeTargets.Class)]
public class DeveloperAuthor() : Attribute
{
    public string Name { get; set; }
    public string Description { get; set; }
}
