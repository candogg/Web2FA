namespace Web2FA.Backend.Shared.Services.Base
{
    /// <summary>
    /// Author: Can DOĞU (CENTECH)
    /// </summary>
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
