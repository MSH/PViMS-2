using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
    [Table(nameof(SystemLog))]
    public class SystemLog : AuditedEntityBase
    {
        public SystemLog(string sender, string eventType, string exceptionCode, string exceptionMessage)
        {
            Sender = sender;
            EventType = eventType;
            ExceptionCode = exceptionCode;
            ExceptionMessage = exceptionMessage;
        }

        [Required]
        public string Sender { get; set; }

        [Required]
        public string EventType { get; set; }

        [Required]
        public string ExceptionCode { get; set; }

        [Required]
        public string ExceptionMessage { get; set; }

        public string ExceptionStackTrace { get; set; }

        public string InnerExceptionMessage { get; set; }

        public string InnerExceptionStackTrace { get; set; }

        public string RemoteIpAddress { get; set; }
    }
}
