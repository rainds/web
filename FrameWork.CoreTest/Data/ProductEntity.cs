using System.ComponentModel.DataAnnotations;

namespace FrameWork.CoreTest.Data
{
    public class ProductEntity 
    {
         [Key]
        public    int  ProductId { get; set; }
       
        public string ProductCode { get; set; }

        public string ProductName { get; set; }

        public string ProductDesc { get; set; }
    }
}