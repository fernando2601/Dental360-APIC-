const express = require('express');
const path = require('path');
const { Pool } = require('pg');
const cors = require('cors');

const app = express();
const port = process.env.PORT || 5000;

// Database connection
const pool = new Pool({
  connectionString: process.env.DATABASE_URL,
  ssl: process.env.NODE_ENV === 'production' ? { rejectUnauthorized: false } : false
});

// Middleware
app.use(cors());
app.use(express.json());

// Serve static files from public directory
app.use(express.static(path.join(__dirname, 'public')));

// Database status endpoint
app.get('/api/database/status', async (req, res) => {
  try {
    const result = await pool.query('SELECT NOW()');
    res.json({
      PostgreSQL: {
        Available: true,
        IsPrimary: true,
        ConnectionString: "PostgreSQL Connected",
        LastChecked: result.rows[0].now
      },
      SqlServer: {
        Available: false,
        IsPrimary: false,
        ConnectionString: "Not configured"
      },
      Recommendations: ["Sistema funcionando com PostgreSQL", "Banco de dados ativo e operacional"]
    });
  } catch (error) {
    res.status(500).json({
      PostgreSQL: {
        Available: false,
        IsPrimary: true,
        ConnectionString: "Connection failed"
      },
      error: error.message
    });
  }
});

// Dashboard stats endpoint
app.get('/api/dashboard/stats', async (req, res) => {
  try {
    const today = new Date().toISOString().split('T')[0];
    
    const [patientsResult, servicesResult, appointmentsResult] = await Promise.all([
      pool.query('SELECT COUNT(*) as count FROM patients'),
      pool.query('SELECT COUNT(*) as count FROM services WHERE is_active = true'),
      pool.query('SELECT COUNT(*) as count FROM appointments WHERE DATE(appointment_date) = $1', [today])
    ]);

    res.json({
      totalClients: parseInt(patientsResult.rows[0].count),
      appointmentsToday: parseInt(appointmentsResult.rows[0].count),
      monthlyRevenue: 45000, // This would need a financial table
      totalServices: parseInt(servicesResult.rows[0].count)
    });
  } catch (error) {
    console.error('Error fetching dashboard stats:', error);
    res.status(500).json({ error: 'Failed to fetch dashboard stats' });
  }
});

// Upcoming appointments endpoint
app.get('/api/appointments/upcoming', async (req, res) => {
  try {
    const query = `
      SELECT 
        a.id,
        p.name as patient_name,
        s.name as service_name,
        st.full_name as staff_name,
        a.appointment_date,
        a.status,
        a.notes,
        s.duration_minutes
      FROM appointments a
      JOIN patients p ON a.patient_id = p.id
      JOIN services s ON a.service_id = s.id
      JOIN staff st ON a.staff_id = st.id
      WHERE a.appointment_date >= NOW()
      ORDER BY a.appointment_date ASC
      LIMIT 10
    `;
    
    const result = await pool.query(query);
    res.json(result.rows);
  } catch (error) {
    console.error('Error fetching upcoming appointments:', error);
    res.status(500).json({ error: 'Failed to fetch upcoming appointments' });
  }
});

// Patients endpoints
app.get('/api/patients', async (req, res) => {
  try {
    const result = await pool.query('SELECT * FROM patients ORDER BY name ASC');
    res.json(result.rows);
  } catch (error) {
    console.error('Error fetching patients:', error);
    res.status(500).json({ error: 'Failed to fetch patients' });
  }
});

app.post('/api/patients', async (req, res) => {
  const { name, email, phone, birth_date, cpf, address, emergency_contact, medical_history, allergies } = req.body;
  try {
    const result = await pool.query(
      'INSERT INTO patients (name, email, phone, birth_date, cpf, address, emergency_contact, medical_history, allergies) VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9) RETURNING *',
      [name, email, phone, birth_date, cpf, address, emergency_contact, medical_history, allergies]
    );
    res.status(201).json(result.rows[0]);
  } catch (error) {
    console.error('Error creating patient:', error);
    res.status(500).json({ error: 'Failed to create patient' });
  }
});

