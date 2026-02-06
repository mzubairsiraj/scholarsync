using System;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Collections.Generic;
using ScholarSync.Models;

namespace ScholarSync.Services
{
    /// <summary>
    /// Service for generating PDF transcripts using System.Drawing.Printing
    /// </summary>
    public class TranscriptPdfService
    {
        private TranscriptModel currentTranscript;
        private string reportType;
        private int currentPage;
        private int currentSemesterIndex;
        private int currentSubjectIndex;
        
        // Page settings
        private readonly int pageWidth = 850;
        private readonly int pageHeight = 1100;
        private readonly int margin = 50;
        private int yPosition;

        // Fonts
        private readonly Font titleFont = new Font("Arial", 18, FontStyle.Bold);
        private readonly Font headerFont = new Font("Arial", 14, FontStyle.Bold);
        private readonly Font subHeaderFont = new Font("Arial", 12, FontStyle.Bold);
        private readonly Font normalFont = new Font("Arial", 10, FontStyle.Regular);
        private readonly Font smallFont = new Font("Arial", 8, FontStyle.Regular);

        // Colors
        private readonly Color headerColor = Color.FromArgb(13, 36, 64); // Dark Navy
        private readonly Color alternateRowColor = Color.FromArgb(245, 245, 245); // Light Gray

