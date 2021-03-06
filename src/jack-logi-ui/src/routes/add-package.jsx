import React from "react";
import Dropdown from "react-dropdown";
import { Navigate } from "react-router-dom";

class AddPackage extends React.Component {
  constructor(props) {
    super(props);
    this.state = { data: [] };
    this.handleSubmit = this.handleSubmit.bind(this);
    this.handleDescriptionChange = this.handleDescriptionChange.bind(this);
    this._onSelect = this._onSelect.bind(this);
  }

  componentDidMount() {
    fetch("https://localhost:5001/api/warehouses")
      .then((resp) => resp.json())
      .then((warehouses) =>
        warehouses.forEach((warehouse) => {
          let data = this.state.data;
          fetch(
            `https://localhost:5001/api/warehouses/${warehouse.id}/locations`
          )
            .then((resp) => resp.json())
            .then((locations) => {
              let warehouseAlreadyAdded =
                data.findIndex((item) => item.name === warehouse.name) > -1;
              if (!warehouseAlreadyAdded) {
                const newWarehouseGroup = {
                  type: "group",
                  name: warehouse.name,
                  className: "warehouse-name",
                  items: [],
                };
                locations.forEach((location) => {
                  newWarehouseGroup.items.push({
                    value: location.id,
                    label: `${location.building}-${location.floor}-${location.corridor}-${location.shelf}`,
                    className: "location-option",
                  });
                });
                data.push(newWarehouseGroup);
                this.setState({ data });
              }
            });
        })
      );
  }

  handleSubmit(e) {
    e.preventDefault();
    const newPackage = {
      description: this.state?.description || "",
      locationId: this.state?.location?.value || 0,
    };
    console.log("You clicked submit.", newPackage);
    fetch("https://localhost:5001/api/packages", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(newPackage),
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

  handleDescriptionChange(e) {
    this.setState({ description: e.target.value });
  }

  _onSelect(option) {
    this.setState({ location: option });
  }

  render() {
    return (
      <div>
        <form onSubmit={this.handleSubmit}>
          <label>
            Description:
            <input
              type="text"
              name="description"
              value={this.state.description}
              onChange={this.handleDescriptionChange}
            />
          </label>
          <br />
          <label>
            Location:
            <Dropdown
              options={this.state.data}
              onChange={this._onSelect}
              value={this.state.location}
              placeholder="Select an option"
            />
          </label>
          <button type="submit">Add Package</button>
        </form>
        {!this.state.addResult && <p>{this.state.errorMessage}</p>}
        {this.state.addResult && <Navigate to="/packages" replace={true} />}
      </div>
    );
  }

  async getLocations() {
    return await fetch("https://localhost:5001/api/warehouses");
  }
}

export default AddPackage;
