// DentalSpa Application JavaScript
class DentalSpaApp {
    constructor() {
        this.currentPage = 'dashboard';
        this.patients = [];
        this.appointments = [];
        this.services = [];
        this.staff = [];
        this.transactions = [];
        this.currentWeek = new Date();
        this.init();
    }

    init() {
        this.setupNavigation();
        this.initializeDemoData();
        this.loadDashboardData();
    }

    initializeDemoData() {
        this.patients = [
            { id: 1, name: 'Maria Silva', phone: '(11) 99999-9999', email: 'maria@email.com', status: 'active', nextAppointment: '15/06/2025 14:00', birth: '1985-03-15', address: 'Rua das Flores, 123' },
            { id: 2, name: 'João Santos', phone: '(11) 88888-8888', email: 'joao@email.com', status: 'active', nextAppointment: '16/06/2025 09:30', birth: '1990-07-22', address: 'Av. Paulista, 456' },
            { id: 3, name: 'Ana Costa', phone: '(11) 77777-7777', email: 'ana@email.com', status: 'active', nextAppointment: null, birth: '1978-11-03', address: 'Rua Consolação, 789' },
            { id: 4, name: 'Pedro Oliveira', phone: '(11) 66666-6666', email: 'pedro@email.com', status: 'inactive', nextAppointment: null, birth: '1995-01-18', address: 'Rua Augusta, 321' },
            { id: 5, name: 'Carla Souza', phone: '(11) 55555-5555', email: 'carla@email.com', status: 'active', nextAppointment: '18/06/2025 16:00', birth: '1988-09-12', address: 'Rua Oscar Freire, 654' }
        ];

        this.services = [
            { id: 1, name: 'Limpeza e Profilaxia', description: 'Limpeza completa com remoção de tártaro e polimento', price: 120.00, duration: 60, category: 'preventivo' },
            { id: 2, name: 'Restauração em Resina', description: 'Restauração estética em resina composta', price: 180.00, duration: 90, category: 'restaurador' },
            { id: 3, name: 'Clareamento Dental', description: 'Clareamento a laser para dentes mais brancos', price: 450.00, duration: 120, category: 'estetico' },
            { id: 4, name: 'Extração de Siso', description: 'Remoção cirúrgica do dente do siso', price: 350.00, duration: 60, category: 'cirurgico' },
            { id: 5, name: 'Manutenção Ortodôntica', description: 'Ajuste e manutenção do aparelho ortodôntico', price: 200.00, duration: 45, category: 'ortodontico' },
            { id: 6, name: 'Implante Dentário', description: 'Implante unitário com coroa em porcelana', price: 2500.00, duration: 180, category: 'cirurgico' }
        ];

        this.staff = [
            { id: 1, name: 'Dr. Carlos Mendoza', role: 'dentista', specialty: 'Periodontia', phone: '(11) 99111-1111', email: 'carlos@dentalspa.com' },
            { id: 2, name: 'Dra. Fernanda Lima', role: 'dentista', specialty: 'Ortodontia', phone: '(11) 99222-2222', email: 'fernanda@dentalspa.com' },
            { id: 3, name: 'Ana Paula Rocha', role: 'higienista', specialty: 'Prevenção', phone: '(11) 99333-3333', email: 'ana.paula@dentalspa.com' },
            { id: 4, name: 'Marcos Ferreira', role: 'assistente', specialty: 'Cirurgia', phone: '(11) 99444-4444', email: 'marcos@dentalspa.com' },
            { id: 5, name: 'Julia Santos', role: 'recepcionista', specialty: 'Atendimento', phone: '(11) 99555-5555', email: 'julia@dentalspa.com' }
        ];

        this.appointments = [
            { id: 1, patientId: 1, patientName: 'Maria Silva', serviceId: 1, serviceName: 'Limpeza e Profilaxia', date: '2025-06-15', time: '14:00', status: 'confirmed', notes: 'Paciente com sensibilidade' },
            { id: 2, patientId: 2, patientName: 'João Santos', serviceId: 3, serviceName: 'Clareamento Dental', date: '2025-06-16', time: '09:30', status: 'scheduled', notes: '' },
            { id: 3, patientId: 5, patientName: 'Carla Souza', serviceId: 2, serviceName: 'Restauração em Resina', date: '2025-06-18', time: '16:00', status: 'confirmed', notes: 'Dente 14' },
            { id: 4, patientId: 3, patientName: 'Ana Costa', serviceId: 1, serviceName: 'Limpeza e Profilaxia', date: '2025-06-20', time: '11:00', status: 'scheduled', notes: '' }
        ];

        this.transactions = [
            { id: 1, description: 'Pagamento - Limpeza Maria Silva', amount: 120.00, type: 'income', category: 'consulta', date: '2025-06-10' },
            { id: 2, description: 'Compra materiais odontológicos', amount: -450.00, type: 'expense', category: 'material', date: '2025-06-09' },
            { id: 3, description: 'Pagamento - Clareamento João Santos', amount: 450.00, type: 'income', category: 'consulta', date: '2025-06-08' },
            { id: 4, description: 'Energia elétrica', amount: -280.00, type: 'expense', category: 'utilidades', date: '2025-06-07' },
            { id: 5, description: 'Pagamento - Restauração Carla Souza', amount: 180.00, type: 'income', category: 'consulta', date: '2025-06-06' }
        ];
    }

