using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers
{
    public static class AppConstant
    {
        public static class ReservationRules
        {
            public const int TimezoneOffsetHours = 7;
            public const int MaxReservationsPerDay = 2;
            public const int MinSlotCount = 1;
            public const int MaxSlotCount = 4;
        }

        public static class Roles
        {
            public const string Admin = "Admin";
            public const string Manager = "Manager";
            public const string Driver = "Driver";
        }
    }
}