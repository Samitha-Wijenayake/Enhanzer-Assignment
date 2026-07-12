import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ItemService {
  private readonly baseUrl = `${environment.apiUrl}/items`;

  constructor(private readonly http: HttpClient) {}

  getItems(search?: string): Observable<string[]> {
    const query = search ? `?search=${encodeURIComponent(search)}` : '';
    return this.http.get<string[]>(`${this.baseUrl}${query}`);
  }
}
