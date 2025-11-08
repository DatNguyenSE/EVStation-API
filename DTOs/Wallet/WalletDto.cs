using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs.Wallet
{
    public class WalletDto
    {
        public decimal Balance { get; set; }
        public decimal Debt { get; set; }
        public bool IsDebt { get; set; }
    }
}