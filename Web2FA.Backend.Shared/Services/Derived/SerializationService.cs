using Newtonsoft.Json;
using Web2FA.Backend.Shared.Extensions;
using Web2FA.Backend.Shared.Services.Base;

namespace Web2FA.Backend.Shared.Services.Derived
{
    public sealed class SerializationService : ServiceSingularBase<SerializationService>
    {
        public T? DeserializeObject<T>(object obj)
        {
            if (obj == null) return Activator.CreateInstance<T>();

            try
            {
                var objStr = Convert.ToString(obj);

                if (objStr == null || objStr.IsNullOrEmpty())
                {
                    return Activator.CreateInstance<T>();
                }

                return JsonConvert.DeserializeObject<T>(objStr, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            }
            catch (Exception ex)
            {
                LogService.DerivedObject.Write(log: ex.ToString(), data: obj.Serialize(), methodName: System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name, exceptionId: 0);
            }

            return Activator.CreateInstance<T>();
        }

        public string SerializeObject(object obj, bool isCamelCase = false)
        {
            try
            {
                if (isCamelCase)
                {
                    return JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
                    });
                }
                else
                {
                    return JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                }
            }
            catch (Exception ex)
            {
                LogService.DerivedObject.Write(log: ex.ToString(), methodName: System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name, exceptionId: 0);
            }

            return string.Empty;
        }
    }
}
