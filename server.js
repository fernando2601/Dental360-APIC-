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
app.use(express.static(path.join(__dirname, 'src')));

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

// Financial data (placeholder for future implementation)
app.get('/api/financial', async (req, res) => {
  try {
    // For now, calculate revenue from completed appointments
    const result = await pool.query(`
      SELECT 
        DATE(a.appointment_date) as date,
        SUM(s.price) as daily_revenue
      FROM appointments a
      JOIN services s ON a.service_id = s.id
      WHERE a.status = 'completed' 
      AND a.appointment_date >= NOW() - INTERVAL '30 days'
      GROUP BY DATE(a.appointment_date)
      ORDER BY date DESC
    `);
    res.json(result.rows);
  } catch (error) {
    console.error('Error fetching financial data:', error);
    res.status(500).json({ error: 'Failed to fetch financial data' });
  }
});

// Handle Angular routing
app.get('/', (req, res) => {
  res.sendFile(path.join(__dirname, 'src', 'index.html'));
});

app.listen(port, '0.0.0.0', () => {
  console.log(`DentalSpa Angular server running on http://0.0.0.0:${port}`);
  console.log('Frontend Angular restaurado com sucesso!');
});