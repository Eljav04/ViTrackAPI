using System.Security.Claims;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VITRACK.Api.DTOs.AttendanceRecords;
using VITRACK.Api.DTOs.Auth;
using VITRACK.Api.Errors;
using VITRACK.Application.Interfaces;
using VITRACK.Common.Helpers;
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
            if (setTime - employeeWorkSchedule.StartTime > allowedLateTime)
            {
                isLateStatus = true;
            }
        }

        AttendanceRecord newRecord = new()
        {
            EmployeeId = userInfo.Id,
            Date = TimeHelper.GetBakuDate(),
            ArrivalTime = setTime,
            ArrivalImgUrl = imgEndPath,
            ArrivalLongitude = request.ArrivalLongitude,
            ArrivalLatitude = request.ArrivalLatitude,
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
        TimeSpan allowedLeaveTime = new(0, 5, 0); // Default 5 minutes
        bool isEarlyLeaveStatus = false;
        if (employeeWorkSchedule is not null)
        {
            if (employeeWorkSchedule.EndTime - setTime > allowedLeaveTime)
            {
                isEarlyLeaveStatus = true;
            }
        }
        existRecord.LeaveTime = setTime;
        existRecord.LeaveImgUrl = imgEndPath;
        existRecord.LeaveLongitude = request.LeaveLongitude;
        existRecord.LeaveLatitude = request.LeaveLatitude;
        existRecord.EarlyLeaveReason = request.EarlyLeaveReason;
        existRecord.IsEarlyLeave = isEarlyLeaveStatus;
        existRecord.UpdatedAt = TimeHelper.GetBakuTime();

        await _repository.UpdateAsync(existRecord);
        return Ok(existRecord);
    }


}