// Services endpoints
app.get('/api/services', async (req, res) => {
  try {
    const result = await pool.query('SELECT * FROM services WHERE is_active = true ORDER BY category, name');
    res.json(result.rows);
  } catch (error) {
    console.error('Error fetching services:', error);
    res.status(500).json({ error: 'Failed to fetch services' });
  }
});

app.post('/api/services', async (req, res) => {
  const { name, description, price, duration_minutes, category } = req.body;
  try {
    const result = await pool.query(
      'INSERT INTO services (name, description, price, duration_minutes, category) VALUES ($1, $2, $3, $4, $5) RETURNING *',
      [name, description, price, duration_minutes, category]
    );
    res.status(201).json(result.rows[0]);
  } catch (error) {
    console.error('Error creating service:', error);
    res.status(500).json({ error: 'Failed to create service' });
  }
});

// Staff endpoints
app.get('/api/staff', async (req, res) => {
  try {
    const result = await pool.query('SELECT * FROM staff WHERE is_active = true ORDER BY full_name');
    res.json(result.rows);
  } catch (error) {
    console.error('Error fetching staff:', error);
    res.status(500).json({ error: 'Failed to fetch staff' });
  }
});

// Appointments endpoints
app.get('/api/appointments', async (req, res) => {
  try {
    const query = `
      SELECT 
        a.*,
        p.name as patient_name,
        s.name as service_name,
        st.full_name as staff_name
      FROM appointments a
      JOIN patients p ON a.patient_id = p.id
      JOIN services s ON a.service_id = s.id
      JOIN staff st ON a.staff_id = st.id
      ORDER BY a.appointment_date DESC
    `;
    const result = await pool.query(query);
    res.json(result.rows);
  } catch (error) {
    console.error('Error fetching appointments:', error);
    res.status(500).json({ error: 'Failed to fetch appointments' });
  }
});

app.post('/api/appointments', async (req, res) => {
  const { patient_id, service_id, staff_id, appointment_date, notes } = req.body;
  try {
    const result = await pool.query(
      'INSERT INTO appointments (patient_id, service_id, staff_id, appointment_date, notes) VALUES ($1, $2, $3, $4, $5) RETURNING *',
      [patient_id, service_id, staff_id, appointment_date, notes]
    );
    res.status(201).json(result.rows[0]);
  } catch (error) {
    console.error('Error creating appointment:', error);
    res.status(500).json({ error: 'Failed to create appointment' });
  }
});

// Financial endpoints
app.get('/api/financial/summary', async (req, res) => {
  try {
    const currentMonth = new Date().getMonth() + 1;
    const currentYear = new Date().getFullYear();
    const today = new Date().toISOString().split('T')[0];
    
    // Monthly revenue from appointments
    const monthlyRevenueResult = await pool.query(`
      SELECT COALESCE(SUM(s.price), 0) as monthly_revenue
      FROM appointments a
      JOIN services s ON a.service_id = s.id
      WHERE EXTRACT(MONTH FROM a.appointment_date) = $1 
      AND EXTRACT(YEAR FROM a.appointment_date) = $2
      AND a.status IN ('confirmed', 'completed')
    `, [currentMonth, currentYear]);
    
    // Today's appointments and revenue
    const todayResult = await pool.query(`
      SELECT 
        COUNT(*) as today_appointments,
        COALESCE(SUM(s.price), 0) as today_revenue
      FROM appointments a
      JOIN services s ON a.service_id = s.id
      WHERE DATE(a.appointment_date) = $1
      AND a.status IN ('confirmed', 'completed', 'scheduled')
    `, [today]);
    
    // Average ticket calculation
    const avgTicketResult = await pool.query(`
      SELECT AVG(s.price) as average_ticket
      FROM appointments a
      JOIN services s ON a.service_id = s.id
      WHERE a.status IN ('confirmed', 'completed')
      AND a.appointment_date >= NOW() - INTERVAL '30 days'
    `);
    
    const monthlyRevenue = parseFloat(monthlyRevenueResult.rows[0].monthly_revenue);
    const monthlyExpenses = monthlyRevenue * 0.65; // Estimate 65% expenses
    const netProfit = monthlyRevenue - monthlyExpenses;
    const profitMargin = monthlyRevenue > 0 ? Math.round((netProfit / monthlyRevenue) * 100) : 0;
    
    res.json({
      monthlyRevenue,
      monthlyExpenses,
      netProfit,
      profitMargin,
      todayAppointments: parseInt(todayResult.rows[0].today_appointments),
      todayRevenue: parseFloat(todayResult.rows[0].today_revenue),
      averageTicket: parseFloat(avgTicketResult.rows[0].average_ticket || 0)
    });
  } catch (error) {
    console.error('Error fetching financial summary:', error);
    res.status(500).json({ error: 'Failed to fetch financial summary' });
  }
});

