using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FrameWork.CoreTest.Data
{
    public class OrderSubEntity 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderSubId { get; set; }

        public int OrderId { get; set; }

        public string ProductCode { get; set; }

        public int Qty { get; set; }

        public decimal Price { get; set; }
    }
}