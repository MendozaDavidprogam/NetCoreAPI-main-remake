//NetCoreAPI-main/Web/Auxiliar/AllowAnonymousAttribute.cs

namespace Web.Auxiliar
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AllowAnonymousAttribute : Attribute
    {
    }
}
