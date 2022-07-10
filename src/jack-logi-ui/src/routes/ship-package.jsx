import * as React from "react";
import { useParams } from "react-router-dom";

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
    console.log("You clicked submit.", shipPackage);
    fetch("https://localhost:5001/api/shipments", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(shipPackage),
    })
      .then((response) => {
        if (response.status != 201) return response.text();
        return response.json();
      })
      .then((data) => {
        console.log("Success:", data);
      })
      .catch((error) => {
        console.error("Error:", error);
        this.setState({ errorMessage: error });
      });
  }

  handleDestinationAddressChange(e) {
    console.log("handleDestinationAddressChange:", e.target.value);
    this.setState({ destinationAddress: e.target.value });
  }

  render() {
    const { id } = this.props.match.params;
    if (this.state.packageItem) {
      const displayErrorMessage = this.state.errorMessage ? "block" : "none";
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
          <div style={{ display: displayErrorMessage }}></div>
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
