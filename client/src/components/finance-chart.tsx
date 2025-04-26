import { useEffect, useState } from "react";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { useQuery } from "@tanstack/react-query";
import { formatCurrency } from "@/lib/utils";
import {
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
  LineChart,
  Line,
  PieChart,
  Pie,
  Cell,
  Legend,
} from "recharts";
import { Loader2 } from "lucide-react";

export function FinanceChart() {
  const [dateRange, setDateRange] = useState("month");
  const [currentTab, setCurrentTab] = useState("overview");

  // Get financial transactions
  const { data: transactions, isLoading } = useQuery({
    queryKey: ['/api/financial-transactions'],
  });

  // Get services for categorization
  const { data: services } = useQuery({
    queryKey: ['/api/services'],
  });

  const [chartData, setChartData] = useState<any[]>([]);
  const [pieData, setPieData] = useState<any[]>([]);
  
  useEffect(() => {
    if (!transactions) return;

    // Process data based on selected date range
    const now = new Date();
    let startDate: Date;
    
    if (dateRange === "week") {
      startDate = new Date(now);
      startDate.setDate(now.getDate() - 7);
    } else if (dateRange === "month") {
      startDate = new Date(now);
      startDate.setMonth(now.getMonth() - 1);
    } else if (dateRange === "quarter") {
      startDate = new Date(now);
      startDate.setMonth(now.getMonth() - 3);
    } else { // year
      startDate = new Date(now);
      startDate.setFullYear(now.getFullYear() - 1);
    }

    // Filter transactions by date
    const filteredTransactions = transactions.filter((t: any) => 
      new Date(t.date) >= startDate && new Date(t.date) <= now
    );

    // Prepare chart data
    const data: any = [];
    const categoryData: Record<string, number> = {};
    
    if (dateRange === "week") {
      // Group by day of week
      const days = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];
      days.forEach(day => {
        data.push({
          name: day.substring(0, 3),
          income: 0,
          expenses: 0,
        });
      });
      
      filteredTransactions.forEach((t: any) => {
        const day = new Date(t.date).getDay();
        if (t.type === "income") {
          data[day].income += Number(t.amount);
        } else {
          data[day].expenses += Number(t.amount);
        }
        
        // Accumulate category data
        categoryData[t.category] = (categoryData[t.category] || 0) + Number(t.amount);
      });
    } else if (dateRange === "month") {
      // Create entries for each day of the month
      const daysInPeriod = 30;
      for (let i = 0; i < daysInPeriod; i++) {
        const date = new Date(now);
        date.setDate(now.getDate() - (daysInPeriod - i - 1));
        
        data.push({
          name: date.getDate().toString(),
          date: date,
          income: 0,
          expenses: 0,
        });
      }
      
      filteredTransactions.forEach((t: any) => {
        const tDate = new Date(t.date);
        const dayIndex = data.findIndex(d => 
          d.date.getDate() === tDate.getDate() &&
          d.date.getMonth() === tDate.getMonth() &&
          d.date.getFullYear() === tDate.getFullYear()
        );
        
        if (dayIndex >= 0) {
          if (t.type === "income") {
            data[dayIndex].income += Number(t.amount);
          } else {
            data[dayIndex].expenses += Number(t.amount);
          }
        }
        
        // Accumulate category data
        categoryData[t.category] = (categoryData[t.category] || 0) + Number(t.amount);
      });
      
      // Clean up data for rendering
      data.forEach(d => {
        d.name = d.date.getDate().toString();
        delete d.date;
      });
    } else if (dateRange === "quarter" || dateRange === "year") {
      // Group by month
      const monthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
      
      const months = dateRange === "quarter" ? 3 : 12;
      for (let i = 0; i < months; i++) {
        const date = new Date(now);
        date.setMonth(now.getMonth() - (months - i - 1));
        
        data.push({
          name: monthNames[date.getMonth()],
          month: date.getMonth(),
          year: date.getFullYear(),
          income: 0,
          expenses: 0,
        });
      }
      
      filteredTransactions.forEach((t: any) => {
        const tDate = new Date(t.date);
        const monthIndex = data.findIndex(d => 
          d.month === tDate.getMonth() && d.year === tDate.getFullYear()
        );
        
        if (monthIndex >= 0) {
          if (t.type === "income") {
            data[monthIndex].income += Number(t.amount);
          } else {
            data[monthIndex].expenses += Number(t.amount);
          }
        }
        
        // Accumulate category data
        categoryData[t.category] = (categoryData[t.category] || 0) + Number(t.amount);
      });
      
      // Clean up data for rendering
      data.forEach(d => {
        delete d.month;
        delete d.year;
      });
    }

    // Prepare pie chart data
    const pieChartData = Object.entries(categoryData).map(([name, value]) => ({ name, value }));
    
    setChartData(data);
    setPieData(pieChartData);
  }, [transactions, dateRange]);

  // Calculate totals
  const totalIncome = chartData?.reduce((sum, entry) => sum + entry.income, 0) || 0;
  const totalExpenses = chartData?.reduce((sum, entry) => sum + entry.expenses, 0) || 0;
  const netIncome = totalIncome - totalExpenses;

  const dateRangeOptions = [
    { value: "week", label: "Last 7 Days" },
    { value: "month", label: "Last 30 Days" },
    { value: "quarter", label: "Last 3 Months" },
    { value: "year", label: "Last 12 Months" },
  ];

  const COLORS = ['#2C7EA1', '#5BC0BE', '#3D9E9C', '#1A6985', '#9AA5B1', '#7B8794', '#616E7C'];

  return (
    <Card className="col-span-full">
      <CardHeader className="flex flex-row items-center justify-between">
        <div>
          <CardTitle>Financial Overview</CardTitle>
          <CardDescription>
            Monitor income, expenses, and financial trends
          </CardDescription>
        </div>
        <Select value={dateRange} onValueChange={setDateRange}>
          <SelectTrigger className="w-[180px]">
            <SelectValue placeholder="Select period" />
          </SelectTrigger>
          <SelectContent>
            {dateRangeOptions.map(option => (
              <SelectItem key={option.value} value={option.value}>
                {option.label}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>
      </CardHeader>
      <CardContent>
        <div className="grid grid-cols-1 sm:grid-cols-3 gap-4 mb-6">
          <div className="bg-muted/50 p-4 rounded-lg">
            <h3 className="text-sm font-medium text-muted-foreground">Total Income</h3>
            <p className="text-2xl font-bold text-success mt-1">{formatCurrency(totalIncome)}</p>
          </div>
          <div className="bg-muted/50 p-4 rounded-lg">
            <h3 className="text-sm font-medium text-muted-foreground">Total Expenses</h3>
            <p className="text-2xl font-bold text-destructive mt-1">{formatCurrency(totalExpenses)}</p>
          </div>
          <div className="bg-muted/50 p-4 rounded-lg">
            <h3 className="text-sm font-medium text-muted-foreground">Net Income</h3>
            <p className={`text-2xl font-bold mt-1 ${netIncome >= 0 ? 'text-success' : 'text-destructive'}`}>
              {formatCurrency(netIncome)}
            </p>
          </div>
        </div>

        <Tabs defaultValue="overview" value={currentTab} onValueChange={setCurrentTab} className="w-full">
          <TabsList className="mb-4">
            <TabsTrigger value="overview">Overview</TabsTrigger>
            <TabsTrigger value="categories">Categories</TabsTrigger>
          </TabsList>
          <TabsContent value="overview" className="w-full">
            {isLoading ? (
              <div className="flex items-center justify-center h-80">
                <Loader2 className="h-8 w-8 animate-spin text-muted-foreground" />
              </div>
            ) : (
              <ResponsiveContainer width="100%" height={400}>
                <BarChart data={chartData}>
                  <CartesianGrid strokeDasharray="3 3" opacity={0.2} />
                  <XAxis dataKey="name" />
                  <YAxis tickFormatter={(value) => `$${value}`} />
                  <Tooltip 
                    formatter={(value) => formatCurrency(value as number)}
                    labelFormatter={(label) => `Date: ${label}`}
                  />
                  <Legend />
                  <Bar dataKey="income" name="Income" fill="#10B981" radius={[4, 4, 0, 0]} />
                  <Bar dataKey="expenses" name="Expenses" fill="#F43F5E" radius={[4, 4, 0, 0]} />
                </BarChart>
              </ResponsiveContainer>
            )}
          </TabsContent>
          <TabsContent value="categories" className="w-full">
            {isLoading ? (
              <div className="flex items-center justify-center h-80">
                <Loader2 className="h-8 w-8 animate-spin text-muted-foreground" />
              </div>
            ) : (
              <ResponsiveContainer width="100%" height={400}>
                <PieChart>
                  <Pie
                    data={pieData}
                    cx="50%"
                    cy="50%"
                    labelLine={false}
                    outerRadius={150}
                    fill="#8884d8"
                    dataKey="value"
                    label={({ name, percent }) => `${name}: ${(percent * 100).toFixed(0)}%`}
                  >
                    {pieData.map((entry, index) => (
                      <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                    ))}
                  </Pie>
                  <Tooltip formatter={(value) => formatCurrency(value as number)} />
                  <Legend />
                </PieChart>
              </ResponsiveContainer>
            )}
          </TabsContent>
        </Tabs>
      </CardContent>
    </Card>
  );
}

export default FinanceChart;