app.get('/api/financial/cash-flow', async (req, res) => {
  try {
    const { period = 'month' } = req.query;
    let interval = '6 months';
    let dateFormat = 'YYYY-MM';
    
    if (period === 'week') {
      interval = '12 weeks';
      dateFormat = 'YYYY-"W"WW';
    }
    
    const result = await pool.query(`
      SELECT 
        TO_CHAR(a.appointment_date, $1) as period,
        SUM(s.price) as revenue,
        COUNT(*) as appointments
      FROM appointments a
      JOIN services s ON a.service_id = s.id
      WHERE a.appointment_date >= NOW() - INTERVAL $2
      AND a.status IN ('confirmed', 'completed')
      GROUP BY TO_CHAR(a.appointment_date, $1)
      ORDER BY period DESC
      LIMIT 6
    `, [dateFormat, interval]);
    
    res.json(result.rows);
  } catch (error) {
    console.error('Error fetching cash flow data:', error);
    res.status(500).json({ error: 'Failed to fetch cash flow data' });
  }
});

app.get('/api/financial/revenue-by-category', async (req, res) => {
  try {
    const result = await pool.query(`
      SELECT 
        s.category,
        SUM(s.price) as total_revenue,
        COUNT(*) as appointments_count
      FROM appointments a
      JOIN services s ON a.service_id = s.id
      WHERE a.status IN ('confirmed', 'completed')
      AND a.appointment_date >= NOW() - INTERVAL '30 days'
      GROUP BY s.category
      ORDER BY total_revenue DESC
    `);
    
    res.json(result.rows);
  } catch (error) {
    console.error('Error fetching revenue by category:', error);
    res.status(500).json({ error: 'Failed to fetch revenue by category' });
  }
});

app.get('/api/financial/daily-revenue', async (req, res) => {
  try {
    const result = await pool.query(`
      SELECT 
        DATE(a.appointment_date) as date,
        SUM(s.price) as daily_revenue,
        COUNT(*) as appointments_count
      FROM appointments a
      JOIN services s ON a.service_id = s.id
      WHERE a.status IN ('confirmed', 'completed') 
      AND a.appointment_date >= NOW() - INTERVAL '30 days'
      GROUP BY DATE(a.appointment_date)
      ORDER BY date DESC
    `);
    res.json(result.rows);
  } catch (error) {
    console.error('Error fetching daily revenue:', error);
    res.status(500).json({ error: 'Failed to fetch daily revenue' });
  }
});

// Serve working test interface
app.get('/test', (req, res) => {
  res.sendFile(path.join(__dirname, 'src', 'test-app.html'));
});

// Handle Angular routing
app.get('/', (req, res) => {
  res.sendFile(path.join(__dirname, 'src', 'test-app.html'));
});

app.listen(port, '0.0.0.0', () => {
  console.log(`DentalSpa Angular server running on http://0.0.0.0:${port}`);
  console.log('Frontend Angular restaurado com sucesso!');
});