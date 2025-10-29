using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers.Enums
{
    public enum StopReason
    {
        // Lý do dừng (ManualStop, BatteryFull, InsufficientFunds, ReservationCompleted, SystemInterruption)
        ManualStop,
        BatteryFull,
        InsufficientFunds,
        ReservationCompleted,
        SystemInterruption
    }
}