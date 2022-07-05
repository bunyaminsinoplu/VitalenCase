using VitalenCase.Models;

namespace VitalenCase.Interface
{
    public interface IFunctions
    {
        Task<string> Encrypt(string clearText, string guideKey);
        Task<string> Decrypt(string cipherText, string guideKey);
        Task<string> CreateJWT(User user);
    }
}
