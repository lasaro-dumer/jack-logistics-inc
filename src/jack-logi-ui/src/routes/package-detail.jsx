import * as React from "react";
import { useParams } from "react-router-dom";

class PackageDetail extends React.Component {
  constructor(props) {
    super(props);
    this.state = {};
  }
  componentDidMount() {}
  render() {
    const { id } = this.props.match.params;
    if (id) {
      this.setState({ packageId: id });
      // if (this.state.packageItem) {
      //   let packageItem = getPackage(id);

      //   return (
      //     <div style={{ padding: "1rem" }}>
      //       <p>Id: {packageItem.id}</p>
      //       <p>Description: {packageItem.description}</p>
      //     </div>
      //   );
      // } else {
      return <div>Loading package</div>;
      // }
    }

    return <div>Not able to find with id equal '{id}'</div>;
  }
}

export default function PackageDetailWrapper(props) {
  const params = useParams();
  return <PackageDetail {...{ ...props, match: { params } }} />;
}
