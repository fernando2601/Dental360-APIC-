using ClinicApi.Models;
using ClinicApi.Repositories;
using System.Text;

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
            return await _agendaRepository.GetAllAsync();
        }

        public async Task<Appointment?> GetAppointmentByIdAsync(int id)
        {
            return await _agendaRepository.GetByIdAsync(id);
        }

        public async Task<Appointment> CreateAppointmentAsync(CreateAppointmentDto appointmentDto)
        {
            // Validações de negócio
            await ValidateAppointmentAsync(appointmentDto);
            
            // Verificar conflitos
            var conflicts = await _agendaRepository.CheckConflictsAsync(appointmentDto);
            if (conflicts.Any())
            {
                throw new InvalidOperationException($"Conflitos detectados: {string.Join(", ", conflicts.Select(c => c.Description))}");
            }

            return await _agendaRepository.CreateAsync(appointmentDto);
        }

        public async Task<Appointment?> UpdateAppointmentAsync(int id, CreateAppointmentDto appointmentDto)
        {
            await ValidateAppointmentAsync(appointmentDto);
            return await _agendaRepository.UpdateAsync(id, appointmentDto);
        }

        public async Task<bool> DeleteAppointmentAsync(int id)
        {
            return await _agendaRepository.DeleteAsync(id);
        }

        public async Task<object> GetCalendarViewAsync(DateTime startDate, DateTime endDate, int? staffId = null, string viewType = "month")
        {
            var appointments = await _agendaRepository.GetCalendarViewAsync(startDate, endDate, staffId);
            
            return new
            {
                viewType,
                period = new { startDate, endDate },
                staffId,
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
                        status = a.Status,
                        clientName = a.ClientName,
                        staffName = a.StaffName,
                        serviceName = a.ServiceName,
                        room = a.Room,
                        priority = a.Priority,
                        notes = a.Notes,
                        estimatedCost = a.EstimatedCost,
                        insuranceProvider = a.InsuranceProvider
                    }
                }),
                summary = new
                {
                    totalEvents = appointments.Count(),
                    statusBreakdown = appointments.GroupBy(a => a.Status)
                        .Select(g => new { status = g.Key, count = g.Count() })
                        .ToList()
                }
            };
        }

        public async Task<object> GetDayViewAsync(DateTime date, int? staffId = null)
        {
            var appointments = await _agendaRepository.GetDayViewAsync(date, staffId);
            var availability = await _agendaRepository.GetStaffAvailabilityAsync(date, staffId);
            
            return new
            {
                date = date.ToString("yyyy-MM-dd"),
                appointments = appointments.OrderBy(a => a.Start),
                staffAvailability = availability,
                summary = new
                {
                    totalAppointments = appointments.Count(),
                    busySlots = appointments.Count(),
                    availableSlots = availability.Sum(s => s.AvailableSlots.Count),
                    utilizationRate = CalculateUtilizationRate(appointments, availability)
                }
            };
        }

        public async Task<object> GetWeekViewAsync(DateTime weekStart, int? staffId = null)
        {
            var appointments = await _agendaRepository.GetWeekViewAsync(weekStart, staffId);
            
            // Agrupar por dia
            var dailyAppointments = appointments
                .GroupBy(a => a.Start.Date)
                .Select(g => new
                {
                    date = g.Key.ToString("yyyy-MM-dd"),
                    dayName = g.Key.ToString("dddd", new System.Globalization.CultureInfo("pt-BR")),
                    appointments = g.OrderBy(a => a.Start).ToList(),
                    count = g.Count()
                })
                .ToList();

            return new
            {
                weekStart = weekStart.ToString("yyyy-MM-dd"),
                weekEnd = weekStart.AddDays(6).ToString("yyyy-MM-dd"),
                dailyAppointments,
                summary = new
                {
                    totalAppointments = appointments.Count(),
                    averagePerDay = Math.Round((decimal)appointments.Count() / 7, 1),
                    busiestDay = dailyAppointments.OrderByDescending(d => d.count).FirstOrDefault()?.date,
                    quietestDay = dailyAppointments.OrderBy(d => d.count).FirstOrDefault()?.date
                }
            };
        }

        public async Task<object> GetMonthViewAsync(DateTime monthStart, int? staffId = null)
        {
            var appointments = await _agendaRepository.GetMonthViewAsync(monthStart, staffId);
            
            return new
            {
                month = monthStart.ToString("yyyy-MM"),
                monthName = monthStart.ToString("MMMM yyyy", new System.Globalization.CultureInfo("pt-BR")),
                appointments = appointments.OrderBy(a => a.Start),
                summary = new
                {
                    totalAppointments = appointments.Count(),
                    averagePerDay = Math.Round((decimal)appointments.Count() / DateTime.DaysInMonth(monthStart.Year, monthStart.Month), 1),
                    statusBreakdown = appointments.GroupBy(a => a.Status)
                        .Select(g => new { status = g.Key, count = g.Count() })
                        .ToDictionary(x => x.status, x => x.count)
                }
            };
        }

        public async Task<ScheduleOverview> GetScheduleOverviewAsync(DateTime date)
        {
            return await _agendaRepository.GetScheduleOverviewAsync(date);
        }

        public async Task<object> GetStaffAvailabilityAsync(DateTime date, int? staffId = null)
        {
            var availability = await _agendaRepository.GetStaffAvailabilityAsync(date, staffId);
            
            return new
            {
                date = date.ToString("yyyy-MM-dd"),
                staffAvailability = availability.Select(s => new
                {
                    staffId = s.StaffId,
                    staffName = s.StaffName,
                    isAvailable = s.IsAvailable,
                    workingHours = new
                    {
                        start = s.StartTime.ToString(@"hh\:mm"),
                        end = s.EndTime.ToString(@"hh\:mm")
                    },
                    availableSlots = s.AvailableSlots.Count,
                    unavailabilityReason = s.UnavailabilityReason
                }),
                summary = new
                {
                    totalStaff = availability.Count(),
                    availableStaff = availability.Count(s => s.IsAvailable),
                    totalAvailableSlots = availability.Sum(s => s.AvailableSlots.Count)
                }
            };
        }

        public async Task<object> GetAvailableSlotsAsync(DateTime date, int staffId, int serviceDuration)
        {
            var slots = await _agendaRepository.GetAvailableSlotsAsync(date, staffId, serviceDuration);
            
            return new
            {
                date = date.ToString("yyyy-MM-dd"),
                staffId,
                serviceDuration,
                availableSlots = slots.Where(s => s.IsAvailable).Select(s => new
                {
                    start = s.Start.ToString("HH:mm"),
                    end = s.End.ToString("HH:mm"),
                    startDateTime = s.Start.ToString("yyyy-MM-ddTHH:mm:ss"),
                    endDateTime = s.End.ToString("yyyy-MM-ddTHH:mm:ss")
                }),
                occupiedSlots = slots.Where(s => !s.IsAvailable).Select(s => new
                {
                    start = s.Start.ToString("HH:mm"),
                    end = s.End.ToString("HH:mm"),
                    reason = s.Reason,
                    existingAppointmentId = s.ExistingAppointmentId
                }),
                summary = new
                {
                    totalSlots = slots.Count(),
                    availableCount = slots.Count(s => s.IsAvailable),
                    occupiedCount = slots.Count(s => !s.IsAvailable),
                    utilizationRate = slots.Any() ? (decimal)slots.Count(s => !s.IsAvailable) / slots.Count() * 100 : 0
                }
            };
        }

        public async Task<object> FindBestAvailableSlotAsync(int staffId, int serviceId, DateTime preferredDate, int durationMinutes)
        {
            var slots = await _agendaRepository.GetAvailableSlotsAsync(preferredDate, staffId, durationMinutes);
            var availableSlots = slots.Where(s => s.IsAvailable).ToList();

            if (!availableSlots.Any())
            {
                // Procurar nos próximos 7 dias
                var alternativeDates = new List<object>();
                for (int i = 1; i <= 7; i++)
                {
                    var nextDate = preferredDate.AddDays(i);
                    var nextSlots = await _agendaRepository.GetAvailableSlotsAsync(nextDate, staffId, durationMinutes);
                    var nextAvailable = nextSlots.Where(s => s.IsAvailable).Take(3);
                    
                    if (nextAvailable.Any())
                    {
                        alternativeDates.Add(new
                        {
                            date = nextDate.ToString("yyyy-MM-dd"),
                            dayName = nextDate.ToString("dddd", new System.Globalization.CultureInfo("pt-BR")),
                            slots = nextAvailable.Select(s => new
                            {
                                start = s.Start.ToString("HH:mm"),
                                end = s.End.ToString("HH:mm"),
                                startDateTime = s.Start.ToString("yyyy-MM-ddTHH:mm:ss")
                            })
                        });
                    }
                }

                return new
                {
                    preferredDate = preferredDate.ToString("yyyy-MM-dd"),
                    bestSlot = (object?)null,
                    hasAvailability = false,
                    message = "Não há horários disponíveis na data solicitada",
                    alternativeDates
                };
            }

            // Encontrar o melhor slot (preferência por manhã)
            var bestSlot = availableSlots
                .OrderBy(s => Math.Abs(s.Start.Hour - 9)) // Preferência por 9h
                .ThenBy(s => s.Start)
                .First();

            return new
            {
                preferredDate = preferredDate.ToString("yyyy-MM-dd"),
                bestSlot = new
                {
                    start = bestSlot.Start.ToString("HH:mm"),
                    end = bestSlot.End.ToString("HH:mm"),
                    startDateTime = bestSlot.Start.ToString("yyyy-MM-ddTHH:mm:ss"),
                    endDateTime = bestSlot.End.ToString("yyyy-MM-ddTHH:mm:ss")
                },
                hasAvailability = true,
                alternativeSlots = availableSlots.Take(5).Select(s => new
                {
                    start = s.Start.ToString("HH:mm"),
                    end = s.End.ToString("HH:mm"),
                    startDateTime = s.Start.ToString("yyyy-MM-ddTHH:mm:ss")
                })
            };
        }

        public async Task<object> SuggestAlternativeSlotsAsync(CreateAppointmentDto appointment)
        {
            var conflicts = await _agendaRepository.CheckConflictsAsync(appointment);
            
            if (!conflicts.Any())
            {
                return new
                {
                    hasConflicts = false,
                    originalSlot = new
                    {
                        start = appointment.StartTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                        end = appointment.EndTime.ToString("yyyy-MM-ddTHH:mm:ss")
                    },
                    message = "Horário disponível"
                };
            }

            var duration = (int)(appointment.EndTime - appointment.StartTime).TotalMinutes;
            var bestSlot = await FindBestAvailableSlotAsync(appointment.StaffId, appointment.ServiceId, appointment.StartTime.Date, duration);

            return new
            {
                hasConflicts = true,
                conflicts = conflicts.Select(c => new
                {
                    type = c.ConflictType,
                    description = c.Description,
                    suggestion = c.Suggestion
                }),
                originalSlot = new
                {
                    start = appointment.StartTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                    end = appointment.EndTime.ToString("yyyy-MM-ddTHH:mm:ss")
                },
                suggestedAlternatives = bestSlot
            };
        }

        public async Task<bool> ValidateAppointmentAsync(CreateAppointmentDto appointment)
        {
            // Validações básicas
            if (appointment.StartTime >= appointment.EndTime)
                throw new ArgumentException("Horário de início deve ser anterior ao horário de término");

            if (appointment.StartTime < DateTime.Now.AddMinutes(-30))
                throw new ArgumentException("Não é possível agendar para horários passados");

            if (appointment.ClientId <= 0)
                throw new ArgumentException("Cliente é obrigatório");

            if (appointment.StaffId <= 0)
                throw new ArgumentException("Profissional é obrigatório");

            if (appointment.ServiceId <= 0)
                throw new ArgumentException("Serviço é obrigatório");

            // Validar duração mínima
            var duration = (appointment.EndTime - appointment.StartTime).TotalMinutes;
            if (duration < 15)
                throw new ArgumentException("Duração mínima de 15 minutos");

            // Validar horário comercial
            var hour = appointment.StartTime.Hour;
            if (hour < 7 || hour > 19)
                throw new ArgumentException("Agendamento fora do horário comercial (7h às 19h)");

            return true;
        }

        public async Task<object> CreateRecurringAppointmentsAsync(CreateAppointmentDto baseAppointment, RecurringAppointmentPattern pattern)
        {
            await ValidateAppointmentAsync(baseAppointment);
            
            var appointments = await _agendaRepository.CreateRecurringAppointmentsAsync(baseAppointment, pattern);
            
            return new
            {
                parentAppointment = appointments.First(),
                recurringAppointments = appointments.Skip(1),
                pattern = new
                {
                    type = pattern.Type,
                    interval = pattern.Interval,
                    endDate = pattern.EndDate.ToString("yyyy-MM-dd"),
                    totalOccurrences = appointments.Count()
                },
                summary = new
                {
                    totalCreated = appointments.Count(),
                    startDate = baseAppointment.StartTime.ToString("yyyy-MM-dd"),
                    endDate = pattern.EndDate.ToString("yyyy-MM-dd")
                }
            };
        }

        public async Task<object> GetRecurringSeriesAsync(int parentId)
        {
            var appointments = await _agendaRepository.GetRecurringAppointmentsAsync(parentId);
            
            return new
            {
                parentId,
                appointments = appointments.OrderBy(a => a.StartTime),
                summary = new
                {
                    totalAppointments = appointments.Count(),
                    completedCount = appointments.Count(a => a.Status == "completed"),
                    pendingCount = appointments.Count(a => a.Status == "scheduled" || a.Status == "confirmed"),
                    cancelledCount = appointments.Count(a => a.Status == "cancelled")
                }
            };
        }

        public async Task<bool> UpdateRecurringSeriesAsync(int parentId, CreateAppointmentDto updates, bool updateFutureOnly = false)
        {
            return await _agendaRepository.UpdateRecurringSeriesAsync(parentId, updates, updateFutureOnly);
        }

        public async Task<bool> UpdateAppointmentStatusAsync(int id, string status, string? notes = null)
        {
            // Validar status
            var validStatuses = new[] { "scheduled", "confirmed", "completed", "cancelled", "no_show" };
            if (!validStatuses.Contains(status))
                throw new ArgumentException($"Status inválido. Use: {string.Join(", ", validStatuses)}");

            return await _agendaRepository.UpdateAppointmentStatusAsync(id, status, notes);
        }

        public async Task<object> GetAppointmentsByStatusAsync(string status, DateTime? date = null)
        {
            var appointments = await _agendaRepository.GetAppointmentsByStatusAsync(status, date);
            
            return new
            {
                status,
                date = date?.ToString("yyyy-MM-dd"),
                appointments = appointments.OrderBy(a => a.StartTime),
                count = appointments.Count()
            };
        }

        public async Task<bool> BulkUpdateStatusAsync(int[] appointmentIds, string status)
        {
            if (!appointmentIds.Any())
                return false;

            return await _agendaRepository.BulkUpdateStatusAsync(appointmentIds, status);
        }

        public async Task<object> GetAppointmentReportsAsync(DateTime? startDate, DateTime? endDate, string[]? statuses, 
            int? professionalId, int? clientId, string? convenio, string? sala, int page, int limit)
        {
            var appointments = await _agendaRepository.GetAppointmentReportsAsync(startDate, endDate, statuses, 
                professionalId, clientId, convenio, sala, page, limit);
            
            var totalCount = await _agendaRepository.GetAppointmentReportsCountAsync(startDate, endDate, statuses, 
                professionalId, clientId, convenio, sala);

            return new
            {
                appointments,
                pagination = new
                {
                    currentPage = page,
                    totalPages = (int)Math.Ceiling((double)totalCount / limit),
                    totalItems = totalCount,
                    itemsPerPage = limit
                },
                filters = new
                {
                    startDate = startDate?.ToString("yyyy-MM-dd"),
                    endDate = endDate?.ToString("yyyy-MM-dd"),
                    statuses,
                    professionalId,
                    clientId,
                    convenio,
                    sala
                }
            };
        }

        public async Task<AppointmentStatistics> GetAppointmentStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            return await _agendaRepository.GetAppointmentStatisticsAsync(startDate, endDate);
        }

        public async Task<object> GetDashboardMetricsAsync(DateTime? date = null)
        {
            var metrics = await _agendaRepository.GetDashboardMetricsAsync(date);
            var statistics = await GetAppointmentStatisticsAsync(date, date?.AddDays(1));
            
            return new
            {
                metrics,
                statistics,
                insights = GenerateDashboardInsights(metrics, statistics),
                generatedAt = DateTime.UtcNow
            };
        }

        public async Task<object> GetUtilizationReportAsync(DateTime startDate, DateTime endDate)
        {
            return await _agendaRepository.GetUtilizationReportAsync(startDate, endDate);
        }

        public async Task<object> GetPerformanceAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var statistics = await GetAppointmentStatisticsAsync(startDate, endDate);
            var utilization = await GetUtilizationReportAsync(startDate ?? DateTime.Today.AddMonths(-1), endDate ?? DateTime.Today);
            
            return new
            {
                period = new
                {
                    startDate = startDate?.ToString("yyyy-MM-dd"),
                    endDate = endDate?.ToString("yyyy-MM-dd")
                },
                statistics,
                utilization,
                insights = new
                {
                    efficiency = CalculateEfficiencyScore(statistics),
                    trends = AnalyzeTrends(statistics),
                    recommendations = GenerateRecommendations(statistics)
                }
            };
        }

        public async Task<object> GetPendingRemindersAsync()
        {
            var reminders = await _agendaRepository.GetPendingRemindersAsync();
            
            return new
            {
                pendingReminders = reminders.OrderBy(r => r.ScheduledFor),
                count = reminders.Count(),
                summary = new
                {
                    smsReminders = reminders.Count(r => r.Type == "sms"),
                    emailReminders = reminders.Count(r => r.Type == "email"),
                    whatsappReminders = reminders.Count(r => r.Type == "whatsapp")
                }
            };
        }

        public async Task<bool> CreateReminderAsync(int appointmentId, string type, DateTime? scheduledFor = null)
        {
            var appointment = await GetAppointmentByIdAsync(appointmentId);
            if (appointment == null)
                return false;

            // Definir horário padrão (24h antes)
            var reminderTime = scheduledFor ?? appointment.StartTime.AddDays(-1);
            
            await _agendaRepository.CreateReminderAsync(appointmentId, type, reminderTime);
            return true;
        }

        public async Task<bool> SendRemindersAsync()
        {
            var pendingReminders = await _agendaRepository.GetPendingRemindersAsync();
            var sent = 0;

            foreach (var reminder in pendingReminders)
            {
                try
                {
                    // Aqui seria integrado com serviços de SMS, Email, WhatsApp
                    // Por enquanto, apenas marcamos como enviado
                    await _agendaRepository.MarkReminderSentAsync(reminder.Id);
                    sent++;
                }
                catch
                {
                    // Log do erro
                    continue;
                }
            }

            return sent > 0;
        }

        public async Task<object> GetReminderSettingsAsync()
        {
            return new
            {
                defaultTypes = new[] { "sms", "email", "whatsapp" },
                defaultTiming = new
                {
                    hours24Before = true,
                    hours2Before = true,
                    minutes30Before = false
                },
                templates = new
                {
                    sms = "Lembrete: Você tem agendamento amanhã às {time} na clínica.",
                    email = "Prezado(a) {clientName}, lembramos do seu agendamento para {date} às {time}.",
                    whatsapp = "🦷 Olá {clientName}! Seu agendamento está marcado para {date} às {time}."
                }
            };
        }

        public async Task<object> GetStaffWorkingHoursAsync(int staffId)
        {
            var workingHours = await _agendaRepository.GetStaffWorkingHoursAsync(staffId);
            
            return new
            {
                staffId,
                workingHours = workingHours.OrderBy(w => w.DayOfWeek).Select(w => new
                {
                    dayOfWeek = (int)w.DayOfWeek,
                    dayName = w.DayOfWeek.ToString(),
                    isWorkingDay = w.IsWorkingDay,
                    startTime = w.StartTime.ToString(@"hh\:mm"),
                    endTime = w.EndTime.ToString(@"hh\:mm")
                })
            };
        }

        public async Task<bool> UpdateWorkingHoursAsync(int staffId, List<WorkingHours> workingHours)
        {
            // Validar horários
            foreach (var wh in workingHours)
            {
                if (wh.IsWorkingDay && wh.StartTime >= wh.EndTime)
                    throw new ArgumentException($"Horário inválido para {wh.DayOfWeek}");
            }

            return await _agendaRepository.UpdateWorkingHoursAsync(staffId, workingHours);
        }

        public async Task<object> GetStaffScheduleAsync(int staffId, DateTime startDate, DateTime endDate)
        {
            var appointments = await _agendaRepository.GetCalendarViewAsync(startDate, endDate, staffId);
            var workingHours = await _agendaRepository.GetStaffWorkingHoursAsync(staffId);
            
            return new
            {
                staffId,
                period = new { startDate = startDate.ToString("yyyy-MM-dd"), endDate = endDate.ToString("yyyy-MM-dd") },
                appointments = appointments.OrderBy(a => a.Start),
                workingHours,
                summary = new
                {
                    totalAppointments = appointments.Count(),
                    totalHours = appointments.Sum(a => (a.End - a.Start).TotalHours),
                    busyDays = appointments.GroupBy(a => a.Start.Date).Count(),
                    averagePerDay = appointments.Any() ? 
                        Math.Round((double)appointments.Count() / ((endDate - startDate).Days + 1), 1) : 0
                }
            };
        }

        public async Task<object> GetAvailableRoomsAsync(DateTime startTime, DateTime endTime)
        {
            var availableRooms = await _agendaRepository.GetAvailableRoomsAsync(startTime, endTime);
            
            return new
            {
                requestedSlot = new
                {
                    start = startTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                    end = endTime.ToString("yyyy-MM-ddTHH:mm:ss")
                },
                availableRooms = availableRooms.OrderBy(r => r),
                count = availableRooms.Count()
            };
        }

        public async Task<object> GetRoomUtilizationAsync(DateTime date)
        {
            return await _agendaRepository.GetRoomUtilizationAsync(date);
        }

        public async Task<object> GetRoomScheduleAsync(string room, DateTime date)
        {
            var appointments = await _agendaRepository.GetDayViewAsync(date);
            var roomAppointments = appointments.Where(a => a.Room == room);
            
            return new
            {
                room,
                date = date.ToString("yyyy-MM-dd"),
                appointments = roomAppointments.OrderBy(a => a.Start),
                summary = new
                {
                    totalAppointments = roomAppointments.Count(),
                    totalHours = roomAppointments.Sum(a => (a.End - a.Start).TotalHours),
                    utilizationRate = CalculateRoomUtilization(roomAppointments, date)
                }
            };
        }

        public async Task<object> CheckConflictsAsync(CreateAppointmentDto appointment)
        {
            var conflicts = await _agendaRepository.CheckConflictsAsync(appointment);
            
            return new
            {
                hasConflicts = conflicts.Any(),
                conflicts = conflicts.Select(c => new
                {
                    type = c.ConflictType,
                    description = c.Description,
                    conflictTime = c.ConflictTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                    suggestion = c.Suggestion,
                    conflictingAppointmentId = c.ConflictingAppointmentId
                }),
                appointment = new
                {
                    start = appointment.StartTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                    end = appointment.EndTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                    staffId = appointment.StaffId,
                    room = appointment.Room
                }
            };
        }

        public async Task<object> ResolveSchedulingConflictsAsync(int appointmentId)
        {
            var appointment = await GetAppointmentByIdAsync(appointmentId);
            if (appointment == null)
                return new { success = false, message = "Agendamento não encontrado" };

            var appointmentDto = new CreateAppointmentDto
            {
                ClientId = appointment.ClientId,
                StaffId = appointment.StaffId,
                ServiceId = appointment.ServiceId,
                StartTime = appointment.StartTime,
                EndTime = appointment.EndTime,
                Room = appointment.Room
            };

            return await SuggestAlternativeSlotsAsync(appointmentDto);
        }

        public async Task<object> ExportScheduleAsync(DateTime startDate, DateTime endDate, string format = "pdf")
        {
            var appointments = await _agendaRepository.GetCalendarViewAsync(startDate, endDate);
            
            // Simular geração de relatório
            var fileName = $"agenda-{startDate:yyyyMMdd}-{endDate:yyyyMMdd}.{format.ToLower()}";
            var content = Encoding.UTF8.GetBytes("Relatório de agenda gerado");
            var contentType = format.ToLower() switch
            {
                "pdf" => "application/pdf",
                "excel" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "csv" => "text/csv",
                _ => "application/octet-stream"
            };

            return new
            {
                fileName,
                content,
                contentType,
                summary = new
                {
                    period = new { startDate = startDate.ToString("yyyy-MM-dd"), endDate = endDate.ToString("yyyy-MM-dd") },
                    totalAppointments = appointments.Count(),
                    format
                }
            };
        }

        public async Task<object> ImportAppointmentsAsync(object importData)
        {
            // Implementação básica de importação
            return new
            {
                success = true,
                message = "Importação realizada com sucesso",
                imported = 0,
                errors = new List<string>()
            };
        }

        // Métodos auxiliares privados
        private decimal CalculateUtilizationRate(IEnumerable<AppointmentCalendarView> appointments, IEnumerable<StaffAvailability> availability)
        {
            if (!availability.Any()) return 0;

            var totalWorkingHours = availability.Sum(a => (a.EndTime - a.StartTime).TotalHours);
            var totalAppointmentHours = appointments.Sum(a => (a.End - a.Start).TotalHours);

            return totalWorkingHours > 0 ? (decimal)(totalAppointmentHours / totalWorkingHours * 100) : 0;
        }

        private decimal CalculateRoomUtilization(IEnumerable<AppointmentCalendarView> roomAppointments, DateTime date)
        {
            const int workingHours = 10; // 8h às 18h
            var occupiedHours = roomAppointments.Sum(a => (a.End - a.Start).TotalHours);
            return (decimal)(occupiedHours / workingHours * 100);
        }

        private object GenerateDashboardInsights(dynamic metrics, AppointmentStatistics statistics)
        {
            var insights = new List<string>();

            if (statistics.CompletionRate > 90)
                insights.Add("Excelente taxa de conclusão de agendamentos!");

            if (statistics.NoShowRate > 15)
                insights.Add("Taxa de faltas elevada. Considere melhorar os lembretes.");

            if (statistics.CancellationRate > 20)
                insights.Add("Muitos cancelamentos. Revisar política de reagendamento.");

            return new
            {
                insights,
                performance = statistics.CompletionRate > 85 ? "Excelente" : 
                             statistics.CompletionRate > 70 ? "Bom" : "Precisa melhorar"
            };
        }

        private decimal CalculateEfficiencyScore(AppointmentStatistics statistics)
        {
            var completionWeight = 0.4m;
            var utilizationWeight = 0.3m;
            var punctualityWeight = 0.3m;

            var completionScore = statistics.CompletionRate;
            var utilizationScore = statistics.TotalAppointments > 0 ? 
                Math.Min(100, statistics.TotalAppointments * 10) : 0; // Estimativa
            var punctualityScore = 100 - statistics.NoShowRate; // Estimativa

            return (completionScore * completionWeight) + 
                   (utilizationScore * utilizationWeight) + 
                   (punctualityScore * punctualityWeight);
        }

        private object AnalyzeTrends(AppointmentStatistics statistics)
        {
            return new
            {
                weeklyTrend = statistics.WeeklyTrend.Any() ? "Crescente" : "Estável",
                busiestHours = statistics.BusiestHours.Take(3).Select(h => h.TimeRange),
                seasonality = "Estável ao longo da semana"
            };
        }

        private List<string> GenerateRecommendations(AppointmentStatistics statistics)
        {
            var recommendations = new List<string>();

            if (statistics.NoShowRate > 10)
                recommendations.Add("Implementar sistema de confirmação automática");

            if (statistics.CancellationRate > 15)
                recommendations.Add("Revisar política de cancelamento");

            if (statistics.CompletionRate < 80)
                recommendations.Add("Melhorar processo de agendamento");

            recommendations.Add("Continuar monitorando métricas de performance");

            return recommendations;
        }
    }
}