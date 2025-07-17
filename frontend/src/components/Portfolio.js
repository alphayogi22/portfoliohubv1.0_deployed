import React, { useState, useEffect } from 'react';
import { fetchPortfolios, createPortfolio } from '../api/portfolioApi';
import { Container, Card, CardContent, CardMedia, Typography, Button, Box, CircularProgress, TextField, Input } from '@mui/material';

function Portfolio() {
  const [portfolio, setPortfolio] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [formData, setFormData] = useState({
    title: '',
    description: '',
    image: null,
    resume: null,
  });

  useEffect(() => {
    const loadPortfolio = async () => {
      try {
        const data = await fetchPortfolios();
        setPortfolio(data.length > 0 ? data[0] : null);
        setLoading(false);
      } catch (error) {
        setError('Failed to load portfolio data.');
        setLoading(false);
      }
    };
    loadPortfolio();
  }, []);

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData({ ...formData, [name]: value });
  };

  const handleFileChange = (e) => {
    const { name, files } = e.target;
    setFormData({ ...formData, [name]: files[0] });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      await createPortfolio(formData);
      // Refresh portfolio data
      const data = await fetchPortfolios();
      setPortfolio(data.length > 0 ? data[0] : null);
      setFormData({ title: '', description: '', image: null, resume: null });
    } catch (error) {
      setError('Failed to upload portfolio.');
    }
  };

  return (
    <Container maxWidth="md" sx={{ minHeight: '100vh', display: 'flex', flexDirection: 'column', alignItems: 'center', bgcolor: '#e3f2fd', py: 4 }}>
      <Typography variant="h3" color="primary" gutterBottom sx={{ textAlign: 'center', mb: 4 }}>
        My Portfolio
      </Typography>

      {/* Upload Form */}
      <Box component="form" onSubmit={handleSubmit} sx={{ mb: 4, width: '100%', maxWidth: 500 }}>
        <TextField
          label="Title"
          name="title"
          value={formData.title}
          onChange={handleInputChange}
          fullWidth
          margin="normal"
          required
        />
        <TextField
          label="Description"
          name="description"
          value={formData.description}
          onChange={handleInputChange}
          fullWidth
          margin="normal"
          multiline
          rows={4}
          required
        />
        <Input
          type="file"
          name="image"
          onChange={handleFileChange}
          inputProps={{ accept: 'image/*' }}
          fullWidth
          sx={{ mt: 2 }}
          required
        />
        <Input
          type="file"
          name="resume"
          onChange={handleFileChange}
          inputProps={{ accept: 'application/pdf' }}
          fullWidth
          sx={{ mt: 2 }}
          required
        />
        <Button type="submit" variant="contained" color="primary" sx={{ mt: 2 }}>
          Upload Portfolio
        </Button>
      </Box>

      {/* Portfolio Display */}
      {loading ? (
        <CircularProgress />
      ) : error ? (
        <Typography variant="h6" color="error" sx={{ textAlign: 'center' }}>
          {error}
        </Typography>
      ) : portfolio ? (
        <Card sx={{ maxWidth: 500, width: '100%', boxShadow: 3, transition: 'transform 0.3s', '&:hover': { transform: 'scale(1.02)' } }}>
          <CardMedia
            component="img"
            height="300"
            image={`http://localhost:5050/api/portfolio/${portfolio.id}/image`}
            alt={portfolio.title}
            sx={{ objectFit: 'cover' }}
          />
          <CardContent>
            <Typography variant="h5" component="div" sx={{ textAlign: 'center', mb: 2 }}>
              {portfolio.title}
            </Typography>
            <Typography variant="body1" color="text.secondary" sx={{ textAlign: 'center', mb: 2 }}>
              {portfolio.description}
            </Typography>
            <Box sx={{ textAlign: 'center' }}>
              <Button
                variant="contained"
                color="primary"
                href={`http://localhost:5050/api/portfolio/${portfolio.id}/resume`}
                download="resume.pdf"
              >
                Download Resume
              </Button>
            </Box>
          </CardContent>
        </Card>
      ) : (
        <Typography variant="h6" color="text.secondary" sx={{ textAlign: 'center' }}>
          No portfolio data available.
        </Typography>
      )}
    </Container>
  );
}

export default Portfolio;