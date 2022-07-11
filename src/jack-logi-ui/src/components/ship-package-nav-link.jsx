import { NavLink, useLocation } from "react-router-dom";

export default function ShipPackageNavLink({ to, packageItem, ...props }) {
  let location = useLocation();
  if (packageItem.shipment) return "";
  return (
    <NavLink
      style={{
        display: "block",
        margin: "1rem 0",
      }}
      to={`/packages/ship/${packageItem.id}` + location.search}
      {...props}
    />
  );
}
