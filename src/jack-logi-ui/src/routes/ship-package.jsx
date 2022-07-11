import * as React from "react";
import { Navigate, useParams } from "react-router-dom";

class ShipPackage extends React.Component {
  constructor(props) {
    super(props);
    this.state = { packageId: null };
    this.handleSubmit = this.handleSubmit.bind(this);
    this.handleDestinationAddressChange =
      this.handleDestinationAddressChange.bind(this);
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

  handleSubmit(e) {
    e.preventDefault();
    const shipPackage = {
      packageId: this.state?.packageItem.id || 0,
      destinationAddressData: this.state?.destinationAddress || "",
    };

    fetch("https://localhost:5001/api/shipments", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(shipPackage),
    })
      .then(async (response) => {
        const success = response.status === 201;
        this.setState({ addResult: success });
        if (success) return response.json();
        return response.text();
      })
      .then((data) => {
        if (this.state.addResult) {
          this.setState({ errorMessage: null });
        } else {
          this.setState({ errorMessage: data });
        }
      })
      .catch((error) => {
        this.setState({ addResult: false });
        this.setState({ errorMessage: error });
      });
  }

  handleDestinationAddressChange(e) {
    this.setState({ destinationAddress: e.target.value });
  }

  render() {
    const { id } = this.props.match.params;
    if (this.state.packageItem) {
      return (
        <div style={{ padding: "1rem" }}>
          <form onSubmit={this.handleSubmit}>
            <p>Id: {this.state.packageItem.id}</p>
            <p>Description: {this.state.packageItem.description}</p>
            <label>
              Destination Address:
              <input
                type="text"
                name="destinationAddress"
                value={this.state.destinationAddress}
                onChange={this.handleDestinationAddressChange}
              />
            </label>
            <br />
            <button type="submit">Ship Package</button>
          </form>
          {!this.state.addResult && <p>{this.state.errorMessage}</p>}
          {this.state.addResult && <Navigate to="/packages" replace={true} />}
        </div>
      );
    }

    if (this.state.loading) {
      return <div>Loading package</div>;
    }

    return <div>Not able to find with id equal '{id}'.</div>;
  }
}

export default function ShipPackageWrapper(props) {
  const params = useParams();
  return <ShipPackage {...{ ...props, match: { params } }} />;
}
