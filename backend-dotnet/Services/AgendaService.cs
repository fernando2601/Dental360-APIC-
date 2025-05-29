using ClinicApi.Models;
using ClinicApi.Repositories;

namespace ClinicApi.Services
{
    public class AgendaService : IAgendaService
    {
        private readonly IAgendaRepository _agendaRepository;

        public AgendaService(IAgendaRepository agendaRepository)
        {
            _agendaRepository = agendaRepository;
        }

        public async Task<IEnumerable<Appointment>> GetAllAppointmentsAsync()
        {
            return await _agendaRepository.GetAllAppointmentsAsync();
        }

        public async Task<Appointment?> GetAppointmentByIdAsync(int id)
        {
            return await _agendaRepository.GetAppointmentByIdAsync(id);
        }

        public async Task<Appointment> CreateAppointmentAsync(CreateAppointmentDto appointmentDto, int createdBy)
        {
            // Validações de negócio
            await ValidateAppointmentAsync(appointmentDto);
            
            // Verificar conflitos
            var isAvailable = await _agendaRepository.IsTimeSlotAvailableAsync(
                appointmentDto.StartTime, appointmentDto.EndTime, appointmentDto.StaffId, appointmentDto.Room);
            
            if (!isAvailable)
            {
                throw new InvalidOperationException("Horário não disponível");
            }

            return await _agendaRepository.CreateAppointmentAsync(appointmentDto, createdBy);
        }

        public async Task<Appointment?> UpdateAppointmentAsync(int id, UpdateAppointmentDto appointmentDto)
        {
            await ValidateUpdateAppointmentAsync(appointmentDto);
            
            // Verificar conflitos se horário foi alterado
            if (appointmentDto.StartTime.HasValue && appointmentDto.EndTime.HasValue)
            {
                var isAvailable = await _agendaRepository.IsTimeSlotAvailableAsync(
                    appointmentDto.StartTime.Value, appointmentDto.EndTime.Value, 
                    appointmentDto.StaffId, null, id);
                
                if (!isAvailable)
                {
                    throw new InvalidOperationException("Horário não disponível");
                }
            }

            return await _agendaRepository.UpdateAppointmentAsync(id, appointmentDto);
        }

        public async Task<bool> DeleteAppointmentAsync(int id)
        {
            return await _agendaRepository.DeleteAppointmentAsync(id);
        }

        public async Task<object> GetCalendarViewAsync(DateTime startDate, DateTime endDate)
        {
            var appointments = await _agendaRepository.GetCalendarViewAsync(startDate, endDate);
            
            return new
            {
                events = appointments.Select(a => new
                {
                    id = a.Id,
                    title = a.Title,
                    start = a.Start.ToString("yyyy-MM-ddTHH:mm:ss"),
                    end = a.End.ToString("yyyy-MM-ddTHH:mm:ss"),
                    backgroundColor = a.StatusColor,
                    borderColor = a.StatusColor,
                    textColor = "#FFFFFF",
                    extendedProps = new
                    {
                        patientName = a.PatientName,
                        serviceName = a.ServiceName,
                        staffName = a.StaffName,
                        room = a.Room,
                        status = a.Status,
                        priority = a.Priority,
                        notes = a.Notes,
                        estimatedCost = a.EstimatedCost
                    }
                }),
                period = new
                {
                    start = startDate.ToString("yyyy-MM-dd"),
                    end = endDate.ToString("yyyy-MM-dd")
                }
            };
        }

        public async Task<object> GetTodayAppointmentsAsync()
        {
            var appointments = await _agendaRepository.GetTodayAppointmentsAsync();
            
            return new
            {
                appointments = appointments.Select(a => new
                {
                    id = a.Id,
                    patientName = a.PatientName,
                    serviceName = a.ServiceName,
                    staffName = a.StaffName,
                    startTime = a.StartTime.ToString("HH:mm"),
                    endTime = a.EndTime.ToString("HH:mm"),
                    status = a.Status,
                    room = a.Room,
                    priority = a.Priority,
                    estimatedCost = a.EstimatedCost
                }).OrderBy(a => a.startTime),
                summary = new
                {
                    total = appointments.Count(),
                    pending = appointments.Count(a => a.Status == "pending"),
                    confirmed = appointments.Count(a => a.Status == "confirmed"),
                    completed = appointments.Count(a => a.Status == "completed")
                }
            };
        }

