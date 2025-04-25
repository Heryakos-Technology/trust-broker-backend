using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using System.Collections.Generic;
using broker.Models;

namespace broker.Dto
{
    public class ReviewDto
    {

[Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReviewId { get; set; }
        public int rate { get; set; }


        public int BrokerId { get; set; } 
       
        public Broker Broker { get; set; }
        public int CustomerId { get; set; }
        
        public Customer Customer { get; set; }











    }
}