    setupNavigation() {
        const navLinks = document.querySelectorAll('.nav-link');
        
        navLinks.forEach(link => {
            link.addEventListener('click', (e) => {
                e.preventDefault();
                const page = link.dataset.page;
                this.navigateTo(page);
            });
        });
    }

    navigateTo(page) {
        // Hide all pages
        const pages = document.querySelectorAll('.page');
        pages.forEach(p => p.classList.add('hidden'));

        // Show target page
        const targetPage = document.getElementById(`${page}-page`);
        if (targetPage) {
            targetPage.classList.remove('hidden');
        }

        // Update active nav link
        const navLinks = document.querySelectorAll('.nav-link');
        navLinks.forEach(link => link.classList.remove('active'));
        
        const activeLink = document.querySelector(`[data-page="${page}"]`);
        if (activeLink) {
            activeLink.classList.add('active');
        }

        this.currentPage = page;
        this.loadPageData(page);
    }

    loadPageData(page) {
        switch(page) {
            case 'dashboard':
                this.loadDashboardData();
                break;
            case 'patients':
                this.loadPatientsData();
                break;
            case 'appointments':
                this.loadAppointmentsData();
                break;
            case 'agenda':
                this.loadAgendaData();
                break;
            case 'services':
                this.loadServicesData();
                break;
            case 'staff':
                this.loadStaffData();
                break;
            case 'financial':
                this.loadFinancialData();
                break;
            case 'inventory':
                this.loadInventoryData();
                break;
            case 'before-after':
                this.loadBeforeAfterData();
                break;
            case 'learning':
                this.loadLearningData();
                break;
            case 'analytics':
                this.loadAnalyticsData();
                break;
            case 'clinic-info':
                this.loadClinicInfoData();
                break;
            case 'whatsapp':
                this.loadWhatsAppData();
                break;
            case 'subscriptions':
                this.loadSubscriptionsData();
                break;
            default:
                break;
        }
    }

    async loadDashboardData() {
        try {
            // Load stats from API
            const statsResponse = await fetch('/api/dashboard/stats');
            if (statsResponse.ok) {
                const stats = await statsResponse.json();
                this.updateDashboardStats(stats);
            }

            // Load recent appointments
            const appointmentsResponse = await fetch('/api/appointments/today');
            if (appointmentsResponse.ok) {
                const appointments = await appointmentsResponse.json();
                this.updateRecentAppointments(appointments);
            }
        } catch (error) {
            console.log('Using demo data for dashboard');
            // Keep current demo data displayed
        }
    }

    updateDashboardStats(stats) {
        document.getElementById('total-patients').textContent = stats.totalPatients || '156';
        document.getElementById('today-appointments').textContent = stats.todayAppointments || '8';
        document.getElementById('monthly-revenue').textContent = `R$ ${(stats.monthlyRevenue || 15750).toLocaleString('pt-BR', {minimumFractionDigits: 2})}`;
        document.getElementById('pending-appointments').textContent = stats.pendingAppointments || '3';
    }

    updateRecentAppointments(appointments) {
        const container = document.getElementById('recent-appointments');
        if (!appointments || appointments.length === 0) {
            container.innerHTML = '<p>Nenhum agendamento para hoje</p>';
            return;
        }

        const appointmentsHtml = appointments.map(apt => `
            <div class="appointment-item">
                <div>
                    <div class="patient-name">${apt.patient_name || apt.name}</div>
                    <div class="appointment-time">${this.formatTime(apt.appointment_date)} - ${apt.service_name || apt.service}</div>
                </div>
                <span class="status-badge ${apt.status}">${this.getStatusLabel(apt.status)}</span>
            </div>
        `).join('');

        container.innerHTML = appointmentsHtml;
    }

