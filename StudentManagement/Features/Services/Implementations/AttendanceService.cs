using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StudentManagement.Features.Data.Models;
using StudentManagement.Features.Data.Enums;
using StudentManagement.Features.Repositories.Interfaces;
using StudentManagement.Features.Services.Interfaces;

namespace StudentManagement.Features.Services.Implementations
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IAttendanceSessionService _attendanceSessionService;

        public AttendanceService(IAttendanceRepository attendanceRepository, IAttendanceSessionService attendanceSessionService)
        {
            _attendanceRepository = attendanceRepository;
            _attendanceSessionService = attendanceSessionService;
        }

        public async Task<IEnumerable<Attendance>> GetAllAttendancesAsync()
        {
            return await _attendanceRepository.GetAllAsync();
        }

        public async Task<Attendance?> GetAttendanceByIdAsync(int id)
        {
            return await _attendanceRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Attendance>> GetStudentAttendancesAsync(int studentId)
        {
            return await _attendanceRepository.GetByStudentIdAsync(studentId);
        }

        public async Task<IEnumerable<Attendance>> GetClassSectionAttendancesAsync(int classSectionId)
        {
            return await _attendanceRepository.GetByClassSectionIdAsync(classSectionId);
        }

        public async Task<IEnumerable<Attendance>> GetClassAttendancesByDateRangeAsync(int classSectionId, DateTime startDate, DateTime endDate)
        {
            return await _attendanceRepository.GetByDateRangeAsync(classSectionId, startDate, endDate);
        }

        public async Task<Attendance> RecordAttendanceAsync(int studentId, int classSectionId, DateTime date, AttendanceStatus status, TimeSpan? timeIn = null, TimeSpan? timeOut = null, string? remarks = null)
        {
            var existing = await _attendanceRepository.GetByStudentAndDateAsync(studentId, classSectionId, date);
            
            if (existing != null)
            {
                existing.Status = status;
                existing.TimeIn = timeIn ?? existing.TimeIn;
                existing.TimeOut = timeOut ?? existing.TimeOut;
                existing.Remarks = remarks ?? existing.Remarks;
                
                await _attendanceRepository.UpdateAsync(existing);
                return existing;
            }

            var attendance = new Attendance
            {
                StudentId = studentId,
                ClassSectionId = classSectionId,
                Date = date.Date,
                Status = status,
                TimeIn = timeIn,
                TimeOut = timeOut,
                Remarks = remarks,
                CreatedAt = DateTime.UtcNow
            };

            return await _attendanceRepository.CreateAsync(attendance);
        }

        public async Task UpdateAttendanceAsync(Attendance attendance)
        {
            await _attendanceRepository.UpdateAsync(attendance);
        }

        public async Task DeleteAttendanceAsync(int id)
        {
            await _attendanceRepository.DeleteAsync(id);
        }

        public async Task<Attendance> SubmitAttendanceCodeAsync(int studentId, string code)
        {
            var session = await _attendanceSessionService.ValidateAndGetSessionAsync(code);
            if (session == null)
            {
                throw new Exception("Invalid or expired attendance code.");
            }

            var classSection = session.ClassSection;
            if (classSection == null)
            {
                throw new Exception("Class section not found.");
            }

            var timeSubmitted = DateTime.UtcNow;
            var localTimeSubmitted = timeSubmitted.ToLocalTime().TimeOfDay;

            AttendanceStatus status = AttendanceStatus.Present;

            if (classSection.StartTime.HasValue)
            {
                var gracePeriod = classSection.StartTime.Value.Add(TimeSpan.FromMinutes(15));
                if (localTimeSubmitted > classSection.EndTime)
                {
                    status = AttendanceStatus.Absent;
                }
                else if (localTimeSubmitted > gracePeriod)
                {
                    status = AttendanceStatus.Late;
                }
            }

            // Check if already submitted
            var existing = await _attendanceRepository.GetByStudentAndDateAsync(studentId, classSection.Id, session.Date);
            if (existing != null)
            {
                throw new Exception("Attendance already submitted for this date.");
            }

            var attendance = new Attendance
            {
                StudentId = studentId,
                ClassSectionId = classSection.Id,
                AttendanceSessionId = session.Id,
                Date = session.Date,
                Status = status,
                TimeIn = localTimeSubmitted,
                CodeUsed = code,
                TimeSubmitted = timeSubmitted,
                CreatedAt = DateTime.UtcNow
            };

            return await _attendanceRepository.CreateAsync(attendance);
        }
    }
}
