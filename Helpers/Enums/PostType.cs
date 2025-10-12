using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers.Enums
{
    public enum PostType
    {
        Normal,   // AC thường (11kW, Type2)
        Fast,     // DC nhanh (60kW, CCS2)
        Scooter   // Xe máy (1.2kW, VinEScooter)
    }
}