    async loadPatientsData() {
        const container = document.getElementById('patients-list');
        if (!container) return;

        try {
            const response = await fetch('/api/patients');
            if (response.ok) {
                const patients = await response.json();
                this.displayPatients(patients);
            } else {
                this.displayDemoPatients();
            }
        } catch (error) {
            this.displayDemoPatients();
        }
    }

    displayPatients(patients) {
        const container = document.getElementById('patients-list');
        if (!patients || patients.length === 0) {
            container.innerHTML = '<p>Nenhum paciente cadastrado</p>';
            return;
        }

        const patientsHtml = patients.map(patient => `
            <div class="appointment-item">
                <div>
                    <div class="patient-name">${patient.name}</div>
                    <div class="appointment-time">${patient.email} | ${patient.phone}</div>
                </div>
            </div>
        `).join('');

        container.innerHTML = patientsHtml;
    }

    displayDemoPatients() {
        const container = document.getElementById('patients-list');
        const demoPatients = [
            { name: 'Maria Silva', email: 'maria@email.com', phone: '(11) 99999-9999' },
            { name: 'João Santos', email: 'joao@email.com', phone: '(11) 88888-8888' },
            { name: 'Ana Costa', email: 'ana@email.com', phone: '(11) 77777-7777' }
        ];
        
        this.displayPatients(demoPatients);
    }

    async loadAppointmentsData() {
        const container = document.getElementById('appointments-list');
        if (!container) return;

        try {
            const response = await fetch('/api/appointments');
            if (response.ok) {
                const appointments = await response.json();
                this.displayAppointments(appointments);
            } else {
                this.displayDemoAppointments();
            }
        } catch (error) {
            this.displayDemoAppointments();
        }
    }

    displayAppointments(appointments) {
        const container = document.getElementById('appointments-list');
        if (!appointments || appointments.length === 0) {
            container.innerHTML = '<p>Nenhum agendamento encontrado</p>';
            return;
        }

        const appointmentsHtml = appointments.map(apt => `
            <div class="appointment-item">
                <div>
                    <div class="patient-name">${apt.patient_name}</div>
                    <div class="appointment-time">${this.formatDateTime(apt.appointment_date)} - ${apt.service_name}</div>
                </div>
                <span class="status-badge ${apt.status}">${this.getStatusLabel(apt.status)}</span>
            </div>
        `).join('');

        container.innerHTML = appointmentsHtml;
    }

    displayDemoAppointments() {
        const container = document.getElementById('appointments-list');
        const demoAppointments = [
            { 
                patient_name: 'Maria Silva', 
                appointment_date: new Date().toISOString(), 
                service_name: 'Limpeza Dental', 
                status: 'confirmed' 
            },
            { 
                patient_name: 'João Santos', 
                appointment_date: new Date(Date.now() + 3600000).toISOString(), 
                service_name: 'Consulta', 
                status: 'scheduled' 
            }
        ];
        
        this.displayAppointments(demoAppointments);
    }

    async loadServicesData() {
        const container = document.getElementById('services-list');
        if (!container) return;

        try {
            const response = await fetch('/api/services');
            if (response.ok) {
                const services = await response.json();
                this.displayServices(services);
            } else {
                this.displayDemoServices();
            }
        } catch (error) {
            this.displayDemoServices();
        }
    }

    displayServices(services) {
        const container = document.getElementById('services-list');
        if (!services || services.length === 0) {
            container.innerHTML = '<p>Nenhum serviço cadastrado</p>';
            return;
        }

        const servicesHtml = services.map(service => `
            <div class="appointment-item">
                <div>
                    <div class="patient-name">${service.name}</div>
                    <div class="appointment-time">${service.description} - ${service.duration} min</div>
                </div>
                <div style="font-weight: 600; color: #059669;">R$ ${service.price.toFixed(2)}</div>
            </div>
        `).join('');

        container.innerHTML = servicesHtml;
    }

