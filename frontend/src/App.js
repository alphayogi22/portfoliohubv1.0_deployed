import Portfolio from './components/Portfolio';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import PortfolioAdmin from './components/PortfolioAdmin';      // Upload Form
import PortfolioViewer from './components//PortfolioViewer'; 
import './App.css';

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/admin" element={<PortfolioAdmin />} />
        <Route path="/:username/resume" element={<PortfolioViewer />} />
      </Routes>
    </Router>
  );
}
export default App;