        public async Task<object> GetUpcomingAppointmentsAsync(int days = 7)
        {
            var appointments = await _agendaRepository.GetUpcomingAppointmentsAsync(days);
            
            return new
            {
                appointments = appointments.GroupBy(a => a.StartTime.Date)
                    .Select(g => new
                    {
                        date = g.Key.ToString("yyyy-MM-dd"),
                        dayName = g.Key.ToString("dddd", new System.Globalization.CultureInfo("pt-BR")),
                        appointmentCount = g.Count(),
                        appointments = g.Select(a => new
                        {
                            id = a.Id,
                            patientName = a.PatientName,
                            serviceName = a.ServiceName,
                            time = a.StartTime.ToString("HH:mm"),
                            status = a.Status
                        }).OrderBy(a => a.time)
                    }).OrderBy(g => g.date),
                summary = new
                {
                    totalDays = days,
                    totalAppointments = appointments.Count(),
                    averagePerDay = Math.Round((decimal)appointments.Count() / days, 1)
                }
            };
        }

        public async Task<bool> IsTimeSlotAvailableAsync(DateTime startTime, DateTime endTime, int? staffId = null, string? room = null, int? excludeAppointmentId = null)
        {
            return await _agendaRepository.IsTimeSlotAvailableAsync(startTime, endTime, staffId, room, excludeAppointmentId);
        }

        public async Task<object> CheckAvailabilityAsync(DateTime startTime, DateTime endTime, int? staffId = null)
        {
            var isAvailable = await _agendaRepository.IsTimeSlotAvailableAsync(startTime, endTime, staffId);
            var conflicts = await _agendaRepository.CheckConflictsAsync(startTime, endTime, staffId, null);
            
            return new
            {
                isAvailable,
                conflicts = conflicts.Select(c => new
                {
                    type = c.ConflictType,
                    description = c.ConflictDescription,
                    conflictingAppointmentId = c.ConflictingAppointmentId
                }),
                suggestion = isAvailable ? null : "Tente outro horário ou profissional"
            };
        }

        public async Task<object> GetAvailableTimeSlotsAsync(DateTime date, int durationMinutes = 60, int? staffId = null)
        {
            var slots = await _agendaRepository.GetAvailabilitySlotsAsync(date, staffId);
            
            return new
            {
                date = date.ToString("yyyy-MM-dd"),
                durationMinutes,
                staffId,
                availableSlots = slots.Where(s => s.IsAvailable).Select(s => new
                {
                    startTime = s.Start.ToString("HH:mm"),
                    endTime = s.End.ToString("HH:mm"),
                    staffName = s.StaffName,
                    room = s.Room
                }).OrderBy(s => s.startTime)
            };
        }

        public async Task<bool> ConfirmAppointmentAsync(int id, int? confirmedBy = null)
        {
            return await _agendaRepository.ConfirmAppointmentAsync(id, confirmedBy);
        }

        public async Task<bool> CancelAppointmentAsync(int id, string reason, int? cancelledBy = null)
        {
            if (string.IsNullOrWhiteSpace(reason))
            {
                throw new ArgumentException("Motivo do cancelamento é obrigatório");
            }

            return await _agendaRepository.CancelAppointmentAsync(id, reason, cancelledBy);
        }

        public async Task<bool> CompleteAppointmentAsync(int id, decimal? actualCost = null, string? notes = null)
        {
            return await _agendaRepository.CompleteAppointmentAsync(id, actualCost, notes);
        }

        public async Task<bool> MarkNoShowAsync(int id, string? notes = null)
        {
            return await _agendaRepository.MarkNoShowAsync(id, notes);
        }

        public async Task<bool> RescheduleAppointmentAsync(RescheduleRequest request, int? rescheduledBy = null)
        {
            // Verificar disponibilidade do novo horário
            var isAvailable = await _agendaRepository.IsTimeSlotAvailableAsync(
                request.NewStartTime, request.NewEndTime, request.NewStaffId, request.NewRoom, request.AppointmentId);
            
            if (!isAvailable)
            {
                throw new InvalidOperationException("Novo horário não está disponível");
            }

            return await _agendaRepository.RescheduleAppointmentAsync(request, rescheduledBy);
        }