    displayDemoServices() {
        const container = document.getElementById('services-list');
        const demoServices = [
            { name: 'Limpeza Dental', description: 'Profilaxia completa', duration: 60, price: 80.00 },
            { name: 'Clareamento', description: 'Clareamento profissional', duration: 90, price: 450.00 },
            { name: 'Restauração', description: 'Restauração em resina', duration: 45, price: 120.00 }
        ];
        
        this.displayServices(demoServices);
    }

    async loadStaffData() {
        const container = document.getElementById('staff-list');
        if (!container) return;

        try {
            const response = await fetch('/api/staff');
            if (response.ok) {
                const staff = await response.json();
                this.displayStaff(staff);
            } else {
                this.displayDemoStaff();
            }
        } catch (error) {
            this.displayDemoStaff();
        }
    }

    displayStaff(staff) {
        const container = document.getElementById('staff-list');
        if (!staff || staff.length === 0) {
            container.innerHTML = '<p>Nenhum profissional cadastrado</p>';
            return;
        }

        const staffHtml = staff.map(member => `
            <div class="appointment-item">
                <div>
                    <div class="patient-name">${member.name}</div>
                    <div class="appointment-time">${member.role} - ${member.specialty}</div>
                </div>
            </div>
        `).join('');

        container.innerHTML = staffHtml;
    }

    displayDemoStaff() {
        const container = document.getElementById('staff-list');
        const demoStaff = [
            { name: 'Dr. João Silva', role: 'Dentista', specialty: 'Clínica Geral' },
            { name: 'Dra. Ana Costa', role: 'Dentista', specialty: 'Ortodontia' },
            { name: 'Maria Santos', role: 'Auxiliar', specialty: 'Higienização' }
        ];
        
        this.displayStaff(demoStaff);
    }

    formatTime(dateString) {
        const date = new Date(dateString);
        return date.toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' });
    }

    formatDateTime(dateString) {
        const date = new Date(dateString);
        return date.toLocaleString('pt-BR', { 
            day: '2-digit', 
            month: '2-digit', 
            year: 'numeric',
            hour: '2-digit', 
            minute: '2-digit' 
        });
    }

    getStatusLabel(status) {
        const labels = {
            'scheduled': 'Agendado',
            'confirmed': 'Confirmado',
            'completed': 'Concluído',
            'cancelled': 'Cancelado',
            'active': 'Ativo',
            'inactive': 'Inativo'
        };
        return labels[status] || status;
    }

    // Load all module data functions
    loadPatientsData() {
        this.displayPatients(this.patients);
    }

    displayPatients(patients) {
        const patientsContainer = document.getElementById('patients-list');
        if (!patientsContainer) return;
        
        patientsContainer.innerHTML = `
            <div class="patients-table">
                <div class="table-header">
                    <div>Nome</div>
                    <div>Telefone</div>
                    <div>Email</div>
                    <div>Próxima Consulta</div>
                    <div>Status</div>
                    <div>Ações</div>
                </div>
                ${patients.map(patient => `
                    <div class="table-row">
                        <div class="patient-name">
                            <div class="patient-avatar">${patient.name.charAt(0).toUpperCase()}</div>
                            <div>
                                <div class="name">${patient.name}</div>
                                <div class="id">ID: ${patient.id}</div>
                            </div>
                        </div>
                        <div>${patient.phone}</div>
                        <div>${patient.email || '-'}</div>
                        <div>${patient.nextAppointment || 'Sem agendamento'}</div>
                        <div><span class="status-badge ${patient.status}">${this.getStatusLabel(patient.status)}</span></div>
                        <div class="actions">
                            <button class="btn-icon" onclick="app.editPatient(${patient.id})" title="Editar">✏️</button>
                            <button class="btn-icon" onclick="app.viewPatientHistory(${patient.id})" title="Histórico">📋</button>
                            <button class="btn-icon" onclick="app.scheduleAppointment(${patient.id})" title="Agendar">📅</button>
                        </div>
                    </div>
                `).join('')}
            </div>
        `;
    }

    loadAppointmentsData() {
        this.displayAppointments(this.appointments);
    }

