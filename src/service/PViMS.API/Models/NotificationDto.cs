using System.ComponentModel.DataAnnotations;

namespace PVIMS.API.Models
{
    public class NotificationDto
    {
        /// <summary>
        /// Identifier of notification
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// Notification icon
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// When the notification was created
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// Details of the notification
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// For routing purposed
        /// </summary>
        public string Route { get; set; }

        /// <summary>
        /// Color scheme to use
        /// </summary>
        public string Color { get; set; }

    }
}
