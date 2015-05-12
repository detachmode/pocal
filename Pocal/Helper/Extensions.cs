using System.Windows.Navigation;

namespace Pocal.Helper
{
    public static class Extensions
    {
        private static object _data;

        /// <summary>
        ///     Navigates to the content specified by uniform resource identifier (URI).
        /// </summary>
        /// <param name="navigationService">The navigation service.</param>
        /// <param name="data">
        ///     The data that you need to pass to the other page
        ///     specified in URI.
        /// </param>
        public static void GoBack(this NavigationService navigationService, object data)
        {
            _data = data;
            navigationService.GoBack();
        }

        /// <summary>
        ///     Gets the navigation data passed from the previous page.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <returns>System.Object.</returns>
        public static object GetNavigationData(this NavigationService service)
        {
            return _data;
        }
    }
}