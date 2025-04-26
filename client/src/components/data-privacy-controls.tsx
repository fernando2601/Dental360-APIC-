import { useState } from "react";
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Switch } from "@/components/ui/switch";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Shield, Download, CheckCircle, Loader2 } from "lucide-react";
import { useToast } from "@/hooks/use-toast";

// Interface for privacy settings
interface PrivacySettings {
  retentionPeriod: string;
  loggingEnabled: boolean;
  trainingEnabled: boolean;
  anonymizationEnabled: boolean;
  autoDeletionEnabled: boolean;
}

export function DataPrivacyControls() {
  // Initial privacy settings (would normally come from an API)
  const [settings, setSettings] = useState<PrivacySettings>({
    retentionPeriod: "90",
    loggingEnabled: true,
    trainingEnabled: true,
    anonymizationEnabled: false,
    autoDeletionEnabled: true,
  });

  const [isSaving, setIsSaving] = useState(false);
  const [isExporting, setIsExporting] = useState(false);
  const { toast } = useToast();

  // Handle settings changes
  const handleRetentionChange = (value: string) => {
    setSettings({ ...settings, retentionPeriod: value });
  };

  const handleLoggingChange = (checked: boolean) => {
    setSettings({ ...settings, loggingEnabled: checked });
  };

  const handleTrainingChange = (checked: boolean) => {
    setSettings({ ...settings, trainingEnabled: checked });
  };

  const handleAnonymizationChange = (checked: boolean) => {
    setSettings({ ...settings, anonymizationEnabled: checked });
  };

  const handleAutoDeletionChange = (checked: boolean) => {
    setSettings({ ...settings, autoDeletionEnabled: checked });
  };

  // Save privacy settings
  const saveSettings = () => {
    setIsSaving(true);
    
    // Simulated API call - would be replaced with actual API call
    setTimeout(() => {
      setIsSaving(false);
      toast({
        title: "Settings Saved",
        description: "Your privacy settings have been updated successfully.",
      });
    }, 1000);
  };

  // Export privacy report
  const exportPrivacyReport = () => {
    setIsExporting(true);
    
    // Simulated report generation
    setTimeout(() => {
      setIsExporting(false);
      
      // Create report content
      const reportDate = new Date().toISOString().split('T')[0];
      const reportContent = `# DentalSpa Privacy Report
Date: ${reportDate}

## Current Settings
- Data Retention Period: ${settings.retentionPeriod} days
- Conversation Logging: ${settings.loggingEnabled ? 'Enabled' : 'Disabled'}
- AI Training Data Usage: ${settings.trainingEnabled ? 'Enabled' : 'Disabled'}
- Data Anonymization: ${settings.anonymizationEnabled ? 'Enabled' : 'Disabled'}
- Automated Deletion: ${settings.autoDeletionEnabled ? 'Enabled' : 'Disabled'}

## Privacy Policy Summary
This report documents the current privacy settings for the DentalSpa management system.
All client data is handled according to these settings and in compliance with relevant privacy regulations.
`;

      // Create and trigger download
      const blob = new Blob([reportContent], { type: 'text/plain' });
      const url = URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = url;
      link.download = `privacy-report-${reportDate}.txt`;
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
      
      toast({
        title: "Report Exported",
        description: "Privacy report has been downloaded successfully.",
      });
    }, 1000);
  };

  return (
    <Card>
      <CardHeader>
        <CardTitle className="flex items-center">
          <Shield className="mr-2 h-5 w-5 text-primary" />
          Data Privacy Controls
        </CardTitle>
        <CardDescription>
          Manage how client data is stored, processed, and protected
        </CardDescription>
      </CardHeader>
      <CardContent className="space-y-6">
        <div className="flex items-center justify-between">
          <div>
            <h3 className="font-medium text-base">Client Data Retention</h3>
            <p className="text-sm text-muted-foreground mt-1">Control how long customer data is stored</p>
          </div>
          <Select value={settings.retentionPeriod} onValueChange={handleRetentionChange}>
            <SelectTrigger className="w-36">
              <SelectValue placeholder="Select period" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="30">30 days</SelectItem>
              <SelectItem value="60">60 days</SelectItem>
              <SelectItem value="90">90 days</SelectItem>
              <SelectItem value="180">180 days</SelectItem>
              <SelectItem value="365">1 year</SelectItem>
            </SelectContent>
          </Select>
        </div>
        
        <div className="flex items-center justify-between">
          <div>
            <h3 className="font-medium text-base">Conversation Logging</h3>
            <p className="text-sm text-muted-foreground mt-1">Save chat transcripts for quality improvement</p>
          </div>
          <Switch
            checked={settings.loggingEnabled}
            onCheckedChange={handleLoggingChange}
          />
        </div>
        
        <div className="flex items-center justify-between">
          <div>
            <h3 className="font-medium text-base">AI Training Data Usage</h3>
            <p className="text-sm text-muted-foreground mt-1">Allow using interactions to improve AI</p>
          </div>
          <Switch
            checked={settings.trainingEnabled}
            onCheckedChange={handleTrainingChange}
          />
        </div>
        
        <div className="flex items-center justify-between">
          <div>
            <h3 className="font-medium text-base">Data Anonymization</h3>
            <p className="text-sm text-muted-foreground mt-1">Remove identifying information from logs</p>
          </div>
          <Switch
            checked={settings.anonymizationEnabled}
            onCheckedChange={handleAnonymizationChange}
          />
        </div>
        
        <div className="flex items-center justify-between">
          <div>
            <h3 className="font-medium text-base">Automated Deletion</h3>
            <p className="text-sm text-muted-foreground mt-1">Schedule regular data purging</p>
          </div>
          <Switch
            checked={settings.autoDeletionEnabled}
            onCheckedChange={handleAutoDeletionChange}
          />
        </div>
      </CardContent>
      <CardFooter className="flex flex-col space-y-2">
        <Button 
          className="w-full" 
          onClick={saveSettings}
          disabled={isSaving}
        >
          {isSaving ? (
            <>
              <Loader2 className="mr-2 h-4 w-4 animate-spin" />
              Saving...
            </>
          ) : (
            <>
              <CheckCircle className="mr-2 h-4 w-4" />
              Save Privacy Settings
            </>
          )}
        </Button>
        <Button 
          variant="outline" 
          className="w-full" 
          onClick={exportPrivacyReport}
          disabled={isExporting}
        >
          {isExporting ? (
            <>
              <Loader2 className="mr-2 h-4 w-4 animate-spin" />
              Generating Report...
            </>
          ) : (
            <>
              <Download className="mr-2 h-4 w-4" />
              Export Current Privacy Report
            </>
          )}
        </Button>
      </CardFooter>
    </Card>
  );
}
