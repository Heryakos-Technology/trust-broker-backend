

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace broker.Models
{
    public class Sales
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SalesId { get; set; }

       
     
        public int Quantity { get; set; }
        public string Color { get; set; }
        public int ProductId { get; set; }
        public int BrokerId { get; set; }
        // navigational propery
        [JsonIgnore]
         public Broker Broker { get; set; }
        public int CustomerId { get; set; }

        [JsonIgnore]    
        public Customer Customer { get; set; }

     




    }

}