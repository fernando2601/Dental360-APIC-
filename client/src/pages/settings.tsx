import { useState } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { apiRequest } from "@/lib/queryClient";
import { useToast } from "@/hooks/use-toast";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Form, FormControl, FormDescription, FormField, FormItem, FormLabel, FormMessage } from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Textarea } from "@/components/ui/textarea";
import { Badge } from "@/components/ui/badge";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { Plus, Search, MessageSquare, Shield, Edit, Trash2, Eye, Loader2 } from "lucide-react";
import { DataPrivacyControls } from "@/components/data-privacy-controls";
import { formatTimeAgo } from "@/lib/utils";

// Form schema for chat template
const chatTemplateFormSchema = z.object({
  title: z.string().min(2, { message: "Title must be at least 2 characters." }),
  category: z.string().min(1, { message: "Category is required." }),
  content: z.string().min(10, { message: "Content must be at least 10 characters." }),
  active: z.boolean().default(true),
});

type ChatTemplateFormValues = z.infer<typeof chatTemplateFormSchema>;

export default function Settings() {
  const [isCreateDialogOpen, setIsCreateDialogOpen] = useState(false);
  const [activeTemplate, setActiveTemplate] = useState<any>(null);
  const [isViewDialogOpen, setIsViewDialogOpen] = useState(false);
  const [isEditDialogOpen, setIsEditDialogOpen] = useState(false);
  const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);
  const [searchQuery, setSearchQuery] = useState("");
  const [categoryFilter, setCategoryFilter] = useState("all");
  const { toast } = useToast();
  const queryClient = useQueryClient();

  // Initialize create form
  const createForm = useForm<ChatTemplateFormValues>({
    resolver: zodResolver(chatTemplateFormSchema),
    defaultValues: {
      title: "",
      category: "",
      content: "",
      active: true,
    },
  });

  // Initialize edit form
  const editForm = useForm<ChatTemplateFormValues>({
    resolver: zodResolver(chatTemplateFormSchema),
    defaultValues: {
      title: "",
      category: "",
      content: "",
      active: true,
    },
  });

  // Fetch chat templates
  const { data: templates, isLoading } = useQuery({
    queryKey: ['/api/chat-templates'],
  });

  // Create template mutation
  const createTemplate = useMutation({
    mutationFn: async (values: ChatTemplateFormValues) => {
      const response = await apiRequest('POST', '/api/chat-templates', values);
      return response.json();
    },
    onSuccess: () => {
      toast({
        title: "Success",
        description: "Chat template created successfully.",
      });
      queryClient.invalidateQueries({ queryKey: ['/api/chat-templates'] });
      setIsCreateDialogOpen(false);
      createForm.reset();
    },
    onError: () => {
      toast({
        title: "Error",
        description: "Failed to create chat template. Please try again.",
        variant: "destructive",
      });
    },
  });

  // Update template mutation
  const updateTemplate = useMutation({
    mutationFn: async (values: ChatTemplateFormValues & { id: number }) => {
      const { id, ...data } = values;
      const response = await apiRequest('PUT', `/api/chat-templates/${id}`, data);
      return response.json();
    },
    onSuccess: () => {
      toast({
        title: "Success",
        description: "Chat template updated successfully.",
      });
      queryClient.invalidateQueries({ queryKey: ['/api/chat-templates'] });
      setIsEditDialogOpen(false);
    },
    onError: () => {
      toast({
        title: "Error",
        description: "Failed to update chat template. Please try again.",
        variant: "destructive",
      });
    },
  });

  // Delete template mutation
  const deleteTemplate = useMutation({
    mutationFn: async (id: number) => {
      const response = await apiRequest('DELETE', `/api/chat-templates/${id}`);
      return response;
    },
    onSuccess: () => {
      toast({
        title: "Success",
        description: "Chat template deleted successfully.",
      });
      queryClient.invalidateQueries({ queryKey: ['/api/chat-templates'] });
      setIsDeleteDialogOpen(false);
      setActiveTemplate(null);
    },
    onError: () => {
      toast({
        title: "Error",
        description: "Failed to delete chat template. Please try again.",
        variant: "destructive",
      });
    },
  });

  // Handle form submissions
  function onCreateSubmit(values: ChatTemplateFormValues) {
    createTemplate.mutate(values);
  }

  function onEditSubmit(values: ChatTemplateFormValues) {
    if (!activeTemplate) return;
    updateTemplate.mutate({ ...values, id: activeTemplate.id });
  }

  function handleEditTemplate(template: any) {
    setActiveTemplate(template);
    editForm.reset({
      title: template.title,
      category: template.category,
      content: template.content,
      active: template.active,
    });
    setIsEditDialogOpen(true);
  }

  function handleViewTemplate(template: any) {
    setActiveTemplate(template);
    setIsViewDialogOpen(true);
  }

  function handleDeleteTemplate(template: any) {
    setActiveTemplate(template);
    setIsDeleteDialogOpen(true);
  }

  // Get unique categories for filter
  const categories = templates
    ? ["all", ...new Set(templates.map((t: any) => t.category))]
    : ["all"];

  // Filter templates
  const filteredTemplates = templates?.filter((template: any) => {
    // Filter by category
    const categoryMatch = categoryFilter === "all" || template.category === categoryFilter;
    
    // Filter by search query
    const searchMatch = !searchQuery || 
      template.title.toLowerCase().includes(searchQuery.toLowerCase()) ||
      template.content.toLowerCase().includes(searchQuery.toLowerCase());
    
    return categoryMatch && searchMatch;
  });

  return (
    <div className="space-y-6">
      <div className="flex flex-col md:flex-row justify-between items-start md:items-center">
        <div>
          <h1 className="text-2xl md:text-3xl font-bold tracking-tight">Settings</h1>
          <p className="text-muted-foreground">Configure your AI chatbot and data privacy settings.</p>
        </div>
      </div>

      <Tabs defaultValue="chatbot" className="w-full">
        <TabsList className="mb-4">
          <TabsTrigger value="chatbot">Chatbot Templates</TabsTrigger>
          <TabsTrigger value="privacy">Data Privacy</TabsTrigger>
        </TabsList>
        
        <TabsContent value="chatbot" className="space-y-6">
          <div className="flex flex-col md:flex-row justify-between items-start md:items-center">
            <div>
              <h2 className="text-xl font-bold">Chat Response Templates</h2>
              <p className="text-muted-foreground">Manage automatic response templates for the AI chatbot.</p>
            </div>
            <Button className="mt-4 md:mt-0" onClick={() => setIsCreateDialogOpen(true)}>
              <Plus className="mr-2 h-4 w-4" />
              New Template
            </Button>
          </div>

          {/* Search and filter section */}
          <div className="flex flex-col sm:flex-row gap-3">
            <div className="relative flex-grow">
              <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-muted-foreground" />
              <Input
                placeholder="Search templates..."
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                className="pl-10"
              />
            </div>
            <Select value={categoryFilter} onValueChange={setCategoryFilter}>
              <SelectTrigger className="w-full sm:w-[180px]">
                <SelectValue placeholder="Category" />
              </SelectTrigger>
              <SelectContent>
                {categories.map((category) => (
                  <SelectItem key={category} value={category}>
                    {category === "all" ? "All Categories" : category}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>

          {/* Templates list */}
          {isLoading ? (
            <div className="flex justify-center py-12">
              <Loader2 className="h-8 w-8 animate-spin text-muted-foreground" />
            </div>
          ) : !filteredTemplates || filteredTemplates.length === 0 ? (
            <Card>
              <CardContent className="flex flex-col items-center justify-center py-10">
                <MessageSquare className="h-16 w-16 text-muted-foreground mb-4" />
                <h3 className="text-lg font-medium mb-2">No templates found</h3>
                <p className="text-sm text-muted-foreground text-center max-w-md mb-6">
                  {categoryFilter === "all" && !searchQuery
                    ? "You haven't created any chat templates yet. Create your first template to get started."
                    : "No templates match your search criteria. Try adjusting your filters."}
                </p>
                <Button onClick={() => setIsCreateDialogOpen(true)}>
                  <Plus className="mr-2 h-4 w-4" />
                  Create Template
                </Button>
              </CardContent>
            </Card>
          ) : (
            <div className="space-y-4">
              {filteredTemplates.map((template: any) => (
                <Card key={template.id} className="hover:border-primary transition-colors">
                  <CardContent className="p-6">
                    <div className="flex justify-between items-start">
                      <div>
                        <h3 className="font-medium text-lg">{template.title}</h3>
                        <p className="text-sm text-muted-foreground">{template.category}</p>
                      </div>
                      <Badge variant={template.active ? "default" : "secondary"}>
                        {template.active ? "Active" : "Inactive"}
                      </Badge>
                    </div>
                    <p className="mt-4 text-sm line-clamp-2">{template.content}</p>
                    <div className="mt-4 flex items-center justify-between">
                      <div className="text-sm text-muted-foreground">
                        <span className="mr-4">Uses: {template.usageCount || 0}</span>
                        <span>{template.lastUsed ? `Last used ${formatTimeAgo(template.lastUsed)}` : "Never used"}</span>
                      </div>
                      <div className="flex space-x-2">
                        <Button variant="ghost" size="sm" onClick={() => handleViewTemplate(template)}>
                          <Eye className="h-4 w-4" />
                          <span className="sr-only">View</span>
                        </Button>
                        <Button variant="ghost" size="sm" onClick={() => handleEditTemplate(template)}>
                          <Edit className="h-4 w-4" />
                          <span className="sr-only">Edit</span>
                        </Button>
                        <Button variant="ghost" size="sm" onClick={() => handleDeleteTemplate(template)}>
                          <Trash2 className="h-4 w-4" />
                          <span className="sr-only">Delete</span>
                        </Button>
                      </div>
                    </div>
                  </CardContent>
                </Card>
              ))}
            </div>
          )}
        </TabsContent>
        
        <TabsContent value="privacy">
          <DataPrivacyControls />
        </TabsContent>
      </Tabs>

      {/* Create Template Dialog */}
      <Dialog open={isCreateDialogOpen} onOpenChange={setIsCreateDialogOpen}>
        <DialogContent className="sm:max-w-[600px]">
          <DialogHeader>
            <DialogTitle>Create New Template</DialogTitle>
            <DialogDescription>
              Create a new automatic response template for the chatbot.
            </DialogDescription>
          </DialogHeader>
          
          <Form {...createForm}>
            <form onSubmit={createForm.handleSubmit(onCreateSubmit)} className="space-y-4">
              <FormField
                control={createForm.control}
                name="title"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Title</FormLabel>
                    <FormControl>
                      <Input {...field} placeholder="Botox Treatment Information" />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              
              <FormField
                control={createForm.control}
                name="category"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Category</FormLabel>
                    <Select
                      value={field.value}
                      onValueChange={field.onChange}
                    >
                      <FormControl>
                        <SelectTrigger>
                          <SelectValue placeholder="Select a category" />
                        </SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        <SelectItem value="Treatment Information">Treatment Information</SelectItem>
                        <SelectItem value="Pricing Questions">Pricing Questions</SelectItem>
                        <SelectItem value="Appointment Scheduling">Appointment Scheduling</SelectItem>
                        <SelectItem value="Post-Treatment Care">Post-Treatment Care</SelectItem>
                        <SelectItem value="General Inquiries">General Inquiries</SelectItem>
                      </SelectContent>
                    </Select>
                    <FormMessage />
                  </FormItem>
                )}
              />
              
              <FormField
                control={createForm.control}
                name="content"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Content</FormLabel>
                    <FormControl>
                      <Textarea
                        {...field}
                        placeholder="The detailed response content that will be shown to clients..."
                        rows={8}
                      />
                    </FormControl>
                    <FormDescription>
                      You can use placeholders like {'{client_name}'}, {'{appointment_date}'}, etc. which will be replaced with actual values when the response is sent.
                    </FormDescription>
                    <FormMessage />
                  </FormItem>
                )}
              />
              
              <FormField
                control={createForm.control}
                name="active"
                render={({ field }) => (
                  <FormItem className="flex flex-row items-center justify-between rounded-lg border p-3">
                    <div className="space-y-0.5">
                      <FormLabel>Active</FormLabel>
                      <FormDescription>
                        Enable this template to be used by the chatbot
                      </FormDescription>
                    </div>
                    <FormControl>
                      <Input
                        type="checkbox"
                        checked={field.value}
                        onChange={field.onChange}
                        className="h-4 w-4"
                      />
                    </FormControl>
                  </FormItem>
                )}
              />
              
              <DialogFooter>
                <Button
                  type="button"
                  variant="outline"
                  onClick={() => setIsCreateDialogOpen(false)}
                >
                  Cancel
                </Button>
                <Button type="submit" disabled={createTemplate.isPending}>
                  {createTemplate.isPending && (
                    <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                  )}
                  Create Template
                </Button>
              </DialogFooter>
            </form>
          </Form>
        </DialogContent>
      </Dialog>

      {/* View Template Dialog */}
      {activeTemplate && (
        <Dialog open={isViewDialogOpen} onOpenChange={setIsViewDialogOpen}>
          <DialogContent className="sm:max-w-[600px]">
            <DialogHeader>
              <DialogTitle>{activeTemplate.title}</DialogTitle>
              <DialogDescription>
                {activeTemplate.category} • {activeTemplate.active ? "Active" : "Inactive"}
              </DialogDescription>
            </DialogHeader>
            
            <div className="space-y-4">
              <div className="bg-muted p-4 rounded-md whitespace-pre-wrap text-sm">
                {activeTemplate.content}
              </div>
              
              <div className="flex justify-between text-sm text-muted-foreground">
                <div>Usage count: {activeTemplate.usageCount || 0}</div>
                <div>
                  {activeTemplate.lastUsed 
                    ? `Last used ${formatTimeAgo(activeTemplate.lastUsed)}` 
                    : "Never used"}
                </div>
              </div>
            </div>
            
            <DialogFooter>
              <Button variant="outline" onClick={() => setIsViewDialogOpen(false)}>
                Close
              </Button>
              <Button onClick={() => {
                setIsViewDialogOpen(false);
                handleEditTemplate(activeTemplate);
              }}>
                Edit Template
              </Button>
            </DialogFooter>
          </DialogContent>
        </Dialog>
      )}

      {/* Edit Template Dialog */}
      {activeTemplate && (
        <Dialog open={isEditDialogOpen} onOpenChange={setIsEditDialogOpen}>
          <DialogContent className="sm:max-w-[600px]">
            <DialogHeader>
              <DialogTitle>Edit Template</DialogTitle>
              <DialogDescription>
                Update the response template details.
              </DialogDescription>
            </DialogHeader>
            
            <Form {...editForm}>
              <form onSubmit={editForm.handleSubmit(onEditSubmit)} className="space-y-4">
                <FormField
                  control={editForm.control}
                  name="title"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Title</FormLabel>
                      <FormControl>
                        <Input {...field} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
                
                <FormField
                  control={editForm.control}
                  name="category"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Category</FormLabel>
                      <Select
                        value={field.value}
                        onValueChange={field.onChange}
                      >
                        <FormControl>
                          <SelectTrigger>
                            <SelectValue placeholder="Select a category" />
                          </SelectTrigger>
                        </FormControl>
                        <SelectContent>
                          <SelectItem value="Treatment Information">Treatment Information</SelectItem>
                          <SelectItem value="Pricing Questions">Pricing Questions</SelectItem>
                          <SelectItem value="Appointment Scheduling">Appointment Scheduling</SelectItem>
                          <SelectItem value="Post-Treatment Care">Post-Treatment Care</SelectItem>
                          <SelectItem value="General Inquiries">General Inquiries</SelectItem>
                        </SelectContent>
                      </Select>
                      <FormMessage />
                    </FormItem>
                  )}
                />
                
                <FormField
                  control={editForm.control}
                  name="content"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Content</FormLabel>
                      <FormControl>
                        <Textarea
                          {...field}
                          rows={8}
                        />
                      </FormControl>
                      <FormDescription>
                        You can use placeholders like {'{client_name}'}, {'{appointment_date}'}, etc. which will be replaced with actual values when the response is sent.
                      </FormDescription>
                      <FormMessage />
                    </FormItem>
                  )}
                />
                
                <FormField
                  control={editForm.control}
                  name="active"
                  render={({ field }) => (
                    <FormItem className="flex flex-row items-center justify-between rounded-lg border p-3">
                      <div className="space-y-0.5">
                        <FormLabel>Active</FormLabel>
                        <FormDescription>
                          Enable this template to be used by the chatbot
                        </FormDescription>
                      </div>
                      <FormControl>
                        <Input
                          type="checkbox"
                          checked={field.value}
                          onChange={field.onChange}
                          className="h-4 w-4"
                        />
                      </FormControl>
                    </FormItem>
                  )}
                />
                
                <DialogFooter>
                  <Button
                    type="button"
                    variant="outline"
                    onClick={() => setIsEditDialogOpen(false)}
                  >
                    Cancel
                  </Button>
                  <Button type="submit" disabled={updateTemplate.isPending}>
                    {updateTemplate.isPending && (
                      <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                    )}
                    Save Changes
                  </Button>
                </DialogFooter>
              </form>
            </Form>
          </DialogContent>
        </Dialog>
      )}

      {/* Delete Confirmation Dialog */}
      {activeTemplate && (
        <Dialog open={isDeleteDialogOpen} onOpenChange={setIsDeleteDialogOpen}>
          <DialogContent>
            <DialogHeader>
              <DialogTitle>Delete Template</DialogTitle>
              <DialogDescription>
                Are you sure you want to delete "{activeTemplate.title}"? This action cannot be undone.
              </DialogDescription>
            </DialogHeader>
            
            <DialogFooter>
              <Button
                variant="outline"
                onClick={() => setIsDeleteDialogOpen(false)}
              >
                Cancel
              </Button>
              <Button 
                variant="destructive" 
                onClick={() => deleteTemplate.mutate(activeTemplate.id)}
                disabled={deleteTemplate.isPending}
              >
                {deleteTemplate.isPending && (
                  <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                )}
                Delete
              </Button>
            </DialogFooter>
          </DialogContent>
        </Dialog>
      )}
    </div>
  );
}
