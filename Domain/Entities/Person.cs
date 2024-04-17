public class Person
{
    public Guid PersonId { get; set; }
    public string? Email { get; set; }
    public byte[] Hash { get; set; } = Array.Empty<byte>();
    public byte[] Salt { get; set; } = Array.Empty<byte>();

}