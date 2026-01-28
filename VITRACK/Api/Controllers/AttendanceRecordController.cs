using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VITRACK.Api.DTOs.AttendanceRecords;
using VITRACK.Api.DTOs.Auth;
using VITRACK.Api.Errors;
using VITRACK.Application.Interfaces;
using VITRACK.Common.Helpers;
using VITRACK.Common.RequestFeatures;
using VITRACK.Common.Services;
using VITRACK.Infrastructure.Entities;

namespace VITRACK.Api.Controllers;

[Route("api/attendance-record")]
[ApiController]
[Authorize]
public class AttendanceRecordController : ControllerBase
{
    private readonly IAttendanceRecordRepository _repository;
    private readonly IImageService _imageService;
    private readonly IWorkScheduleRepository _workScheduleRepository;

    public AttendanceRecordController(
        IAttendanceRecordRepository repository,
        IImageService imageService,
        IWorkScheduleRepository workScheduleRepository)
    {
        _repository = repository;
        _imageService = imageService;
        _workScheduleRepository = workScheduleRepository;
    }

    [HttpGet("get-by-id/{id}")]
    [Authorize]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var record = await _repository.GetByIdAsync(id);
        if (record is null)
        {
            return NotFound(new ResponseErrors
            {
                ErrorCodeSetter = ErrorCodeEnum.ATTENDANCE_RECORD_NOT_FOUND,
                Message = ErrorCodes.ATTENDANCE_RECORD_NOT_FOUND
            });
        }
        return Ok(record);
    }

    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll([FromQuery] AttendanceParametrs attendanceParametrs)
    {
        var records = await _repository.GetAllAsync(attendanceParametrs);
        return Ok(new
        {
            items = records,
            metaData = records.MetaData
        });
    }

    [HttpGet("get-my-records")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> GetMyRecords([FromQuery] AttendanceParametrs attendanceParametrs)
    {
        UserInfo? userInfo =
             JwtService.GetCurrentUserInfo(HttpContext.User.Identity as ClaimsIdentity);

        if (userInfo?.Id is null) return StatusCode(500);

        var records = await _repository.GetByEmployeeIdAsync(userInfo.Id, attendanceParametrs);
        return Ok(new
        {
            items = records,
            metaData = records.MetaData
        });
    }

    [HttpGet("get-current-status")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> GetCurrentStatus()
    {
        UserInfo? userInfo =
             JwtService.GetCurrentUserInfo(HttpContext.User.Identity as ClaimsIdentity);

        if (userInfo?.Id is null) return StatusCode(500);

        var existRecord = await _repository.GetByDateAsync(userInfo.Id, TimeHelper.GetBakuDate());
        CurrentAttendance currentAttendance = new();

        if (existRecord is not null)
        {
            currentAttendance.Date = existRecord.Date;
            currentAttendance.ArrivalTime = existRecord.ArrivalTime;
            currentAttendance.LeaveTime = existRecord.LeaveTime;
            currentAttendance.IsLate = existRecord.IsLate;
            currentAttendance.IsEarlyLeave = existRecord.IsEarlyLeave;
        }

        return Ok(currentAttendance);
    }

    [HttpPost("check-in")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> CheckIn([FromForm] CheckInRequest request)
    {
        UserInfo? userInfo =
             JwtService.GetCurrentUserInfo(HttpContext.User.Identity as ClaimsIdentity);

        if (userInfo?.Id is null) return StatusCode(500);
        string? imgEndPath = null;

        var existRecord = await _repository.GetByDateAsync(userInfo.Id, TimeHelper.GetBakuDate());

        if (existRecord is not null)
            return BadRequest(new ResponseErrors
            {
                ErrorCodeSetter = ErrorCodeEnum.ATTENDANCE_RECORD_ALREADY_EXISTS,
                Message = ErrorCodes.ATTENDANCE_RECORD_ALREADY_EXISTS
            });

        // Handling arrival image upload
        if (request.ArrivalImg is not null)
        {
            FileParamsValidator fileValidator = new()
            {
                AllowImage = true,
                MaxFileSize = 15,
                AllowNullable = true
            };

            if (!fileValidator.IsValidFile(request.ArrivalImg))
            {
                return BadRequest(new ResponseErrors
                {
                    ErrorCodeSetter = ErrorCodeEnum.INPUT_ERROR,
                    Message = fileValidator.ErrorMessage
                });
            }

            try
            {
                imgEndPath = await _imageService.SaveImageAsync(
                    request.ArrivalImg, "attendance");

                if (imgEndPath is null)
                {
                    return BadRequest(new ResponseErrors
                    {
                        ErrorCodeSetter = ErrorCodeEnum.UNXEPECTED_ERROR,
                        Message = "Şəkil yadda saxlanarkən xəta baş verdi."
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseErrors
                {
                    ErrorCodeSetter = ErrorCodeEnum.INTERNAL_SERVER_ERROR,
                    Message = $"Server xetasi: {ex.Message}"
                });
            }
        }

        // Adjusting arrival time with 2 minutes tolerance
        TimeOnly setTime = TimeHelper.GetBakuTimeOnly();
        if (request.ArrivalTime is not null && setTime > request.ArrivalTime)
        {
            TimeSpan timeDiff = setTime - request.ArrivalTime.Value;
            if (timeDiff.Duration().TotalMinutes <= 2)
            {
                setTime = request.ArrivalTime.Value;
            }
        }

        // Checking if employee is late according to work schedule
        WorkSchedule? employeeWorkSchedule = await _workScheduleRepository.GetByUserAsync(userInfo.Id);
        TimeSpan allowedLateTime = new(0, 10, 0); // Default 10 minutes
        bool isLateStatus = false;
        if (employeeWorkSchedule is not null)
        {
            TimeOnly allowedArrivalTime = employeeWorkSchedule.StartTime.Add(allowedLateTime);

            if (setTime > allowedArrivalTime)
            {
                isLateStatus = true;
            }
        }

        string? rawLat = request.ArrivalLatitude?.Replace(',', '.');
        string? rawLng = request.ArrivalLongitude?.Replace(',', '.');

        double finalLat = 0;
        double finalLng = 0;
        bool latParsResult = false;
        bool lngParsResult = false;

        if (!string.IsNullOrEmpty(rawLat))
        {
            latParsResult = double.TryParse(rawLat, CultureInfo.InvariantCulture, out finalLat);
        }

        if (!string.IsNullOrEmpty(rawLng))
        {
            lngParsResult = double.TryParse(rawLng, CultureInfo.InvariantCulture, out finalLng);
        }

        AttendanceRecord newRecord = new()
        {
            EmployeeId = userInfo.Id,
            Date = TimeHelper.GetBakuDate(),
            ArrivalTime = setTime,
            ArrivalImgUrl = imgEndPath,
            ArrivalLongitude = latParsResult ? finalLat : null,
            ArrivalLatitude = lngParsResult ? finalLng : null,
            LateReason = request.LateReason,
            IsLate = isLateStatus,
            CreatedAt = TimeHelper.GetBakuTime()
        };

        var createdRecord = await _repository.CreateAsync(newRecord);
        return Ok(createdRecord);
    }

    [HttpPost("check-out")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> CheckOut([FromForm] CheckOutRequest request)
    {
        UserInfo? userInfo =
             JwtService.GetCurrentUserInfo(HttpContext.User.Identity as ClaimsIdentity);

        if (userInfo?.Id is null) return StatusCode(500);
        string? imgEndPath = null;

        var existRecord = await _repository.GetByDateAsync(userInfo.Id, TimeHelper.GetBakuDate());

        if (existRecord is null)
            return BadRequest(new ResponseErrors
            {
                ErrorCodeSetter = ErrorCodeEnum.ATTENDANCE_RECORD_NOT_FOUND,
                Message = ErrorCodes.ATTENDANCE_RECORD_NOT_FOUND
            });
        if (existRecord.UpdatedAt is not null)
        {
            return BadRequest(new ResponseErrors
            {
                ErrorCodeSetter = ErrorCodeEnum.ATTENDANCE_RECORD_ALREADY_EXISTS,
                Message = ErrorCodes.ATTENDANCE_RECORD_ALREADY_EXISTS
            });
        }

        // Handling leave image upload
        if (request.LeaveImg is not null)
        {
            FileParamsValidator fileValidator = new()
            {
                AllowImage = true,
                MaxFileSize = 15,
                AllowNullable = true
            };

            if (!fileValidator.IsValidFile(request.LeaveImg))
            {
                return BadRequest(new ResponseErrors
                {
                    ErrorCodeSetter = ErrorCodeEnum.INPUT_ERROR,
                    Message = fileValidator.ErrorMessage
                });
            }

            try
            {
                imgEndPath = await _imageService.SaveImageAsync(
                    request.LeaveImg, "attendance");

                if (imgEndPath is null)
                {
                    return BadRequest(new ResponseErrors
                    {
                        ErrorCodeSetter = ErrorCodeEnum.UNXEPECTED_ERROR,
                        Message = "Şəkil yadda saxlanarkən xəta baş verdi."
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseErrors
                {
                    ErrorCodeSetter = ErrorCodeEnum.INTERNAL_SERVER_ERROR,
                    Message = $"Server xetasi: {ex.Message}"
                });
            }
        }

        TimeOnly setTime = TimeHelper.GetBakuTimeOnly();

        // Checking if employee is early leave according to work schedule
        WorkSchedule? employeeWorkSchedule = await _workScheduleRepository.GetByUserAsync(userInfo.Id);
        TimeSpan earlyLeaveTime = new(0, 5, 0); // Default 5 minutes
        bool isEarlyLeaveStatus = false;
        if (employeeWorkSchedule is not null)
        {
            TimeOnly allowedLeaveTime = employeeWorkSchedule.EndTime;
            setTime = setTime.Add(earlyLeaveTime);
            if (setTime < allowedLeaveTime)
            {
                isEarlyLeaveStatus = true;
            }
        }

        string? rawLat = request.LeaveLatitude?.Replace(',', '.');
        string? rawLng = request.LeaveLongitude?.Replace(',', '.');

        double finalLat = 0;
        double finalLng = 0;
        bool latParsResult = false;
        bool lngParsResult = false;

        if (!string.IsNullOrEmpty(rawLat))
        {
            latParsResult = double.TryParse(rawLat, CultureInfo.InvariantCulture, out finalLat);
        }

        if (!string.IsNullOrEmpty(rawLng))
        {
            lngParsResult = double.TryParse(rawLng, CultureInfo.InvariantCulture, out finalLng);
        }

        existRecord.LeaveTime = setTime;
        existRecord.LeaveImgUrl = imgEndPath;
        existRecord.LeaveLongitude = lngParsResult ? finalLng : null;
        existRecord.LeaveLatitude = latParsResult ? finalLat : null;
        existRecord.EarlyLeaveReason = request.EarlyLeaveReason;
        existRecord.IsEarlyLeave = isEarlyLeaveStatus;
        existRecord.UpdatedAt = TimeHelper.GetBakuTime();

        await _repository.UpdateAsync(existRecord);
        return Ok(existRecord);
    }


}
