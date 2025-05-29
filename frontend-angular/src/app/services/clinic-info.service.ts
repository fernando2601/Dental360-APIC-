import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface ClinicInfo {
  id: number;
  name: string;
  address: string;
  phone: string;
  email: string;
  website: string | null;
  description: string | null;
  logoUrl: string | null;
  socialMedia: any;
  businessHours: any;
  createdAt: Date;
}

export interface CreateClinicInfo {
  name: string;
  address: string;
  phone: string;
  email: string;
  website?: string;
  description?: string;
  logoUrl?: string;
  socialMedia?: any;
  businessHours?: any;
}

@Injectable({
  providedIn: 'root'
})
export class ClinicInfoService {
  private apiUrl = '/api/clinic-info';

  constructor(private http: HttpClient) {}

  getClinicInfo(): Observable<ClinicInfo> {
    return this.http.get<ClinicInfo>(this.apiUrl);
  }

  updateClinicInfo(clinicInfo: Partial<CreateClinicInfo>): Observable<ClinicInfo> {
    return this.http.put<ClinicInfo>(this.apiUrl, clinicInfo);
  }

  uploadLogo(formData: FormData): Observable<{ logoUrl: string }> {
    return this.http.post<{ logoUrl: string }>(`${this.apiUrl}/logo`, formData);
  }

  getStats(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/stats`);
  }
}