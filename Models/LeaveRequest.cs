using System;

namespace PayrollManagementAPI.Models
{
    public class LeaveRequest
    {
        public int Id { get; set; }

        public int EPFNo { get; set; }

        public string Reason { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string Status { get; set; } = "Pending";
    }
}