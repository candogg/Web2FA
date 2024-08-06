namespace Web2FA.Backend.Shared.Services.Base
{
    public abstract class ServiceSingularBase<T> where T : class, new()
    {
        private static T? derivedObject;

        public static T DerivedObject
        {
            get
            {
                return derivedObject ??= Activator.CreateInstance<T>();
            }
        }
    }
}
