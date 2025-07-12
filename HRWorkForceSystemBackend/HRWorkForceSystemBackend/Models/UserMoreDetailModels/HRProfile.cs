using HRWorkForceSystemBackend.Models.AuthModels;

namespace HRWorkForceSystemBackend.Models.UserMoreDetailModels
{
    public class HRProfile
    {
        public int Id { get; set; }

        public string HRId { get; set; }  // e.g. HR001
        public string Department { get; set; }
        public string Gender { get; set; }
        public DateTime StartDate { get; set; }
        public string Contact { get; set; }
        public string Accounts { get; set; }

        // Foreign Key
        public int HRUserId { get; set; }
        public HRUser HRUser { get; set; }

    }
}
