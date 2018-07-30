using System.ComponentModel.DataAnnotations;
using FrameWork.Core.Data;

namespace FrameWork.CoreTest.Data
{
    public class ProductEntity : IEntity
    {
        [Key]
        public int ProductId { get; set; }

        public string ProductCode { get; set; }

        public string ProductName { get; set; }

        public string ProductDesc { get; set; }
    }
}