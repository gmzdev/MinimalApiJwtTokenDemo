using System.Security.Cryptography;
using System.Text;

public class PasswordHasher : IDisposable
{
    private readonly HMACSHA512 x = new(Encoding.UTF8.GetBytes("MinimalApiDemo"));

    public async Task<byte[]> Hash(string password, byte[] salt)
    {
        var bytes = Encoding.UTF8.GetBytes(password);

        var allBytes = new byte[bytes.Length + salt.Length];
        Buffer.BlockCopy(bytes, 0, allBytes, 0, bytes.Length);
        Buffer.BlockCopy(salt, 0, allBytes, bytes.Length, salt.Length);

        return await x.ComputeHashAsync(new MemoryStream(allBytes));
    }

    public void Dispose() => x.Dispose();
}