        public async Task<object> GetAppointmentsWithFiltersAsync(AgendaFilters filters)
        {
            var appointments = await _agendaRepository.GetAppointmentsWithFiltersAsync(filters);
            var totalCount = await _agendaRepository.GetAppointmentsCountAsync(filters);
            
            return new
            {
                appointments = appointments.Select(a => new
                {
                    id = a.Id,
                    patientName = a.PatientName,
                    serviceName = a.ServiceName,
                    staffName = a.StaffName,
                    startTime = a.StartTime.ToString("yyyy-MM-dd HH:mm"),
                    endTime = a.EndTime.ToString("HH:mm"),
                    status = a.Status,
                    priority = a.Priority,
                    room = a.Room,
                    estimatedCost = a.EstimatedCost
                }),
                pagination = new
                {
                    currentPage = filters.Page,
                    totalPages = (int)Math.Ceiling((double)totalCount / filters.Limit),
                    totalItems = totalCount,
                    itemsPerPage = filters.Limit
                },
                filters = new
                {
                    startDate = filters.StartDate?.ToString("yyyy-MM-dd"),
                    endDate = filters.EndDate?.ToString("yyyy-MM-dd"),
                    patientId = filters.PatientId,
                    staffId = filters.StaffId,
                    status = filters.Status,
                    priority = filters.Priority
                }
            };
        }

        public async Task<object> GetAppointmentsByPatientAsync(int patientId)
        {
            var appointments = await _agendaRepository.GetAppointmentsByPatientAsync(patientId);
            
            return new
            {
                patientId,
                appointments = appointments.Select(a => new
                {
                    id = a.Id,
                    serviceName = a.ServiceName,
                    staffName = a.StaffName,
                    startTime = a.StartTime.ToString("yyyy-MM-dd HH:mm"),
                    status = a.Status,
                    actualCost = a.ActualCost
                }).OrderByDescending(a => a.startTime),
                summary = new
                {
                    totalAppointments = appointments.Count(),
                    lastAppointment = appointments.OrderByDescending(a => a.StartTime).FirstOrDefault()?.StartTime.ToString("yyyy-MM-dd"),
                    totalSpent = appointments.Where(a => a.ActualCost.HasValue).Sum(a => a.ActualCost.Value)
                }
            };
        }

        public async Task<object> GetAppointmentsByStaffAsync(int staffId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var appointments = await _agendaRepository.GetAppointmentsByStaffAsync(staffId, startDate, endDate);
            
            return new
            {
                staffId,
                period = new
                {
                    startDate = startDate?.ToString("yyyy-MM-dd"),
                    endDate = endDate?.ToString("yyyy-MM-dd")
                },
                appointments = appointments.Select(a => new
                {
                    id = a.Id,
                    patientName = a.PatientName,
                    serviceName = a.ServiceName,
                    startTime = a.StartTime.ToString("yyyy-MM-dd HH:mm"),
                    status = a.Status,
                    room = a.Room
                }).OrderBy(a => a.startTime),
                summary = new
                {
                    totalAppointments = appointments.Count(),
                    completedAppointments = appointments.Count(a => a.Status == "completed"),
                    completionRate = appointments.Any() ? 
                        Math.Round((decimal)appointments.Count(a => a.Status == "completed") / appointments.Count() * 100, 1) : 0
                }
            };
        }

        public async Task<AgendaStatistics> GetAgendaStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            return await _agendaRepository.GetAgendaStatisticsAsync(startDate, endDate);
        }

        public async Task<object> GetDashboardMetricsAsync()
        {
            return await _agendaRepository.GetDashboardMetricsAsync();
        }

        public async Task<object> GetHourlyDistributionAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var distribution = await _agendaRepository.GetHourlyDistributionAsync(startDate, endDate);
            
