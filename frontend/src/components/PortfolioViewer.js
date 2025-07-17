import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import { fetchPortfolioByUsername } from '../api/portfolioApi';
import './PortfolioViewer.css';
import { Typography, Button, CircularProgress } from '@mui/material';

function PortfolioViewer() {
  const { username } = useParams();
  const [portfolio, setPortfolio] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const loadPortfolio = async () => {
      try {
        const data = await fetchPortfolioByUsername(username);
        setPortfolio(data || null);
      } catch {
        setPortfolio(null);
      } finally {
        setLoading(false);
      }
    };

    loadPortfolio();
  }, [username]);

  if (loading) {
    return <CircularProgress sx={{ display: 'block', mx: 'auto', mt: 10 }} />;
  }

  if (!portfolio) {
    return (
      <Typography align="center" sx={{ mt: 5 }}>
        No portfolio available for <strong>{username}</strong>
      </Typography>
    );
  }

  return (
    <div className="portfolio-main-div">
      <div className="portfolio-card">
        <Typography className="portfolio-title" style={{ whiteSpace: 'pre-line' }}>
          {portfolio.name
            .toUpperCase()
            .split(' ')
            .map(word => word.split('').join('\n'))
            .join('\n\n')}
        </Typography>

        <div className="portfolio-card-details">
          <img
            className="portfolio-image"
            src={`http://localhost:5050/api/portfolio/${portfolio.id}/image`}
            alt={portfolio.title}
          />

          <h4 style={{ textAlign: 'center', margin: '10px 0' }}>{portfolio.title}</h4>

          <Typography className="portfolio-description">
            {portfolio.description}
          </Typography>

          <Button
            variant="contained"
            className="download-button"
            href={`http://localhost:5050/api/portfolio/${portfolio.id}/resume`}
            download="resume.pdf"
          >
            Download Resume
          </Button>
        </div>
      </div>
    </div>
  );
}

export default PortfolioViewer;
