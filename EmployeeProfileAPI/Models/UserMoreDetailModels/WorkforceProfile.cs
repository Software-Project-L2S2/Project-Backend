using EmployeeProfileAPI.Models.AuthModels;


namespace EmployeeProfileAPI.Models.UserMoreDetailModels
{
    public class WorkforceProfile
    {
        public int Id { get; set; }


        public string WorkforceId { get; set; }

        public string Name { get; set; }

        public string Gender { get; set; }

        public int Age {  get; set; }  
        public string Department { get; set; }

        public DateTime StartDate { get; set; }
        public string JobTitle { get; set; }

        public string JobCategory {  get; set; }

        public int WorkforceUserId { get; set; }  // FK
        public WorkforceUser WorkforceUser { get; set; }

    }
}
