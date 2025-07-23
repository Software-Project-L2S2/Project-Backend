using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


namespace HRWorkForceSystemBackend.Models.WorkforceModels
{
    public class Summary
    {
        public int TotalPromotions { get; set; }
        public int TotalExits { get; set; }
        public int TotalTransfers { get; set; }
        public int TotalAttritions { get; set; }

         public int TotalEmployees { get; set; } 
    }
}