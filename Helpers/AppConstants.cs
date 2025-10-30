using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Humanizer;

namespace API.Helpers
{
    public static class AppConstant
    {
        public static class ChargingRules
        {
            public const int IDLE_GRACE_MINUTES = 1;
        }
        
        public static class ReservationRules
        {
            public const int TimezoneOffsetHours = 7;
            public const int MaxReservationsPerDay = 2;
            public const int MinSlotCount = 1;
            public const int MaxSlotCount = 4;
            public const int CancellationCutoffMinutes = 20;
            public const int NoShowGracePeriodMinutes = 15;
            public const int slotDurationMinutes = 60;
        }

        public static class Roles
        {
            public const string Admin = "Admin";
            public const string Manager = "Manager";
            public const string Driver = "Driver";
            public const string Operator = "Operator";
            public const string Technician = "Technician";
        }
    }
}