        /// <summary>
        /// Generates a full transcript PDF for all semesters
        /// </summary>
        public bool GenerateFullTranscript(TranscriptModel transcript, string filePath)
        {
            try
            {
                currentTranscript = transcript;
                reportType = "Full";
                currentPage = 1;
                currentSemesterIndex = 0;
                currentSubjectIndex = 0;

                return GeneratePdf(filePath);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating full transcript: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Generates a semester-specific transcript PDF
        /// </summary>
        public bool GenerateSemesterTranscript(TranscriptModel transcript, string filePath)
        {
            try
            {
                currentTranscript = transcript;
                reportType = "Semester";
                currentPage = 1;
                currentSemesterIndex = 0;
                currentSubjectIndex = 0;

                return GeneratePdf(filePath);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating semester transcript: {ex.Message}", ex);
            }
        }

        private bool GeneratePdf(string filePath)
        {
            // Create a PrintDocument
            PrintDocument printDoc = new PrintDocument();
            printDoc.PrintPage += new PrintPageEventHandler(PrintPage);

            // Set up the print settings to save as PDF
            // This will use the Microsoft Print to PDF printer if available
            PrinterSettings printerSettings = new PrinterSettings();
            
            // Try to find PDF printer
            foreach (string printer in PrinterSettings.InstalledPrinters)
            {
                if (printer.ToLower().Contains("pdf") || printer.ToLower().Contains("xps"))
                {
                    printerSettings.PrinterName = printer;
                    break;
                }
            }

            printDoc.PrinterSettings = printerSettings;
            
            // Set the file name
            printDoc.PrinterSettings.PrintToFile = true;
            printDoc.PrinterSettings.PrintFileName = filePath;

            // Print the document
            printDoc.Print();

            return true;
        }

        private void PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            yPosition = margin;

            // Draw header on first page
            if (currentPage == 1 && currentSemesterIndex == 0 && currentSubjectIndex == 0)
            {
                DrawHeader(g);
            }

            // Draw semester data
            bool hasMoreData = DrawSemesterData(g, e);

            // Draw footer
            DrawFooter(g, e);

            // Check if we need more pages
            e.HasMorePages = hasMoreData;
            
            if (hasMoreData)
            {
                currentPage++;
            }
        }

        private void DrawHeader(Graphics g)
        {
            // University name
            string universityName = "Government College of Management Sciences";
            SizeF universitySize = g.MeasureString(universityName, titleFont);
            g.DrawString(universityName, titleFont, Brushes.Black, 
                (pageWidth - universitySize.Width) / 2, yPosition);
            yPosition += 30;

            // Location
            string location = "Dera Ismail Khan, Pakistan";
            SizeF locationSize = g.MeasureString(location, normalFont);
            g.DrawString(location, normalFont, Brushes.Black, 
                (pageWidth - locationSize.Width) / 2, yPosition);
            yPosition += 25;

            // Transcript title
            string transcriptTitle = reportType == "Full" ? "COMPLETE ACADEMIC TRANSCRIPT" : "SEMESTER TRANSCRIPT";
            SizeF titleSize = g.MeasureString(transcriptTitle, headerFont);
            g.DrawString(transcriptTitle, headerFont, new SolidBrush(headerColor), 
                (pageWidth - titleSize.Width) / 2, yPosition);
            yPosition += 30;

            // Horizontal line
            g.DrawLine(new Pen(headerColor, 2), margin, yPosition, pageWidth - margin, yPosition);
            yPosition += 20;

            // Student information box
            DrawStudentInfo(g);
            yPosition += 20;
        }

        private void DrawStudentInfo(Graphics g)
        {
            int boxHeight = 120;
            Rectangle infoBox = new Rectangle(margin, yPosition, pageWidth - 2 * margin, boxHeight);
            
            // Draw border
            g.DrawRectangle(new Pen(headerColor, 1), infoBox);
            
            // Fill background
            g.FillRectangle(new SolidBrush(Color.FromArgb(240, 240, 240)), infoBox);

            int leftCol = margin + 15;
            int rightCol = margin + 400;
            int rowHeight = 25;
            int currentY = yPosition + 15;

            // Student Name
            g.DrawString("Student Name:", subHeaderFont, Brushes.Black, leftCol, currentY);
            g.DrawString(currentTranscript.StudentName, normalFont, Brushes.Black, leftCol + 120, currentY);
            currentY += rowHeight;

            // Roll Number
            g.DrawString("Roll Number:", subHeaderFont, Brushes.Black, leftCol, currentY);
            g.DrawString(currentTranscript.RollNumber, normalFont, Brushes.Black, leftCol + 120, currentY);

            // CNIC
            g.DrawString("CNIC:", subHeaderFont, Brushes.Black, rightCol, currentY - rowHeight);
            g.DrawString(currentTranscript.CNIC, normalFont, Brushes.Black, rightCol + 80, currentY - rowHeight);
            currentY += rowHeight;

            // Program
            g.DrawString("Program:", subHeaderFont, Brushes.Black, leftCol, currentY);
            g.DrawString(currentTranscript.Program, normalFont, Brushes.Black, leftCol + 120, currentY);

            // Department
            g.DrawString("Department:", subHeaderFont, Brushes.Black, rightCol, currentY);
            g.DrawString(currentTranscript.Department, normalFont, Brushes.Black, rightCol + 100, currentY);

            yPosition += boxHeight + 10;
        }

        private bool DrawSemesterData(Graphics g, PrintPageEventArgs e)
        {
            int availableHeight = e.MarginBounds.Height - yPosition - 100; // Reserve space for footer

            while (currentSemesterIndex < currentTranscript.Semesters.Count)
            {
                SemesterTranscriptData semester = currentTranscript.Semesters[currentSemesterIndex];

                // Check if we have space for semester header
                if (availableHeight < 150)
                {
                    return true; // Need new page
                }

                // Draw semester header (only once per semester)
                if (currentSubjectIndex == 0)
                {
                    DrawSemesterHeader(g, semester);
                    availableHeight -= 80;
                }

                // Draw subjects table
                bool needsMorePages = DrawSubjectsTable(g, semester, ref availableHeight);
                
                if (needsMorePages)
                {
                    return true; // Need new page for more subjects
                }

                // Draw semester summary
                if (availableHeight >= 100)
                {
                    DrawSemesterSummary(g, semester);
                    availableHeight -= 60;
                    yPosition += 30; // Space before next semester
                }
                else
                {
                    return true; // Need new page for summary
                }

                // Move to next semester
                currentSemesterIndex++;
                currentSubjectIndex = 0;
            }

            // Draw overall summary for full transcript
            if (reportType == "Full" && availableHeight >= 100)
            {
                DrawOverallSummary(g);
            }

            return false; // No more pages needed
        }

        private void DrawSemesterHeader(Graphics g, SemesterTranscriptData semester)
        {
            // Semester title bar
            Rectangle headerRect = new Rectangle(margin, yPosition, pageWidth - 2 * margin, 35);
            g.FillRectangle(new SolidBrush(headerColor), headerRect);
            
            string semesterTitle = semester.SemesterName;
            if (semester.StartDate.HasValue && semester.EndDate.HasValue)
            {
                semesterTitle += $" ({semester.StartDate.Value:MMM yyyy} - {semester.EndDate.Value:MMM yyyy})";
            }
            
            g.DrawString(semesterTitle, subHeaderFont, Brushes.White, margin + 10, yPosition + 8);
            yPosition += 40;
        }

        private bool DrawSubjectsTable(Graphics g, SemesterTranscriptData semester, ref int availableHeight)
        {
            // Table header
            if (currentSubjectIndex == 0)
            {
                DrawTableHeader(g);
                availableHeight -= 30;
            }

            // Draw subjects
            int rowHeight = 25;
            int startIndex = currentSubjectIndex;

            for (int i = startIndex; i < semester.Subjects.Count; i++)
            {
                if (availableHeight < rowHeight + 50) // Reserve space
                {
                    currentSubjectIndex = i;
                    return true; // Need new page
                }

                SubjectResultData subject = semester.Subjects[i];
                DrawTableRow(g, subject, i % 2 == 0);
                availableHeight -= rowHeight;
                currentSubjectIndex++;
            }

            currentSubjectIndex = 0; // Reset for next semester
            return false;
        }

        private void DrawTableHeader(Graphics g)
        {
            int x = margin;
            int headerHeight = 30;
            
            // Header background
            Rectangle headerRect = new Rectangle(x, yPosition, pageWidth - 2 * margin, headerHeight);
            g.FillRectangle(new SolidBrush(Color.FromArgb(230, 230, 230)), headerRect);
            g.DrawRectangle(Pens.Black, headerRect);

            // Column widths
            int codeWidth = 80;
            int nameWidth = 250;
            int creditWidth = 60;
            int marksWidth = 100;
            int percentWidth = 80;
            int gradeWidth = 60;
            int gpaWidth = 60;

            // Draw headers
            DrawCenteredText(g, "Code", subHeaderFont, Brushes.Black, x, yPosition, codeWidth, headerHeight);
            x += codeWidth;
            
            DrawCenteredText(g, "Subject Name", subHeaderFont, Brushes.Black, x, yPosition, nameWidth, headerHeight);
            x += nameWidth;
            
            DrawCenteredText(g, "Credit", subHeaderFont, Brushes.Black, x, yPosition, creditWidth, headerHeight);
            x += creditWidth;
            
            DrawCenteredText(g, "Marks", subHeaderFont, Brushes.Black, x, yPosition, marksWidth, headerHeight);
            x += marksWidth;
            
            DrawCenteredText(g, "%age", subHeaderFont, Brushes.Black, x, yPosition, percentWidth, headerHeight);
            x += percentWidth;
            
            DrawCenteredText(g, "Grade", subHeaderFont, Brushes.Black, x, yPosition, gradeWidth, headerHeight);
            x += gradeWidth;
            
            DrawCenteredText(g, "GPA", subHeaderFont, Brushes.Black, x, yPosition, gpaWidth, headerHeight);

            yPosition += headerHeight;
        }

        private void DrawTableRow(Graphics g, SubjectResultData subject, bool alternate)
        {
            int x = margin;
            int rowHeight = 25;
            
            // Row background (zebra striping)
            if (alternate)
            {
                Rectangle rowRect = new Rectangle(x, yPosition, pageWidth - 2 * margin, rowHeight);
                g.FillRectangle(new SolidBrush(alternateRowColor), rowRect);
            }

            // Column widths (same as header)
            int codeWidth = 80;
            int nameWidth = 250;
            int creditWidth = 60;
            int marksWidth = 100;
            int percentWidth = 80;
            int gradeWidth = 60;
            int gpaWidth = 60;

            // Draw cell borders and data
            DrawTableCell(g, subject.SubjectCode, x, yPosition, codeWidth, rowHeight, true);
            x += codeWidth;
            
            DrawTableCell(g, subject.SubjectName, x, yPosition, nameWidth, rowHeight, false);
            x += nameWidth;
            
            DrawTableCell(g, subject.CreditHours.ToString(), x, yPosition, creditWidth, rowHeight, true);
            x += creditWidth;
            
            DrawTableCell(g, $"{subject.ObtainedMarks:F0}/{subject.TotalMarks}", x, yPosition, marksWidth, rowHeight, true);
            x += marksWidth;
            
            DrawTableCell(g, $"{subject.Percentage:F2}%", x, yPosition, percentWidth, rowHeight, true);
            x += percentWidth;
            
            DrawTableCell(g, subject.GradeLetter, x, yPosition, gradeWidth, rowHeight, true);
            x += gradeWidth;
            
            DrawTableCell(g, $"{subject.GPA:F2}", x, yPosition, gpaWidth, rowHeight, true);

            yPosition += rowHeight;
        }

        private void DrawTableCell(Graphics g, string text, int x, int y, int width, int height, bool centered)
        {
            Rectangle cellRect = new Rectangle(x, y, width, height);
            g.DrawRectangle(Pens.Black, cellRect);

            if (centered)
            {
                DrawCenteredText(g, text, normalFont, Brushes.Black, x, y, width, height);
            }
            else
            {
                g.DrawString(text, normalFont, Brushes.Black, x + 5, y + 5);
            }
        }

        private void DrawCenteredText(Graphics g, string text, Font font, Brush brush, int x, int y, int width, int height)
        {
            SizeF textSize = g.MeasureString(text, font);
            float textX = x + (width - textSize.Width) / 2;
            float textY = y + (height - textSize.Height) / 2;
            g.DrawString(text, font, brush, textX, textY);
        }

        private void DrawSemesterSummary(Graphics g, SemesterTranscriptData semester)
        {
            yPosition += 10;
            
            int summaryX = pageWidth - margin - 300;
            Rectangle summaryBox = new Rectangle(summaryX, yPosition, 300, 45);
            
            g.FillRectangle(new SolidBrush(Color.FromArgb(240, 240, 240)), summaryBox);
            g.DrawRectangle(new Pen(headerColor, 1), summaryBox);

            int textY = yPosition + 8;
            g.DrawString($"Semester Credit Hours: {semester.SemesterCreditHours}", subHeaderFont, Brushes.Black, summaryX + 10, textY);
            textY += 22;
            g.DrawString($"Semester GPA: {semester.SemesterGPA:F2} / 4.0", subHeaderFont, new SolidBrush(headerColor), summaryX + 10, textY);

            yPosition += 55;
        }

        private void DrawOverallSummary(Graphics g)
        {
            yPosition += 20;
            
            // Draw a prominent summary box
            Rectangle summaryBox = new Rectangle(margin, yPosition, pageWidth - 2 * margin, 60);
            g.FillRectangle(new SolidBrush(headerColor), summaryBox);
            g.DrawRectangle(new Pen(Color.Black, 2), summaryBox);

            int textY = yPosition + 10;
            int centerX = pageWidth / 2;

            // Overall CGPA
            string cgpaText = $"OVERALL CGPA: {currentTranscript.OverallCGPA:F2} / 4.0";
            SizeF cgpaSize = g.MeasureString(cgpaText, headerFont);
            g.DrawString(cgpaText, headerFont, Brushes.White, centerX - cgpaSize.Width / 2, textY);
            
            textY += 30;
            
            // Total Credit Hours
            string creditText = $"Total Credit Hours: {currentTranscript.TotalCreditHours}";
            SizeF creditSize = g.MeasureString(creditText, subHeaderFont);
            g.DrawString(creditText, subHeaderFont, Brushes.White, centerX - creditSize.Width / 2, textY);

            yPosition += 70;
        }

        private void DrawFooter(Graphics g, PrintPageEventArgs e)
        {
            int footerY = e.MarginBounds.Bottom + 20;
            
            // Generated date
            string dateText = $"Generated on: {DateTime.Now:MMMM dd, yyyy}";
            g.DrawString(dateText, smallFont, Brushes.Black, margin, footerY);

            // Page number
            string pageText = $"Page {currentPage}";
            SizeF pageSize = g.MeasureString(pageText, smallFont);
            g.DrawString(pageText, smallFont, Brushes.Black, pageWidth - margin - pageSize.Width, footerY);

            footerY += 20;

            // Signature line
            int signatureY = footerY + 40;
            g.DrawLine(Pens.Black, pageWidth - margin - 200, signatureY, pageWidth - margin, signatureY);
            
            string signatureText = "Controller of Examinations";
            g.DrawString(signatureText, smallFont, Brushes.Black, pageWidth - margin - 200, signatureY + 5);
        }
    }
}
