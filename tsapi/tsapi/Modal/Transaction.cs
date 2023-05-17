

using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace tsapi.Modal
{
    public class Transactions
    {
        public List<TransactionDetails> TransactionList { get; set; }

    }

    public class TransactionDetails
    {
        [Required]
        [StringLength(50)]
        public string UniqueIdentityKey { get; set; }

        [Required]
        [StringLength(50)]
        public string AccountIBAN { get; set; }

        [Required]
        public DateTime ValueDate { get; set; }

        [Required]
        [StringLength(100)]
        public string Description { get; set; }

        [Required]
        public string Currency { get; set; }


        [Required]
        public decimal Amount { get; set; }
    }



}
