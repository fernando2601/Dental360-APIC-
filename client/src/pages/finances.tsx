import { useState } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { apiRequest } from "@/lib/queryClient";
import { useToast } from "@/hooks/use-toast";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Textarea } from "@/components/ui/textarea";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { Plus, ArrowUp, ArrowDown, Loader2 } from "lucide-react";
import FinanceChart from "@/components/finance-chart";
import { formatCurrency, formatDate } from "@/lib/utils";

// Form schema for transaction
const transactionFormSchema = z.object({
  type: z.enum(["income", "expense"], { 
    required_error: "Transaction type is required." 
  }),
  category: z.string().min(1, { 
    message: "Category is required." 
  }),
  amount: z.coerce.number().positive({ 
    message: "Amount must be a positive number." 
  }),
  date: z.string().min(1, {
    message: "Date is required."
  }),
  description: z.string().optional(),
  paymentMethod: z.string().optional(),
  clientId: z.coerce.number().optional(),
  appointmentId: z.coerce.number().optional(),
});

type TransactionFormValues = z.infer<typeof transactionFormSchema>;

export default function Finances() {
  const [isCreateDialogOpen, setIsCreateDialogOpen] = useState(false);
  const { toast } = useToast();
  const queryClient = useQueryClient();

  // Initialize form
  const form = useForm<TransactionFormValues>({
    resolver: zodResolver(transactionFormSchema),
    defaultValues: {
      type: "income",
      category: "",
      amount: 0,
      date: new Date().toISOString().split('T')[0],
      description: "",
      paymentMethod: "",
      clientId: undefined,
      appointmentId: undefined,
    },
  });

  // Fetch financial transactions
  const { data: transactions, isLoading: isLoadingTransactions } = useQuery({
    queryKey: ['/api/financial-transactions'],
  });

  // Fetch clients for reference
  const { data: clients } = useQuery({
    queryKey: ['/api/clients'],
  });

  // Fetch appointments for reference
  const { data: appointments } = useQuery({
    queryKey: ['/api/appointments'],
  });

  // Create transaction mutation
  const createTransaction = useMutation({
    mutationFn: async (values: TransactionFormValues) => {
      const response = await apiRequest('POST', '/api/financial-transactions', values);
      return response.json();
    },
    onSuccess: () => {
      toast({
        title: "Success",
        description: "Transaction recorded successfully.",
      });
      queryClient.invalidateQueries({ queryKey: ['/api/financial-transactions'] });
      setIsCreateDialogOpen(false);
      form.reset({
        type: "income",
        category: "",
        amount: 0,
        date: new Date().toISOString().split('T')[0],
        description: "",
        paymentMethod: "",
        clientId: undefined,
        appointmentId: undefined,
      });
    },
    onError: () => {
      toast({
        title: "Error",
        description: "Failed to record transaction. Please try again.",
        variant: "destructive",
      });
    },
  });

  // Handle form submission
  function onSubmit(values: TransactionFormValues) {
    createTransaction.mutate(values);
  }

  // Calculate financial summary
  const totalIncome = transactions?.filter((t: any) => t.type === "income")
    .reduce((sum: number, t: any) => sum + Number(t.amount), 0) || 0;
  
  const totalExpenses = transactions?.filter((t: any) => t.type === "expense")
    .reduce((sum: number, t: any) => sum + Number(t.amount), 0) || 0;
  
  const netIncome = totalIncome - totalExpenses;

  // Get recent transactions
  const recentTransactions = transactions
    ? [...transactions]
        .sort((a: any, b: any) => new Date(b.date).getTime() - new Date(a.date).getTime())
        .slice(0, 10)
    : [];

  return (
    <div className="space-y-6">
      <div className="flex flex-col md:flex-row justify-between items-start md:items-center">
        <div>
          <h1 className="text-2xl md:text-3xl font-bold tracking-tight">Gestão Financeira</h1>
          <p className="text-muted-foreground">Acompanhe receitas, despesas e desempenho financeiro.</p>
        </div>
        <Button className="mt-4 md:mt-0" onClick={() => setIsCreateDialogOpen(true)}>
          <Plus className="mr-2 h-4 w-4" />
          Registrar Transação
        </Button>
      </div>

      {/* Summary Cards */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        <Card>
          <CardHeader className="pb-2">
            <CardTitle className="text-md font-medium">Receita Total</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-success">{formatCurrency(totalIncome)}</div>
            <p className="text-xs text-muted-foreground">Total de ganhos</p>
          </CardContent>
        </Card>
        <Card>
          <CardHeader className="pb-2">
            <CardTitle className="text-md font-medium">Despesas Totais</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-destructive">{formatCurrency(totalExpenses)}</div>
            <p className="text-xs text-muted-foreground">Total de despesas</p>
          </CardContent>
        </Card>
        <Card>
          <CardHeader className="pb-2">
            <CardTitle className="text-md font-medium">Lucro Líquido</CardTitle>
          </CardHeader>
          <CardContent>
            <div className={`text-2xl font-bold ${netIncome >= 0 ? 'text-success' : 'text-destructive'}`}>
              {formatCurrency(netIncome)}
            </div>
            <p className="text-xs text-muted-foreground">Lucro/Prejuízo</p>
          </CardContent>
        </Card>
      </div>

      {/* Charts and Transactions */}
      <Tabs defaultValue="charts" className="w-full">
        <TabsList className="mb-4">
          <TabsTrigger value="charts">Gráficos & Análises</TabsTrigger>
          <TabsTrigger value="transactions">Transações</TabsTrigger>
        </TabsList>
        <TabsContent value="charts">
          <FinanceChart />
        </TabsContent>
        <TabsContent value="transactions">
          <Card>
            <CardHeader>
              <CardTitle>Transações Recentes</CardTitle>
              <CardDescription>Suas atividades financeiras mais recentes</CardDescription>
            </CardHeader>
            <CardContent>
              {isLoadingTransactions ? (
                <div className="flex justify-center py-8">
                  <Loader2 className="h-8 w-8 animate-spin text-muted-foreground" />
                </div>
              ) : recentTransactions.length === 0 ? (
                <div className="text-center py-8">
                  <p className="text-muted-foreground">Nenhuma transação registrada ainda.</p>
                </div>
              ) : (
                <div className="rounded-md border">
                  <div className="grid grid-cols-7 p-4 font-medium border-b">
                    <div className="col-span-2">Descrição</div>
                    <div>Categoria</div>
                    <div>Data</div>
                    <div>Cliente</div>
                    <div>Forma de Pagamento</div>
                    <div className="text-right">Valor</div>
                  </div>
                  <div className="divide-y">
                    {recentTransactions.map((transaction: any) => {
                      const client = clients?.find((c: any) => c.id === transaction.clientId);
                      
                      return (
                        <div key={transaction.id} className="grid grid-cols-7 p-4 hover:bg-muted/50">
                          <div className="col-span-2 font-medium">{transaction.description || "N/A"}</div>
                          <div>{transaction.category}</div>
                          <div>{formatDate(transaction.date)}</div>
                          <div>{client ? client.fullName : "N/A"}</div>
                          <div>{transaction.paymentMethod || "N/A"}</div>
                          <div className="text-right flex justify-end items-center">
                            {transaction.type === "income" ? (
                              <ArrowUp className="mr-1 h-4 w-4 text-success" />
                            ) : (
                              <ArrowDown className="mr-1 h-4 w-4 text-destructive" />
                            )}
                            <span className={transaction.type === "income" ? "text-success" : "text-destructive"}>
                              {formatCurrency(transaction.amount)}
                            </span>
                          </div>
                        </div>
                      );
                    })}
                  </div>
                </div>
              )}
            </CardContent>
          </Card>
        </TabsContent>
      </Tabs>

      {/* Create Transaction Dialog */}
      <Dialog open={isCreateDialogOpen} onOpenChange={setIsCreateDialogOpen}>
        <DialogContent className="sm:max-w-[500px]">
          <DialogHeader>
            <DialogTitle>Registrar Transação Financeira</DialogTitle>
            <DialogDescription>
              Digite os detalhes para a nova transação.
            </DialogDescription>
          </DialogHeader>
          
          <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
              <FormField
                control={form.control}
                name="type"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Tipo de Transação</FormLabel>
                    <Select
                      value={field.value}
                      onValueChange={field.onChange}
                    >
                      <FormControl>
                        <SelectTrigger>
                          <SelectValue placeholder="Selecione o tipo de transação" />
                        </SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        <SelectItem value="income">Receita</SelectItem>
                        <SelectItem value="expense">Despesa</SelectItem>
                      </SelectContent>
                    </Select>
                    <FormMessage />
                  </FormItem>
                )}
              />
              
              <FormField
                control={form.control}
                name="category"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Categoria</FormLabel>
                    <FormControl>
                      <Input {...field} placeholder="Serviço, Produto, Aluguel, etc." />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              
              <FormField
                control={form.control}
                name="amount"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Valor (R$)</FormLabel>
                    <FormControl>
                      <Input type="number" step="0.01" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              
              <FormField
                control={form.control}
                name="date"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Data</FormLabel>
                    <FormControl>
                      <Input type="date" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              
              <FormField
                control={form.control}
                name="description"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Descrição</FormLabel>
                    <FormControl>
                      <Textarea
                        {...field}
                        placeholder="Breve descrição da transação"
                        rows={2}
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              
              <FormField
                control={form.control}
                name="paymentMethod"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Forma de Pagamento</FormLabel>
                    <Select
                      value={field.value || ""}
                      onValueChange={field.onChange}
                    >
                      <FormControl>
                        <SelectTrigger>
                          <SelectValue placeholder="Selecione a forma de pagamento" />
                        </SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        <SelectItem value="cash">Dinheiro</SelectItem>
                        <SelectItem value="credit">Cartão de Crédito</SelectItem>
                        <SelectItem value="debit">Cartão de Débito</SelectItem>
                        <SelectItem value="transfer">Transferência Bancária</SelectItem>
                        <SelectItem value="insurance">Convênio</SelectItem>
                        <SelectItem value="other">Outro</SelectItem>
                      </SelectContent>
                    </Select>
                    <FormMessage />
                  </FormItem>
                )}
              />
              
              {form.watch("type") === "income" && (
                <FormField
                  control={form.control}
                  name="clientId"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Cliente (Opcional)</FormLabel>
                      <Select
                        value={field.value?.toString() || ""}
                        onValueChange={(value) => field.onChange(value ? Number(value) : undefined)}
                      >
                        <FormControl>
                          <SelectTrigger>
                            <SelectValue placeholder="Selecione o cliente" />
                          </SelectTrigger>
                        </FormControl>
                        <SelectContent>
                          <SelectItem value="">Nenhum</SelectItem>
                          {clients?.map((client: any) => (
                            <SelectItem key={client.id} value={client.id.toString()}>
                              {client.fullName}
                            </SelectItem>
                          ))}
                        </SelectContent>
                      </Select>
                      <FormMessage />
                    </FormItem>
                  )}
                />
              )}
              
              <DialogFooter>
                <Button
                  type="button"
                  variant="outline"
                  onClick={() => setIsCreateDialogOpen(false)}
                >
                  Cancelar
                </Button>
                <Button type="submit" disabled={createTransaction.isPending}>
                  {createTransaction.isPending && (
                    <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                  )}
                  Registrar Transação
                </Button>
              </DialogFooter>
            </form>
          </Form>
        </DialogContent>
      </Dialog>
    </div>
  );
}
