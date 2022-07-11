import * as React from "react";

import { useLocation, NavLink } from "react-router-dom";
import { getPackagesAsync } from "../backend/packages-service";
import ShipPackageNavLink from "../components/ship-package-nav-link";

function QueryNavLink({ to, ...props }) {
  let location = useLocation();
  return <NavLink to={to + location.search} {...props} />;
}

function locationName(packageItem) {
  if (packageItem.location)
    return `${packageItem?.location.building}-${packageItem?.location.floor}-${packageItem?.location.corridor}-${packageItem?.location.shelf}`;
  return "";
}

class ListPackages extends React.Component {
  constructor(props) {
    super(props);
    this.state = { packages: [], searchParams: null };
    this.updateFilter = this.updateFilter.bind(this);
    this.updatePendingShipmentFilter =
      this.updatePendingShipmentFilter.bind(this);
  }

  componentDidMount() {
    getPackagesAsync().then((packages) => this.setState({ packages }));
  }

  updateFilter(event) {
    let filter = event.target.value;
    if (filter) {
      this.setState({ searchParams: filter });
    } else {
      this.setState({ searchParams: null });
    }
  }

  updatePendingShipmentFilter(event) {
    this.setState({ pendingShipmentFilter: event.target.checked });
  }

  render() {
    return (
      <div>
        <div style={{ borderRight: "solid 1px", padding: "1rem" }}>
          <label style={{ padding: "1rem" }}>
            Package Description contains
            <input
              value={this.state.searchParams || ""}
              onChange={this.updateFilter}
              style={{ marginLeft: "1rem" }}
            />
          </label>
          <label style={{ padding: "1rem" }}>
            Only Pending Shipment
            <input
              type={"checkbox"}
              checked={this.state.pendingShipmentFilter || false}
              onChange={this.updatePendingShipmentFilter}
            ></input>
          </label>
          <table>
            <tr>
              <th>Id</th>
              <th>Description</th>
              {/* <th>State</th> */}
              <th>Location</th>
              <th>Shipment</th>
            </tr>
            {this.state.packages
              .filter((packageItem) => {
                if (this.state.pendingShipmentFilter)
                  return (
                    !packageItem.shipmentId || packageItem.shipmentId === 0
                  );

                let filter = this.state.searchParams;
                if (!filter) return true;
                let description = packageItem.description.toLowerCase();
                return description.includes(filter.toLowerCase());
              })
              .map((packageItem) => (
                <tr key={packageItem.id}>
                  <td>{packageItem.id}</td>
                  <td>
                    <QueryNavLink
                      style={{ display: "block", margin: "1rem 0" }}
                      to={`/packages/${packageItem.id}`}
                    >
                      {packageItem.description}
                    </QueryNavLink>
                  </td>
                  <td>{locationName(packageItem)}</td>
                  <td>{packageItem?.shipment?.id}</td>
                  <td>
                    <ShipPackageNavLink packageItem={packageItem}>
                      Ship Package
                    </ShipPackageNavLink>
                  </td>
                </tr>
              ))}
          </table>
        </div>
      </div>
    );
  }
}

export default ListPackages;
