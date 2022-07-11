import * as React from "react";
import { useParams } from "react-router-dom";
import ShipPackageNavLink from "../components/ship-package-nav-link";

function locationName(packageItem) {
  if (packageItem.location)
    return `${packageItem?.location.building}-${packageItem?.location.floor}-${packageItem?.location.corridor}-${packageItem?.location.shelf}`;
  return "";
}

class PackageDetail extends React.Component {
  constructor(props) {
    super(props);
    this.state = {};
  }

  componentDidMount() {
    if (this.props) {
      const { id } = this.props.match.params;
      this.setState({ packageId: id });

      if (id && !this.state.loading) {
        fetch(`https://localhost:5001/api/packages/${id}`)
          .then((resp) => resp.json())
          .then((packageItem) => {
            this.setState({ packageItem, loading: true });
          })
          .catch((reason) => {
            this.setState({ reason, loading: true });
          })
          .finally(() => {
            this.setState({ loading: true });
          });
      }
    }
  }

  render() {
    const { id } = this.props.match.params;
    if (this.state.packageItem) {
      return (
        <div style={{ padding: "1rem" }}>
          <form onSubmit={this.handleSubmit}>
            <p>Id: {this.state.packageItem.id}</p>
            <p>Description: {this.state.packageItem.description}</p>
            {this.state.packageItem.location && (
              <p>{locationName(this.state.packageItem)}</p>
            )}
            <ShipPackageNavLink
              key={this.state.packageItem.id}
              packageItem={this.state.packageItem}
            >
              Ship Package
            </ShipPackageNavLink>
          </form>
        </div>
      );
    }

    if (this.state.loading) {
      return <div>Loading package</div>;
    }

    return <div>Not able to find with id equal '{id}'.</div>;
  }
}

export default function PackageDetailWrapper(props) {
  const params = useParams();
  return <PackageDetail {...{ ...props, match: { params } }} />;
}
