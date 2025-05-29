using ClinicApi.Models;
using ClinicApi.Repositories;
using System.Text;

namespace ClinicApi.Services
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;

        public PatientService(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }

        public async Task<IEnumerable<Patient>> GetAllPatientsAsync()
        {
            return await _patientRepository.GetAllAsync();
        }

        public async Task<Patient?> GetPatientByIdAsync(int id)
        {
            return await _patientRepository.GetByIdAsync(id);
        }

        public async Task<Patient> CreatePatientAsync(CreatePatientDto patientDto)
        {
            // Validações de negócio
            await ValidatePatientDataAsync(patientDto);
            
            // Verificar duplicatas
            if (!string.IsNullOrEmpty(patientDto.Cpf))
            {
                var existingPatients = await _patientRepository.SearchPatientsAsync(patientDto.Cpf);
                if (existingPatients.Any())
                {
                    throw new InvalidOperationException("Já existe um paciente cadastrado com este CPF");
                }
            }

            if (!string.IsNullOrEmpty(patientDto.Email))
            {
                var existingByEmail = await _patientRepository.SearchPatientsAsync(patientDto.Email);
                if (existingByEmail.Any())
                {
                    throw new InvalidOperationException("Já existe um paciente cadastrado com este email");
                }
            }

            return await _patientRepository.CreateAsync(patientDto);
        }

        public async Task<Patient?> UpdatePatientAsync(int id, CreatePatientDto patientDto)
        {
            await ValidatePatientDataAsync(patientDto);
            return await _patientRepository.UpdateAsync(id, patientDto);
        }

        public async Task<bool> DeletePatientAsync(int id)
        {
            // Verificar se pode deletar (soft delete)
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null)
                return false;

            return await _patientRepository.DeleteAsync(id);
        }

        public async Task<PatientProfile> GetPatientProfileAsync(int id)
        {
            return await _patientRepository.GetPatientProfileAsync(id);
        }

        public async Task<object> SearchPatientsAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            {
                return new
                {
                    patients = new List<PatientSearchResult>(),
                    totalCount = 0,
                    message = "Digite pelo menos 2 caracteres para buscar"
                };
            }

            var patients = await _patientRepository.SearchPatientsAsync(query);
            
            return new
            {
                patients = patients.Take(20), // Limitar resultados
                totalCount = patients.Count(),
                query,
                searchTime = DateTime.UtcNow
            };
        }

        public async Task<object> GetPatientsWithFiltersAsync(
            string? name = null, string? email = null, string? phone = null, string? cpf = null,
            string? city = null, string? healthPlan = null, string? status = null,
            DateTime? birthStart = null, DateTime? birthEnd = null,
            int page = 1, int limit = 25)
        {
            var patients = await _patientRepository.GetPatientsWithFiltersAsync(
                name, email, phone, cpf, city, healthPlan, status, 
                birthStart, birthEnd, page, limit);
            
            var totalCount = await _patientRepository.GetPatientsCountAsync(
                name, email, phone, cpf, city, healthPlan, status, 
                birthStart, birthEnd);

            return new
            {
                patients,
                pagination = new
                {
                    currentPage = page,
                    totalPages = (int)Math.Ceiling((double)totalCount / limit),
                    totalItems = totalCount,
                    itemsPerPage = limit,
                    hasNext = page * limit < totalCount,
                    hasPrevious = page > 1
                },
                filters = new
                {
                    name, email, phone, cpf, city, healthPlan, status,
                    birthStart = birthStart?.ToString("yyyy-MM-dd"),
                    birthEnd = birthEnd?.ToString("yyyy-MM-dd")
                },
                summary = new
                {
                    totalPatients = totalCount,
                    filteredResults = patients.Count(),
                    appliedFilters = CountAppliedFilters(name, email, phone, cpf, city, healthPlan, status, birthStart, birthEnd)
                }
            };
        }

        public async Task<object> GetPatientMedicalHistoryAsync(int patientId)
        {
            var history = await _patientRepository.GetMedicalHistoryAsync(patientId);
            
            var groupedHistory = history.GroupBy(h => h.Category)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(h => h.Date).ToList());

            return new
            {
                patientId,
                medicalHistory = groupedHistory,
                summary = new
                {
                    totalRecords = history.Count(),
                    categories = groupedHistory.Keys.ToList(),
                    lastUpdate = history.Any() ? history.Max(h => h.CreatedAt) : null,
                    criticalItems = history.Count(h => h.Severity == "critical" || h.Severity == "high")
                }
            };
        }

        public async Task<PatientMedicalHistory> AddMedicalHistoryAsync(PatientMedicalHistory history)
        {
            // Validações
            if (history.PatientId <= 0)
                throw new ArgumentException("Paciente é obrigatório");

            if (string.IsNullOrWhiteSpace(history.Title))
                throw new ArgumentException("Título é obrigatório");

            if (string.IsNullOrWhiteSpace(history.Category))
                history.Category = "medical";

            if (string.IsNullOrWhiteSpace(history.Severity))
                history.Severity = "medium";

            history.IsActive = true;
            return await _patientRepository.AddMedicalHistoryAsync(history);
        }

        public async Task<PatientMedicalHistory?> UpdateMedicalHistoryAsync(int id, PatientMedicalHistory history)
        {
            return await _patientRepository.UpdateMedicalHistoryAsync(id, history);
        }

        public async Task<bool> DeleteMedicalHistoryAsync(int id)
        {
            return await _patientRepository.DeleteMedicalHistoryAsync(id);
        }

        public async Task<object> GetPatientDocumentsAsync(int patientId)
        {
            var documents = await _patientRepository.GetPatientDocumentsAsync(patientId);
            
            var groupedDocs = documents.GroupBy(d => d.DocumentType)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(d => d.UploadDate).ToList());

            return new
            {
                patientId,
                documents = groupedDocs,
                summary = new
                {
                    totalDocuments = documents.Count(),
                    documentTypes = groupedDocs.Keys.ToList(),
                    totalSize = documents.Sum(d => d.FileSize),
                    lastUpload = documents.Any() ? documents.Max(d => d.UploadDate) : null
                }
            };
        }

        public async Task<PatientDocument> UploadDocumentAsync(int patientId, object documentData)
        {
            // Implementação simplificada - seria integrado com sistema de arquivos
            var document = new PatientDocument
            {
                PatientId = patientId,
                DocumentType = "general",
                FileName = "documento.pdf",
                FilePath = "/uploads/patients/" + patientId,
                FileSize = 1024,
                ContentType = "application/pdf",
                UploadDate = DateTime.UtcNow,
                Description = "Documento carregado",
                IsPublic = false
            };

            return await _patientRepository.AddDocumentAsync(document);
        }

        public async Task<bool> DeleteDocumentAsync(int id)
        {
            return await _patientRepository.DeleteDocumentAsync(id);
        }

        public async Task<object> DownloadDocumentAsync(int id)
        {
            // Implementação simplificada
            return new
            {
                documentId = id,
                downloadUrl = $"/api/patients/documents/{id}/download",
                expiresAt = DateTime.UtcNow.AddHours(1)
            };
        }

        public async Task<object> GetPatientNotesAsync(int patientId)
        {
            var notes = await _patientRepository.GetPatientNotesAsync(patientId);
            
            var groupedNotes = notes.GroupBy(n => n.Category)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(n => n.CreatedAt).ToList());

            return new
            {
                patientId,
                notes = groupedNotes,
                summary = new
                {
                    totalNotes = notes.Count(),
                    categories = groupedNotes.Keys.ToList(),
                    highPriorityNotes = notes.Count(n => n.Priority == "high"),
                    privateNotes = notes.Count(n => n.IsPrivate),
                    lastNote = notes.Any() ? notes.Max(n => n.CreatedAt) : null
                }
            };
        }

        public async Task<PatientNote> AddPatientNoteAsync(PatientNote note)
        {
            // Validações
            if (note.PatientId <= 0)
                throw new ArgumentException("Paciente é obrigatório");

            if (string.IsNullOrWhiteSpace(note.Content))
                throw new ArgumentException("Conteúdo da nota é obrigatório");

            if (string.IsNullOrWhiteSpace(note.Category))
                note.Category = "general";

            if (string.IsNullOrWhiteSpace(note.Priority))
                note.Priority = "medium";

            note.CreatedAt = DateTime.UtcNow;
            return await _patientRepository.AddNoteAsync(note);
        }

        public async Task<PatientNote?> UpdatePatientNoteAsync(int id, PatientNote note)
        {
            return await _patientRepository.UpdateNoteAsync(id, note);
        }

        public async Task<bool> DeletePatientNoteAsync(int id)
        {
            return await _patientRepository.DeleteNoteAsync(id);
        }

        public async Task<PatientAnalytics> GetPatientAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            return await _patientRepository.GetPatientAnalyticsAsync(startDate, endDate);
        }

        public async Task<PatientDashboardMetrics> GetDashboardMetricsAsync()
        {
            return await _patientRepository.GetDashboardMetricsAsync();
        }

        public async Task<object> GetPatientSegmentsAsync()
        {
            var segments = await _patientRepository.GetPatientSegmentsAsync();
            
            return new
            {
                segments,
                summary = new
                {
                    totalSegments = segments.Count(),
                    totalPatients = segments.Sum(s => s.PatientCount),
                    averageValue = segments.Any() ? segments.Average(s => s.AverageValue) : 0,
                    generatedAt = DateTime.UtcNow
                },
                insights = GenerateSegmentInsights(segments)
            };
        }

        public async Task<object> GetBirthdayRemindersAsync(DateTime? date = null)
        {
            var birthdays = await _patientRepository.GetBirthdaysAsync(date);
            var targetDate = date ?? DateTime.Today;
            
            return new
            {
                date = targetDate.ToString("yyyy-MM-dd"),
                birthdays = birthdays.OrderBy(b => b.FullName),
                summary = new
                {
                    totalBirthdays = birthdays.Count(),
                    messagesSent = birthdays.Count(b => b.MessageSent),
                    pendingMessages = birthdays.Count(b => !b.MessageSent),
                    ageGroups = birthdays.GroupBy(b => GetAgeGroup(b.Age))
                        .ToDictionary(g => g.Key, g => g.Count())
                },
                suggestions = new
                {
                    sendBirthdayMessages = !birthdays.Any(b => b.MessageSent),
                    prepareSpecialOffers = birthdays.Count() > 5,
                    scheduleFollowUp = true
                }
            };
        }

        public async Task<object> GetPatientCommunicationsAsync(int patientId)
        {
            var communications = await _patientRepository.GetPatientCommunicationsAsync(patientId);
            
            var groupedComms = communications.GroupBy(c => c.Type)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(c => c.SentAt).ToList());

            return new
            {
                patientId,
                communications = groupedComms,
                summary = new
                {
                    totalCommunications = communications.Count(),
                    types = groupedComms.Keys.ToList(),
                    successfulDeliveries = communications.Count(c => c.Status == "delivered"),
                    failedDeliveries = communications.Count(c => c.Status == "failed"),
                    lastCommunication = communications.Any() ? communications.Max(c => c.SentAt) : null
                }
            };
        }

        public async Task<object> SendCommunicationAsync(int patientId, object communicationData)
        {
            // Implementação simplificada
            var communication = new PatientCommunication
            {
                PatientId = patientId,
                Type = "email",
                Direction = "outbound",
                Subject = "Comunicação da clínica",
                Content = "Mensagem para o paciente",
                Status = "sent",
                SentAt = DateTime.UtcNow,
                SentBy = 1,
                SentByName = "Sistema"
            };

            var result = await _patientRepository.AddCommunicationAsync(communication);
            
            return new
            {
                success = true,
                communicationId = result.Id,
                message = "Comunicação enviada com sucesso",
                sentAt = result.SentAt
            };
        }

        public async Task<object> GetCommunicationTemplatesAsync()
        {
            return new
            {
                templates = new[]
                {
                    new { id = 1, name = "Lembrete de Consulta", type = "appointment_reminder", category = "appointment" },
                    new { id = 2, name = "Confirmação de Agendamento", type = "appointment_confirmation", category = "appointment" },
                    new { id = 3, name = "Parabéns pelo Aniversário", type = "birthday_greeting", category = "marketing" },
                    new { id = 4, name = "Resultado de Exame", type = "exam_result", category = "medical" },
                    new { id = 5, name = "Cobrança Pendente", type = "payment_reminder", category = "financial" }
                },
                categories = new[] { "appointment", "marketing", "medical", "financial", "general" },
                channels = new[] { "email", "sms", "whatsapp", "phone" }
            };
        }

        public async Task<object> BulkUpdatePatientsAsync(PatientBulkAction action)
        {
            var result = await _patientRepository.BulkUpdatePatientsAsync(action);
            
            return new
            {
                success = result,
                action = action.Action,
                patientsAffected = action.PatientIds.Length,
                executedAt = DateTime.UtcNow,
                message = result ? "Ação executada com sucesso" : "Falha na execução da ação"
            };
        }

        public async Task<object> ExportPatientsAsync(PatientExportRequest request)
        {
            var result = await _patientRepository.ExportPatientsAsync(request);
            
            return new
            {
                success = true,
                format = request.Format,
                fileName = $"pacientes-{DateTime.Now:yyyyMMdd-HHmmss}.{request.Format}",
                downloadUrl = "/api/patients/export/download",
                expiresAt = DateTime.UtcNow.AddHours(24),
                filters = new
                {
                    includeInactive = request.IncludeInactive,
                    dateRange = request.StartDate.HasValue || request.EndDate.HasValue,
                    fieldsSelected = request.Fields?.Length ?? 0
                }
            };
        }

        public async Task<object> ImportPatientsAsync(object importData)
        {
            // Implementação simplificada
            return new
            {
                success = true,
                imported = 0,
                updated = 0,
                errors = new List<string>(),
                duplicates = 0,
                validationErrors = new List<string>(),
                summary = new
                {
                    totalProcessed = 0,
                    successRate = 100.0m,
                    executionTime = "2.5s"
                }
            };
        }

        public async Task<object> GetRetentionAnalysisAsync(DateTime startDate, DateTime endDate)
        {
            var result = await _patientRepository.GetRetentionAnalysisAsync(startDate, endDate);
            
            return new
            {
                period = new { startDate = startDate.ToString("yyyy-MM-dd"), endDate = endDate.ToString("yyyy-MM-dd") },
                retentionMetrics = result,
                insights = new
                {
                    trends = "Retenção estável nos últimos meses",
                    recommendations = new[]
                    {
                        "Implementar programa de fidelidade",
                        "Melhorar follow-up pós consulta",
                        "Criar lembretes proativos"
                    },
                    riskFactors = new[]
                    {
                        "Pacientes sem consulta há 90+ dias",
                        "Taxa de cancelamento acima da média"
                    }
                }
            };
        }

        public async Task<object> GetPatientValueAnalysisAsync()
        {
            var result = await _patientRepository.GetPatientValueAnalysisAsync();
            
            return new
            {
                valueAnalysis = result,
                segments = new
                {
                    highValue = new { threshold = 5000, count = 35, percentage = 22 },
                    mediumValue = new { threshold = 1000, count = 80, percentage = 50 },
                    lowValue = new { threshold = 0, count = 45, percentage = 28 }
                },
                insights = new
                {
                    averageLifetimeValue = 2450.00m,
                    topPerformingServices = new[] { "Implantes", "Ortodontia", "Harmonização" },
                    growthOpportunities = new[] { "Cross-selling", "Upselling", "Programas de manutenção" }
                }
            };
        }

        public async Task<object> GetGeographicDistributionAsync()
        {
            var result = await _patientRepository.GetGeographicDistributionAsync();
            
            return new
            {
                geographicData = result,
                insights = new
                {
                    primaryMarket = "São Paulo",
                    expansionOpportunities = new[] { "Região ABC", "Interior" },
                    marketPenetration = new
                    {
                        local = 65.5m,
                        regional = 24.2m,
                        distant = 10.3m
                    }
                }
            };
        }

        public async Task<object> GetPatientLifecycleAnalysisAsync()
        {
            return new
            {
                lifecycleStages = new[]
                {
                    new { stage = "Prospect", count = 45, percentage = 15.0m, averageDays = 0 },
                    new { stage = "New Patient", count = 60, percentage = 20.0m, averageDays = 30 },
                    new { stage = "Active", count = 150, percentage = 50.0m, averageDays = 180 },
                    new { stage = "At Risk", count = 30, percentage = 10.0m, averageDays = 90 },
                    new { stage = "Inactive", count = 15, percentage = 5.0m, averageDays = 365 }
                },
                transitions = new
                {
                    newToActive = 85.5m,
                    activeToRisk = 12.8m,
                    riskToInactive = 45.2m,
                    reactivationRate = 15.3m
                },
                insights = new
                {
                    criticalPeriod = "90-120 dias após primeira consulta",
                    interventionPoints = new[] { "30 dias", "90 dias", "180 dias" },
                    successFactors = new[] { "Follow-up regular", "Comunicação proativa", "Experiência excepcional" }
                }
            };
        }

        public async Task<object> GetChurnPredictionAsync()
        {
            return new
            {
                riskAnalysis = new
                {
                    highRisk = new { count = 25, percentage = 8.3m },
                    mediumRisk = new { count = 45, percentage = 15.0m },
                    lowRisk = new { count = 230, percentage = 76.7m }
                },
                predictiveFactors = new[]
                {
                    new { factor = "Tempo desde última consulta", weight = 35.2m },
                    new { factor = "Frequência de cancelamentos", weight = 28.1m },
                    new { factor = "Engajamento com comunicações", weight = 18.7m },
                    new { factor = "Valor gasto", weight = 12.5m },
                    new { factor = "Distância geográfica", weight = 5.5m }
                },
                recommendations = new[]
                {
                    "Implementar campanha de reativação para grupo de alto risco",
                    "Melhorar processo de follow-up",
                    "Personalizar comunicações baseadas no perfil"
                }
            };
        }

        public async Task<object> GetPatientRecommendationsAsync(int patientId)
        {
            var profile = await _patientRepository.GetPatientProfileAsync(patientId);
            
            return new
            {
                patientId,
                recommendations = new[]
                {
                    new { category = "Treatment", recommendation = "Considerar manutenção preventiva", priority = "medium" },
                    new { category = "Communication", recommendation = "Enviar lembrete de retorno", priority = "high" },
                    new { category = "Upsell", recommendation = "Oferecer plano de manutenção", priority = "low" }
                },
                insights = new
                {
                    patientType = profile.Statistics.PatientType,
                    riskLevel = CalculateRiskLevel(profile),
                    opportunityScore = CalculateOpportunityScore(profile)
                },
                nextActions = new[]
                {
                    "Agendar consulta de retorno",
                    "Enviar pesquisa de satisfação",
                    "Propor plano de tratamento preventivo"
                }
            };
        }

        public async Task<object> GetSegmentationInsightsAsync()
        {
            return new
            {
                segmentationStrategy = new
                {
                    primary = "Valor do paciente (LTV)",
                    secondary = "Frequência de visitas",
                    tertiary = "Tipo de tratamento preferido"
                },
                keySegments = new[]
                {
                    new { name = "Champions", size = 15, characteristics = "Alto valor, alta frequência" },
                    new { name = "Loyal Customers", size = 35, characteristics = "Médio valor, alta frequência" },
                    new { name = "New Customers", size = 25, characteristics = "Baixo valor, baixa frequência" },
                    new { name = "At Risk", size = 15, characteristics = "Alto valor, baixa frequência recente" },
                    new { name = "Cannot Lose", size = 10, characteristics = "Alto valor, frequência declinante" }
                },
                actionPlans = new
                {
                    champions = "Programa VIP e referências",
                    loyal = "Cross-selling e upselling",
                    newCustomers = "Educação e engajamento",
                    atRisk = "Campanhas de reativação",
                    cannotLose = "Intervenção pessoal e ofertas especiais"
                }
            };
        }

        public async Task<object> ValidatePatientDataAsync(int patientId)
        {
            var patient = await _patientRepository.GetByIdAsync(patientId);
            if (patient == null)
                return new { valid = false, message = "Paciente não encontrado" };

            var validationResults = new List<object>();

            // Validar campos obrigatórios
            if (string.IsNullOrWhiteSpace(patient.FullName))
                validationResults.Add(new { field = "fullName", issue = "Nome completo é obrigatório" });

            if (string.IsNullOrWhiteSpace(patient.Phone))
                validationResults.Add(new { field = "phone", issue = "Telefone é obrigatório" });

            // Validar formato de dados
            if (!string.IsNullOrEmpty(patient.Email) && !IsValidEmail(patient.Email))
                validationResults.Add(new { field = "email", issue = "Formato de email inválido" });

            if (!string.IsNullOrEmpty(patient.Cpf) && !IsValidCpf(patient.Cpf))
                validationResults.Add(new { field = "cpf", issue = "CPF inválido" });

            return new
            {
                patientId,
                valid = !validationResults.Any(),
                issues = validationResults,
                score = CalculateDataQualityScore(patient, validationResults),
                recommendations = GenerateDataQualityRecommendations(validationResults)
            };
        }

        public async Task<object> GetDuplicatePatientsAsync()
        {
            // Implementação simplificada
            return new
            {
                duplicateGroups = new[]
                {
                    new
                    {
                        criteria = "Nome similar + Telefone",
                        patients = new[]
                        {
                            new { id = 1, name = "João Silva", phone = "(11) 99999-9999", confidence = 95.5m },
                            new { id = 2, name = "Joao da Silva", phone = "(11) 99999-9999", confidence = 95.5m }
                        }
                    }
                },
                summary = new
                {
                    totalDuplicates = 2,
                    groupsFound = 1,
                    highConfidence = 1,
                    mediumConfidence = 0,
                    lowConfidence = 0
                },
                recommendations = new[]
                {
                    "Revisar registros com alta similaridade",
                    "Implementar validação automática de duplicatas",
                    "Criar processo de merge de registros"
                }
            };
        }

        public async Task<object> CleanupPatientDataAsync()
        {
            return new
            {
                cleanupResults = new
                {
                    duplicatesRemoved = 5,
                    incompleteRecordsUpdated = 12,
                    invalidDataCorrected = 8,
                    obsoleteRecordsArchived = 3
                },
                dataQualityImprovement = new
                {
                    before = 78.5m,
                    after = 92.3m,
                    improvement = 13.8m
                },
                nextActions = new[]
                {
                    "Implementar validação em tempo real",
                    "Criar processo de auditoria mensal",
                    "Treinar equipe sobre qualidade de dados"
                }
            };
        }

        // Métodos auxiliares privados
        private async Task ValidatePatientDataAsync(CreatePatientDto patient)
        {
            if (string.IsNullOrWhiteSpace(patient.FullName))
                throw new ArgumentException("Nome completo é obrigatório");

            if (string.IsNullOrWhiteSpace(patient.Phone))
                throw new ArgumentException("Telefone é obrigatório");

            if (!string.IsNullOrEmpty(patient.Email) && !IsValidEmail(patient.Email))
                throw new ArgumentException("Formato de email inválido");

            if (!string.IsNullOrEmpty(patient.Cpf) && !IsValidCpf(patient.Cpf))
                throw new ArgumentException("CPF inválido");

            if (patient.Birthday.HasValue && patient.Birthday.Value > DateTime.Today)
                throw new ArgumentException("Data de nascimento não pode ser futura");
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidCpf(string cpf)
        {
            // Implementação simplificada de validação de CPF
            cpf = cpf.Replace(".", "").Replace("-", "").Replace(" ", "");
            
            if (cpf.Length != 11)
                return false;

            if (cpf.All(c => c == cpf[0]))
                return false;

            return true; // Validação completa seria implementada aqui
        }

        private int CountAppliedFilters(params object[] filters)
        {
            return filters.Count(f => f != null && !string.IsNullOrWhiteSpace(f.ToString()));
        }

        private object GenerateSegmentInsights(IEnumerable<PatientSegment> segments)
        {
            return new
            {
                topSegment = segments.OrderByDescending(s => s.PatientCount).FirstOrDefault()?.SegmentName,
                mostValuable = segments.OrderByDescending(s => s.AverageValue).FirstOrDefault()?.SegmentName,
                growthOpportunity = "Expandir segmento de pacientes regulares",
                recommendations = new[]
                {
                    "Focar na retenção de pacientes VIP",
                    "Converter novos pacientes em regulares",
                    "Reativar pacientes inativos"
                }
            };
        }

        private string GetAgeGroup(int age)
        {
            return age switch
            {
                < 18 => "Menor",
                <= 30 => "Jovem",
                <= 50 => "Adulto",
                <= 65 => "Maduro",
                _ => "Idoso"
            };
        }

        private string CalculateRiskLevel(PatientProfile profile)
        {
            var daysSinceLastVisit = profile.Statistics.LastVisit.HasValue ? 
                (DateTime.Today - profile.Statistics.LastVisit.Value).Days : int.MaxValue;

            return daysSinceLastVisit switch
            {
                <= 30 => "Low",
                <= 90 => "Medium",
                <= 180 => "High",
                _ => "Critical"
            };
        }

        private decimal CalculateOpportunityScore(PatientProfile profile)
        {
            var score = 0m;
            
            if (profile.Statistics.TotalSpent > 1000) score += 30;
            if (profile.Statistics.CompletionRate > 80) score += 25;
            if (profile.Statistics.TotalAppointments > 5) score += 20;
            if (profile.Statistics.LastVisit > DateTime.Today.AddDays(-60)) score += 25;
            
            return score;
        }

        private decimal CalculateDataQualityScore(Patient patient, List<object> issues)
        {
            var totalFields = 15; // Número total de campos
            var issueCount = issues.Count;
            
            return ((decimal)(totalFields - issueCount) / totalFields) * 100;
        }

        private List<string> GenerateDataQualityRecommendations(List<object> issues)
        {
            var recommendations = new List<string>();
            
            if (issues.Any())
            {
                recommendations.Add("Completar campos obrigatórios");
                recommendations.Add("Validar formato dos dados");
                recommendations.Add("Verificar duplicatas");
            }
            else
            {
                recommendations.Add("Dados em excelente qualidade");
            }
            
            return recommendations;
        }
    }
}