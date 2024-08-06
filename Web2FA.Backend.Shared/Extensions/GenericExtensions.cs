using Web2FA.Backend.Shared.Services.Derived;

namespace Web2FA.Backend.Shared.Extensions
{
    public static class GenericExtensions
    {
        public static string Serialize(this object item)
        {
            return SerializationService.DerivedObject.SerializeObject(item);
        }

        public static T? Deserialize<T>(this object obj)
        {
            try
            {
                return SerializationService.DerivedObject.DeserializeObject<T>(obj);
            }
            catch (Exception ex)
            {
                LogService.DerivedObject.Write(log: ex.ToString(), methodName: System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name, exceptionId: 0);
            }

            return default;
        }

        public static bool In<T>(this T item, params T[] items)
        {
            if (items == null) return false;

            return items.Contains(item);
        }
    }
}
