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

// API Routes
app.get('/api/patients', async (req, res) => {
  try {
    const result = await pool.query('SELECT * FROM patients ORDER BY created_at DESC');
    res.json(result.rows);
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
});

app.get('/api/appointments', async (req, res) => {
  try {
    const result = await pool.query(`
      SELECT a.*, p.name as patient_name, s.name as service_name, st.full_name as staff_name
      FROM appointments a
      LEFT JOIN patients p ON a.patient_id = p.id
      LEFT JOIN services s ON a.service_id = s.id
      LEFT JOIN staff st ON a.staff_id = st.id
      ORDER BY a.appointment_date DESC
    `);
    res.json(result.rows);
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
});

app.get('/api/services', async (req, res) => {
  try {
    const result = await pool.query('SELECT * FROM services ORDER BY name');
    res.json(result.rows);
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
});

app.get('/api/staff', async (req, res) => {
  try {
    const result = await pool.query('SELECT * FROM staff ORDER BY full_name');
    res.json(result.rows);
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
});

app.get('/api/financial/summary', async (req, res) => {
  try {
    const monthlyResult = await pool.query(`
      SELECT SUM(s.price) as total
      FROM appointments a
      JOIN services s ON a.service_id = s.id
      WHERE a.status IN ('completed', 'confirmed')
      AND a.appointment_date >= DATE_TRUNC('month', CURRENT_DATE)
    `);
    
    const avgResult = await pool.query(`
      SELECT AVG(s.price) as average
      FROM appointments a
      JOIN services s ON a.service_id = s.id
      WHERE a.status IN ('completed', 'confirmed')
    `);

    res.json({
      monthlyRevenue: monthlyResult.rows[0].total || 0,
      averageTicket: avgResult.rows[0].average || 0
    });
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
});

// Serve static files
app.use(express.static(path.join(__dirname, 'src')));
app.use('/node_modules', express.static(path.join(__dirname, 'node_modules')));

// Serve index.html for all non-API routes
app.get('*', (req, res) => {
  if (!req.path.startsWith('/api')) {
    res.sendFile(path.join(__dirname, 'src', 'index.html'));
  }
});

app.listen(port, '0.0.0.0', () => {
  console.log(`DentalSpa server running on http://0.0.0.0:${port}`);
  console.log('API endpoints available at /api/*');
  console.log('Frontend serving from /src directory');
});