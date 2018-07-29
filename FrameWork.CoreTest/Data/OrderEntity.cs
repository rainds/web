using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FrameWork.Core.Data;

namespace FrameWork.CoreTest.Data
{
    public class OrderEntity : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; set; }

        public string CreateUser { get; set; }

        public DateTime CreateTime { get; set; }
    }
}