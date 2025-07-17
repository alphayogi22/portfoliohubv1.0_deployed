import React, { useState, useRef } from 'react';
import { createPortfolio } from '../api/portfolioApi';
import './PortfolioAdmin.css';
import {
  Container, Typography, Box, TextField, Input, Button
} from '@mui/material';

function PortfolioAdmin() {
  const [formData, setFormData] = useState({
    name: '',
    title: '',
    description: '',
    image: null,
    resume: null,
  });

  const imageInputRef = useRef(null);
  const resumeInputRef = useRef(null);

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
      const sanitizedForm = {
        ...formData,
        name: formData.name.trim().toLowerCase(), // ✅ sanitize
      };

      await createPortfolio(sanitizedForm);
      alert('Portfolio uploaded successfully!');

      setFormData({
        name: '',
        title: '',
        description: '',
        image: null,
        resume: null,
      });

      if (imageInputRef.current) imageInputRef.current.value = '';
      if (resumeInputRef.current) resumeInputRef.current.value = '';
    } catch {
      alert('Failed to upload portfolio.');
    }
  };

  return (
    <div className="admin-container">
      <div className="admin-card">
        <Typography className="admin-title">Upload Portfolio</Typography>
        <Box component="form" onSubmit={handleSubmit}>
          <TextField
            label="Full Name"
            name="name"
            value={formData.name}
            onChange={handleInputChange}
            fullWidth
            required
            margin="normal"
          />
          <TextField
            label="Title"
            name="title"
            value={formData.title}
            onChange={handleInputChange}
            fullWidth
            required
            margin="normal"
          />
          <TextField
            label="Description"
            name="description"
            value={formData.description}
            onChange={handleInputChange}
            fullWidth
            required
            multiline
            rows={4}
            margin="normal"
          />
          <Input
            type="file"
            name="image"
            inputProps={{ accept: 'image/*' }}
            onChange={handleFileChange}
            className="admin-input"
            inputRef={imageInputRef}
          />
          <Input
            type="file"
            name="resume"
            inputProps={{ accept: 'application/pdf' }}
            onChange={handleFileChange}
            className="admin-input"
            inputRef={resumeInputRef}
          />
          <Button type="submit" variant="contained" fullWidth className="upload-button">
            Upload
          </Button>
        </Box>
      </div>
    </div>
  );
}

export default PortfolioAdmin;
