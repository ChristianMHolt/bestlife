using System;

namespace BitLifeClone
{
    public class Job
    {
        public string Title { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public string RequiredEducation { get; set; } = string.Empty;

        public Job(string title, decimal salary, string requiredEducation)
        {
            Title = title;
            Salary = salary;
            RequiredEducation = requiredEducation;
        }
    }
}
