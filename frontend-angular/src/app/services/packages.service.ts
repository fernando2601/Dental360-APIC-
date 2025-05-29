import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Package {
  id: number;
  name: string;
  description: string;
  services: string[];
  originalPrice: string;
  packagePrice: string;
  discount: string;
  validityDays: number;
  active: boolean;
  createdAt: Date;
}

export interface CreatePackage {
  name: string;
  description: string;
  services: string[];
  originalPrice: string;
  packagePrice: string;
  discount: string;
  validityDays: number;
  active?: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class PackagesService {
  private apiUrl = '/api/packages';

  constructor(private http: HttpClient) {}

  getPackages(): Observable<Package[]> {
    return this.http.get<Package[]>(this.apiUrl);
  }

  getPackage(id: number): Observable<Package> {
    return this.http.get<Package>(`${this.apiUrl}/${id}`);
  }

  createPackage(packageData: CreatePackage): Observable<Package> {
    return this.http.post<Package>(this.apiUrl, packageData);
  }

  updatePackage(id: number, packageData: Partial<CreatePackage>): Observable<Package> {
    return this.http.put<Package>(`${this.apiUrl}/${id}`, packageData);
  }

  deletePackage(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}