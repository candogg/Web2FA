using System.Diagnostics;
using Web2FA.Backend.Shared.Extensions;
using Web2FA.Backend.Shared.Services.Base;

namespace Web2FA.Backend.Shared.Services.Derived
{
    /// <summary>
    /// Author: Can DOĞU (CENTECH)
    /// </summary>
    public sealed class LogService : ServiceSingularBase<LogService>
    {
        public void Write(string log, int exceptionId, EventLogEntryType eventType = EventLogEntryType.Error, object? data = null, string? methodName = "")
        {
            Task.Run(() =>
            {
                try
                {
                    CheckEventSource();

                    log += data != null ? $"{Environment.NewLine}{data.Serialize()}" : string.Empty;

                    log += $"{Environment.NewLine}MethodName: {methodName}";

                    EventLog.WriteEntry("MediLOGApplication", log, eventType, exceptionId);
                }
                catch
                { }
            });
        }

        /// <summary>
        /// Eventlog'da event source yok ise yaratır
        /// </summary>
        public void CheckEventSource()
        {
            try
            {
                if (!EventLog.SourceExists("MediLOGApplication"))
                {
                    EventLog.CreateEventSource("MediLOGApplication", "MediLOGLogs");
                }
            }
            catch
            { }
        }
    }
}
