using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities.Wallet
{
    public class Wallet
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey(nameof(UserId))]
        public AppUser appUser { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Dept { get; set; } = 0;
        public bool IsDept { get; set; } = false;

        public ICollection<WalletTransaction> Transactions { get; set; } = new List<WalletTransaction>();
    }
}