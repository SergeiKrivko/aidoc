namespace AiDoc.Application.Shemas;

public class Feature
{
    public required string Name { get; set; }
    public Feature[] Children { get; set; } = [];
}