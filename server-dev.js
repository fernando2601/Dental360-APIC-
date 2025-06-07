const express = require('express');
const path = require('path');
const { createProxyMiddleware } = require('http-proxy-middleware');

const app = express();
const PORT = process.env.PORT || 4200;

// Serve static files from src directory for development
app.use(express.static(path.join(__dirname, 'src')));

// Basic API routes for development
app.get('/api/health', (req, res) => {
  res.json({ status: 'OK', message: 'DentalSpa API is running' });
});

// Mock authentication endpoint
app.post('/api/auth/login', (req, res) => {
  res.json({ 
    token: 'mock-jwt-token',
    user: { id: 1, name: 'Admin', role: 'administrator' }
  });
});

// Mock data endpoints
app.get('/api/dashboard/stats', (req, res) => {
  res.json({
    totalPatients: 245,
    todayAppointments: 12,
    monthlyRevenue: 85400,
    completedTreatments: 89
  });
});

app.get('/api/clients', (req, res) => {
  res.json([
    { id: 1, name: 'Maria Silva', phone: '(11) 99999-9999', email: 'maria@email.com' },
    { id: 2, name: 'João Santos', phone: '(11) 88888-8888', email: 'joao@email.com' }
  ]);
});

// Serve index.html for all routes (SPA support)
app.get('*', (req, res) => {
  res.sendFile(path.join(__dirname, 'src', 'index.html'));
});

app.listen(PORT, '0.0.0.0', () => {
  console.log(`🦷 DentalSpa Development Server running on http://0.0.0.0:${PORT}`);
  console.log(`📱 Frontend: Angular application`);
  console.log(`🔧 Backend: Mock API endpoints`);
});