using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace Pocal
{
    public static class Extensions
    {
        private static object Data;

        /// <summary>
        /// Navigates to the content specified by uniform resource identifier (URI).
        /// </summary>
        /// <param name="navigationService">The navigation service.</param>
        /// <param name="source">The URI of the content to navigate to.</param>
        /// <param name="data">The data that you need to pass to the other page 
        /// specified in URI.</param>
        public static void GoBack(this NavigationService navigationService, object data)
        {
            Data = data;
            navigationService.GoBack();
        }

        /// <summary>
        /// Gets the navigation data passed from the previous page.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <returns>System.Object.</returns>
        public static object GetNavigationData(this NavigationService service)
        {
            return Data;
        }
    }
}
