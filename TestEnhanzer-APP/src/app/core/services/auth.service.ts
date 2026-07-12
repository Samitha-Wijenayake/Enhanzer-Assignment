import { Injectable, computed, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';

import { environment } from '../../../environments/environment';
import { AuthSession, LoginRequest, LoginResponse } from '../models/auth.model';
import { LocationDto } from '../models/location.model';

const SESSION_KEY = 'te.session';
const LOCATIONS_KEY = 'te.locations';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly baseUrl = `${environment.apiUrl}/auth`;

  private readonly sessionSig = signal<AuthSession | null>(this.readSession());
  // private readonly locationsSig = signal<LocationDto[]>(this.readLocations());

  /** Reactive, read-only view of the current session. */
  readonly session = this.sessionSig.asReadonly();
  // readonly locations = this.locationsSig.asReadonly();
  readonly isAuthenticated = computed(() => {
    const session = this.sessionSig();
    if (!session) {
      return false;
    }
    return new Date(session.expiresAtUtc).getTime() > Date.now();
  });
  readonly username = computed(() => this.sessionSig()?.username ?? '');

  constructor(private readonly http: HttpClient) {}

  login(request: LoginRequest): Observable<LoginResponse> {
    const loginUrl = `${this.baseUrl}/login`;

  return this.http.post<LoginResponse>(loginUrl, request).pipe(
    tap((response) => {
      this.saveAuth(response);
    })
  );
  }

  logout(): void {
    localStorage.removeItem(SESSION_KEY);
    localStorage.removeItem(LOCATIONS_KEY);
    this.sessionSig.set(null);
    // this.locationsSig.set([]);
  }

  getToken(): string | null {
    return this.sessionSig()?.token ?? null;
  }

  private saveAuth(response: LoginResponse): void {
    const session: AuthSession = {
      token: response.token,
      expiresAtUtc: response.expiresAtUtc,
      username: response.username,
    };
    localStorage.setItem(SESSION_KEY, JSON.stringify(session));
    // localStorage.setItem(LOCATIONS_KEY, JSON.stringify(response.locations ?? []));
    this.sessionSig.set(session);
    // this.locationsSig.set(response.locations ?? []);
  }

  private readSession(): AuthSession | null {
    try {
      const raw = localStorage.getItem(SESSION_KEY);
      return raw ? (JSON.parse(raw) as AuthSession) : null;
    } catch {
      return null;
    }
  }

  private readLocatdions(): LocationDto[] {
    try {
      const raw = localStorage.getItem(LOCATIONS_KEY);
      return raw ? (JSON.parse(raw) as LocationDto[]) : [];
    } catch {
      return [];
    }
  }
}