    displayAppointments(appointments) {
        const appointmentsContainer = document.getElementById('appointments-list');
        if (!appointmentsContainer) return;
        
        appointmentsContainer.innerHTML = `
            <div class="appointments-table">
                <div class="table-header">
                    <div>Paciente</div>
                    <div>Serviço</div>
                    <div>Data</div>
                    <div>Horário</div>
                    <div>Status</div>
                    <div>Ações</div>
                </div>
                ${appointments.map(appointment => `
                    <div class="table-row">
                        <div class="patient-name">
                            <div class="patient-avatar">${appointment.patientName.charAt(0).toUpperCase()}</div>
                            <div>
                                <div class="name">${appointment.patientName}</div>
                                <div class="id">ID: ${appointment.patientId}</div>
                            </div>
                        </div>
                        <div>${appointment.serviceName}</div>
                        <div>${this.formatDate(appointment.date)}</div>
                        <div>${appointment.time}</div>
                        <div><span class="status-badge ${appointment.status}">${this.getStatusLabel(appointment.status)}</span></div>
                        <div class="actions">
                            <button class="btn-icon" onclick="app.editAppointment(${appointment.id})" title="Editar">✏️</button>
                            <button class="btn-icon" onclick="app.confirmAppointment(${appointment.id})" title="Confirmar">✅</button>
                            <button class="btn-icon" onclick="app.cancelAppointment(${appointment.id})" title="Cancelar">❌</button>
                        </div>
                    </div>
                `).join('')}
            </div>
        `;
    }

    loadServicesData() {
        this.displayServices(this.services);
    }

    displayServices(services) {
        const servicesContainer = document.getElementById('services-grid');
        if (!servicesContainer) return;
        
        servicesContainer.innerHTML = services.map(service => `
            <div class="service-card">
                <div class="service-header">
                    <div>
                        <h4>${service.name}</h4>
                        <span class="service-category">${service.category}</span>
                    </div>
                    <div class="service-price">R$ ${service.price.toFixed(2)}</div>
                </div>
                <div class="service-description">${service.description}</div>
                <div class="service-details">
                    <span>⏱️ ${service.duration} min</span>
                    <span>📋 ${service.category}</span>
                </div>
                <div class="actions" style="margin-top: 1rem;">
                    <button class="btn-icon" onclick="app.editService(${service.id})" title="Editar">✏️</button>
                    <button class="btn-icon" onclick="app.deleteService(${service.id})" title="Excluir">🗑️</button>
                </div>
            </div>
        `).join('');
    }

    loadStaffData() {
        this.displayStaff(this.staff);
    }

    displayStaff(staff) {
        const staffContainer = document.getElementById('staff-grid');
        if (!staffContainer) return;
        
        staffContainer.innerHTML = staff.map(member => `
            <div class="staff-card">
                <div class="staff-avatar">${member.name.charAt(0).toUpperCase()}</div>
                <div class="staff-name">${member.name}</div>
                <div class="staff-role">${this.getRoleLabel(member.role)}</div>
                <div class="staff-specialty">${member.specialty}</div>
                <div class="staff-contact">
                    <div>📞 ${member.phone}</div>
                    <div>📧 ${member.email}</div>
                </div>
                <div class="actions" style="margin-top: 1rem;">
                    <button class="btn-icon" onclick="app.editStaff(${member.id})" title="Editar">✏️</button>
                    <button class="btn-icon" onclick="app.viewStaffSchedule(${member.id})" title="Agenda">📅</button>
                </div>
            </div>
        `).join('');
    }

    loadFinancialData() {
        this.displayFinancialSummary();
        this.displayTransactions(this.transactions);
    }

    displayFinancialSummary() {
        const revenue = this.transactions.filter(t => t.type === 'income').reduce((sum, t) => sum + t.amount, 0);
        const expenses = this.transactions.filter(t => t.type === 'expense').reduce((sum, t) => sum + Math.abs(t.amount), 0);
        const netProfit = revenue - expenses;

        document.getElementById('monthly-revenue').textContent = `R$ ${revenue.toFixed(2)}`;
        document.getElementById('monthly-expenses').textContent = `R$ ${expenses.toFixed(2)}`;
        document.getElementById('net-profit').textContent = `R$ ${netProfit.toFixed(2)}`;
        document.getElementById('pending-payments').textContent = `R$ 0,00`;

        // Update net profit color
        const netProfitElement = document.getElementById('net-profit');
        netProfitElement.className = `amount ${netProfit >= 0 ? 'positive' : 'negative'}`;
    }

    displayTransactions(transactions) {
        const transactionsContainer = document.getElementById('transactions-list');
        if (!transactionsContainer) return;
        
        transactionsContainer.innerHTML = transactions.map(transaction => `
            <div class="transaction-item">
                <div class="transaction-info">
                    <h4>${transaction.description}</h4>
                    <div class="transaction-category">${transaction.category} • ${this.formatDate(transaction.date)}</div>
                </div>
                <div class="transaction-amount ${transaction.type}">
                    ${transaction.type === 'income' ? '+' : ''}R$ ${Math.abs(transaction.amount).toFixed(2)}
                </div>
            </div>
        `).join('');
    }

