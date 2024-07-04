using HNG_WEB_API.Models;

namespace HNG_WEB_API.Service
{
    public interface IHngService
    {
        Task<Visitor> GetVisitor(string ipStack2, string city, string name);
    }
}
