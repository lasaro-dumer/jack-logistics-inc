import { NavLink, useLocation } from "react-router-dom";

export default function ShipPackageNavLink({ to, packageItem, ...props }) {
  const showShipLink = packageItem.shipment ? "none" : "block";
  let location = useLocation();
  return (
    <NavLink
      style={{
        display: showShipLink,
        margin: "1rem 0",
      }}
      to={`/packages/ship/${packageItem.id}` + location.search}
      {...props}
    />
  );
}