    loadAgendaData() {
        this.generateCalendar();
    }

    generateCalendar() {
        const calendarContainer = document.getElementById('calendar-grid');
        if (!calendarContainer) return;

        const timeSlots = ['08:00', '09:00', '10:00', '11:00', '14:00', '15:00', '16:00', '17:00'];
        const weekDays = ['Seg', 'Ter', 'Qua', 'Qui', 'Sex', 'Sáb'];
        
        let calendarHTML = '<div class="time-slot"></div>';
        weekDays.forEach(day => {
            calendarHTML += `<div class="day-header">${day}</div>`;
        });

        timeSlots.forEach(time => {
            calendarHTML += `<div class="time-slot">${time}</div>`;
            weekDays.forEach((day, index) => {
                const hasAppointment = Math.random() > 0.7; // Random appointments for demo
                calendarHTML += `
                    <div class="appointment-slot" onclick="app.openAppointmentModal()">
                        ${hasAppointment ? '<div class="appointment-block">Consulta</div>' : ''}
                    </div>
                `;
            });
        });

        calendarContainer.innerHTML = calendarHTML;
    }

    // Stub functions for other modules
    loadInventoryData() {
        const container = document.querySelector('#inventory-page .card');
        if (container) {
            container.innerHTML = `
                <div class="card-header">
                    <h3>Gestão de Estoque</h3>
                    <button class="btn btn-primary">+ Novo Item</button>
                </div>
                <div class="empty-state">
                    <h3>Estoque em Desenvolvimento</h3>
                    <p>Módulo de controle de materiais odontológicos em breve.</p>
                </div>
            `;
        }
    }

    loadBeforeAfterData() {
        const container = document.querySelector('#before-after-page .card');
        if (container) {
            container.innerHTML = `
                <div class="card-header">
                    <h3>Galeria Antes e Depois</h3>
                    <button class="btn btn-primary">+ Nova Foto</button>
                </div>
                <div class="empty-state">
                    <h3>Galeria em Desenvolvimento</h3>
                    <p>Sistema de upload e organização de fotos em breve.</p>
                </div>
            `;
        }
    }

    loadLearningData() {
        const container = document.querySelector('#learning-page .card');
        if (container) {
            container.innerHTML = `
                <div class="card-header">
                    <h3>Centro de Aprendizado</h3>
                    <button class="btn btn-primary">+ Novo Curso</button>
                </div>
                <div class="empty-state">
                    <h3>Plataforma de Ensino em Desenvolvimento</h3>
                    <p>Cursos e treinamentos para a equipe em breve.</p>
                </div>
            `;
        }
    }

    loadAnalyticsData() {
        const container = document.querySelector('#analytics-page .card');
        if (container) {
            container.innerHTML = `
                <div class="card-header">
                    <h3>Analytics e Relatórios</h3>
                    <button class="btn btn-primary">Gerar Relatório</button>
                </div>
                <div class="empty-state">
                    <h3>Analytics em Desenvolvimento</h3>
                    <p>Dashboard com métricas avançadas em breve.</p>
                </div>
            `;
        }
    }

    loadClinicInfoData() {
        const container = document.querySelector('#clinic-info-page .card');
        if (container) {
            container.innerHTML = `
                <div class="card-header">
                    <h3>Informações da Clínica</h3>
                    <button class="btn btn-primary">Editar</button>
                </div>
                <div class="clinic-info">
                    <h4>DentalSpa Clínica Odontológica</h4>
                    <p>📍 Rua das Flores, 123 - Centro, São Paulo - SP</p>
                    <p>📞 (11) 3333-4444</p>
                    <p>📧 contato@dentalspa.com.br</p>
                    <p>🕒 Segunda a Sexta: 8h às 18h | Sábado: 8h às 12h</p>
                </div>
            `;
        }
    }

    loadWhatsAppData() {
        const container = document.querySelector('#whatsapp-page .card');
        if (container) {
            container.innerHTML = `
                <div class="card-header">
                    <h3>WhatsApp Business</h3>
                    <button class="btn btn-primary">Configurar</button>
                </div>
                <div class="empty-state">
                    <h3>Integração WhatsApp em Desenvolvimento</h3>
                    <p>Sistema de mensagens automáticas em breve.</p>
                </div>
            `;
        }
    }

