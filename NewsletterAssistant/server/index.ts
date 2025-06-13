import express from 'express';
import cors from 'cors';
import path from 'path';

const app = express();
const PORT = process.env.PORT || 3001;

// Middleware
app.use(cors());
app.use(express.json());
app.use(express.static('public'));

// Routes
app.get('/api/health', (req, res) => {
  res.json({ status: 'OK', timestamp: new Date().toISOString() });
});

app.get('/api/newsletters', (req, res) => {
  res.json([
    {
      id: 1,
      title: 'Newsletter Semanal',
      description: 'Conteúdo semanal sobre tecnologia',
      status: 'draft',
      createdAt: '2025-05-15T10:00:00Z'
    },
    {
      id: 2,
      title: 'Newsletter Mensal',
      description: 'Resumo mensal de novidades',
      status: 'published',
      createdAt: '2025-05-01T09:00:00Z'
    }
  ]);
});

app.post('/api/newsletters', (req, res) => {
  const { title, description } = req.body;
  
  const newNewsletter = {
    id: Date.now(),
    title,
    description,
    status: 'draft',
    createdAt: new Date().toISOString()
  };
  
  res.status(201).json(newNewsletter);
});

// Serve client files
app.get('*', (req, res) => {
  res.sendFile(path.join(__dirname, '../../client/dist/index.html'));
});

app.listen(PORT, () => {
  console.log(`Newsletter Assistant Server running on port ${PORT}`);
  console.log(`Environment: ${process.env.NODE_ENV || 'development'}`);
});