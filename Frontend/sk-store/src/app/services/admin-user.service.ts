import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { UserDto, UserFilterParameters, UserForAdminUpdateDto, RoleDto } from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class AdminUserService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/users`;
  private rolesApiUrl = `${environment.apiUrl}/api/roles`; // Giả sử có API này

  getUsers(filters: UserFilterParameters): Observable<HttpResponse<UserDto[]>> {
    let params = new HttpParams()
      .set('pageNumber', filters.pageNumber.toString())
      .set('pageSize', filters.pageSize.toString());

    if (filters.searchTerm) {
      params = params.append('searchTerm', filters.searchTerm);
    }
    if (filters.roleId) {
      params = params.append('roleId', filters.roleId.toString());
    }
    if (filters.isActive !== null && filters.isActive !== undefined) {
      params = params.append('isActive', filters.isActive.toString());
    }

    return this.http.get<UserDto[]>(this.apiUrl, { params, observe: 'response' });
  }

  updateUser(id: number, data: UserForAdminUpdateDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, data);
  }

  // Tạm thời hardcode Roles vì backend chưa có API
  // Khi có API, bạn sẽ thay thế bằng: return this.http.get<RoleDto[]>(this.rolesApiUrl);
  getRoles(): Observable<RoleDto[]> {
    const hardcodedRoles: RoleDto[] = [
      { roleId: 1, roleName: 'Admin' },
      { roleId: 2, roleName: 'User' }
    ];
    return of(hardcodedRoles);
  }
}