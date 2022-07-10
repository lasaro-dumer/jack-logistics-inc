import { Link, Outlet } from 'react-router-dom';
import './App.css';

function App() {
  return (
    <div>
      <h1>Jack Logistics Inc.</h1>
      <nav
        style={{
          borderBottom: "solid 1px",
          paddingBottom: "1rem",
        }}
      >
        <Link to="/packages/new">Add Package</Link> | {" "}
        <Link to="/packages">Packages</Link> | {" "}
        <Link to="/shipments">Shipments</Link>
      </nav>
      <Outlet />
    </div>
  );
}

export default App;
