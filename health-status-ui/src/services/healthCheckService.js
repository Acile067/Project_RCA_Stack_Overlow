export async function fetchHealthCheckSummary() {
  const response = await fetch(
    `${import.meta.env.VITE_BACKEND_API_URL}healthchecks/summary`
  );
  if (!response.ok) {
    throw new Error("Failed to fetch health check summary");
  }
  const data = await response.json();
  return {
    total: data.Total,
    okCount: data.OkCount,
    notOkCount: data.NotOkCount,
    availabilityPercentage: data.AvailabilityPercentage,
  };
}