            return new
            {
                period = new
                {
                    startDate = startDate?.ToString("yyyy-MM-dd"),
                    endDate = endDate?.ToString("yyyy-MM-dd")
                },
                distribution = distribution.Select(h => new
                {
                    hour = h.Hour,
                    hourLabel = $"{h.Hour:00}:00",
                    count = h.Count,
                    percentage = h.Percentage
                }).OrderBy(h => h.hour)
            };
        }

        public async Task<object> GetWeeklyDistributionAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var distribution = await _agendaRepository.GetWeeklyDistributionAsync(startDate, endDate);
            
            return new
            {
                period = new
                {
                    startDate = startDate?.ToString("yyyy-MM-dd"),
                    endDate = endDate?.ToString("yyyy-MM-dd")
                },
                distribution = distribution.Select(d => new
                {
                    dayOfWeek = d.DayOfWeek.Trim(),
                    count = d.Count,
                    percentage = d.Percentage,
                    revenue = d.Revenue
                })
            };
        }

        public async Task<object> GetStatusDistributionAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var distribution = await _agendaRepository.GetStatusDistributionAsync(startDate, endDate);
            
            return new
            {
                period = new
                {
                    startDate = startDate?.ToString("yyyy-MM-dd"),
                    endDate = endDate?.ToString("yyyy-MM-dd")
                },
                distribution = distribution.Select(s => new
                {
                    status = s.Status,
                    statusLabel = s.StatusLabel,
                    count = s.Count,
                    percentage = s.Percentage,
                    color = s.Color
                })
            };
        }

        public async Task<bool> BulkUpdateAppointmentsAsync(BulkAppointmentAction action, int? updatedBy = null)
        {
            return await _agendaRepository.BulkUpdateAppointmentsAsync(action, updatedBy);
        }

        public async Task<object> GetAppointmentReportAsync(DateTime startDate, DateTime endDate, string reportType)
        {
            return await _agendaRepository.GetAppointmentReportAsync(startDate, endDate, reportType);
        }

        public async Task<object> GetStaffProductivityReportAsync(DateTime startDate, DateTime endDate)
        {
            return await _agendaRepository.GetStaffProductivityReportAsync(startDate, endDate);
        }

        public async Task<object> GetServiceUtilizationReportAsync(DateTime startDate, DateTime endDate)
        {
            return await _agendaRepository.GetServiceUtilizationReportAsync(startDate, endDate);
        }

        public async Task<object> GetAvailableRoomsAsync(DateTime startTime, DateTime endTime)
        {
            var rooms = await _agendaRepository.GetAvailableRoomsAsync(startTime, endTime);
            
            return new
            {
                timeSlot = new
                {
                    startTime = startTime.ToString("yyyy-MM-dd HH:mm"),
                    endTime = endTime.ToString("yyyy-MM-dd HH:mm")
                },
                availableRooms = rooms.Select(r => new
                {
                    name = r,
                    isAvailable = true
                })
            };
        }

        public async Task<object> GetRoomUtilizationAsync(DateTime startDate, DateTime endDate)
        {
            return await _agendaRepository.GetRoomUtilizationAsync(startDate, endDate);
        }

        public async Task<bool> SendAppointmentNotificationAsync(int appointmentId, string type)
        {
            return type switch
            {
                "confirmation" => await _agendaRepository.SendAppointmentConfirmationAsync(appointmentId),
                "reminder" => await _agendaRepository.SendAppointmentReminderAsync(appointmentId, type),
                "rescheduling" => await _agendaRepository.SendReschedulingNotificationAsync(appointmentId),
                "cancellation" => await _agendaRepository.SendCancellationNotificationAsync(appointmentId),
                _ => false
            };
        }

        // Métodos auxiliares privados
        private async Task ValidateAppointmentAsync(CreateAppointmentDto appointmentDto)
        {
            if (appointmentDto.PatientId <= 0)
                throw new ArgumentException("Paciente é obrigatório");

            if (appointmentDto.StartTime >= appointmentDto.EndTime)
                throw new ArgumentException("Horário de início deve ser anterior ao horário de término");

            if (appointmentDto.StartTime < DateTime.Now.AddMinutes(-30))
                throw new ArgumentException("Não é possível agendar consultas no passado");

            // Outras validações podem ser adicionadas aqui
        }

        private async Task ValidateUpdateAppointmentAsync(UpdateAppointmentDto appointmentDto)
        {
            if (appointmentDto.StartTime.HasValue && appointmentDto.EndTime.HasValue)
            {
                if (appointmentDto.StartTime >= appointmentDto.EndTime)
                    throw new ArgumentException("Horário de início deve ser anterior ao horário de término");
            }

            // Outras validações podem ser adicionadas aqui
        }
    }
}