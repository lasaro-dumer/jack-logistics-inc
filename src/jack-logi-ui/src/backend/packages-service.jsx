export async function getPackagesAsync() {
  return await fetch("https://localhost:5001/api/packages")
    .then((response) => {
      return response.json();
    })
    .catch((reason) => {
      console.error(reason);
    });
}
