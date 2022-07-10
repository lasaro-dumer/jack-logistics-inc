import * as React from "react";

import { useLocation, NavLink } from "react-router-dom";
import { getPackagesAsync } from "../backend/packages-service";

function QueryNavLink({ to, ...props }) {
  let location = useLocation();
  return <NavLink to={to + location.search} {...props} />;
}

function ShipPackageNavLink({ to, ...props }) {
  let location = useLocation();
  return <NavLink to={to + location.search} {...props} />;
}

function locationName(packageItem) {
  if (packageItem.location)
    return `${packageItem?.location.building}-${packageItem?.location.floor}-${packageItem?.location.corridor}-${packageItem?.location.shelf}`;
  return "";
}

function showShipLink(packageItem) {
  return packageItem.shipment ? "none" : "block";
}

class ListPackages extends React.Component {
  constructor(props) {
    super(props);
    this.state = { packages: [], searchParams: null };
    this.updateFilter = this.updateFilter.bind(this);
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

  render() {
    return (
      <div>
        <div style={{ borderRight: "solid 1px", padding: "1rem" }}>
          <input
            value={this.state.searchParams || ""}
            onChange={this.updateFilter}
          />
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
                let filter = this.state.searchParams;
                if (!filter) return true;
                let description = packageItem.description.toLowerCase();
                return description.includes(filter.toLowerCase());
              })
              .map((packageItem) => (
                <tr>
                  <td>{packageItem.id}</td>
                  {/* <td>
                    <QueryNavLink
                      key={packageItem.id}
                      style={{ display: "block", margin: "1rem 0" }}
                      to={`/packages/${packageItem.id}`}
                    >
                      {packageItem.description}
                    </QueryNavLink>
                  </td> */}
                  <td>{packageItem.description}</td>
                  {/* <td>{packageItem.state}</td> */}
                  <td>{locationName(packageItem)}</td>
                  <td>{packageItem?.shipment?.id}</td>
                  <td>
                    <ShipPackageNavLink
                      key={packageItem.id}
                      style={{
                        display: showShipLink(packageItem),
                        margin: "1rem 0",
                      }}
                      to={`/packages/ship/${packageItem.id}`}
                    >
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