    loadSubscriptionsData() {
        const container = document.querySelector('#subscriptions-page .card');
        if (container) {
            container.innerHTML = `
                <div class="card-header">
                    <h3>Planos de Assinatura</h3>
                    <button class="btn btn-primary">+ Novo Plano</button>
                </div>
                <div class="empty-state">
                    <h3>Sistema de Assinaturas em Desenvolvimento</h3>
                    <p>Planos mensais e anuais para pacientes em breve.</p>
                </div>
            `;
        }
    }

    // Utility functions
    formatDate(dateString) {
        const date = new Date(dateString);
        return date.toLocaleDateString('pt-BR');
    }

    formatDateTime(dateString) {
        const date = new Date(dateString);
        return date.toLocaleString('pt-BR');
    }

    getRoleLabel(role) {
        const labels = {
            'dentista': 'Dentista',
            'assistente': 'Assistente',
            'recepcionista': 'Recepcionista',
            'higienista': 'Higienista',
            'auxiliar': 'Auxiliar'
        };
        return labels[role] || role;
    }

    // Modal functions
    editPatient(id) {
        console.log('Editing patient:', id);
        // Implementation for editing patient
    }

    viewPatientHistory(id) {
        console.log('Viewing patient history:', id);
        // Implementation for viewing patient history
    }

    scheduleAppointment(patientId) {
        this.openAppointmentModal();
        if (patientId) {
            document.getElementById('appointment-patient').value = patientId;
        }
    }

    editAppointment(id) {
        console.log('Editing appointment:', id);
        // Implementation for editing appointment
    }

    confirmAppointment(id) {
        const appointment = this.appointments.find(a => a.id === id);
        if (appointment) {
            appointment.status = 'confirmed';
            this.loadAppointmentsData();
        }
    }

    cancelAppointment(id) {
        const appointment = this.appointments.find(a => a.id === id);
        if (appointment) {
            appointment.status = 'cancelled';
            this.loadAppointmentsData();
        }
    }

    editService(id) {
        console.log('Editing service:', id);
        // Implementation for editing service
    }

    deleteService(id) {
        if (confirm('Tem certeza que deseja excluir este serviço?')) {
            this.services = this.services.filter(s => s.id !== id);
            this.loadServicesData();
        }
    }

    editStaff(id) {
        console.log('Editing staff member:', id);
        // Implementation for editing staff
    }

    viewStaffSchedule(id) {
        console.log('Viewing staff schedule:', id);
        // Implementation for viewing staff schedule
    }
}

// Global functions for modals
function openPatientModal() {
    document.getElementById('patient-modal').classList.add('show');
}

function openAppointmentModal() {
    document.getElementById('appointment-modal').classList.add('show');
    // Populate patient and service dropdowns
    populatePatientDropdown();
    populateServiceDropdown();
}

function openServiceModal() {
    document.getElementById('service-modal').classList.add('show');
}

function openStaffModal() {
    document.getElementById('staff-modal').classList.add('show');
}

function openTransactionModal(type) {
    const modal = document.getElementById('transaction-modal');
    const title = document.getElementById('transaction-modal-title');
    const categorySelect = document.getElementById('transaction-category');
    
    document.getElementById('transaction-type').value = type;
    title.textContent = type === 'income' ? 'Nova Receita' : 'Nova Despesa';
    
    // Populate categories based on type
    const incomeCategories = ['consulta', 'procedimento', 'produto', 'outros'];
    const expenseCategories = ['material', 'equipamento', 'utilidades', 'salario', 'outros'];
    const categories = type === 'income' ? incomeCategories : expenseCategories;
    
    categorySelect.innerHTML = '<option value="">Selecione uma categoria</option>' +
        categories.map(cat => `<option value="${cat}">${cat}</option>`).join('');
    
    modal.classList.add('show');
}

function closeModal(modalId) {
    document.getElementById(modalId).classList.remove('show');
}

function populatePatientDropdown() {
    const select = document.getElementById('appointment-patient');
    if (app && app.patients) {
        select.innerHTML = '<option value="">Selecione um paciente</option>' +
            app.patients.map(p => `<option value="${p.id}">${p.name}</option>`).join('');
    }
}

