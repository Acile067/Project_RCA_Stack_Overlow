import { useEffect, useState } from "react";
import { fetchHealthCheckSummary } from "./services/healthCheckService";
import {
  PieChart,
  Pie,
  Cell,
  Legend,
  Tooltip,
  ResponsiveContainer,
} from "recharts";

function App() {
  const [summary, setSummary] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    async function loadData() {
      try {
        const data = await fetchHealthCheckSummary();
        setSummary(data);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    }
    loadData();
  }, []);

  if (loading) {
    return <div className="text-center mt-10 text-xl">Loading...</div>;
  }

  if (error) {
    return (
      <div className="text-center mt-10 text-red-500 text-xl">
        Error: {error}
      </div>
    );
  }

  const chartData = [
    { name: "OK", value: summary.okCount },
    { name: "NOT OK", value: summary.notOkCount },
  ];

  const COLORS = ["#22c55e", "#ef4444"];

  return (
    <div className="max-w-3xl mx-auto mt-10 p-6 bg-white shadow rounded-2xl">
      <h1 className="text-2xl font-bold mb-6 text-center">
        Service Health Summary (last 3h)
      </h1>

      <div className="grid grid-cols-2 gap-6 text-center">
        <div className="p-4 rounded-lg bg-green-100">
          <p className="text-lg font-semibold">OK</p>
          <p className="text-3xl font-bold text-green-600">{summary.okCount}</p>
        </div>
        <div className="p-4 rounded-lg bg-red-100">
          <p className="text-lg font-semibold">NOT OK</p>
          <p className="text-3xl font-bold text-red-600">
            {summary.notOkCount}
          </p>
        </div>
      </div>

      <div className="mt-6 p-4 text-center bg-gray-50 rounded-lg">
        <p className="text-lg font-semibold">Availability</p>
        <p className="text-3xl font-bold text-blue-600">
          {summary.availabilityPercentage.toFixed(2)}%
        </p>
      </div>

      <div className="mt-6 text-gray-600 text-sm text-center">
        Total checks: {summary.total}
      </div>

      {/* Pie chart */}
      <div className="mt-10 w-full h-64">
        <ResponsiveContainer>
          <PieChart>
            <Pie
              data={chartData}
              cx="50%"
              cy="50%"
              outerRadius={100}
              dataKey="value"
              label
            >
              {chartData.map((entry, index) => (
                <Cell key={`cell-${index}`} fill={COLORS[index]} />
              ))}
            </Pie>
            <Tooltip />
            <Legend />
          </PieChart>
        </ResponsiveContainer>
      </div>
    </div>
  );
}

export default App;
