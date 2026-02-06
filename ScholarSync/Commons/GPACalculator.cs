using System;

namespace ScholarSync.Commons
{
   
    public static class GPACalculator
    {
        
        public static decimal CalculateGPA(decimal percentage)
        {
           
            
            if (percentage >= 80)
            {
                return 4.00m;
            }
            else if (percentage >= 75 && percentage < 80)
            {
                
                decimal gpa = 3.50m + ((percentage - 75) / 5m) * 0.49m;
                return Math.Round(gpa, 2);
            }
            else if (percentage >= 70 && percentage < 75)
            {
                
                decimal gpa = 3.00m + ((percentage - 70) / 5m) * 0.49m;
                return Math.Round(gpa, 2);
            }
            else if (percentage >= 65 && percentage < 70)
            {
                
                decimal gpa = 2.50m + ((percentage - 65) / 5m) * 0.49m;
                return Math.Round(gpa, 2);
            }
            else if (percentage >= 60 && percentage < 65)
            {
                
                decimal gpa = 2.00m + ((percentage - 60) / 5m) * 0.49m;
                return Math.Round(gpa, 2);
            }
            else if (percentage >= 55 && percentage < 60)
            {
                
                decimal gpa = 1.50m + ((percentage - 55) / 5m) * 0.49m;
                return Math.Round(gpa, 2);
            }
            else if (percentage >= 50 && percentage < 55)
            {
                
                decimal gpa = 1.00m + ((percentage - 50) / 5m) * 0.49m;
                return Math.Round(gpa, 2);
            }
            else
            {
               
                return 0.00m;
            }
        }

       
        public static string CalculateGradeLetter(decimal percentage)
        {
            
            
            if (percentage >= 80)
                return "A+";
            else if (percentage >= 75)
                return "A";
            else if (percentage >= 70)
                return "B+";
            else if (percentage >= 65)
                return "B";
            else if (percentage >= 60)
                return "C";
            else if (percentage >= 55)
                return "D+";
            else if (percentage >= 50)
                return "D";
            else
                return "F";
        }

        
        //public static decimal CalculateSemesterGPA(decimal[] subjectGPAs, int[] creditHours)
        //{
        //    if (subjectGPAs == null || creditHours == null)
        //        throw new ArgumentNullException("Provide Correct Subjects GPA and Credit Hours List");
                
        //    if (subjectGPAs.Length != creditHours.Length)
        //        throw new ArgumentException("Subject GPAs and credit Hours must have same length");
                
        //    if (subjectGPAs.Length == 0)
        //        return 0.00m;

        //    decimal totalQualityPoints = 0;
        //    int totalCredits = 0;

        //    for (int i = 0; i < subjectGPAs.Length; i++)
        //    {
                
        //        totalQualityPoints += subjectGPAs[i] * creditHours[i];
        //        totalCredits += creditHours[i];
        //    }

        //    if (totalCredits == 0)
        //        return 0.00m;

            
        //    decimal semesterGPA = totalQualityPoints / totalCredits;
        //    return Math.Round(semesterGPA, 2);
        //}

        
        //public static decimal CalculateCGPA(decimal[] allSubjectGPAs, int[] allCreditHours)
        //{
            
        //    return CalculateSemesterGPA(allSubjectGPAs, allCreditHours);
        //}

        
        public static decimal PassingPercentage => 50.00m;

        
        //public static decimal PassingGPA => 1.00m;

        
        public static bool IsPassing(decimal percentage)
        {
            return percentage >= PassingPercentage;
        }

        
     
    }
}