function populateServiceDropdown() {
    const select = document.getElementById('appointment-service');
    if (app && app.services) {
        select.innerHTML = '<option value="">Selecione um serviço</option>' +
            app.services.map(s => `<option value="${s.id}">${s.name} - R$ ${s.price.toFixed(2)}</option>`).join('');
    }
}

function savePatient() {
    const form = document.getElementById('patient-form');
    const formData = new FormData(form);
    
    const newPatient = {
        id: app.patients.length + 1,
        name: document.getElementById('patient-name').value,
        phone: document.getElementById('patient-phone').value,
        email: document.getElementById('patient-email').value,
        birth: document.getElementById('patient-birth').value,
        address: document.getElementById('patient-address').value,
        status: 'active',
        nextAppointment: null
    };
    
    app.patients.push(newPatient);
    app.loadPatientsData();
    closeModal('patient-modal');
    form.reset();
}

function saveAppointment() {
    const newAppointment = {
        id: app.appointments.length + 1,
        patientId: parseInt(document.getElementById('appointment-patient').value),
        patientName: app.patients.find(p => p.id === parseInt(document.getElementById('appointment-patient').value))?.name || '',
        serviceId: parseInt(document.getElementById('appointment-service').value),
        serviceName: app.services.find(s => s.id === parseInt(document.getElementById('appointment-service').value))?.name || '',
        date: document.getElementById('appointment-date').value,
        time: document.getElementById('appointment-time').value,
        notes: document.getElementById('appointment-notes').value,
        status: 'scheduled'
    };
    
    app.appointments.push(newAppointment);
    app.loadAppointmentsData();
    closeModal('appointment-modal');
    document.getElementById('appointment-form').reset();
}

function saveService() {
    const newService = {
        id: app.services.length + 1,
        name: document.getElementById('service-name').value,
        description: document.getElementById('service-description').value,
        price: parseFloat(document.getElementById('service-price').value),
        duration: parseInt(document.getElementById('service-duration').value),
        category: document.getElementById('service-category').value
    };
    
    app.services.push(newService);
    app.loadServicesData();
    closeModal('service-modal');
    document.getElementById('service-form').reset();
}

function saveStaff() {
    const newStaff = {
        id: app.staff.length + 1,
        name: document.getElementById('staff-name').value,
        role: document.getElementById('staff-role').value,
        specialty: document.getElementById('staff-specialty').value,
        phone: document.getElementById('staff-phone').value,
        email: document.getElementById('staff-email').value
    };
    
    app.staff.push(newStaff);
    app.loadStaffData();
    closeModal('staff-modal');
    document.getElementById('staff-form').reset();
}

function saveTransaction() {
    const type = document.getElementById('transaction-type').value;
    const amount = parseFloat(document.getElementById('transaction-amount').value);
    
    const newTransaction = {
        id: app.transactions.length + 1,
        description: document.getElementById('transaction-description').value,
        amount: type === 'expense' ? -amount : amount,
        type: type,
        category: document.getElementById('transaction-category').value,
        date: document.getElementById('transaction-date').value
    };
    
    app.transactions.push(newTransaction);
    app.loadFinancialData();
    closeModal('transaction-modal');
    document.getElementById('transaction-form').reset();
}

function filterPatients() {
    const searchTerm = document.getElementById('patient-search').value.toLowerCase();
    const filteredPatients = app.patients.filter(patient => 
        patient.name.toLowerCase().includes(searchTerm) ||
        patient.phone.includes(searchTerm) ||
        patient.email.toLowerCase().includes(searchTerm)
    );
    app.displayPatients(filteredPatients);
}

function filterAppointments() {
    const statusFilter = document.getElementById('status-filter').value;
    const dateFilter = document.getElementById('date-filter').value;
    
    let filteredAppointments = app.appointments;
    
    if (statusFilter) {
        filteredAppointments = filteredAppointments.filter(apt => apt.status === statusFilter);
    }
    
    if (dateFilter) {
        filteredAppointments = filteredAppointments.filter(apt => apt.date === dateFilter);
    }
    
    app.displayAppointments(filteredAppointments);
}

function previousWeek() {
    app.currentWeek.setDate(app.currentWeek.getDate() - 7);
    app.generateCalendar();
}

function nextWeek() {
    app.currentWeek.setDate(app.currentWeek.getDate() + 7);
    app.generateCalendar();
}

// Global app instance
let app;

// Initialize app when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    app = new DentalSpaApp();
});