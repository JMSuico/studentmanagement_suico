using System;

namespace StudentManagement.Features.Helpers
{
    public static class QRHelper
    {
        // This is a placeholder for QR logic.
        // In a real application, you might use a library like QRCoder or ZXing.Net.
        
        public static string GenerateQRCodeDataForStudent(string studentNumber)
        {
            // Simple structured data string for the QR code
            return $"STUDENT:{studentNumber}:{Guid.NewGuid().ToString().Substring(0, 8)}";
        }

        public static string GenerateQRCodeDataForAttendance(int classSectionId, DateTime date)
        {
            return $"ATTENDANCE:{classSectionId}:{date:yyyyMMdd}";
        }

        public static bool IsValidStudentQRCode(string qrData, out string studentNumber)
        {
            studentNumber = string.Empty;
            if (string.IsNullOrWhiteSpace(qrData)) return false;

            var parts = qrData.Split(':');
            if (parts.Length == 3 && parts[0] == "STUDENT")
            {
                studentNumber = parts[1];
                return true;
            }

            return false;
        }
    }
}
