import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class StaffService {
  private apiUrl = '/api/staff';

  constructor(private http: HttpClient) {}

  getStaff(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  getStaffMember(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  createStaffMember(staff: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, staff);
  }

  updateStaffMember(id: number, staff: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${id}`, staff);
  }

  deleteStaffMember(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }

  getStaffStats(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/stats`);
  }
}