import React from 'react';
import ReactDOM from 'react-dom/client';
import './index.css';
import {
  BrowserRouter,
  Routes,
  Route,
} from "react-router-dom";
import App from './App';
import reportWebVitals from './reportWebVitals';
import AddPackage from './routes/add-package';
import ListPackages from './routes/list-packages';
import PackageDetailWrapper from './routes/package-detail';
import ShipPackageWrapper from './routes/ship-package';

const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(
  <React.StrictMode>
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<App />}>
          <Route path="packages">
            <Route index element={<ListPackages />} />
            <Route path="new" element={<AddPackage />} />
            <Route path="ship/:id" element={<ShipPackageWrapper />} />
            <Route path=":id" element={<PackageDetailWrapper />} />
          </Route>
        </Route>
      </Routes>
    </BrowserRouter>
  </React.StrictMode>
);

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
