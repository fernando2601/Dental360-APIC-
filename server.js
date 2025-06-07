const express = require('express');
const path = require('path');
const app = express();
const port = process.env.PORT || 4200;

// Middleware
app.use(express.json());
app.use(express.static(path.join(__dirname, 'src')));

// API endpoints for the .NET backend integration
app.get('/api/database/status', (req, res) => {
  res.json({
    PostgreSQL: {
      Available: true,
      IsPrimary: true,
      ConnectionString: "PostgreSQL Connected"
    },
    SqlServer: {
      Available: false,
      IsPrimary: false,
      ConnectionString: "Not configured"
    },
    Recommendations: ["Sistema funcionando com PostgreSQL"]
  });
});

app.get('/api/dashboard/stats', (req, res) => {
  res.json({
    totalClients: 150,
    appointmentsToday: 8,
    monthlyRevenue: 45000,
    totalServices: 25
  });
});

app.get('/api/appointments/upcoming', (req, res) => {
  res.json([
    {
      id: 1,
      clientName: "Maria Silva",
      serviceName: "Limpeza",
      dateTime: new Date().toISOString(),
      status: "Confirmado"
    },
    {
      id: 2,
      clientName: "João Santos",
      serviceName: "Clareamento",
      dateTime: new Date(Date.now() + 3600000).toISOString(),
      status: "Pendente"
    }
  ]);
});

// Handle Angular routing
app.get('/', (req, res) => {
  res.sendFile(path.join(__dirname, 'src', 'index.html'));
});

app.listen(port, '0.0.0.0', () => {
  console.log(`DentalSpa Angular server running on http://0.0.0.0:${port}`);
  console.log('Frontend Angular restaurado com sucesso!');
});