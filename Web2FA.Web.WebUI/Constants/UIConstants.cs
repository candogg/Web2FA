using Web2FA.Backend.Shared.Extensions;

namespace Web2FA.Web.WebUI.Constants
{
    public static class UIConstants
    {
        private static string? apiUrl;

        public static string? ApiUrl
        {
            get
            {
                return apiUrl;
            }
            set
            {
                apiUrl = value;

                if (apiUrl != null && apiUrl.IsNotNullOrEmpty() && !apiUrl.EndsWith("/"))
                {
                    apiUrl = $"{apiUrl}/";
                }
            }
        }
    }
}
