import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { LocationDto } from '../models/location.model';

@Injectable({ providedIn: 'root' })
export class LocationService {
  private readonly baseUrl = `${environment.apiUrl}/locations`;

  constructor(private readonly http: HttpClient) {}

  getLocations(): Observable<LocationDto[]> {
    return this.http.get<LocationDto[]>(this.baseUrl);
  }
}
