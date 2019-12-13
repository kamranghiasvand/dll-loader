using DllService.app.Model;
using DllService.Model;

namespace DllService.service
{
    public interface IDllContext
    {
        void Init();
        void LoadDll(DllEntity entity);
        object CallMethod(TypeLoadEntity entity);
    } 
}