import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, shareReplay } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { BrandDto } from '../models/brand.model';

@Injectable({
  providedIn: 'root'
})
export class BrandService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/brands`;

  private brands$: Observable<BrandDto[]> | null = null;

  getBrands(): Observable<BrandDto[]> {
    if (!this.brands$) {
      this.brands$ = this.http.get<BrandDto[]>(this.apiUrl).pipe(
        shareReplay(1),
        catchError(this.handleError)
      );
    }
    return this.brands$;
  }

  private handleError(error: any): Observable<never> {
    console.error('An error occurred in BrandService:', error);
    throw new Error('Failed to load brands. Please try again later.');
  }
}