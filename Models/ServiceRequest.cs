
using System;

namespace MunicipalServicesApp.Models
{
    public class ServiceRequest
    {
        public int RequestId { get; set; }
        public string ExternalReferenceNumber { get; set; }
        public string ResidentName { get; set; }
        public string Category { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public DateTime DateSubmitted { get; set; }
        public DateTime? DateUpdated { get; set; }

        public override string ToString()
        {
            return "#" + RequestId + " - " +
                   Category + " - " + Status;
        }
    }
}
