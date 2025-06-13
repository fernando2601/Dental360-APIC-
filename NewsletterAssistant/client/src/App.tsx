import React, { useState, useEffect } from 'react'
import './App.css'

interface Newsletter {
  id: number;
  title: string;
  description: string;
  status: string;
  createdAt: string;
}

function App() {
  const [newsletters, setNewsletters] = useState<Newsletter[]>([]);
  const [loading, setLoading] = useState(true);
  const [newTitle, setNewTitle] = useState('');
  const [newDescription, setNewDescription] = useState('');

  useEffect(() => {
    fetchNewsletters();
  }, []);

  const fetchNewsletters = async () => {
    try {
      const response = await fetch('/api/newsletters');
      const data = await response.json();
      setNewsletters(data);
    } catch (error) {
      console.error('Error fetching newsletters:', error);
    } finally {
      setLoading(false);
    }
  };

  const createNewsletter = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      const response = await fetch('/api/newsletters', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          title: newTitle,
          description: newDescription,
        }),
      });
      
      if (response.ok) {
        const newNewsletter = await response.json();
        setNewsletters([...newsletters, newNewsletter]);
        setNewTitle('');
        setNewDescription('');
      }
    } catch (error) {
      console.error('Error creating newsletter:', error);
    }
  };

  if (loading) {
    return <div className="loading">Carregando...</div>;
  }

  return (
    <div className="App">
      <header className="App-header">
        <h1>Newsletter Assistant</h1>
        <p>Sistema de Gestão de Newsletters - Maio 2025</p>
      </header>

      <main className="main-content">
        <section className="newsletter-form">
          <h2>Criar Nova Newsletter</h2>
          <form onSubmit={createNewsletter}>
            <div className="form-group">
              <label htmlFor="title">Título:</label>
              <input
                type="text"
                id="title"
                value={newTitle}
                onChange={(e) => setNewTitle(e.target.value)}
                required
                placeholder="Digite o título da newsletter"
              />
            </div>
            <div className="form-group">
              <label htmlFor="description">Descrição:</label>
              <textarea
                id="description"
                value={newDescription}
                onChange={(e) => setNewDescription(e.target.value)}
                required
                placeholder="Digite a descrição da newsletter"
                rows={4}
              />
            </div>
            <button type="submit" className="submit-btn">
              Criar Newsletter
            </button>
          </form>
        </section>

        <section className="newsletters-list">
          <h2>Newsletters Existentes</h2>
          {newsletters.length === 0 ? (
            <p>Nenhuma newsletter encontrada.</p>
          ) : (
            <div className="newsletters-grid">
              {newsletters.map((newsletter) => (
                <div key={newsletter.id} className="newsletter-card">
                  <h3>{newsletter.title}</h3>
                  <p className="description">{newsletter.description}</p>
                  <div className="newsletter-meta">
                    <span className={`status ${newsletter.status}`}>
                      {newsletter.status === 'draft' ? 'Rascunho' : 'Publicado'}
                    </span>
                    <span className="date">
                      {new Date(newsletter.createdAt).toLocaleDateString('pt-BR')}
                    </span>
                  </div>
                </div>
              ))}
            </div>
          )}
        </section>
      </main>
    </div>
  )
}

export default App