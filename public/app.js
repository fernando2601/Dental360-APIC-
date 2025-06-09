// DentalSpa Application JavaScript
class DentalSpaApp {
    constructor() {
        this.currentPage = 'dashboard';
        this.init();
    }

    init() {
        this.setupNavigation();
        this.loadDashboardData();
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
            case 'services':
                this.loadServicesData();
                break;
            case 'staff':
                this.loadStaffData();
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
            'cancelled': 'Cancelado'
        };
        return labels[status] || status;
    }
}

// Initialize app when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    new DentalSpaApp();
});