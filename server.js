const express = require('express');
const cors = require('cors');
const path = require('path');

const app = express();
const PORT = process.env.PORT || 5000;

// Middleware
app.use(cors());
app.use(express.json());
app.use(express.urlencoded({ extended: true }));

// Health check endpoint
app.get('/api/health', (req, res) => {
  res.json({ 
    status: 'OK', 
    timestamp: new Date().toISOString(),
    message: 'API Backend funcionando corretamente'
  });
});

// Endpoints da API
app.get('/api/patients', (req, res) => {
  res.json([
    {
      id: 1,
      name: 'Maria Silva',
      phone: '(11) 99999-9999',
      email: 'maria@email.com',
      birthDate: '1985-03-15',
      address: 'Rua das Flores, 123',
      status: 'active'
    },
    {
      id: 2,
      name: 'João Santos',
      phone: '(11) 88888-8888',
      email: 'joao@email.com',
      birthDate: '1990-07-22',
      address: 'Av. Paulista, 456',
      status: 'active'
    }
  ]);
});

app.post('/api/patients', (req, res) => {
  const { name, phone, email, birthDate, address } = req.body;
  
  const newPatient = {
    id: Date.now(),
    name,
    phone,
    email,
    birthDate,
    address,
    status: 'active',
    createdAt: new Date().toISOString()
  };
  
  res.status(201).json(newPatient);
});

app.get('/api/appointments', (req, res) => {
  res.json([
    {
      id: 1,
      patientId: 1,
      patientName: 'Maria Silva',
      serviceId: 1,
      serviceName: 'Limpeza e Profilaxia',
      date: '2025-06-20',
      time: '14:00',
      status: 'scheduled',
      notes: 'Paciente com sensibilidade'
    },
    {
      id: 2,
      patientId: 2,
      patientName: 'João Santos',
      serviceId: 2,
      serviceName: 'Clareamento Dental',
      date: '2025-06-21',
      time: '09:30',
      status: 'confirmed',
      notes: ''
    }
  ]);
});

app.post('/api/appointments', (req, res) => {
  const { patientId, serviceId, date, time, notes } = req.body;
  
  const newAppointment = {
    id: Date.now(),
    patientId,
    serviceId,
    date,
    time,
    notes,
    status: 'scheduled',
    createdAt: new Date().toISOString()
  };
  
  res.status(201).json(newAppointment);
});

app.get('/api/services', (req, res) => {
  res.json([
    {
      id: 1,
      name: 'Limpeza e Profilaxia',
      description: 'Limpeza completa com remoção de tártaro e polimento',
      price: 120.00,
      duration: 60,
      category: 'preventivo'
    },
    {
      id: 2,
      name: 'Clareamento Dental',
      description: 'Clareamento a laser para dentes mais brancos',
      price: 450.00,
      duration: 120,
      category: 'estetico'
    },
    {
      id: 3,
      name: 'Restauração em Resina',
      description: 'Restauração estética em resina composta',
      price: 180.00,
      duration: 90,
      category: 'restaurador'
    }
  ]);
});

app.post('/api/services', (req, res) => {
  const { name, description, price, duration, category } = req.body;
  
  const newService = {
    id: Date.now(),
    name,
    description,
    price: parseFloat(price),
    duration: parseInt(duration),
    category,
    createdAt: new Date().toISOString()
  };
  
  res.status(201).json(newService);
});

app.get('/api/staff', (req, res) => {
  res.json([
    {
      id: 1,
      name: 'Dr. Carlos Mendoza',
      role: 'dentista',
      specialty: 'Periodontia',
      phone: '(11) 99111-1111',
      email: 'carlos@dentalspa.com'
    },
    {
      id: 2,
      name: 'Dra. Fernanda Lima',
      role: 'dentista',
      specialty: 'Ortodontia',
      phone: '(11) 99222-2222',
      email: 'fernanda@dentalspa.com'
    }
  ]);
});

app.get('/api/dashboard/stats', (req, res) => {
  res.json({
    totalPatients: 156,
    todayAppointments: 8,
    monthlyRevenue: 15800.50,
    pendingAppointments: 3,
    completedAppointments: 142,
    activeServices: 12
  });
});

app.get('/api/financial/summary', (req, res) => {
  res.json({
    monthlyRevenue: 15800.50,
    monthlyExpenses: 8500.00,
    netProfit: 7300.50,
    pendingPayments: 2400.00
  });
});

app.get('/api/transactions', (req, res) => {
  res.json([
    {
      id: 1,
      description: 'Pagamento - Limpeza Maria Silva',
      amount: 120.00,
      type: 'income',
      category: 'consulta',
      date: '2025-06-18'
    },
    {
      id: 2,
      description: 'Compra materiais odontológicos',
      amount: -450.00,
      type: 'expense',
      category: 'material',
      date: '2025-06-17'
    }
  ]);
});

// Default route
app.get('/', (req, res) => {
  res.json({
    message: 'DentalSpa API Backend',
    version: '1.0.0',
    status: 'running',
    endpoints: [
      'GET /api/health',
      'GET /api/patients',
      'POST /api/patients',
      'GET /api/appointments',
      'POST /api/appointments',
      'GET /api/services',
      'POST /api/services',
      'GET /api/staff',
      'GET /api/dashboard/stats',
      'GET /api/financial/summary',
      'GET /api/transactions'
    ]
  });
});

// 404 handler
app.use((req, res) => {
  res.status(404).json({
    error: 'Endpoint não encontrado',
    path: req.path,
    method: req.method
  });
});

// Error handler
app.use((err, req, res, next) => {
  console.error(err.stack);
  res.status(500).json({
    error: 'Erro interno do servidor',
    message: err.message
  });
});

app.listen(PORT, '0.0.0.0', () => {
  console.log(`DentalSpa API Backend running on http://0.0.0.0:${PORT}`);
  console.log(`Environment: ${process.env.NODE_ENV || 'development'}`);
  console.log('API Endpoints disponíveis:');
  console.log('- GET /api/health');
  console.log('- GET /api/patients');
  console.log('- POST /api/patients');
  console.log('- GET /api/appointments');
  console.log('- POST /api/appointments');
  console.log('- GET /api/services');
  console.log('- GET /api/staff');
  console.log('- GET /api/dashboard/stats');
});